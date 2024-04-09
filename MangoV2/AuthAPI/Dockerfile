# Use the official .NET Core SDK image as a base
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY MangoV2/AuthAPI/AuthAPI.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY MangoV2/AuthAPI/ ./

# Build the application
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the built application from the build environment
COPY --from=build-env /app/out .

# Expose port 80 to the outside world
EXPOSE 80

# Define the command to run the application when the container starts
ENTRYPOINT ["dotnet", "AuthAPI.dll"]
