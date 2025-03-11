# Updating guide

There are some common changes that need to be made when changes are made to the difficulty calculators

## Updating difficulty attributes

To ensure our caching functions correctly, we need to make sure we serialise the required difficulty attributes.

These can be checked by looking at the `FromDatabaseAttributes` method of the difficulty attribute classes and adding them to any skill difficulty values attributes (eg. `AimDifficulty`).

## Updating test values

Often the test values need to be updated. The easiest way to check for updated values is to use [osu-tools](https://github.com/ppy/osu-tools) to run the diffcalc with settings matching our tests.

### osu

```sh
$ dotnet run -- simulate osu ~/git/difficalcy/Difficalcy.Osu/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json

$ dotnet run -- simulate osu ~/git/difficalcy/Difficalcy.Osu/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod DT

$ dotnet run -- simulate osu ~/git/difficalcy/Difficalcy.Osu/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod HD \
    --mod HR \
    --mod FL \
    --mod DT \
    --mod-option DT_speed_change=2 \
    --misses 5 \
    --mehs 4 \
    --goods 3 \
    --large-tick-misses 81 \
    --slider-tail-misses 31 \
    --combo 200

$ dotnet run -- simulate osu ~/git/difficalcy/Difficalcy.Osu/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod CL \
    --mod HD \
    --mod HR \
    --mod FL \
    --mod DT \
    --mod-option DT_speed_change=2 \
    --misses 5 \
    --mehs 4 \
    --goods 3 \
    --large-tick-misses 81 \
    --slider-tail-misses 31 \
    --combo 200
```

### taiko

```sh
$ dotnet run -- simulate taiko ~/git/difficalcy/Difficalcy.Taiko/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json

$ dotnet run -- simulate taiko ~/git/difficalcy/Difficalcy.Taiko/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod DT

$ dotnet run -- simulate taiko ~/git/difficalcy/Difficalcy.Taiko/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod HR \
    --mod DT \
    --mod-option DT_speed_change=2 \
    --misses 5 \
    --goods 3 \
    --combo 150
```

### catch

```sh
$ dotnet run -- simulate catch ~/git/difficalcy/Difficalcy.Catch/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json
$ dotnet run -- simulate catch ~/git/difficalcy/Difficalcy.Catch/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod DT
$ dotnet run -- simulate catch ~/git/difficalcy/Difficalcy.Catch/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod HR \
    --mod DT \
    --mod-option DT_speed_change=2 \
    --misses 5 \
    --droplets 18 \
    --tiny-droplets 200 \
    --combo 100
```

### mania

```sh
$ dotnet run -- simulate mania ~/git/difficalcy/Difficalcy.Mania/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --greats 0
$ dotnet run -- simulate mania ~/git/difficalcy/Difficalcy.Mania/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod DT \
    --greats 0
$ dotnet run -- simulate mania ~/git/difficalcy/Difficalcy.Mania/Resources/Testing/Beatmaps/diffcalc-test.osu \
    --json \
    --mod DT \
    --mod-option DT_speed_change=2 \
    --misses 5 \
    --mehs 4 \
    --oks 3 \
    --goods 2 \
    --greats 1
```
