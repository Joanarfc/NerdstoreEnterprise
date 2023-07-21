
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./services/NSE.ShoppingCart.API/NSE.ShoppingCart.API.csproj", "services/NSE.ShoppingCart.API/"]
COPY ["./building blocks/NSE.MessageBus/NSE.MessageBus.csproj", "building blocks/NSE.MessageBus/"]
COPY ["./building blocks/NSE.Core/NSE.Core.csproj", "building blocks/NSE.Core/"]
COPY ["./building blocks/NSE.WebAPI.Core/NSE.WebAPI.Core.csproj", "building blocks/NSE.WebAPI.Core/"]
RUN dotnet restore "services/NSE.ShoppingCart.API/NSE.ShoppingCart.API.csproj"
COPY . .
WORKDIR "services/NSE.ShoppingCart.API"
RUN dotnet build "NSE.ShoppingCart.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NSE.ShoppingCart.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.ShoppingCart.API.dll"]