# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["FeedLens.API/FeedLens.API.csproj", "FeedLens.API/"]
COPY ["FeedLens.Services/FeedLens.Services.csproj", "FeedLens.Services/"]
COPY ["FeedLens.Repositories/FeedLens.Repositories.csproj", "FeedLens.Repositories/"]
COPY ["FeedLens.Domain/FeedLens.Domain.csproj", "FeedLens.Domain/"]
COPY ["FeedLens.Helpers/FeedLens.Helpers.csproj", "FeedLens.Helpers/"]

# Restore dependencies
RUN dotnet restore "FeedLens.API/FeedLens.API.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/FeedLens.API"
RUN dotnet publish "FeedLens.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FeedLens.API.dll"]