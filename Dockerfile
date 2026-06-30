FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled AS base

LABEL org.opencontainers.image.source https://github.com/Syriiin/difficalcy

WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="Production"
ENV BEATMAP_DIRECTORY="/beatmaps"
ENV DOWNLOAD_MISSING_BEATMAPS="true"
ENV BEATMAP_DOWNLOAD_URL="https://osu.ppy.sh/osu/{beatmapId}"

VOLUME ${BEATMAP_DIRECTORY}

USER app

# -----------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-base
WORKDIR /src

COPY ./Directory.Build.props ./

# AOT compilation dependencies
RUN apt-get update && apt-get install -y clang zlib1g-dev && rm -rf /var/lib/apt/lists/*

# Restore main project
COPY ./Difficalcy/Difficalcy.csproj ./Difficalcy/
COPY ./Difficalcy.Api/Difficalcy.Api.csproj ./Difficalcy.Api/
COPY ./Difficalcy.Osu/Difficalcy.Osu.csproj ./Difficalcy.Osu/
COPY ./Difficalcy.Taiko/Difficalcy.Taiko.csproj ./Difficalcy.Taiko/
COPY ./Difficalcy.Catch/Difficalcy.Catch.csproj ./Difficalcy.Catch/
COPY ./Difficalcy.Mania/Difficalcy.Mania.csproj ./Difficalcy.Mania/

RUN dotnet restore ./Difficalcy.Api/Difficalcy.Api.csproj

# Copy source
COPY ./Difficalcy/ ./Difficalcy/
COPY ./Difficalcy.Api/ ./Difficalcy.Api/
COPY ./Difficalcy.Osu/ ./Difficalcy.Osu/
COPY ./Difficalcy.Taiko/ ./Difficalcy.Taiko/
COPY ./Difficalcy.Catch/ ./Difficalcy.Catch/
COPY ./Difficalcy.Mania/ ./Difficalcy.Mania/

RUN mkdir -p /beatmaps && chmod -R 777 /beatmaps

# -----------------------------------------------------------------------------

FROM build-base AS build
RUN dotnet publish ./Difficalcy.Api/Difficalcy.Api.csproj -o /app/difficalcy --runtime linux-x64 --self-contained true \
    && rm -f /app/difficalcy/*.dbg /app/difficalcy/*.pdb /app/difficalcy/*.Development.json

# -----------------------------------------------------------------------------

FROM base AS publish
LABEL org.opencontainers.image.description "Lazer powered osu! difficulty calculator API"
COPY --from=build --chown=app:app /beatmaps /beatmaps
COPY --from=build /app/difficalcy .
ENTRYPOINT ["./Difficalcy.Api"]

# -----------------------------------------------------------------------------

FROM build-base AS build-slim
COPY ./tools/StripResources/StripResources.csproj ./tools/StripResources/
RUN dotnet restore ./tools/StripResources/StripResources.csproj
COPY ./tools/StripResources/ ./tools/StripResources/
RUN dotnet build ./tools/StripResources/StripResources.csproj -o /tools && \
    /tools/StripResources \
        /root/.nuget/packages/ppy.osu.game.resources/*/lib/netstandard2.1/osu.Game.Resources.dll \
        /tmp/osu.Game.Resources.dll && \
    cp /tmp/osu.Game.Resources.dll \
        /root/.nuget/packages/ppy.osu.game.resources/*/lib/netstandard2.1/osu.Game.Resources.dll && \
    dotnet publish ./Difficalcy.Api/Difficalcy.Api.csproj -o /app/difficalcy --runtime linux-x64 --self-contained true && \
    rm -f /app/difficalcy/*.dbg /app/difficalcy/*.pdb /app/difficalcy/*.Development.json /app/difficalcy/*.so /app/difficalcy/*.so.*

# -----------------------------------------------------------------------------

FROM base AS slim
LABEL org.opencontainers.image.description "Lazer powered osu! difficulty calculator API (slim)"
COPY --from=build-slim --chown=app:app /beatmaps /beatmaps
COPY --from=build-slim /app/difficalcy .
ENTRYPOINT ["./Difficalcy.Api"]
