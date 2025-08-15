# Versioning

difficalcy follows the [semver](https://semver.org/) versioning style, however there is some nuance that should be noted.

## Major versions

A new major version will only be released when there are _breaking_ changes to public interface.

- Adding a new required parameter on an endpoint
- Removing a parameter from an endpoint
- Removing an entire endpoint
- etc...

## Minor versions

A new minor version can indicate one of two things:

1. A non-breaking change to the API
    - Adding a new optional parameter which defaults to the previous functionality
    - Adding a new endpoint
    - etc...
1. An update to the underlying difficulty calculators
    - This may be seen as a breaking change, since it changes the functionality, however as the API does not change, we consider it only a minor update

## Patch versions

A new patch version indicates general bug fixes or inconsequential package updates.
