FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
RUN dotnet restore "WebAPI/WebAPI.csproj"
COPY WebAPI/ WebAPI/
COPY Database/ Database/
WORKDIR "/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get -y update; apt-get -y install curl
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/build/Database.dacpac .
ENTRYPOINT ["dotnet", "PharmacySystem.WebAPI.dll"]
