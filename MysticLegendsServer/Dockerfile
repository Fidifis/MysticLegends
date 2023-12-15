#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN DEBIAN_FRONTEND=noninteractive apt-get update && \
    apt-get install -y --no-install-recommends jq curl && \ # used by health check
    rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MysticLegendsServer/MysticLegendsServer.csproj", "MysticLegendsServer/"]
RUN dotnet restore "MysticLegendsServer/MysticLegendsServer.csproj"
COPY . .
WORKDIR "/src/MysticLegendsServer"
RUN dotnet build "MysticLegendsServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MysticLegendsServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MysticLegendsServer.dll"]