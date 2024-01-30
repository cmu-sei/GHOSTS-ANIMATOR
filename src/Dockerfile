# Copyright 2022 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5000

COPY --from=build-env /app/out .
COPY ./ghosts-animator-api/config ./config

LABEL "product"="GHOSTS ANIMATOR"
LABEL "maintainer"="Dustin Updyke <ddupdyke[@]sei.cmu.edu>"
LABEL "version"="Browse to /swagger"

EXPOSE 5000

ENTRYPOINT ["dotnet", "ghosts.animator.api.dll"]
