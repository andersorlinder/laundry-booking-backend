# Docker Setup for PostgreSQL

This guide helps you set up PostgreSQL using Docker for the Laundry Booking API.

## Prerequisites

- Docker and Docker Compose installed on your system

## Quick Start

### Using Docker Run

```bash
docker run --name laundry-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=laundry_booking \
  -p 5432:5432 \
  -d postgres:latest
```

### Using Docker Compose

Create a `docker-compose.yml` file in the project root:

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
    networks:
      - laundry-network

networks:
  laundry-network:
    driver: bridge

volumes:
  postgres_data:
```

Then run:

```bash
docker-compose up -d
```

## Verify Connection

Test the PostgreSQL connection:

```bash
psql -h localhost -U postgres -d laundry_booking -c "SELECT 1;"
```

Or using Docker:

```bash
docker exec -it laundry-postgres psql -U postgres -d laundry_booking -c "SELECT 1;"
```

## Stop and Remove

```bash
# Stop the container
docker stop laundry-postgres

# Remove the container
docker rm laundry-postgres

# Remove the volume (if using docker-compose)
docker-compose down -v
```

## Connection String

Use this connection string in `appsettings.Development.json`:

```
Host=localhost;Port=5432;Database=laundry_booking;Username=postgres;Password=postgres
```

## Database Tools

### pgAdmin (Web UI for PostgreSQL)

Add this to your `docker-compose.yml`:

```yaml
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: pgadmin
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "5050:80"
    networks:
      - laundry-network
    depends_on:
      - postgres
```

Access pgAdmin at `http://localhost:5050`

Credentials:
- Email: `admin@example.com`
- Password: `admin`
