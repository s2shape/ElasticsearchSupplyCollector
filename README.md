# ElasticsearchSupplyCollector
A supply collector designed to connect to Elasticsearch

To run Elasticsearch in docker please go to the root directory where docker-complose.yaml is and run this command:

`docker-compose up`

Two docker containers are supposed to start. The first one is the actual Elasticsearch server and the second one is Kibana (GUI for ES). Kibana accessed using this link http://localhost:5601/.

To run the tests please execute:

`./run-tests.sh`