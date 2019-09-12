docker-compose up -d

sleep 20

set ELASTIC_HOST=localhost
set ELASTIC_PORT=9200

cd ElasticsearchSupplyCollector/ElasticsearchDataLoader

dotnet build

dotnet run

cd ../ElasticsearchSupplyCollector.Tests

dotnet test

cd ../../

docker-compose down -v