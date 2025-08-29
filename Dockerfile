FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MotorcycleRental.Api/MotorcycleRental.Api.csproj", "src/MotorcycleRental.Api/"]
COPY ["src/MotorcycleRental.Infrastructure/MotorcycleRental.Infrastructure.csproj", "src/MotorcycleRental.Infrastructure/"]
COPY ["src/MotorcycleRental.Application/MotorcycleRental.Application.csproj", "src/MotorcycleRental.Application/"]
COPY ["src/MotorcycleRental.Domain/MotorcycleRental.Domain.csproj", "src/MotorcycleRental.Domain/"]

RUN dotnet restore "src/MotorcycleRental.Api/MotorcycleRental.Api.csproj"
COPY . .
WORKDIR "/src/src/MotorcycleRental.Api"
RUN dotnet build "MotorcycleRental.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MotorcycleRental.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MotorcycleRental.Api.dll"]