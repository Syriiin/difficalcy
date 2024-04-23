# Getting Started

## TL;DR Example recommended setup

```yaml
services:
  difficalcy-osu:
    image: ghcr.io/syriiin/difficalcy-osu:latest
    environment:
      - REDIS_CONFIGURATION=cache:6379
    ports:
      - "5000:80"
    volumes:
      - beatmaps:/app/beatmaps
    depends_on:
      - cache

  cache:
    image: redis:latest

  volumes:
    beatmaps:
```

## Available calculators

difficalcy is available for all four official osu! rulesets:

- osu! - `ghcr.io/syriiin/difficalcy-osu:latest`
- osu!taiko - `ghcr.io/syriiin/difficalcy-taiko:latest`
- osu!catch - `ghcr.io/syriiin/difficalcy-catch:latest`
- osu!mania - `ghcr.io/syriiin/difficalcy-mania:latest`

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

## Configuration

difficalcy is designed to be simple to get up and running, so there are no _required_ configurations.

By default, the beatmap cache will be lost when the container is restarted, and there will be no caching for calculations.

### Environment Variables

| Environment variable  | Default         | Description                                                |
| --------------------- | --------------- | ---------------------------------------------------------- |
| `BEATMAP_DIRECTORY`   | `/app/beatmaps` | The directory difficalcy uses for storing beatmap files    |
| `REDIS_CONFIGURATION` |                 | The address of the redis server to use for beatmap caching |
