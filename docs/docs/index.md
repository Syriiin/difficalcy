# Welcome to difficalcy

Difficalcy is a a simple HTTP API interface for the official osu! difficulty calculators.

## Basic usage

Run the server:

```sh
docker run -p 5000:80 ghcr.io/syriiin/difficalcy-osu:latest
```

Call the API:

```sh
curl "localhost:5000/api/calculation?BeatmapId=658127"
```

Get your lazer powered calculations:

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

See [Getting Started](./getting-started.md) for a full example setup.
