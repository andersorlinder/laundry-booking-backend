# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY laundry-booking-backend/laundry-booking-backend.csproj laundry-booking-backend/
RUN dotnet restore laundry-booking-backend/laundry-booking-backend.csproj

# Copy everything else and build
COPY laundry-booking-backend/ laundry-booking-backend/
WORKDIR /src/laundry-booking-backend
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (Render uses PORT environment variable)
EXPOSE 10000
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "laundry-booking-backend.dll"]
