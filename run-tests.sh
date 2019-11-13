#!/bin/sh

docker-compose up -d

sleep 20

export ELASTIC_HOST=localhost
export ELASTIC_PORT=9200
export ES_JAVA_OPTS="-Xms512m -Xmx512m"
export ELASTICSEARCH_URL="http://localhost:9200"

dotnet restore -s https://www.myget.org/F/s2/ -s https://api.nuget.org/v3/index.json
dotnet build
dotnet publish

pushd ElasticsearchSupplyCollectorLoader/bin/Debug/netcoreapp2.2/publish
dotnet SupplyCollectorDataLoader.dll -xunit ElasticsearchSupplyCollector $ELASTICSEARCH_URL
popd

dotnet test

docker-compose down -v