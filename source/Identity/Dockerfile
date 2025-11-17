# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/SharedLibrarySolution.csproj ./source/SharedLibrary/SharedLibrarySolution/
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution

# Copy Identity projects (all layers)
COPY source/Identity/IdentityService.Application/*.csproj ./source/Identity/IdentityService.Application/
COPY source/Identity/IdentityService.Domain/*.csproj ./source/Identity/IdentityService.Domain/
COPY source/Identity/IdentityService.Infrastructure/*.csproj ./source/Identity/IdentityService.Infrastructure/
COPY source/Identity/IdentityService.Presentation/*.csproj ./source/Identity/IdentityService.Presentation/

# Restore dependencies
WORKDIR /src/source/Identity/IdentityService.Presentation
RUN dotnet restore

# Copy full source code (after restore)
WORKDIR /src
COPY source/Identity ./source/Identity

# Build and publish
WORKDIR /src/source/Identity/IdentityService.Presentation
RUN dotnet publish IdentityService.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "IdentityService.Presentation.dll"]
