# Usar la imagen base de .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos de la aplicación y construir
COPY . ./
RUN dotnet publish -c Release -o out

# Usar la imagen base de .NET Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Exponer el puerto que usará la aplicación
EXPOSE 80

# Establecer el comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "user_service.dll"]
