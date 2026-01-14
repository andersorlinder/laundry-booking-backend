# Laundry Booking API - Setup and Testing Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Database Setup](#database-setup)
3. [Configuration](#configuration)
4. [Running the Application](#running-the-application)
5. [API Testing](#api-testing)
6. [Troubleshooting](#troubleshooting)

## Prerequisites

- .NET 10.0 SDK or higher
- PostgreSQL 12+ or Docker
- Visual Studio Code, Visual Studio, or another C# IDE
- curl, Postman, or another API testing tool (optional)

## Database Setup

### Option 1: PostgreSQL Installed Locally

1. Start PostgreSQL service on your system
2. Create database and user (if not using default):
   ```sql
   CREATE DATABASE laundry_booking;
   CREATE USER laundry_user WITH PASSWORD 'secure_password';
   GRANT ALL PRIVILEGES ON DATABASE laundry_booking TO laundry_user;
   ```

3. Update `appsettings.Development.json` with your credentials:
   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=laundry_booking;Username=laundry_user;Password=secure_password"
   ```

### Option 2: Docker (Recommended for Development)

1. Install Docker and Docker Compose
2. Navigate to project root directory
3. Create `docker-compose.yml`:
   ```yaml
   version: '3.8'
   services:
     postgres:
       image: postgres:latest
       container_name: laundry-postgres
       environment:
         POSTGRES_USER: postgres
         POSTGRES_PASSWORD: postgres
         POSTGRES_DB: laundry_booking
       ports:
         - "5432:5432"
       volumes:
         - postgres_data:/var/lib/postgresql/data

   volumes:
     postgres_data:
   ```

4. Start PostgreSQL:
   ```bash
   docker-compose up -d
   ```

5. Verify connection:
   ```bash
   docker exec -it laundry-postgres psql -U postgres -d laundry_booking -c "SELECT 1;"
   ```

## Configuration

### 1. JWT Settings

Edit `appsettings.Development.json`:

**For Development** (already configured):
```json
"JwtSettings": {
  "Secret": "dev-secret-key-change-in-production-with-strong-random-key",
  "Issuer": "laundry-booking-api",
  "Audience": "laundry-booking-client"
}
```

**For Production** (in `appsettings.json`):
Generate a strong random key (use PowerShell or online tool):
```powershell
# PowerShell
$bytes = New-Object System.Byte[] 32
$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::new()
$rng.GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

### 2. Connection String

Ensure the connection string in your `appsettings.Development.json` matches your PostgreSQL setup:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=laundry_booking_dev;Username=postgres;Password=postgres"
}
```

## Running the Application

### Method 1: Using dotnet CLI

```bash
cd laundry-booking-backend/laundry-booking-backend
dotnet restore
dotnet build
dotnet run
```

The API will start at: `https://localhost:5001`

### Method 2: Using Visual Studio

1. Open the `.sln` file
2. Press `F5` or select Debug → Start Debugging
3. The application launches in your default browser

### Method 3: Using Visual Studio Code

1. Install the C# extension
2. Open the folder in VS Code
3. Press `F5` or use the Run menu
4. Select ".NET Core" as the environment

## API Testing

### Using the Test File

The project includes `test-api.http` for testing with the REST Client extension:

1. Install "REST Client" extension in VS Code
2. Open `test-api.http`
3. Click "Send Request" above each endpoint

### Using cURL

#### 1. Register a New User
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "pin": "1234"
  }' \
  --insecure
```

**Expected Response** (201):
```json
{
  "userId": 1,
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### 2. Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "pin": "1234"
  }' \
  --insecure
```

Save the returned `token` for the next requests.

#### 3. Book a Time Slot
Replace `YOUR_TOKEN` with the token from login:

```bash
curl -X POST https://localhost:5001/api/bookings/book \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "bookingDate": "2026-01-20",
    "timeSlotNumber": 1
  }' \
  --insecure
```

#### 4. Check Available Slots
```bash
curl -X GET "https://localhost:5001/api/bookings/available/2026-01-20" \
  --insecure
```

**Response**:
```json
{
  "date": "2026-01-20T00:00:00",
  "availableSlots": [2, 3],
  "bookedSlots": [
    {
      "id": 1,
      "bookingDate": "2026-01-20T00:00:00",
      "timeSlotNumber": 1,
      "createdAt": "2026-01-14T10:30:00Z"
    }
  ]
}
```

#### 5. Get Your Bookings
```bash
curl -X GET https://localhost:5001/api/bookings/my-bookings \
  -H "Authorization: Bearer YOUR_TOKEN" \
  --insecure
```

#### 6. Unbook a Slot
```bash
curl -X DELETE https://localhost:5001/api/bookings/unbook/1 \
  -H "Authorization: Bearer YOUR_TOKEN" \
  --insecure
```

### Using Postman

1. Import the API endpoints using the URL: `https://localhost:5001/swagger/v1/swagger.json`
2. Or manually create requests:
   - **Register**: POST to `https://localhost:5001/api/auth/register`
   - **Login**: POST to `https://localhost:5001/api/auth/login`
   - **Book**: POST to `https://localhost:5001/api/bookings/book` (add Bearer token)
   - **Get Bookings**: GET to `https://localhost:5001/api/bookings/my-bookings` (add Bearer token)
   - **Available**: GET to `https://localhost:5001/api/bookings/available/{date}`
   - **Unbook**: DELETE to `https://localhost:5001/api/bookings/unbook/{slotId}` (add Bearer token)

## Complete Test Workflow

1. **Register** - Get a token
2. **Check Available Slots** - See what's available for a date
3. **Book Slot 1** - Book the first time slot
4. **Check Available Slots** - Verify slot 1 is now booked
5. **Book Slot 2** - Book the second time slot
6. **Get My Bookings** - List all your bookings
7. **Unbook Slot 1** - Cancel the first booking
8. **Get My Bookings** - Verify the cancellation
9. **Check Available Slots** - Verify slot 1 is available again

## Common Errors and Troubleshooting

### Error: "Unable to connect to the database"
- **Cause**: PostgreSQL is not running or connection string is incorrect
- **Solution**:
  - Verify PostgreSQL is running: `psql --version`
  - Check connection string in `appsettings.json`
  - If using Docker, ensure container is running: `docker ps`

### Error: "JWT settings are not configured"
- **Cause**: Missing JWT configuration in appsettings.json
- **Solution**: Add JWT settings to `appsettings.json`:
  ```json
  "JwtSettings": {
    "Secret": "your-secret-key",
    "Issuer": "laundry-booking-api",
    "Audience": "laundry-booking-client"
  }
  ```

### Error: "PIN must be exactly 4 digits"
- **Cause**: PIN provided is not exactly 4 numeric digits
- **Solution**: Ensure PIN is exactly 4 digits (e.g., "1234", not "123" or "12345")

### Error: "Failed to book time slot"
- **Cause**: Slot already booked, invalid date, or other business logic error
- **Solution**:
  - Check available slots first with `/api/bookings/available/{date}`
  - Ensure booking date is today or in the future
  - Ensure time slot number is 1, 2, or 3

### Error: "Unauthorized" (401)
- **Cause**: Missing or invalid JWT token
- **Solution**:
  - Include Authorization header: `Authorization: Bearer <token>`
  - Verify token is not expired
  - Ensure token format is correct

### HTTPS/SSL Certificate Error
- **On Windows (PowerShell)**:
  ```powershell
  dotnet dev-certs https --clean
  dotnet dev-certs https --trust
  ```

- **With curl**: Add `--insecure` flag
- **With Postman**: Disable SSL verification in Settings

## Database Migrations

The application automatically applies migrations on startup. To manually run migrations:

```bash
# In the project directory
dotnet ef database update
```

To revert to a previous migration:
```bash
dotnet ef database update <PreviousMigrationName>
```

To create a new migration after model changes:
```bash
dotnet ef migrations add <MigrationName>
```

## Performance Considerations

- **Indexes**: Database has unique indexes on:
  - User email (prevents duplicate emails)
  - (BookingDate, TimeSlotNumber) (prevents double bookings of same slot)
- **Token Expiration**: JWT tokens expire after 24 hours
- **Date Handling**: All dates are stored in UTC for consistency

## Next Steps

1. Customize JWT expiration time if needed (currently 24 hours in AuthService.cs)
2. Add email verification for user registration
3. Add rate limiting for API endpoints
4. Implement refresh tokens for JWT
5. Add logging and monitoring
6. Set up CI/CD pipeline with GitHub Actions or Azure Pipelines
