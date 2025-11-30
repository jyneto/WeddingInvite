# WeddingInvite.Api

A .NET 9 Web API for managing wedding invitations, guests, tables, and bookings.

## Features

- **Guest Management:** Add, update, delete, and list guests.
- **Table Management:** Add, update, delete, and list tables.
- **Booking System:** Book tables for guests with party size and time validation.
- **Admin Authorization:** All endpoints are protected and require admin role.
- **DTO-based API:** Uses DTOs for clean data transfer and validation.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server or SQLite (update connection string as needed)
- Visual Studio 2022+ (recommended)

### Setup

1. **Clone the repository:**
	git clone https://github.com/jyneto/WeddingInvite.git cd WeddingInvite.Api
2. **Restore dependencies:**
	dotnet restore
3. **Configure your database connection** in `appsettings.json`.

4. **Apply migrations and update the database:**
	dotnet ef database update
5. **Run the API:**
	dotnet run

# WeddingInvite.Api — Migration Notes & Rationale

This repository was transitioned from a simple table booking sample into a focused wedding invitation management API. The changes preserve the original assignment goals (CRUD, validation, booking rules) while adding RSVP flows, authentication, and stronger validation so the app meets the assignment requirements and real-world edge cases.

---

## High level summary of changes

- New domain focus: wedding invitation management (guests, tables, bookings, optional menu).
- Authentication: JWT-based auth added with `AuthController` (login / token issuing).
- Controllers:
  - `AdminGuestController` — admin CRUD for guests
  - `GuestController` — public RSVP endpoint and guest lookup
  - `BookingController` — booking creation / query / update / delete (booking logic preserved)
  - `TableController`, `MenuController` — manage tables and menu items
- Services & Repositories refactor:
  - Split into service and repository layers per entity (`Guest`, `Table`, `Booking`, `Menu`, `Auth`)
  - Added repository methods used by service business rules: email uniqueness, booking overlap checks, used seats, table usage checks
- DTOs and validation:
  - Stronger validation attributes in DTOs (e.g., `BookingCreateDTO`, `GuestCreateDTO`)
  - `GuestCreateDTO` implements `IValidatableObject` to require `TableId` when `IsAttending=true`
- Booking logic preserved and improved:
  - Booking overlap detection (`BookingOverlapAsync`)
  - Seat accounting (`UsedSeatsAsync`) and remaining-seat checks before creating/updating bookings
  - `GetAvailableTablesAsync` computes availability per time slot
- DB / Models:
  - `WeddingDbContext` (renamed/updated) with relationships between `Guest` and `Table`
  - Models updated to include FK navigation and required fields
- Fix for the reported issue:
  - Guest creation now validates table existence and capacity before saving when `IsAttending = true`. This prevents guests from being persisted when a requested table is unavailable or booking validation fails.

---

## Why this still satisfies the original assignment criteria

The assignment asked for a system that supports:
- CRUD operations for entities — implemented via Controllers + Services + Repositories for Guests, Tables, Bookings, Menu.
- Validation and business rules — DTO validation and service-layer checks enforce required rules:
  - Table capacity checks (guest & booking)
  - Booking overlap prevention
  - Email uniqueness for guests
- Data persistence — implemented with EF Core; repository layer abstracts DB access; migrations included.
- Web API surface — RESTful endpoints provided for each main entity.
- Authentication & authorization — admin-restricted endpoints and public RSVP endpoint implemented.

All core functional requirements remain implemented, and additional validation and authentication strengthen correctness and security without violating the assignment scope.

---

## Notable implementation details (short)

- Booking rules:
  - Slot duration is read from `EventPolicy` and applied consistently.
  - Times normalized to UTC before validation.
  - Overlap check: bookings overlap if `existing.Start < newEnd && newStart < existing.End`.
  - Seat accounting uses a SUM of overlapping bookings to decide remaining seats.

- Guest/RSVP behavior:
  - `GuestCreateDTO` requires `TableId` if `IsAttending` is true (validated on the DTO and again in service).
  - `GuestService` now checks `ITableRespiratory.GetByIdAsync(...)` and `IGuestRepository.CountByTableAsync(...)` before persisting an attending guest to prevent saving when the table is full.
  - If you require atomic creation of guest + booking in a single request that rolls back both on failure, implement a single endpoint and wrap operations in a DB transaction.

- Dependency Injection:
  - `GuestService` constructor now depends on `ITableRespiratory` in addition to `IGuestRepository`. Update DI registration in `Program.cs` accordingly.

---

## Run / Setup (quick)

1. Restore dependencies:
   - `dotnet restore`
2. Apply EF migrations / update DB:
   - __dotnet ef database update__
3. Run:
   - `dotnet run` or use Visual Studio -> __Debug > Start Debugging__
4. Use Swagger (if enabled) or call endpoints directly.

Files to inspect for startup & DI:
- `Program.cs` — DI, authentication, swagger, CORS
- `appsettings.json` — JWT settings, connection string
- `WeddingDbContext` — DbSets and relationships

---

## Endpoints (examples)
- `POST /api/guest` — public RSVP (validates table if `IsAttending = true`)
- `GET /api/admin/guest` — admin: list all guests
- `POST /api/bookings` — create booking (admin)
- `POST /api/bookings/available` — check table availability for a time slot
- `POST /api/auth/login` — obtain JWT token

(See controllers for exact routes and required DTOs.)

---

## Known limitations & recommendations
- Improve error messages if you want more user-friendly responses in a UI.


