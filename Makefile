COMPOSE_RUN_TOOLING = docker compose -f docker-compose.tooling.yml run --rm --build tooling

help:	## Show this help
	@fgrep -h "##" $(MAKEFILE_LIST) | fgrep -v fgrep | sed -e 's/\\$$//' | sed -e 's/##//'

bash:	## Opens bash shell in tooling container
	$(COMPOSE_RUN_TOOLING) bash

test:	## Runs test suite
	$(COMPOSE_RUN_TOOLING) dotnet test
