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

# Build runtime image (install Kerberos lib so Npgsql doesn't warn about libgssapi_krb5.so.2)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=publish /app/publish .
# Ensure appsettings.json is included (copy from build stage if not in publish)
COPY --from=build /src/Stamps.Web/appsettings.json /app/appsettings.json
ENTRYPOINT ["dotnet", "Stamps.Web.dll"]
