FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="Production"
ENV BEATMAP_DIRECTORY="/beatmaps"
ENV DOWNLOAD_MISSING_BEATMAPS="true"
ENV BEATMAP_DOWNLOAD_URL="https://osu.ppy.sh/osu/{beatmapId}"

VOLUME ${BEATMAP_DIRECTORY}
# chmod 777 so that this volume can be read/written by other containers that might use different uids

USER app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# AOT compilation dependencies
RUN apt-get update && apt-get install -y clang zlib1g-dev && rm -rf /var/lib/apt/lists/*

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

RUN mkdir -p /beatmaps && chmod -R 777 /beatmaps

RUN dotnet publish ./Difficalcy.Catch/Difficalcy.Catch.csproj -o /app/difficalcy-catch --runtime linux-x64 --self-contained true \
    && rm -f /app/difficalcy-catch/*.dbg
RUN dotnet publish ./Difficalcy.Mania/Difficalcy.Mania.csproj -o /app/difficalcy-mania --runtime linux-x64 --self-contained true \
    && rm -f /app/difficalcy-mania/*.dbg
RUN dotnet publish ./Difficalcy.Osu/Difficalcy.Osu.csproj -o /app/difficalcy-osu --runtime linux-x64 --self-contained true \
    && rm -f /app/difficalcy-osu/*.dbg
RUN dotnet publish ./Difficalcy.Taiko/Difficalcy.Taiko.csproj -o /app/difficalcy-taiko --runtime linux-x64 --self-contained true \
    && rm -f /app/difficalcy-taiko/*.dbg

FROM base AS difficalcy-catch
LABEL org.opencontainers.image.description "Lazer powered osu!catch difficulty calculator API"
COPY --from=build --chown=app:app /beatmaps /beatmaps
COPY --from=build /app/difficalcy-catch .
ENTRYPOINT ["./Difficalcy.Catch"]

FROM base AS difficalcy-mania
LABEL org.opencontainers.image.description "Lazer powered osu!mania difficulty calculator API"
COPY --from=build --chown=app:app /beatmaps /beatmaps
COPY --from=build /app/difficalcy-mania .
ENTRYPOINT ["./Difficalcy.Mania"]

FROM base AS difficalcy-osu
LABEL org.opencontainers.image.description "Lazer powered osu! difficulty calculator API"
COPY --from=build --chown=app:app /beatmaps /beatmaps
COPY --from=build /app/difficalcy-osu .
ENTRYPOINT ["./Difficalcy.Osu"]

FROM base AS difficalcy-taiko
LABEL org.opencontainers.image.description "Lazer powered osu!taiko difficulty calculator API"
COPY --from=build --chown=app:app /beatmaps /beatmaps
COPY --from=build /app/difficalcy-taiko .
ENTRYPOINT ["./Difficalcy.Taiko"]
