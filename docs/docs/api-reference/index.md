# API Reference

While all difficalcy calculators have the same routes, their parameters and responses differ due to the differences in the rulesets' scores and calculators.

Thus, each calculator has it's own API Reference:

- [difficalcy-osu](./difficalcy-osu.md)
- [difficalcy-taiko](./difficalcy-taiko.md)
- [difficalcy-catch](./difficalcy-catch.md)
- [difficalcy-mania](./difficalcy-mania.md)

In general, each calculator exposes:

| Endpoint                      | Description                                           |
| ----------------------------- | ----------------------------------------------------- |
| `GET /api/info`               | Returns details of the running calculation engine     |
| `GET /api/difficulty`         | Calculates difficulty of a beatmap                    |
| `GET /api/performance`        | Calculates performance of a score                     |
| `GET /api/calculation`        | Calculates both difficulty and performance of a score |
| `POST /api/batch/difficulty`  | Calculates a batch of difficulties                    |
| `POST /api/batch/performance` | Calculates a batch of performances                    |
| `POST /api/batch/calculation` | Calculates a batch of difficulties and performances   |
