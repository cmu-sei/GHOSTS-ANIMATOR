# Copyright 2022 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5000
ENV GHOSTS_ANIMATOR_VERSION=0.1.1.0
COPY --from=build-env /app/out .
COPY ./ghosts-animator-api/config ./config

LABEL "product"="GHOSTS ANIMATOR"
LABEL "maintainer"="Dustin Updyke <ddupdyke[@]sei.cmu.edu>"
LABEL "version"="Browse to /swagger"

ENTRYPOINT ["dotnet", "ghosts.animator.api.dll"]
