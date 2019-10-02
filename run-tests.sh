#!/bin/sh

docker-compose up -d

sleep 20

export ELASTIC_HOST=localhost
export ELASTIC_PORT=9200

cd ElasticsearchDataLoader

dotnet build

dotnet run

cd ../ElasticsearchSupplyCollector.Tests

dotnet test

cd ..

docker-compose down -v