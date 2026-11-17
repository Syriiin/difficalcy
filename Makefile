COMPOSE_E2E = docker compose -f compose.e2e.yaml
COMPOSE_E2E_RUN = $(COMPOSE_E2E) run --rm --build e2e-test-runner
COMPOSE_APP_DEV = docker compose -f compose.yaml -f compose.override.yaml
COMPOSE_RUN_DOCS = docker compose -f compose.yaml -f compose.override.yaml run --rm --build docs
REPO = ghcr.io/syriiin/difficalcy

help:	## Show this help
	@fgrep -h "##" $(MAKEFILE_LIST) | fgrep -v fgrep | sed -e 's/\\$$//' -e 's/##//' -e 's/check-flake[[:space:]]*//'

check-flake:
	@if [ -z "$$DIFFICALCY_FLAKE_HASH" ]; then \
		echo "Error: not in the Nix dev shell. Run 'nix develop' (or 'make shell') first." >&2; \
		exit 1; \
	fi
	@if [ "$$DIFFICALCY_FLAKE_HASH" != "$$(cat flake.nix flake.lock | sha256sum | cut -d' ' -f1)" ]; then \
		echo "Error: flake.nix or flake.lock changed since the shell started. Re-enter with 'nix develop'." >&2; \
		exit 1; \
	fi

shell:  ## Runs a bash shell with dev tooling
	nix develop

test:	check-flake	## Runs test suite
	dotnet test

test-e2e:	check-flake	## Runs E2E test suite (slim + full)
	$(COMPOSE_E2E_RUN)
	$(COMPOSE_E2E) down --volumes

update-e2e-snapshot:	check-flake	## Regenerates the E2E response snapshot
	UPDATE_SNAPSHOTS=true $(COMPOSE_E2E_RUN)
	$(COMPOSE_E2E) down --volumes

build-dev:	check-flake	## Builds development docker images
	$(COMPOSE_APP_DEV) build

start-dev: build-dev	## Starts development environment
	$(COMPOSE_APP_DEV) up -d

clean-dev:	check-flake	## Cleans development environment
	$(COMPOSE_APP_DEV) down --remove-orphans

reset-dev:	check-flake	## Resets development environment
	$(COMPOSE_APP_DEV) down --remove-orphans --volumes

update-api-reference:	check-flake	## Updates OpenAPI schemas in docs site
	scripts/update-api-reference.sh

check-api-reference: check-flake	## Checks OpenAPI schemas are updated
	scripts/check-api-reference.sh

build-docs:	check-flake	## Builds documentation site
	$(COMPOSE_RUN_DOCS) build --strict --clean

restore-dotnet-tooling: check-flake
	dotnet tool restore

check-formatting: restore-dotnet-tooling	## Checks code formatting
	dotnet tool run csharpier check .
	treefmt --ci

fix-formatting: restore-dotnet-tooling	## Fix code formatting
	dotnet tool run csharpier format .
	treefmt

VERSION =
release:	check-flake	## Pushes docker images to ghcr.io and creates a github release
ifndef VERSION
	$(error VERSION is undefined)
endif
ifndef GITHUB_TOKEN
	$(error GITHUB_TOKEN env var is not set)
endif
ifndef GITHUB_USERNAME
	$(error GITHUB_USERNAME env var is not set)
endif
ifneq "$(shell git branch --show-current)" "master"
	$(error This command can only be run on the master branch)
endif
ifneq "$(shell git diff --name-only master)" ""
	$(error There are uncommitted changes in the working directory)
endif
	echo $$GITHUB_TOKEN | docker login ghcr.io --username $$GITHUB_USERNAME --password-stdin
	docker build . --target publish \
	    -t $(REPO):$(VERSION) \
	    -t $(REPO):latest \
	    -t $(REPO):$(VERSION)-slim \
	    -t $(REPO):latest-slim
	docker build . --target publish-full \
	    -t $(REPO):$(VERSION)-full \
	    -t $(REPO):latest-full
	docker push $(REPO):$(VERSION)
	docker push $(REPO):latest
	docker push $(REPO):$(VERSION)-slim
	docker push $(REPO):latest-slim
	docker push $(REPO):$(VERSION)-full
	docker push $(REPO):latest-full
	gh release create "$(VERSION)" --generate-notes

VERSION =
pre-release:	check-flake	## Pushes docker images to ghcr.io and creates a github prerelease
ifndef VERSION
	$(error VERSION is undefined)
endif
ifndef GITHUB_TOKEN
	$(error GITHUB_TOKEN env var is not set)
endif
ifndef GITHUB_USERNAME
	$(error GITHUB_USERNAME env var is not set)
endif
ifneq "$(shell git diff --name-only HEAD)" ""
	$(error There are uncommitted changes in the working directory)
endif
	echo $$GITHUB_TOKEN | docker login ghcr.io --username $$GITHUB_USERNAME --password-stdin
	docker build . --target publish -t $(REPO):$(VERSION) -t $(REPO):$(VERSION)-slim
	docker build . --target publish-full -t $(REPO):$(VERSION)-full
	docker push $(REPO):$(VERSION)
	docker push $(REPO):$(VERSION)-slim
	docker push $(REPO):$(VERSION)-full
	gh release create "$(VERSION)" --generate-notes --prerelease --target "$(shell git branch --show-current)"
