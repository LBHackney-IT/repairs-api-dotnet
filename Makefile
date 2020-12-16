.PHONY: setup
setup:
	docker-compose build

.PHONY: shell
shell:
	docker-compose run repairs-api bash

.PHONY: build
build:
	docker-compose build repairs-api

.PHONY: serve
serve: build
	docker-compose up repairs-api

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: start-db
start-db:
	docker-compose up -d test-database &

.PHONY: stop-db
stop-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database

.PHONY: restart-db
restart-db: | stop-db start-db

.PHONY: update-db
update-db: start-db
	-dotnet tool install -g dotnet-ef
	CONNECTION_STRING="Host=127.0.0.1;Port=5432;Username=postgres;Password=mypassword;Database=testdb" \
	dotnet ef database update -p RepairsApi -c RepairsApi.V1.Infrastructure.RepairsContext

.PHONY: test
test: start-db
	docker-compose build repairs-api-test && docker-compose up repairs-api-test
