FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CallAlert.Api/CallAlert.Api.csproj", "CallAlert.Api/"]
RUN dotnet restore "CallAlert.Api/CallAlert.Api.csproj"

COPY . .
WORKDIR "/src/CallAlert.Api"
RUN dotnet publish "CallAlert.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "CallAlert.Api.dll"]


