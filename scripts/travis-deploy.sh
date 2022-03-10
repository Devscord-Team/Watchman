docker build -t $DOCKERHUB_LOGIN/watchman-web .
docker login -p $DOCKERHUB_PASS -u $DOCKERHUB_LOGIN
docker push $DOCKERHUB_LOGIN/watchman-web:latest