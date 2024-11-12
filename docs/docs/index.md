# Welcome to difficalcy

Difficalcy is a simple HTTP API interface for the official osu! difficulty calculators.

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

See [Getting Started](./getting-started.md) for a full example setup.
