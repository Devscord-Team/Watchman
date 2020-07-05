docker build -t $DOCKERHUB_LOGIN/watchman .
docker login --password $DOCKERHUB_PASS --username $DOCKERHUB_LOGIN
docker push $DOCKERHUB_LOGIN/watchman:latest
ssh $SSH_USER@$SERVER_ADDRESS
curl -o docker-compose.yml https://raw.githubusercontent.com/Devscord-Team/Watchman/master/docker-compose.yml
docker-compose up -d