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

## API Endpoints

- `api/admin/guest` - Manage guests (GET, POST, PUT, DELETE)
- `api/table` - Manage tables (GET, POST, PUT, DELETE)
- `api/booking` - Manage bookings (GET, POST, PUT, DELETE)

All endpoints require authentication and the `Admin` role.

## Improvements Needed

- Add comprehensive unit and integration tests.
- Improve error handling and validation messages.
- Add Swagger/OpenAPI documentation.
- Implement logging and monitoring.
- Refactor for better separation of concerns if needed.
- Add user management and roles beyond admin.
- Seed initial data for development/testing.
- Consider using SecuritySchemeType.APIKey or OAuth2 for better security.
- Refresh token for better security.

## Known Issues

- Some DTOs may lack required properties for certain operations.
- Table and guest assignment logic may need refinement for edge cases.

## Contributing

Pull requests are welcome! Please open issues for bugs or feature requests.

## License

MIT License

---

*For questions or suggestions, please open an issue on GitHub.*
