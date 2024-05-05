# difficalcy

difficalcy is an osu! difficulty and performance calculator REST API.

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
      "calculatorVersion": "2023.1114.1.0",
      "calculatorUrl": "https://nuget.org/packages/ppy.osu.Game.Rulesets.Osu/2023.1114.1.0"
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
        "aim": 3.4715033440194416,
        "speed": 3.4738667283055444,
        "flashlight": 0,
        "total": 7.25543964698689
      },
      "performance": {
        "aim": 220.83646290283872,
        "speed": 231.26239294786578,
        "accuracy": 142.3199671239901,
        "flashlight": 0,
        "total": 614.5217398659557
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
