
### Intelligent Geocoding & Fallback-First Caching

One of the primary engineering goals was to map users worldwide accurately without overloading external APIs, while ensuring no profile ever remains "locationless" due to typos or API failures.

To solve this, I implemented a **"Fallback-First" Caching Strategy**:

1.  **🔍 Cache Lookup (Layer 1)**
    The system first checks the local `GeoCache` using a composite key (`City|Country`). If found, it serves the coordinates immediately (**0 API calls**).

2.  **🛡️ Immediate Fallback (Safety Net)**
    If the city is missing from the cache, the system **immediately creates a database record** using the *Country's* known center coordinates and flags it as `IsVerified = false`.
    * *Result:* This guarantees the user is mapped instantly, even before the external request is made.

3.  **📡 API Refinement (Layer 2)**
    Only after the fallback is secured does the system query the **Nominatim API** in the background.

4.  **✅ Verification & Update**
    * **Success:** If the API resolves the exact city, the database record is **updated** with the precise coordinates and set to `IsVerified = true`.
    * **Failure (Typo/Error):** The system silently keeps the previously saved Country coordinates. The profile remains functional on the map but is flagged for **admin review**.

> **📉 Impact:** This architecture ensures **100% data availability**. Every user is mapped immediately (at least to their country level), while API usage is minimized strictly to new, unique locations.

#### Geocoding Workflow Diagram
![Smart Geocoding Workflow Diagram](../assets/diagrams/geocoding-flowchart.svg)

## 🔗 Key code references

### Backend (API)
- 🧠 **Geocoding service (core flow):**  
  - [`GeocodingService.cs`](../backend/src/AlumniApi/Services/Geocoding/Geocoding.cs)
  - Interface: [`IGeocodingService.cs`](../backend/src/AlumniApi/Services/Geocoding/IGeocoding.cs)

- 🧩 **Normalization / cache-key:**  
  - [`StringHelper.cs`](../backend/src/AlumniApi/Helpers/StringHelper.cs)

- 🧾 **Endpoint that triggers geocoding:**  
  - `POST /api/membership/apply` → [`MembershipController.cs`](../backend/src/AlumniApi/Controllers/MembershipController.cs)

- 🌍 **Map data endpoint:**  
  - `GET /api/membership/map` → [`MembershipController.cs`](../backend/src/AlumniApi/Controllers/MembershipController.cs)

- 🗃️ **Data models / caching table:**  
  - [`GeoCache model`](../backend/src/AlumniApi/Models/GeoCache.cs)  
  - (optional) DbContext: [`AlumniContext`](../backend/src/AlumniApi/Models/AlumniContext.cs)

- ⚙️ **HttpClient configuration:**  
  - [`Program.cs`](../backend/src/AlumniApi/Program.cs)
 
  -  ⚙️ **Unit test Geocoding:**  
  - [`GeocodingTests.cs`](../backend/tests/GeocodingTests.cs)

### Frontend (World Map)
- 🗺️ **World map component:**  
  - [`WorldMap` / `MapPage`](../frontend/src/components/WorldMap/WorldMap.jsx)



### 💡 Why this approach? (Project Constraints & Quality Assurance)
This architecture was specifically chosen to meet two critical client requirements:
1.  **Zero-Cost Operation:** The client required a sustainable system without recurring monthly costs (e.g., Google Maps API billing). Using Nominatim (OpenStreetMap) solved this but required strict rate-limiting and caching.
2.  **100% Data Integrity:** While automation handles 95% of cases, the "Admin Review" feature ensures that no user is ever lost or mapped incorrectly due to API limitations. This **Human-in-the-Loop** approach guarantees a pristine alumni directory.

## 📌 Notes
- External APIs are protected from abuse via caching and fallback rules.
- Failed/uncertain locations are never silently accepted—they’re tracked for admin verification.
