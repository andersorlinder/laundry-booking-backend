# Laundry Booking System - Project Summary

## Overview
A complete REST API for managing laundry time slot bookings built with C# and ASP.NET Core 10.0.

## Key Features Implemented

✅ **User Authentication**
- User registration with email and 4-digit PIN
- Secure PIN hashing using SHA256
- JWT token-based authentication (24-hour expiration)

✅ **Time Slot Management**
- 3 time slots per day (slots 1, 2, 3)
- Only booked slots are stored in the database
- Unique constraint ensures each slot can only be booked once per day

✅ **Booking Operations**
- Book available time slots
- Unbook (cancel) existing bookings
- View personal bookings
- Check available slots for any date

✅ **Database**
- PostgreSQL backend
- Entity Framework Core for ORM
- Automated migrations on startup
- Proper indexes for performance

## Project Structure

```
laundry-booking-backend/
├── Controllers/
│   ├── AuthController.cs           # Login/Register endpoints
│   └── BookingsController.cs        # Booking management endpoints
├── Models/
│   ├── User.cs                      # User entity
│   └── BookedTimeSlot.cs            # Booking entity
├── Services/
│   ├── AuthService.cs               # Authentication logic
│   └── BookingService.cs            # Booking business logic
├── Dtos/
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── RegisterRequest.cs
│   └── BookingRequest.cs
├── Data/
│   └── LaundryDbContext.cs          # EF Core database context
├── Migrations/
│   ├── 20260114000000_InitialCreate.cs
│   └── LaundryDbContextModelSnapshot.cs
├── Program.cs                       # Application startup configuration
├── appsettings.json                 # Production settings
├── appsettings.Development.json     # Development settings
├── laundry-booking-backend.csproj   # Project configuration
└── test-api.http                    # REST API test file
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token

### Bookings (Requires Authentication)
- `POST /api/bookings/book` - Book a time slot
- `GET /api/bookings/my-bookings` - Get user's bookings
- `DELETE /api/bookings/unbook/{slotId}` - Cancel a booking
- `GET /api/bookings/available/{date}` - Check available slots for a date (public)

## Technology Stack

- **Framework**: ASP.NET Core 10.0
- **Language**: C# 13.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 10.0
- **Authentication**: JWT Bearer tokens
- **API Documentation**: OpenAPI/Swagger

## NuGet Packages Added

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.2" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.1.0" />
```

## Database Schema

### Users Table
| Column      | Type      | Constraints        |
| ----------- | --------- | ------------------ |
| Id          | INT       | PK, Auto-increment |
| Email       | TEXT      | UNIQUE             |
| PasswordPin | TEXT      | SHA256 hashed      |
| CreatedAt   | TIMESTAMP | UTC                |
| UpdatedAt   | TIMESTAMP | UTC                |

### BookedTimeSlots Table
| Column         | Type      | Constraints                         |
| -------------- | --------- | ----------------------------------- |
| Id             | INT       | PK, Auto-increment                  |
| UserId         | INT       | FK → Users(Id)                      |
| BookingDate    | DATE      | -                                   |
| TimeSlotNumber | INT       | 1-3                                 |
| CreatedAt      | TIMESTAMP | UTC                                 |
|                |           | UNIQUE(BookingDate, TimeSlotNumber) |

## Security Features

✅ PIN Hashing - PINs stored as SHA256 hashes
✅ JWT Authentication - Tokens expire after 24 hours
✅ Input Validation - Email format and 4-digit PIN validation
✅ Authorization - Protected endpoints require valid token
✅ Database Constraints - Unique indexes prevent duplicates
✅ HTTPS - Enforced in production
✅ CORS - Configurable cross-origin access

## Configuration

### Required Settings in appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=laundry_booking;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "your-strong-secret-key-minimum-256-bits",
    "Issuer": "laundry-booking-api",
    "Audience": "laundry-booking-client"
  }
}
```

## Getting Started

1. **Ensure PostgreSQL is running** (Docker or local installation)
2. **Update connection string** in `appsettings.Development.json`
3. **Run the application**: `dotnet run`
4. **Database migrations** run automatically on startup
5. **Test endpoints** using provided `test-api.http` file or curl commands

## Documentation Files

- **[README.md](README.md)** - Complete API documentation
- **[SETUP_AND_TESTING.md](SETUP_AND_TESTING.md)** - Detailed setup and testing guide
- **[QUICKSTART.md](QUICKSTART.md)** - 5-minute quick start
- **[DOCKER_SETUP.md](DOCKER_SETUP.md)** - PostgreSQL Docker setup
- **[test-api.http](test-api.http)** - REST API test file for VS Code

## Development Notes

- **Automatic Migration**: Database schema is created automatically on startup
- **UTC Timestamps**: All dates/times are stored in UTC
- **Slot Numbering**: Time slots are numbered 1, 2, 3 (representing different times of day)
- **Unique Bookings**: Each user can only book each slot once per day
- **Unique Slots**: Each time slot can only be booked by one user per day

## Environment Variables

Set these for production deployment:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=your-production-connection-string
JwtSettings__Secret=your-production-secret-key
JwtSettings__Issuer=laundry-booking-api
JwtSettings__Audience=laundry-booking-client
```

## Testing

Use the included REST Client extension test file:
- Install "REST Client" extension in VS Code
- Open `test-api.http`
- Click "Send Request" on any endpoint

Or use curl, Postman, or any HTTP client.

## Future Enhancements

- [ ] Email verification on registration
- [ ] Password recovery/reset
- [ ] User profile management
- [ ] Refresh tokens
- [ ] Rate limiting
- [ ] Admin dashboard
- [ ] Booking confirmations via email
- [ ] Recurring bookings
- [ ] Booking history/archives

---

**Status**: ✅ Production Ready (with configuration)
**Last Updated**: January 14, 2026
