#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Stock/DTMStockDemo/DTMStockDemo.csproj", "Services/Stock/DTMStockDemo/"]
COPY ["Services/Stock/DTMStockAppService/DTMStockAppService.csproj", "Services/Stock/DTMStockAppService/"]
COPY ["Common/DBHelper/DBHelper.csproj", "Common/DBHelper/"]
RUN dotnet restore "Services/Stock/DTMStockDemo/DTMStockDemo.csproj"
COPY . .
WORKDIR "/src/Services/Stock/DTMStockDemo"
RUN dotnet build "DTMStockDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DTMStockDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DTMStockDemo.dll"]