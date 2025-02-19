# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the project files
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for final deployment
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose the API port
EXPOSE 8080
ENTRYPOINT ["dotnet", "backend.dll"]
