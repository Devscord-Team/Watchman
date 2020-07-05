docker build -t $DOCKERHUB_LOGIN/watchman .
docker login --username $DOCKERHUB_LOGIN
$DOCKERHUB_PASS
docker push gagyn/watchman:latest
ssh $SERVER_ADRESS
$SERVER_PASS
curl -o docker-compose.yml https://raw.githubusercontent.com/Devscord-Team/Watchman/master/docker-compose.yml
docker-compose up -d