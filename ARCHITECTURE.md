# Architecture & Flow Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT APPLICATION                        │
│                  (Web/Mobile/Desktop App)                        │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTPS Requests
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                   ASP.NET CORE API (Port 5001)                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │                    Controllers                              │ │
│  │  ┌──────────────────┐       ┌──────────────────────────┐  │ │
│  │  │ AuthController   │       │ BookingsController       │  │ │
│  │  │ - Register       │       │ - Book Slot              │  │ │
│  │  │ - Login          │       │ - Unbook Slot            │  │ │
│  │  │                  │       │ - Get My Bookings        │  │ │
│  │  │                  │       │ - Check Availability     │  │ │
│  │  └─────────┬────────┘       └────────────┬─────────────┘  │ │
│  │            │                             │                 │ │
│  │            └─────────────────┬───────────┘                │ │
│  │                              │                            │ │
│  │  ┌───────────────────────────▼────────────────────────┐  │ │
│  │  │              Services Layer                         │  │ │
│  │  │  ┌────────────────────┐   ┌─────────────────────┐ │  │ │
│  │  │  │  AuthService       │   │ BookingService      │ │  │ │
│  │  │  │ - PIN Validation   │   │ - Slot Management   │ │  │ │
│  │  │  │ - PIN Hashing      │   │ - Availability      │ │  │ │
│  │  │  │ - JWT Generation   │   │ - Booking Logic     │ │  │ │
│  │  │  └────────────────────┘   └─────────────────────┘ │  │ │
│  │  └───────────────────────────┬────────────────────────┘  │ │
│  │                              │                            │ │
│  │  ┌───────────────────────────▼────────────────────────┐  │ │
│  │  │      Entity Framework Core (Data Access)           │  │ │
│  │  │  ┌────────────────────────────────────────────┐   │  │ │
│  │  │  │        LaundryDbContext                    │   │  │ │
│  │  │  │ - DbSet<User>                              │   │  │ │
│  │  │  │ - DbSet<BookedTimeSlot>                    │   │  │ │
│  │  │  └────────────────────────────────────────────┘   │  │ │
│  │  └───────────────────────────┬────────────────────────┘  │ │
│  └────────────────────────────┬─┘───────────────────────────┘ │
│                               │ SQL Queries                    │
└───────────────────────────────┼────────────────────────────────┘
                                │
┌───────────────────────────────▼────────────────────────────────┐
│                    PostgreSQL Database                          │
│  ┌────────────────────────────────────────────────────────────┐│
│  │ Tables:                                                    ││
│  │ ┌─────────────────┐         ┌──────────────────────────┐ ││
│  │ │      Users      │         │  BookedTimeSlots         │ ││
│  │ ├─────────────────┤         ├──────────────────────────┤ ││
│  │ │ Id (PK)         │         │ Id (PK)                  │ ││
│  │ │ Email (UNIQUE)  │◄────────│ UserId (FK) → Users(Id)  │ ││
│  │ │ PasswordPin     │         │ BookingDate              │ ││
│  │ │ CreatedAt       │         │ TimeSlotNumber (1-3)     │ ││
│  │ │ UpdatedAt       │         │ CreatedAt                │ ││
│  │ │                 │         │ UNIQUE(Date, SlotNumber) │ ││
│  │ └─────────────────┘         └──────────────────────────┘ ││
│  └────────────────────────────────────────────────────────────┘│
└────────────────────────────────────────────────────────────────┘
```

## Authentication Flow

```
┌────────────┐                                                ┌──────────────┐
│   Client   │                                                │  API Server  │
└─────┬──────┘                                                └──────┬───────┘
      │                                                              │
      │ 1. POST /api/auth/register                                  │
      │    {email: "user@example.com", pin: "1234"}               │
      ├─────────────────────────────────────────────────────────►  │
      │                                                              │ Hash PIN
      │                                                              │ Create User
      │                                                              │ Generate JWT
      │ 2. Response: LoginResponse                                  │
      │    {userId: 1, email: "...", token: "eyJ..."}            │
      │  ◄─────────────────────────────────────────────────────────┤
      │                                                              │
      │ Store Token                                                 │
      │                                                              │
      │ 3. GET /api/bookings/my-bookings                            │
      │    Header: Authorization: Bearer eyJ...                     │
      ├─────────────────────────────────────────────────────────►  │
      │                                                              │ Verify JWT
      │                                                              │ Get User ID
      │                                                              │ Fetch Bookings
      │ 4. Response: [Bookings...]                                  │
      │  ◄─────────────────────────────────────────────────────────┤
      │                                                              │

