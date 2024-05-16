# Configuration

difficalcy is designed to be simple to get up and running, so there are no _required_ configurations.

However it would be a good idea to consider these configuration options for a real application deployment.

## Environment Variables

| Environment variable  | Default | Description                                                                                     |
| --------------------- | ------- | ----------------------------------------------------------------------------------------------- |
| `REDIS_CONFIGURATION` |         | The address of the redis server to use for beatmap caching. By default, there will be no cache. |

## Docker volumes

By default these paths will use anonymous volumes, but you may want to create and mount named volumes here, to ensure persistent storage, or to share with other containers.

| Path        | Description                                                                                             |
| ----------- | ------------------------------------------------------------------------------------------------------- |
| `/beatmaps` | This is the path where difficalcy will use to cache beatmaps. Beatmaps are stored as `<beatmapid>.osu`. |
