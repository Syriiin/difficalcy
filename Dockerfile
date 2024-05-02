FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19 AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BEATMAP_DIRECTORY=/beatmaps

VOLUME ${BEATMAP_DIRECTORY}
RUN mkdir ${BEATMAP_DIRECTORY} && chown -R app:app ${BEATMAP_DIRECTORY}

USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
WORKDIR /src

COPY ./Difficalcy/Difficalcy.csproj ./Difficalcy/
COPY ./Difficalcy.Catch/Difficalcy.Catch.csproj ./Difficalcy.Catch/
COPY ./Difficalcy.Mania/Difficalcy.Mania.csproj ./Difficalcy.Mania/
COPY ./Difficalcy.Osu/Difficalcy.Osu.csproj ./Difficalcy.Osu/
COPY ./Difficalcy.Taiko/Difficalcy.Taiko.csproj ./Difficalcy.Taiko/

RUN dotnet restore ./Difficalcy.Catch/Difficalcy.Catch.csproj
RUN dotnet restore ./Difficalcy.Mania/Difficalcy.Mania.csproj
RUN dotnet restore ./Difficalcy.Osu/Difficalcy.Osu.csproj
RUN dotnet restore ./Difficalcy.Taiko/Difficalcy.Taiko.csproj

COPY ./Difficalcy/ ./Difficalcy/
COPY ./Difficalcy.Catch/ ./Difficalcy.Catch/
COPY ./Difficalcy.Mania/ ./Difficalcy.Mania/
COPY ./Difficalcy.Osu/ ./Difficalcy.Osu/
COPY ./Difficalcy.Taiko/ ./Difficalcy.Taiko/

RUN dotnet publish ./Difficalcy.Catch/Difficalcy.Catch.csproj -o /app/difficalcy-catch --runtime linux-musl-x64 --self-contained false
RUN dotnet publish ./Difficalcy.Mania/Difficalcy.Mania.csproj -o /app/difficalcy-mania --runtime linux-musl-x64 --self-contained false
RUN dotnet publish ./Difficalcy.Osu/Difficalcy.Osu.csproj -o /app/difficalcy-osu --runtime linux-musl-x64 --self-contained false
RUN dotnet publish ./Difficalcy.Taiko/Difficalcy.Taiko.csproj -o /app/difficalcy-taiko --runtime linux-musl-x64 --self-contained false

FROM base AS difficalcy-catch
LABEL org.opencontainers.image.description "Lazer powered osu!catch difficulty calculator API"
COPY --from=build /app/difficalcy-catch .
ENTRYPOINT ["./Difficalcy.Catch"]

FROM base AS difficalcy-mania
LABEL org.opencontainers.image.description "Lazer powered osu!mania difficulty calculator API"
COPY --from=build /app/difficalcy-mania .
ENTRYPOINT ["./Difficalcy.Mania"]

FROM base AS difficalcy-osu
LABEL org.opencontainers.image.description "Lazer powered osu! difficulty calculator API"
COPY --from=build /app/difficalcy-osu .
ENTRYPOINT ["./Difficalcy.Osu"]

FROM base AS difficalcy-taiko
LABEL org.opencontainers.image.description "Lazer powered osu!taiko difficulty calculator API"
COPY --from=build /app/difficalcy-taiko .
ENTRYPOINT ["./Difficalcy.Taiko"]
