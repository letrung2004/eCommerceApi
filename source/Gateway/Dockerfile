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

# Copy Gateway project - ✅ SỬA: Đường dẫn đúng
COPY source/Gateway/ApiGateway.Presentation/*.csproj ./source/Gateway/ApiGateway.Presentation/

# Restore dependencies - ✅ SỬA: Restore với đường dẫn đầy đủ
RUN dotnet restore source/Gateway/ApiGateway.Presentation/ApiGateway.Presentation.csproj --ignore-failed-sources

# Copy full source code
COPY source/SharedLibrary/ ./source/SharedLibrary/
COPY source/Gateway/ ./source/Gateway/

# Xóa NuGet.config nếu có
RUN find /src -name "NuGet.config" -type f -delete || true

# Build and publish - ✅ SỬA: WORKDIR đúng
WORKDIR /src/source/Gateway/ApiGateway.Presentation
RUN dotnet publish -c Release -o /app/publish --no-restore

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8000
ENV ASPNETCORE_URLS=http://+:8000

# ✅ SỬA: Tên DLL đúng
ENTRYPOINT ["dotnet", "ApiGateway.Presentation.dll"]