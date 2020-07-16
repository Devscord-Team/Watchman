docker build -t $DOCKERHUB_LOGIN/watchman .
docker login -p $DOCKERHUB_PASS -u $DOCKERHUB_LOGIN
docker push $DOCKERHUB_LOGIN/watchman:latest
ssh-keyscan -H $SERVER_ADDRESS >> ~/.ssh/known_hosts
ssh $SSH_USER@$SERVER_ADDRESS -i /tmp/deploy_rsa 'curl -o docker-compose.yml https://raw.githubusercontent.com/Devscord-Team/Watchman/master/docker-compose.yml && docker-compose pull && docker-compose down && docker-compose up -d && exit'