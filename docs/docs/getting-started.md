# Getting Started

## TL;DR Example recommended setup

`redis.conf`
```
maxmemory 100mb
maxmemory-policy allkeys-lru
```

`compose.yaml`
```yaml
services:
  difficalcy-osu:
    image: ghcr.io/syriiin/difficalcy-osu:latest
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

See [API Reference](./api-reference/index.md) for available endpoints.

## Available calculators

difficalcy is available for all four official osu! rulesets:

- osu! - `ghcr.io/syriiin/difficalcy-osu`
- osu!taiko - `ghcr.io/syriiin/difficalcy-taiko`
- osu!catch - `ghcr.io/syriiin/difficalcy-catch`
- osu!mania - `ghcr.io/syriiin/difficalcy-mania`

See [the github packages](https://github.com/Syriiin?tab=packages&repo_name=difficalcy) for the latest list.

For this tutorial, we'll stick with the osu! calculator.

## How to run difficalcy

difficalcy calculators are published as docker images, so you can run it anywhere docker runs.

### Docker

You can run it with docker directly:

```sh
docker run -p 5000:80 ghcr.io/syriiin/difficalcy-osu:latest
```

### Docker Compose

You can run it with docker compose:

```yaml
services:
  difficalcy-osu:
    image: ghcr.io/syriiin/difficalcy-osu:latest
    ports:
      - "5000:80"
```

## How to run a calculation

You can use the `GET /api/calculation` endpoint to calculate the difficulty and performance of a score.

For example, to calculate an SS on [xi - Blue Zenith [FOUR DIMENSIONS]](https://osu.ppy.sh/beatmapsets/292301#osu/658127):

```sh
curl "localhost:5000/api/calculation?BeatmapId=658127"
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

However, using the querystring to pass parameters can be annoying, especially for mods.

You can use the `POST /api/batch/calculation` endpoint to efficiently calculate the difficulty and performance of one or more scores in a batch by passing a JSON body.

For example, the same request as above:

```sh
curl "localhost:5000/api/batch/calculation" \
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
curl "localhost:5000/api/batch/calculation" \
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
curl "localhost:5000/api/batch/calculation" \
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
curl "localhost:5000/api/batch/calculation" \
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
  difficalcy-osu:
    image: ghcr.io/syriiin/difficalcy-osu:latest
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
