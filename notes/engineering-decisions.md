# 🧠 Engineering Decisions

This document explains the **key architectural and production decisions**  
behind the Alumni Platform.  
The focus is not only on *how* the system works, but *why* specific  
engineering trade-offs were made.

---

# 📍 Geocoding Without External Cost

The system was designed to operate **without recurring API expenses**  
while still providing reliable global location mapping.

### Key decisions

- A **local GeoCache table** is always checked first to avoid unnecessary external calls.
- If a city is not found, the system **immediately falls back to country-level coordinates**, ensuring:
  - **100% map availability**
  - **no user profile without a location**
- Country coordinates are **pre-seeded in the database** (~200 countries).
- External geocoding (Nominatim) runs **only when necessary**.
- Unverified results are stored with `IsVerified = false` and later fixed via  
  **admin review and re-geocoding**.

### Result

A **zero-cost**, resilient geolocation system with guaranteed data availability.

---

# 🛡 Public Application Security Model

The membership application endpoint is **publicly accessible**,  
which required a **defense-in-depth** approach rather than relying  
on a single protection layer.

### Security layers

1. **Nginx rate limiting & request filtering**  
   Prevents automated flooding and repeated submissions from the same IP.

2. **Honeypot hidden field**  
   Bots filling the invisible `Website` field are **silently ignored**  
   to avoid feedback loops.

3. **CAPTCHA verification**  
   Blocks automated submissions **before any expensive processing** occurs.

4. **Client & server validation**  
   Improves UX and ensures correct structure, but  
   **is not trusted as the final guarantee**.

5. **Database constraints (final safety net)**  
   - UNIQUE normalized email  
   - CHECK rules for dates and coordinates  
   Guarantees integrity even if validation is bypassed.

### Result

The endpoint remains **public and usable**,  
while personal data stays **protected and consistent**.

---

# 🔐 Least-Privilege Database Access

To reduce the impact of potential abuse or vulnerabilities:

- The application DB user has **INSERT-only permission** on `AlumniProfiles`.
- Direct **SELECT access to personal data is restricted**.
- Public reads are exposed only through **sanitized views or ID-based queries**.
- **UPDATE and DELETE are denied** for the application user.

### Result

Even in case of compromise, **data exposure and damage are minimized**.

---

# ✉️ Asynchronous Email Delivery (Outbox Pattern)

Sending emails directly during HTTP requests can:

- slow down responses  
- fail unpredictably due to SMTP or network issues  

### Decision

- Emails are stored in a **database outbox table** during the request.
- A **background worker** processes and sends them asynchronously.
- Retry and failure handling are **centralized and controlled**.

### Result

Fast API responses and **reliable email delivery independent  
of the request lifecycle**.

---

# 🏗 Production-First Deployment Philosophy

The system is designed with **production clarity and control** in mind:

- **Linux VPS + Nginx reverse proxy**
- **ASP.NET Core running as a systemd service**
- **Environment-based secrets (never stored in the repository)**
- **VPN-restricted administrative access**

### Result

Predictable, secure deployment **without hidden platform magic**.

---

# 🧩 Core Engineering Principles

Across the entire system, several guiding principles were followed:

- **Validation improves UX. Constraints guarantee integrity.**
- **Public endpoints must assume hostile traffic.**
- **Costs must scale to zero when possible.**
- **Production architecture should be explicit and understandable.**
- **Security is achieved through layers, not a single mechanism.**

---

# 📌 Summary

The Alumni Platform is not only a functional application,  
but a **production-oriented engineering system** designed for:

- **resilience**
- **data integrity**
- **security**
- **cost sustainability**
- **operational clarity**

These decisions reflect a mindset focused on **real-world deployment**,  
not just local development.
