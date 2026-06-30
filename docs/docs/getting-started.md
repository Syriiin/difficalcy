# Getting Started

## TL;DR Example recommended setup

`compose.yaml`
```yaml
services:
  difficalcy:
    image: ghcr.io/syriiin/difficalcy:latest # PIN ME
    environment:
      - REDIS_CONFIGURATION=cache:6379
    ports:
      - 5000:80
    volumes:
      - beatmaps:/beatmaps
    depends_on:
      - cache

  cache:
    image: redis:latest
    volumes:
      - ./redis.conf:/usr/local/etc/redis/redis.conf
      - redis-data:/data

  volumes:
    beatmaps:
    redis-data:
```

`redis.conf`
```
maxmemory 100mb
maxmemory-policy allkeys-lru
```

See [API Reference](./api-reference/index.md) for available endpoints.

## How to run difficalcy

difficalcy is published as a docker image, so you can run it anywhere docker runs.

### Docker

You can run it with docker directly:

```sh
docker run -p 5000:80 ghcr.io/syriiin/difficalcy:latest
```

### Docker Compose

You can run it with docker compose:

```yaml
services:
  difficalcy:
    image: ghcr.io/syriiin/difficalcy:latest
    ports:
      - "5000:80"
```

## Available calculators

difficalcy is available for all four official osu! rulesets under a single service. Each calculator is accessible by its key:

- `osu` - osu!
- `taiko` - osu!taiko
- `catch` - osu!catch
- `mania` - osu!mania

You can list available calculators at runtime with the `/api/calculators` endpoint:

```sh
curl "localhost:5000/api/calculators"
```

```json
{
  "osu": {
    "rulesetName": "osu!",
    "calculatorName": "Official osu!",
    "calculatorPackage": "osu.Game.Rulesets.Osu",
    "calculatorVersion": "2026.527.0.0",
    "calculatorUrl": "https://nuget.org/packages/ppy.osu.Game.Rulesets.Osu/2026.527.0.0"
  },
  "taiko": {
    "rulesetName": "osu!taiko",
    "calculatorName": "Official osu!taiko",
    "calculatorPackage": "osu.Game.Rulesets.Taiko",
    "calculatorVersion": "2026.527.0.0",
    "calculatorUrl": "https://nuget.org/packages/ppy.osu.Game.Rulesets.Taiko/2026.527.0.0"
  },
  "catch": {
    "rulesetName": "osu!catch",
    "calculatorName": "Official osu!catch",
    "calculatorPackage": "osu.Game.Rulesets.Catch",
    "calculatorVersion": "2026.527.0.0",
    "calculatorUrl": "https://nuget.org/packages/ppy.osu.Game.Rulesets.Catch/2026.527.0.0"
  },
  "mania": {
    "rulesetName": "osu!mania",
    "calculatorName": "Official osu!mania",
    "calculatorPackage": "osu.Game.Rulesets.Mania",
    "calculatorVersion": "2026.527.0.0",
    "calculatorUrl": "https://nuget.org/packages/ppy.osu.Game.Rulesets.Mania/2026.527.0.0"
  }
}
```

For this tutorial, we'll stick with the osu! calculator.

For other rulesets, see the [API Reference](./api-reference/index.md).

## How to run a calculation

You can use the `GET /api/calculators/osu/calculation` endpoint to perform basic difficulty and performance calculations of a score.

