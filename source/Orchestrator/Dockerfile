# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Clear NuGet cache
ENV NUGET_PACKAGES=/root/.nuget/packages
RUN dotnet nuget locals all --clear

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/*.csproj ./source/SharedLibrary/SharedLibrarySolution/

# Copy Inventory gRPC (dependency)
COPY source/Inventory/InventoryService.gRPC/*.csproj ./source/Inventory/InventoryService.gRPC/

# Copy Payment gRPC (dependency)
COPY source/Payment/PaymentService.gRPC/*.csproj ./source/Payment/PaymentService.gRPC/

# Copy Orchestrator project
COPY source/Orchestrator/OrderSaga.Worker/*.csproj ./source/Orchestrator/OrderSaga.Worker/

# Restore dependencies
RUN dotnet restore source/Orchestrator/OrderSaga.Worker/OrderSaga.Worker.csproj --ignore-failed-sources

# Copy full source code
COPY source/SharedLibrary/ ./source/SharedLibrary/
COPY source/Inventory/ ./source/Inventory/
COPY source/Payment/ ./source/Payment/
COPY source/Orchestrator/ ./source/Orchestrator/

# XÓA appsettings từ InventoryService.gRPC để tránh conflict
RUN rm -f /src/source/Inventory/InventoryService.gRPC/appsettings*.json
RUN rm -f /src/source/Payment/PaymentService.gRPC/appsettings*.json


# Xóa NuGet.config nếu có
RUN find /src -name "NuGet.config" -type f -delete || true

# Build and publish
WORKDIR /src/source/Orchestrator/OrderSaga.Worker
RUN dotnet publish -c Release -o /app/publish --no-restore

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Worker service không cần EXPOSE port (không có HTTP endpoint)
# Chỉ kết nối đến RabbitMQ và gRPC clients

ENTRYPOINT ["dotnet", "OrderSaga.Worker.dll"]