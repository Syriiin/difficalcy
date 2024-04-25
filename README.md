# difficalcy

difficalcy is an osu! difficulty and performance calculator REST API.

## Basic usage

1.  Run the container
    ```sh
    docker run -p 5000:80 ghcr.io/syriiin/difficalcy-osu:latest
    ```
1.  Query the API

    ```sh
    curl "localhost:5000/api/calculator/info"
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
    curl "localhost:5000/api/calculator/calculation?BeatmapId=658127"
    ```

    ```json
    {
      "difficulty": {
        "aim": 3.471503344019442,
        "speed": 3.4738667283055444,
        "flashlight": 4.58994045567377,
        "total": 7.255439646986892
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
      - beatmaps:/home/app/beatmaps
    depends_on:
      - cache

  cache:
    image: redis:latest

  volumes:
    beatmaps:
```

## Development Setup

```sh
make start-dev
```