For example, to calculate an SS on [xi - Blue Zenith [FOUR DIMENSIONS]](https://osu.ppy.sh/beatmapsets/292301#osu/658127):

```sh
curl "localhost:5000/api/calculators/osu/calculation?BeatmapId=658127"
```

```json
{
  "accuracy": 1,
  "combo": 2402,
  "difficulty": {
    "aim": 3.486559350583331,
    "speed": 3.401805899214971,
    "flashlight": 0,
    "total": 7.218144469196162
  },
  "performance": {
    "aim": 223.2319016752279,
    "speed": 216.94931341785514,
    "accuracy": 142.3199671239901,
    "flashlight": 0,
    "total": 607.3436935784534
  }
}
```

However, the querystring is very limiting for passing parameters, so it's restricted to only beatmap ID and comma separated mods ( eg. `Mods=HD,HR`).

You can use the `POST /api/calculators/osu/batch/calculation` endpoint to efficiently calculate the difficulty and performance of one or more scores in a batch by passing a JSON body with full parameters.

For example, the same request as above:

```sh
curl "localhost:5000/api/calculators/osu/batch/calculation" \
  --json '[
    {
      "beatmapId": "658127"
    }
  ]'
```

```json
[
  {
    "accuracy": 1,
    "combo": 2402,
    "difficulty": {
      "aim": 3.486559350583331,
      "speed": 3.401805899214971,
      "flashlight": 0,
      "total": 7.218144469196162
    },
    "performance": {
      "aim": 223.2319016752279,
      "speed": 216.94931341785514,
      "accuracy": 142.3199671239901,
      "flashlight": 0,
      "total": 607.3436935784534
    }
  }
]
```

With HDHR:

```sh
curl "localhost:5000/api/calculators/osu/batch/calculation" \
  --json '[
    {
      "beatmapId": "658127",
      "mods": [
        {"acronym": "HD"},
        {"acronym": "HR"}
      ]
    }
  ]'
```

```json
[
  {
    "accuracy": 1,
    "combo": 2402,
    "difficulty": {
      "aim": 3.781787817014634,
      "speed": 3.6376548985196338,
      "flashlight": 0,
      "total": 7.776694277815145
    },
    "performance": {
      "aim": 310.44353837189016,
      "speed": 294.27692286092105,
      "accuracy": 233.88299810086727,
      "flashlight": 0,
      "total": 873.3504729333756
    }
  }
]
```

With [24 100s and 2 misses with a max combo of 2364](https://osu.ppy.sh/scores/453746931):

```sh
curl "localhost:5000/api/calculators/osu/batch/calculation" \
  --json '[
    {
      "beatmapId": "658127",
      "mods": [
        {"acronym": "HD"},
        {"acronym": "HR"}
      ],
      "oks": 24,
      "misses": 2,
      "combo": 2364
    }
  ]'
```

```json
[
  {
    "accuracy": 0.9908768373035985,
    "combo": 2364,
    "difficulty": {
      "aim": 3.781787817014634,
      "speed": 3.6376548985196338,
      "flashlight": 0,
      "total": 7.776694277815145
    },
    "performance": {
      "aim": 269.1655178289345,
      "speed": 252.09191398987699,
      "accuracy": 187.63970625353224,
      "flashlight": 0,
      "total": 738.5847641442473
    }
  }
]
```

Mod settings are also supported:

```sh
curl "localhost:5000/api/calculators/osu/batch/calculation" \
  --json '[
    {
      "beatmapId": "658127",
      "mods": [
        {
          "acronym": "DT",
          "settings": {
            "speed_change": "1.2"
          }
        }
      ]
    }
  ]'
```

```json
[
  {
    "accuracy": 1,
    "combo": 2402,
    "difficulty": {
      "aim": 4.09090844208357,
      "speed": 4.156262773610562,
      "flashlight": 0,
      "total": 8.641107338148792
    },
    "performance": {
      "aim": 364.8582844353347,
      "speed": 405.96269551748065,
      "accuracy": 192.72297482793658,
      "flashlight": 0,
      "total": 1007.207697579252
    }
  }
]
```

There is also a `/beatmapdetails` endpoint for getting various specifics about a beatmap:

```sh
$ curl "localhost:5000/api/calculators/osu/beatmapdetails?BeatmapId=658127"
```

```json
{
  "circleCount": 1760,
  "sliderCount": 210,
  "spinnerCount": 3,
  "sliderTickCount": 219,
  "circleSize": 4,
  "approachRate": 9.6,
  "accuracy": 9,
  "drainRate": 6,
  "artist": "xi",
  "title": "Blue Zenith",
  "difficultyName": "FOUR DIMENSIONS",
  "author": "Asphyxia",
  "maxCombo": 2402,
  "length": 250800,
  "minBPM": 200,
  "maxBPM": 200,
  "commonBPM": 200,
  "baseVelocity": 1.8,
  "tickRate": 1
}
```

## Recommended setup

In a real deployment, caching is important, so including a redis instance and persistent volumes for both beatmaps and redis data will help you a lot.

Additionally, including a redis config to set a max memory and LRU cache eviction policy is a good idea to keep memory usage from running away.

For real deployments, I also recommend you NOT use the `latest` tag, as this could cause issues if there is a major version released.
You are better off checking for the current latest version in the [releases](https://github.com/Syriiin/difficalcy/releases) and pinning it manually.

`redis.conf`
```
maxmemory 100mb
maxmemory-policy allkeys-lru
```

`compose.yaml`
```yaml
services:
  difficalcy:
    image: ghcr.io/syriiin/difficalcy:latest # PIN ME
    environment:
      - REDIS_CONFIGURATION=cache:6379
    ports:
      - 5000:80
    volumes:
      - beatmaps:/beatmaps
    depends_on:
      - cache

  cache:
    image: redis:latest
    volumes:
      - ./redis.conf:/usr/local/etc/redis/redis.conf
      - redis-data:/data

  volumes:
    beatmaps:
    redis-data:
```

See [Configuration](./configuration.md) for a full list of configuration options.