JWT Token Components:
┌──────────┬──────────┬───────────────────────────────────────┐
│  Header  │  Payload │          Signature                    │
├──────────┼──────────┼───────────────────────────────────────┤
│ Algorithm│ UserId   │ HMACSHA256(                           │
│ Type     │ Email    │   secret_key,                        │
│          │ Exp      │   header + payload                   │
│          │ Iat      │ )                                     │
└──────────┴──────────┴───────────────────────────────────────┘

Expiration: 24 hours from generation
```

## Booking Flow

```
┌────────────┐                                                ┌──────────────┐
│   Client   │                                                │  API Server  │
└─────┬──────┘                                                └──────┬───────┘
      │                                                              │
      │ 1. GET /api/bookings/available/2026-01-20                  │
      │    (Check what's available)                                │
      ├─────────────────────────────────────────────────────────►  │
      │                                                              │ Query bookings
      │                                                              │ for that date
      │ 2. Response: {availableSlots: [1,2,3], bookedSlots: [...]} │
      │  ◄─────────────────────────────────────────────────────────┤
      │                                                              │
      │ (User decides to book slot 1)                              │
      │                                                              │
      │ 3. POST /api/bookings/book                                  │
      │    Header: Authorization: Bearer eyJ...                     │
      │    {bookingDate: "2026-01-20", timeSlotNumber: 1}          │
      ├─────────────────────────────────────────────────────────►  │
      │                                                              │ Validate JWT
      │                                                              │ Check availability
      │                                                              │ Validate date
      │                                                              │ Check constraints
      │                                                              │ Create booking
      │ 4. Response: BookedTimeSlotDto (201 Created)               │
      │  ◄─────────────────────────────────────────────────────────┤
      │                                                              │
      │ (Later, user wants to unbook)                              │
      │                                                              │
      │ 5. DELETE /api/bookings/unbook/1                           │
      │    Header: Authorization: Bearer eyJ...                     │
      ├─────────────────────────────────────────────────────────►  │
      │                                                              │ Validate JWT
      │                                                              │ Check ownership
      │                                                              │ Delete booking
      │ 6. Response: 204 No Content                                 │
      │  ◄─────────────────────────────────────────────────────────┤
      │                                                              │
```

## Time Slot Constraints

```
┌──────────────────────────────────────────────────────────────┐
│              Daily Time Slot Configuration                     │
├──────────────────────────────────────────────────────────────┤
│                                                                │
│  Slot 1: [08:00 - 10:00]  ─► Can book or unbook              │
│  Slot 2: [10:30 - 12:30]  ─► Can book or unbook              │
│  Slot 3: [14:00 - 16:00]  ─► Can book or unbook              │
│                                                                │
│  Note: Times are conceptual - system stores only:             │
│    - Date (YYYY-MM-DD)                                        │
│    - Slot Number (1, 2, or 3)                                 │
│                                                                │
└──────────────────────────────────────────────────────────────┘

Database Uniqueness Constraint:
┌─────────────────────────────────────────────┐
│  (BookingDate, TimeSlotNumber) = UNIQUE      │
├─────────────────────────────────────────────┤
│  Prevents:                                   │
│  - Same user booking same slot twice         │
│  - Two users booking the same slot           │
│  - Any duplicate (Date, Slot) combination    │
│                                               │
│  Example:                                    │
│  ┌────────────┬────────────┬─────────────┐  │
│  │   Date     │   Slot     │ User ID     │  │
│  ├────────────┼────────────┼─────────────┤  │
│  │ 2026-01-20 │ 1          │ 1           │  │
│  │ 2026-01-20 │ 1          │ 2           │ ✗ │
│  │ 2026-01-20 │ 2          │ 1           │ ✓ │
│  │ 2026-01-21 │ 1          │ 1           │ ✓ │
│  └────────────┴────────────┴─────────────┘  │
└─────────────────────────────────────────────┘
```

## Security Layers

```
┌───────────────────────────────────────────────┐
│        Client Request → API Processing         │
├───────────────────────────────────────────────┤
│                                               │
│  Layer 1: HTTPS Transport                     │
│  ├─ Encrypted in transit                      │
│  └─ Certificate validation                    │
│                                               │
│  Layer 2: Input Validation                    │
│  ├─ Email format validation                   │
│  ├─ PIN must be 4 digits                      │
│  ├─ Date format validation                    │
│  └─ SlotNumber range validation (1-3)        │
│                                               │
│  Layer 3: Authentication (AuthService)        │
│  ├─ PIN hashing with SHA256                   │
│  ├─ Constant-time comparison                  │
│  └─ JWT generation with secret                │
│                                               │
│  Layer 4: Authorization                       │
│  ├─ JWT token validation                      │
│  ├─ Expiration check (24 hours)              │
│  └─ Signature verification                    │
│                                               │
│  Layer 5: Business Logic Validation           │
│  ├─ User ownership verification               │
│  ├─ Booking availability check                │
│  ├─ Date constraint validation                │
│  └─ Slot uniqueness verification              │
│                                               │
│  Layer 6: Database Constraints                │
│  ├─ UNIQUE email index                        │
│  ├─ UNIQUE (date, slot) constraint           │
│  ├─ Foreign key constraints                   │
│  └─ NOT NULL constraints                      │
│                                               │
└───────────────────────────────────────────────┘
```

## Request/Response Flow for Book Slot

```
CLIENT REQUEST:
┌────────────────────────────────────────┐
│ POST /api/bookings/book                │
├────────────────────────────────────────┤
│ Headers:                               │
│  Content-Type: application/json        │
│  Authorization: Bearer <JWT_TOKEN>    │
├────────────────────────────────────────┤
│ Body:                                  │
│ {                                      │
│   "bookingDate": "2026-01-20",        │
│   "timeSlotNumber": 1                  │
│ }                                      │
└────────────────────────────────────────┘
                  │
                  ▼
┌────────────────────────────────────────┐
│ BookingsController.BookTimeSlot()      │
├────────────────────────────────────────┤
│ 1. Extract JWT token                   │
│ 2. Validate token                      │
│ 3. Extract user ID from token          │
│ 4. Call BookingService.BookAsync()    │
└────────────────────────────────────────┘
                  │
                  ▼
┌────────────────────────────────────────┐
│ BookingService.BookTimeSlotAsync()     │
├────────────────────────────────────────┤
│ 1. Validate SlotNumber (1-3)           │
│ 2. Validate date (not past)            │
│ 3. Check user not already booked       │
│ 4. Check slot not already taken        │
│ 5. Create BookedTimeSlot record        │
│ 6. Save to database                    │
│ 7. Return BookedTimeSlotDto            │
└────────────────────────────────────────┘
                  │
                  ▼
┌────────────────────────────────────────┐
│ API RESPONSE (201 Created):            │
├────────────────────────────────────────┤
│ {                                      │
│   "id": 5,                             │
│   "bookingDate": "2026-01-20T...",    │
│   "timeSlotNumber": 1,                 │
│   "createdAt": "2026-01-14T10:30Z"    │
│ }                                      │
└────────────────────────────────────────┘
```

---

This architecture is designed for:
- ✅ Security: Multiple validation layers
- ✅ Scalability: Service-oriented design
- ✅ Maintainability: Clear separation of concerns
- ✅ Reliability: Database constraints prevent inconsistencies
- ✅ Performance: Efficient queries with proper indexes
