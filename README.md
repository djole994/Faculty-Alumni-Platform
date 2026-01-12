# 🎓 Faculty Alumni Platform (Showcase)

> **Public Showcase / Subset**
>
> This repository contains a curated public subset of the project (selected modules, docs, and examples).
> The full production codebase is **private**.
>
> ✅ No secrets (keys, connection strings, tokens) are included.  
> ✅ No institution-specific data, naming, or internal infrastructure details are included.  
> ✅ UI/branding and production deployment configuration are intentionally omitted.

---

## What’s included in this public showcase

- Backend: selected API modules (DTOs, services, helpers) related to geocoding & validation  
- Frontend: map component (Leaflet) + example integration  
- Docs: architecture notes and flow diagrams  
- Tests: minimal test coverage for showcased modules  

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
- 💳 View financial reports and subscription statuses (Administrative Dashboard) — **financial module**  
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
- **Leaflet / React-Leaflet** - map rendering

### Integrations
- **Nominatim (OpenStreetMap)** - location geocoding  

---

## 🚀 Key Challenges & Solutions

<details>
<summary><strong>1. Intelligent Geocoding & Fallback-First Caching</strong></summary>

<br/>

One of the primary engineering goals was to map users worldwide accurately without overloading external APIs, while still handling typos and imperfect data entry gracefully.

#### Geocoding Workflow Diagram
![Smart Geocoding Workflow Diagram](assets/diagrams/geocoding-flowchart.svg)

### 💻 Implementation shortcuts

- 🧠 **Geocoding Core Logic**  
  [`GeocodingService.ResolveLocationAsync`](backend/src/AlumniApi/Services/Geocoding/Geocoding.cs)  
  *The heart of the system: handles local cache checks, API requests, and fallback saving.*

- 🧩 **Cache Key Generator**  
  [`StringHelper.GenerateSearchKey`](backend/src/AlumniApi/Helpers/StringHelper.cs)  
  *Normalizes city and country names to ensure cache hits.*

- 🧭 **API Controllers**  
  [`MembershipController.SubmitApplication`](backend/src/AlumniApi/Controllers/MembershipController.cs) &  
  [`GetMap`](backend/src/AlumniApi/Controllers/MembershipController.cs)  
  *Endpoints for processing applications and serving map data.*

- ⚙️ **Service Configuration**  
  [`Program.cs`](backend/src/AlumniApi/Program.cs)  
  *Dependency Injection and HttpClient setup.*

➡️ Details: [`docs/geocoding.md`](docs/geocoding.md)

</details>

---

<details>
<summary><strong>2. Async Email Delivery (Outbox Pattern + Background Worker)</strong></summary>

<br/>

To prevent slow or unreliable SMTP servers from blocking API requests, email delivery is fully decoupled from the request lifecycle using a DB-backed outbox and a background worker.

#### Email Outbox Workflow Diagram
![Email Outbox Workflow Diagram](assets/diagrams/email-outbox-flowchart.svg)

### 💻 Implementation shortcuts

- 📩 **Outbox Insert (API layer)**  
  [`MembershipController.SubmitApplication`](backend/src/AlumniApi/Controllers/MembershipController.cs)  
  *Creates the email message and inserts it into the EmailOutbox table (no direct SMTP call).*

- 🔁 **Background Worker**  
  [`EmailOutboxWorker`](backend/src/AlumniApi/Services/Email/EmailOutboxWorker.cs)  
  *Periodically polls pending outbox records and sends emails asynchronously.*

- 🔄 **Retry & Backoff Logic**  
  [`EmailOutboxWorker.GetNextDelay`](backend/src/AlumniApi/Services/Email/EmailOutboxWorker.cs)  
  *Handles retry scheduling and marks emails as Failed after max attempts.*

- ✉️ **SMTP Abstraction**  
  [`IEmailSender`](backend/src/AlumniApi/Services/Email/IEmailSender.cs)  
  *Encapsulates SMTP transport and keeps API and worker decoupled from the provider.*

- ⚙️ **Service Configuration**  
  [`Program.cs`](backend/src/AlumniApi/Program.cs)  
  *Registers the background worker and SMTP sender via Dependency Injection.*

➡️ Details: [`docs/email-outbox.md`](docs/email-outbox.md)

</details>

