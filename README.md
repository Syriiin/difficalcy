# difficalcy

difficalcy is an osu! difficulty and performance calculator REST API.

See the docs for configuration details https://difficalcy.syrin.me/

## Basic usage

1.  Run the container
    ```sh
    docker run -p 5000:80 ghcr.io/syriiin/difficalcy-osu:latest
    ```
1.  Query the API

    ```sh
    curl "localhost:5000/api/info"
    ```

    ```json
    {
      "rulesetName": "osu!",
      "calculatorName": "Official osu!",
      "calculatorPackage": "osu.Game.Rulesets.Osu",
      "calculatorVersion": "2024.1023.0.0",
      "calculatorUrl": "https://nuget.org/packages/ppy.osu.Game.Rulesets.Osu/2024.1023.0.0"
    }
    ```

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

## Example docker compose setup

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
      - redis-data:/data

  volumes:
    beatmaps:
    redis-data:
```

## Development Setup

```sh
make start-dev
```
