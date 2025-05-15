FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["loue-ma-caisse/loue-ma-caisse.csproj", "loue-ma-caisse/"]
RUN dotnet restore "loue-ma-caisse/loue-ma-caisse.csproj"
COPY . .
WORKDIR "/src/loue-ma-caisse"
RUN dotnet build "loue-ma-caisse.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "loue-ma-caisse.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "loue-ma-caisse.dll"]
