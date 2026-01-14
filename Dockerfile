# Use the official .NET 10.0 SDK image as build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Stamps.Web/Stamps.Web.csproj Stamps.Web/
COPY Stamps.Shared/Stamps.Shared.csproj Stamps.Shared/
RUN dotnet restore Stamps.Web/Stamps.Web.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/Stamps.Web
RUN dotnet build Stamps.Web.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish Stamps.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stamps.Web.dll"]
