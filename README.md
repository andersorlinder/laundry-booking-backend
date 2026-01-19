# Laundry Booking API

A REST API for managing laundry time slot bookings with user authentication. Users can register with email, apartment number, and a 4-digit PIN, then book and unbook laundry time slots. The system supports 3 time slots per day.

## Features

- **User Authentication**: Register and login with email, apartment number, and 4-digit PIN
- **JWT Token-based Authorization**: Secure API endpoints with JWT tokens
- **Apartment Tracking**: Each user and booking is associated with a 6-character apartment number
- **Time Slot Management**: 3 time slots per day (slot 1, 2, 3)
- **Booking System**: Users can book and unbook laundry slots
- **Booking Visibility**: View booked slots with apartment numbers for any date range
- **PostgreSQL Database**: Persistent data storage
- **Input Validation**: Email, PIN, and apartment number format validation

## Prerequisites

- .NET 10.0 or higher
- PostgreSQL database (local or remote)
- Visual Studio Code or Visual Studio 2022+

## Setup Instructions

### 1. Configure Database Connection

Edit `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=laundry_booking;Username=postgres;Password=postgres"
}
```

### 2. Configure JWT Settings

Update JWT settings in `appsettings.json`:

```json
"JwtSettings": {
  "Secret": "your-super-secret-key-with-minimum-256-bits-of-entropy-for-production",
  "Issuer": "laundry-booking-api",
  "Audience": "laundry-booking-client"
}
```

**Important**: For production, use a strong, randomly generated secret key of at least 256 bits.

### 3. Create Database

The application automatically applies migrations on startup. Ensure PostgreSQL is running and the database credentials in `appsettings.json` are correct.

### 4. Run the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001`

## API Endpoints

### Authentication

#### Register User
- **Endpoint**: `POST /api/auth/register`
- **Authentication**: Not required
- **Request Body**:
  ```json
  {
    "email": "user@example.com",
    "pin": "1234",
    "apartmentNumber": "A10231"
  }
  ```
- **Response** (201):
  ```json
  {
    "userId": 1,
    "email": "user@example.com",
    "apartmentNumber": "A10231",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
  ```
- **Validation**:
  - Email must be valid format
  - PIN must be exactly 4 digits
  - Apartment number must be exactly 6 characters

#### Login User
- **Endpoint**: `POST /api/auth/login`
- **Authentication**: Not required
- **Request Body**:
  ```json
  {
    "email": "user@example.com",
    "pin": "1234"
  }
  ```
- **Response** (200):
  ```json
  {
    "userId": 1,
    "email": "user@example.com",
    "apartmentNumber": "A10231",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
  ```

### Bookings

#### Book a Time Slot
- **Endpoint**: `POST /api/bookings/book`
- **Authentication**: Required (Bearer token)
- **Request Body**:
  ```json
  {
    "bookingDate": "2026-01-20",
    "timeSlotNumber": 1
  }
  ```
- **Response** (201):
  ```json
  {
    "id": 5,
    "userId": 1,
    "apartmentNumber": "A10231",
    "bookingDate": "2026-01-20T00:00:00",
    "timeSlotNumber": 1,
    "createdAt": "2026-01-14T10:30:00Z"
  }
  ```

#### Get User's Bookings
- **Endpoint**: `GET /api/bookings/my-bookings`
- **Authentication**: Required (Bearer token)
- **Response** (200):
  ```json
  [
    {
      "id": 5,
      "userId": 1,
      "apartmentNumber": "A10231",
      "bookingDate": "2026-01-20T00:00:00",
      "timeSlotNumber": 1,
      "createdAt": "2026-01-14T10:30:00Z"
    }
  ]
  ```

#### Unbook a Time Slot
- **Endpoint**: `DELETE /api/bookings/unbook/{slotId}`
- **Authentication**: Required (Bearer token)
- **Response**: 204 No Content

#### Get Booked Slots for a Date Range
- **Endpoint**: `GET /api/bookings/booked?date=2026-01-20&daysAhead=7`
- **Authentication**: Not required
- **Query Parameters**:
  - `date`: Start date in format `yyyy-MM-dd` (e.g., `2026-01-20`) - required
  - `daysAhead`: Number of days to query (optional, default: 1)
- **Response** (200):
  ```json
  {
    "startDate": "2026-01-20T00:00:00",
    "endDate": "2026-01-26T00:00:00",
    "daysAhead": 7,
    "bookingsByDate": [
      {
        "date": "2026-01-20T00:00:00",
        "bookedSlots": [
          {
            "id": 5,
            "userId": 1,
            "apartmentNumber": "A10231",
            "bookingDate": "2026-01-20T00:00:00",
            "timeSlotNumber": 1,
            "createdAt": "2026-01-14T10:30:00Z"
          }
        ]
      }
    ]
  }
  ```

## Usage Examples

### Using cURL

#### Register
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "pin": "1234",
    "apartmentNumber": "A10231"
  }'
```

#### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "pin": "1234"
  }'
```

#### Book a Slot (replace TOKEN with the JWT token received from login)
```bash
curl -X POST https://localhost:5001/api/bookings/book \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN" \
  -d '{
    "bookingDate": "2026-01-20",
    "timeSlotNumber": 1
  }'
```

#### Get Available Slots
```bash
curl -X GET "https://localhost:5001/api/bookings/available/2026-01-20"
```

## Error Handling

The API returns appropriate HTTP status codes:

- **200 OK**: Successful request
- **201 Created**: Resource successfully created
- **204 No Content**: Successful deletion
- **400 Bad Request**: Invalid input or business logic violation
- **401 Unauthorized**: Missing or invalid authentication token
- **404 Not Found**: Resource not found

Error responses include a message explaining the issue:
```json
{
  "message": "PIN must be exactly 4 digits"
}
```

## Database Schema

### Users Table
- `Id` (INT, Primary Key)
- `Email` (TEXT, Unique)
- `PasswordPin` (TEXT, Hashed)
- `ApartmentNumber` (TEXT, Exactly 6 characters)
- `CreatedAt` (TIMESTAMP)
- `UpdatedAt` (TIMESTAMP)

### BookedTimeSlots Table
- `Id` (INT, Primary Key)
- `UserId` (INT, Foreign Key)
- `BookingDate` (DATE)
- `TimeSlotNumber` (INT, 1-3)
- `CreatedAt` (TIMESTAMP)
- Unique constraint on (BookingDate, TimeSlotNumber) - ensures only one booking per slot per day

## Security Notes

1. **PIN Hashing**: Passwords are hashed using SHA256 before storage
2. **JWT Tokens**: Tokens expire after 24 hours
3. **Database**: Connection uses SSL by default with PostgreSQL
4. **HTTPS**: API enforces HTTPS in production
5. **CORS**: Configure CORS settings in `Program.cs` for your frontend domain

## Development

### Project Structure
```
laundry-booking-backend/
├── Controllers/          # API endpoint controllers
├── Data/                # Database context
├── Migrations/          # Database migrations
├── Models/              # Entity models
├── Services/            # Business logic services
├── Dtos/               # Data transfer objects
├── Program.cs          # Application startup configuration
├── appsettings.json    # Configuration settings
└── laundry-booking-backend.csproj
```

## License

MIT License
