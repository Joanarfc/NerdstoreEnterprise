
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./services/NSE.Identity.API/NSE.Identity.API.csproj", "services/NSE.Identity.API/"]
COPY ["./building blocks/NSE.Core/NSE.Core.csproj", "building blocks/NSE.Core/"]
COPY ["./building blocks/NSE.MessageBus/NSE.MessageBus.csproj", "building blocks/NSE.MessageBus/"]
COPY ["./building blocks/NSE.WebAPI.Core/NSE.WebAPI.Core.csproj", "building blocks/NSE.WebAPI.Core/"]
RUN dotnet restore "services/NSE.Identity.API/NSE.Identity.API.csproj"
COPY . .
WORKDIR "services/NSE.Identity.API"
RUN dotnet build "NSE.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NSE.Identity.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.Identity.API.dll"]