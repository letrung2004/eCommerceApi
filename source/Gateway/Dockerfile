# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/SharedLibrarySolution.csproj ./source/SharedLibrary/SharedLibrarySolution/
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution

# Copy Gateway project
COPY source/Gateway/ApiGateway.Presentation/ApiGateway.Presentation.csproj ./source/Gateway/ApiGateway.Presentation/
COPY source/Gateway/ApiGateway.Presentation ./source/Gateway/ApiGateway.Presentation

# Restore and build
WORKDIR /src/source/Gateway/ApiGateway.Presentation
RUN dotnet restore ApiGateway.Presentation.csproj
RUN dotnet publish ApiGateway.Presentation.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ApiGateway.Presentation.dll"]