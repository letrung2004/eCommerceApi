# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/SharedLibrarySolution.csproj ./source/SharedLibrary/SharedLibrarySolution/
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution

# Copy InventoryService (vì ProductService tham chiếu Inventory)
COPY source/Inventory ./source/Inventory

# Copy Product project (FIX QUAN TRỌNG)
COPY source/Product/ProductService.Presentation/ProductService.Presentation.csproj ./source/Product/ProductService.Presentation/
COPY source/Product ./source/Product

# Copy Basket project
COPY source/Basket/BasketService.Presentation/BasketService.Presentation.csproj ./source/Basket/BasketService.Presentation/
COPY source/Basket/BasketService.Presentation ./source/Basket/BasketService.Presentation

# Restore and build
WORKDIR /src/source/Basket/BasketService.Presentation
RUN dotnet restore BasketService.Presentation.csproj
RUN dotnet publish BasketService.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BasketService.Presentation.dll"]