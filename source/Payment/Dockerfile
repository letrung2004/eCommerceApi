# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY eCommerceApi.sln .

# Copy SharedLibrary
COPY source/SharedLibrary/SharedLibrarySolution/SharedLibrarySolution.csproj ./source/SharedLibrary/SharedLibrarySolution/
COPY source/SharedLibrary/SharedLibrarySolution ./source/SharedLibrary/SharedLibrarySolution


# Copy Payment project
COPY source/Payment/PaymentService.gRPC/PaymentService.gRPC.csproj ./source/Payment/PaymentService.gRPC/
COPY source/Payment/PaymentService.gRPC ./source/Payment/PaymentService.gRPC

# Restore and build
WORKDIR /src/source/Payment/PaymentService.gRPC
RUN dotnet restore PaymentService.gRPC.csproj
RUN dotnet publish PaymentService.gRPC.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PaymentService.gRPC.dll"]