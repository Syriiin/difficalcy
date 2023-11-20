COMPOSE_RUN_TOOLING = docker compose -f docker-compose.tooling.yml run --rm --build tooling
COMPOSE_APP_DEV = docker compose -f docker-compose.yml

help:	## Show this help
	@fgrep -h "##" $(MAKEFILE_LIST) | fgrep -v fgrep | sed -e 's/\\$$//' | sed -e 's/##//'

bash:	## Opens bash shell in tooling container
	$(COMPOSE_RUN_TOOLING) bash

test:	## Runs test suite
	$(COMPOSE_RUN_TOOLING) dotnet test

build-dev:	## Builds development docker images
	$(COMPOSE_APP_DEV) build

start-dev:	## Starts development environment
	$(COMPOSE_APP_DEV) up -d

clean-dev:	## Cleans development environment
	$(COMPOSE_APP_DEV) down --remove-orphans
