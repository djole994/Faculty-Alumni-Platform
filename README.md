# 🎓 Medical Faculty Alumni Platform

A centralized digital platform connecting students and graduates of the Medical Faculty.

This project is currently under active development as a modern solution for the Alumni community, aimed at enabling former students to stay connected, track scientific events, and share professional opportunities.

---

## 📖 About the Project

The Alumni Platform transforms how the Medical Faculty interacts with its graduates.  
Replacing outdated lists and manual workflows, the application provides a modern, interactive experience where users can:

- ✅ Create professional profiles and become **verified members** of the organization  
- 🌍 Visualize global Alumni presence via an **interactive world map**  
- 🗓️ Register online for **congresses and educational events**  
- 💼 Access an exclusive **job board** and read inspiring **Alumni Stories**  
- 💳 View financial reports and subscription statuses (Administrative Dashboard). **financial module**  
- ⚙️ Benefit from automation (**geolocation, verification**) and scalability (**data caching**)  

---

## 🛠 Tech Stack

### Backend
- **.NET 8 (ASP.NET Core Web API)** - service-oriented architecture  
- **Entity Framework Core** - Code-First approach  
- **SQL Server** - relational database  
- **Dependency Injection** + **Repository Pattern**

### Frontend
- **React** - SPA (Single Page Application)  
- **Axios** - HTTP client  
- **Leaflet / React-Leaflet** - map rendering

### Integrations
- **Nominatim (OpenStreetMap)** - location geocoding  

---

## 🚀 Key Challenge: Intelligent Geocoding System

A notable technical detail of this project is the custom geocoding system designed to map users worldwide without unnecessary API calls, while handling typos gracefully.

To solve this, I implemented a **Hybrid Caching Strategy** with fallback logic:

1. **Check the local `GeoCache`** first  
2. If missing, request coordinates from **Nominatim API**  
3. If the API can’t resolve the city (e.g., typo), the system:
   - falls back to **Country coordinates**
   - flags the profile for **manual admin review**

### Geocoding Workflow Diagram
> *(Add diagram image here)*  
> Example:
> `docs/images/geocoding-workflow.png`

---

## 📌 Notes
- External APIs are protected from abuse via caching and fallback rules.
- Failed/uncertain locations are never silently accepted—they’re tracked for admin verification.

