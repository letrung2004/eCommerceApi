# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/SharedLibrarySolution.csproj ./source/SharedLibrary/SharedLibrarySolution/
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution

# Copy Product service projects (all layers if have)
COPY source/Product/ProductService.Presentation/*.csproj ./source/Product/ProductService.Presentation/

# Restore dependencies
WORKDIR /src/source/Product/ProductService.Presentation
RUN dotnet restore

# Copy full source code (after restore)
WORKDIR /src
COPY source/Product ./source/Product

# Build and publish
WORKDIR /src/source/Product/ProductService.Presentation
RUN dotnet publish ProductService.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ProductService.Presentation.dll"]
