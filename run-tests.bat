docker-compose up -d

sleep 20

cd ElasticsearchSupplyCollector/ElasticsearchDataLoader

dotnet build

dotnet run

cd ../ElasticsearchSupplyCollector.Tests

dotnet test

cd ../../

docker-compose down -v