# difficalcy-catch

## `GET /api/calculator/info`

### Response

```json
{
  "rulesetName": "string",
  "calculatorName": "string",
  "calculatorPackage": "string",
  "calculatorVersion": "string",
  "calculatorUrl": "string"
}
```

## `GET /api/calculator/difficulty`

### Query Parameters

| Name         | Description | Required | Type    |
| ------------ | ----------- | -------- | ------- |
| Accuracy     |             | No       | double  |
| Combo        |             | No       | integer |
| Misses       |             | No       | integer |
| TinyDroplets |             | No       | integer |
| Droplets     |             | No       | integer |
| BeatmapId    |             | Yes      | string  |
| Mods         |             | No       | integer |

### Response

```json
{
  "total": 0
}
```

## `GET /api/calculator/performance`

### Query Parameters

| Name         | Description | Required | Type    |
| ------------ | ----------- | -------- | ------- |
| Accuracy     |             | No       | double  |
| Combo        |             | No       | integer |
| Misses       |             | No       | integer |
| TinyDroplets |             | No       | integer |
| Droplets     |             | No       | integer |
| BeatmapId    |             | Yes      | string  |
| Mods         |             | No       | integer |

### Response

```json
{
  "total": 0
}
```

## `GET /api/calculator/calculation`

### Query Parameters

| Name         | Description | Required | Type    |
| ------------ | ----------- | -------- | ------- |
| Accuracy     |             | No       | double  |
| Combo        |             | No       | integer |
| Misses       |             | No       | integer |
| TinyDroplets |             | No       | integer |
| Droplets     |             | No       | integer |
| BeatmapId    |             | Yes      | string  |
| Mods         |             | No       | integer |

### Response

```json
{
  "difficulty": {
    "total": 0
  },
  "performance": {
    "total": 0
  }
}
```

## `POST /api/calculator/batch/difficulty`

### Body Parameters

```json
[
  {
    "beatmapId": "string",
    "mods": 0,
    "accuracy": 0,
    "combo": 0,
    "misses": 0,
    "tinyDroplets": 0,
    "droplets": 0
  }
]
```

### Response

```json
[
  {
    "total": 0
  }
]
```

## `POST /api/calculator/batch/performance`

### Body Parameters

```json
[
  {
    "beatmapId": "string",
    "mods": 0,
    "accuracy": 0,
    "combo": 0,
    "misses": 0,
    "tinyDroplets": 0,
    "droplets": 0
  }
]
```

### Response

```json
[
  {
    "total": 0
  }
]
```

## `POST /api/calculator/batch/calculation`

### Body Parameters

```json
[
  {
    "beatmapId": "string",
    "mods": 0,
    "accuracy": 0,
    "combo": 0,
    "misses": 0,
    "tinyDroplets": 0,
    "droplets": 0
  }
]
```

### Response

```json
[
  {
    "difficulty": {
      "total": 0
    },
    "performance": {
      "total": 0
    }
  }
]
```
