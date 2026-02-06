// Interactive World Map Component
// Demonstrates: Leaflet integration, AbortController pattern, Responsive layout, and i18n support.

import { useMemo, useState, useEffect } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import { useTranslation } from "react-i18next";
import L from "leaflet";
import "./WorldMap.css";

// Base URL is pulled from environment variables, never hardcoded
const API_BASE = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/$/, "");

// Custom CSS-based icon for better performance than image assets
const alumniIcon = L.divIcon({
  className: "alumniPin",
  html: `<span class="alumniPin__pulse"></span><span class="alumniPin__dot"></span>`,
  iconSize: [26, 26],
  iconAnchor: [13, 13],
  popupAnchor: [0, -20],
});

export default function WorldMap({
  // Generic endpoint structure
  mapUrl = `${API_BASE}/api/membership/map/public?verifiedOnly=true&precision=2`,
  loginUrl = "/login",
}) {
  const { t } = useTranslation();
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);

  const [locations, setLocations] = useState([]); // Expects: [{lat, lng, count}]
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");

  // Handle window resize for responsive map behavior
  useEffect(() => {
    const handleResize = () => setIsMobile(window.innerWidth < 768);
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  // Fetch data with AbortController to prevent memory leaks on unmount
  useEffect(() => {
    const ac = new AbortController();

    (async () => {
      try {
        setLoading(true);
        setErr("");

        const res = await fetch(mapUrl, { signal: ac.signal });
        if (!res.ok) throw new Error("Failed to fetch map data");

        const data = await res.json();
        setLocations(Array.isArray(data) ? data : []);
      } catch (e) {
        if (!ac.signal.aborted) {
          console.error(e); // Log error internally
          setErr("Could not load alumni data.");
        }
      } finally {
        if (!ac.signal.aborted) setLoading(false);
      }
    })();

    return () => ac.abort();
  }, [mapUrl]);

  // Restrict map panning to the world view
  const worldBounds = useMemo(() => [[-85, -180], [85, 180]], []);

  const safeNum = (v) => (typeof v === "number" && Number.isFinite(v) ? v : null);

  const formatCount = (n) => {
    if (!n || n < 1) return t("map.count_unknown", "Members");
    if (n === 1) return t("map.count_one", "1 Member");
    return t("map.count_many", "{{count}} Members", { count: n });
  };

  return (
    <div className="mapContainerCard">
      <MapContainer
        center={[25, 10]}
        zoom={3}
        minZoom={2.5}
        maxBounds={worldBounds}
        maxBoundsViscosity={1.0}
        scrollWheelZoom={!isMobile}
        attributionControl={false}
        className="leafletMap"
      >
        {/* Using CartoDB Dark Matter tiles for modern look */}
        <TileLayer
          url="https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png"
          noWrap
          bounds={worldBounds}
        />

        {locations.map((loc, idx) => {
          const lat = safeNum(loc?.lat);
          const lng = safeNum(loc?.lng);
          const count = safeNum(loc?.count) ?? 0;

          if (lat === null || lng === null) return null;

          return (
            <Marker
              key={`${lat}-${lng}-${idx}`}
              position={[lat, lng]}
              icon={alumniIcon}
              riseOnHover
            >
              <Popup className="customPopup" closeButton autoPan>
                <div className="popupCard">
                  <div className="popupHeader">
                    <div className="popupTitle">{formatCount(count)}</div>
                    <div className="popupSub">
                      {t("map.login_hint", "Log in to view full profiles")}
                    </div>
                  </div>

                  <div style={{ paddingTop: 10 }}>
                    <a className="popupBtn popupBtn--primary" href={loginUrl}>
                      {t("map.btn_login", "Connect")}
                    </a>
                  </div>
                </div>
              </Popup>
            </Marker>
          );
        })}
      </MapContainer>

      {/* Floating Overlay UI */}
      <div className="mapOverlay">
        <div className="liveIndicator">
          <span className="blinkDot"></span>
          {t("map.live_status", "Live Map")}
        </div>

        <span className="overlaySubtitle">{t("map.org_name", "Global Alumni Network")}</span>
        <h2 className="overlayTitle">{t("map.title", "Our Community Worldwide")}</h2>
        <p className="overlayDesc">
          {t(
            "map.description",
            "Explore our growing network of graduates across the globe."
          )}
        </p>

        {loading && <div className="mapLoader">Loading data...</div>}
        {err && <div className="mapError">{err}</div>}
      </div>

      <div className="mapInteractionHint">
        {isMobile ? "Tap markers for details" : "Click markers for details"}
      </div>
    </div>
  );
}
