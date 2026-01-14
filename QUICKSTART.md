# Quick Start Guide

Get the Laundry Booking API running in 5 minutes!

## 1. Start PostgreSQL (if not already running)

**Using Docker** (recommended):
```bash
docker run --name laundry-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=laundry_booking \
  -p 5432:5432 \
  -d postgres:latest
```

**Or using installed PostgreSQL:**
- Just ensure PostgreSQL service is running

## 2. Clone/Open the Project
```bash
cd laundry-booking-backend/laundry-booking-backend
```

## 3. Verify Configuration

Check `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=laundry_booking_dev;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "dev-secret-key-change-in-production-with-strong-random-key",
    "Issuer": "laundry-booking-api",
    "Audience": "laundry-booking-client"
  }
}
```

## 4. Run the Application
```bash
dotnet run
```

API is now available at: **https://localhost:5001**

## 5. Test the API

### Register a user:
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","pin":"1234"}' \
  --insecure
```

Save the `token` from the response.

### Book a slot:
```bash
curl -X POST https://localhost:5001/api/bookings/book \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <YOUR_TOKEN>" \
  -d '{"bookingDate":"2026-01-20","timeSlotNumber":1}' \
  --insecure
```

### Check available slots:
```bash
curl -X GET "https://localhost:5001/api/bookings/available/2026-01-20" \
  --insecure
```

## Done! 🎉

For detailed documentation, see [README.md](README.md)

For complete setup and testing guide, see [SETUP_AND_TESTING.md](SETUP_AND_TESTING.md)
