#!/bin/bash
set -e

echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "${SA_PASSWORD}" -Q "SELECT 1" -C > /dev/null 2>&1; do
  echo "SQL Server is unavailable - sleeping"
  sleep 2
done

echo "SQL Server is up - executing migrations"

# Run migrations
dotnet ef database update --no-build

echo "Migrations completed - starting application"

# Start the application
exec dotnet InventoryService.gRPC.dll