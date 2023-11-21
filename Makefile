COMPOSE_TOOLING_RUN = docker compose -f docker-compose.tooling.yml run --rm --build tooling
COMPOSE_E2E = docker compose -f docker-compose.e2e.yml
COMPOSE_E2E_RUN = $(COMPOSE_E2E) run --rm --build e2e-test-runner
COMPOSE_APP_DEV = docker compose -f docker-compose.yml

help:	## Show this help
	@fgrep -h "##" $(MAKEFILE_LIST) | fgrep -v fgrep | sed -e 's/\\$$//' | sed -e 's/##//'

bash:	## Opens bash shell in tooling container
	$(COMPOSE_TOOLING_RUN) bash

test:	## Runs test suite
	$(COMPOSE_TOOLING_RUN) dotnet test

test-e2e:	## Runs E2E test suite
	$(COMPOSE_E2E_RUN)
	$(COMPOSE_E2E) down

build-dev:	## Builds development docker images
	$(COMPOSE_APP_DEV) build

start-dev: build-dev	## Starts development environment
	$(COMPOSE_APP_DEV) up -d

clean-dev:	## Cleans development environment
	$(COMPOSE_APP_DEV) down --remove-orphans
