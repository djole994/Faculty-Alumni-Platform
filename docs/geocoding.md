
### Intelligent Geocoding & Hybrid Caching

One of the primary engineering goals was to map users worldwide accurately without overloading external APIs, while still handling typos and imperfect data entry gracefully.

To solve this, I implemented a **Hybrid Caching Strategy** with smart fallback logic:

1. **Check the local `GeoCache`** first (Layer 1) to prevent redundant API calls.
2. If missing, request coordinates from **Nominatim API** (Layer 2).
3. If the API can’t resolve the city (e.g., due to a typo), the system:
   - Automatically falls back to **Country center coordinates**.
   - Flags the profile for **manual admin verification**.

> **Impact:** This approach reduced external API calls by approx. 80% during testing and prevents the "empty map" issue for users with misspelled cities.

#### Geocoding Workflow Diagram
![Smart Geocoding Workflow Diagram](assets/diagrams/geocoding-flowchart.svg)

### 💡 Why this approach? (Project Constraints & Quality Assurance)
This architecture was specifically chosen to meet two critical client requirements:
1.  **Zero-Cost Operation:** The client required a sustainable system without recurring monthly costs (e.g., Google Maps API billing). Using Nominatim (OpenStreetMap) solved this but required strict rate-limiting and caching.
2.  **100% Data Integrity:** While automation handles 95% of cases, the "Admin Review" feature ensures that no user is ever lost or mapped incorrectly due to API limitations. This **Human-in-the-Loop** approach guarantees a pristine alumni directory.

## 📌 Notes
- External APIs are protected from abuse via caching and fallback rules.
- Failed/uncertain locations are never silently accepted—they’re tracked for admin verification.
