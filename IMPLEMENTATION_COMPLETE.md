# 🎉 Laundry Booking REST API - COMPLETE

Your complete REST API for a laundry booking system has been successfully created!

## 📦 What's Been Created

### Core Application Files
- ✅ Models: `User.cs`, `BookedTimeSlot.cs`
- ✅ Database Context: `LaundryDbContext.cs` with PostgreSQL support
- ✅ Services: `AuthService.cs`, `BookingService.cs` with full business logic
- ✅ Controllers: `AuthController.cs`, `BookingsController.cs`
- ✅ DTOs: Request and response objects for all endpoints
- ✅ Database Migrations: Automatic schema creation
- ✅ Configuration: `appsettings.json`, `appsettings.Development.json`

### Documentation
- 📖 **README.md** - Complete API documentation with examples
- 🚀 **QUICKSTART.md** - Get running in 5 minutes
- 📝 **SETUP_AND_TESTING.md** - Detailed setup, configuration, and testing guide
- 🐳 **DOCKER_SETUP.md** - PostgreSQL with Docker instructions
- 📋 **PROJECT_SUMMARY.md** - Complete project overview
- 🧪 **test-api.http** - REST Client test file for VS Code

### Infrastructure
- 🐳 **docker-compose.yml** - PostgreSQL + pgAdmin (optional)
- 📄 **.gitignore** - Git configuration

## 🚀 Quick Start (5 Minutes)

### 1. Start PostgreSQL
```bash
docker-compose up -d
```

Or use your local PostgreSQL installation.

### 2. Run the API
```bash
cd laundry-booking-backend/laundry-booking-backend
dotnet run
```

API available at: **https://localhost:5001**

### 3. Test It
```bash
# Register
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","pin":"1234"}' \
  --insecure

# Book a slot (replace TOKEN with the one from register)
curl -X POST https://localhost:5001/api/bookings/book \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{"bookingDate":"2026-01-20","timeSlotNumber":1}' \
  --insecure

# Check available slots
curl -X GET "https://localhost:5001/api/bookings/available/2026-01-20" \
  --insecure
```

## 🎯 API Features

### Authentication Endpoints
```
POST   /api/auth/register      - Register new user (email + 4-digit PIN)
POST   /api/auth/login         - Login and get JWT token
```

### Booking Endpoints (Authenticated)
```
POST   /api/bookings/book      - Book a time slot
GET    /api/bookings/my-bookings  - Get your bookings
DELETE /api/bookings/unbook/{id}  - Cancel a booking
GET    /api/bookings/available/{date} - Check available slots (public)
```

## 🔐 Security

- ✅ 4-digit PIN validation
- ✅ SHA256 PIN hashing
- ✅ JWT authentication (24-hour expiration)
- ✅ Role-based authorization
- ✅ Input validation
- ✅ Database constraints
- ✅ HTTPS enforced

## 💾 Database

- ✅ PostgreSQL backend
- ✅ Entity Framework Core ORM
- ✅ Automatic migrations on startup
- ✅ Unique constraints to prevent double-booking
- ✅ Proper indexes for performance

## 📊 Database Schema

**Users Table:**
- Id, Email (unique), PasswordPin (hashed), CreatedAt, UpdatedAt

**BookedTimeSlots Table:**
- Id, UserId (FK), BookingDate, TimeSlotNumber (1-3)
- Unique constraint: (BookingDate, TimeSlotNumber) - prevents double-booking

## 🔧 Configuration

### Update Connection String
Edit `appsettings.Development.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=laundry_booking_dev;Username=postgres;Password=postgres"
}
```

### Update JWT Secret (Production)
Edit `appsettings.json`:
```json
"JwtSettings": {
  "Secret": "your-strong-random-key-minimum-256-bits",
  "Issuer": "laundry-booking-api",
  "Audience": "laundry-booking-client"
}
```

## 📚 Documentation

| File                 | Purpose                                       |
| -------------------- | --------------------------------------------- |
| README.md            | Complete API documentation with cURL examples |
| QUICKSTART.md        | 5-minute setup guide                          |
| SETUP_AND_TESTING.md | Detailed setup, troubleshooting, and testing  |
| DOCKER_SETUP.md      | PostgreSQL Docker setup instructions          |
| PROJECT_SUMMARY.md   | Project overview and architecture             |
| test-api.http        | REST Client tests for VS Code                 |

## 🧪 Testing

### Using VS Code REST Client
1. Install "REST Client" extension
2. Open `test-api.http`
3. Click "Send Request" on each endpoint

### Using cURL
See documentation files for detailed cURL examples

### Using Postman
Import from: `https://localhost:5001/swagger/v1/swagger.json`

## 🌟 Key Features

✅ **3 Time Slots Per Day** - Only slots 1, 2, 3 available
✅ **Email + PIN Login** - Secure authentication
✅ **Only Store Bookings** - Not empty slots (efficient!)
✅ **Unbook Support** - Users can cancel bookings
✅ **Availability Check** - See what's booked for any date
✅ **JWT Tokens** - 24-hour expiration for security
✅ **Automatic Migrations** - Database setup on startup

## ✨ Next Steps

1. **Start PostgreSQL**: `docker-compose up -d`
2. **Configure Connection**: Update `appsettings.Development.json`
3. **Run Application**: `dotnet run`
4. **Test Endpoints**: Use provided test file or cURL
5. **Read Documentation**: See README.md for complete API reference

## 📞 Troubleshooting

**Can't connect to database?**
- Ensure PostgreSQL is running
- Check connection string in appsettings
- For Docker: `docker-compose up -d`

**Getting JWT error?**
- Verify JWT settings in appsettings.json
- Ensure secret key is configured

**PIN validation failed?**
- PIN must be exactly 4 digits (e.g., "1234")

**Time slot already booked?**
- Check available slots with GET /api/bookings/available/{date}
- Each slot can only be booked once per day

## 🎊 You're All Set!

The API is ready for:
- ✅ Development
- ✅ Testing
- ✅ Deployment to production (with proper configuration)

Start with QUICKSTART.md or README.md to begin using the API!

---

**Technology Stack**: C# | .NET Core 10 | ASP.NET Core | Entity Framework Core | PostgreSQL | JWT

**Status**: ✅ Complete and Ready to Use
