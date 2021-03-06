#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 8080


ENV PATH_BASE = "/ap1/v1"

#RUN apt-get -y update
## Upgrade already installed packages:
#RUN apt-get -y upgrade

RUN groupadd --gid 5000 newuser \ 
    && useradd --home-dir /home/newuser --create-home --uid 5000 \ 
        --gid 5000 --shell /bin/sh --skel /dev/null newuser

USER newuser

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["RithV.Services.CORE.API.csproj", ""]
RUN dotnet restore "./RithV.Services.CORE.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "RithV.Services.CORE.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RithV.Services.CORE.API.csproj" -c Release -o /app/publish

ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED = "true"
ENV PORT=8080

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RithV.Services.CORE.API.dll"]