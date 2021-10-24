## Docker
- open repository directory in terminal

### Build and run

```sh
# start
docker-compose up --build
# stop: Ctrl+C
```
or
```sh
# start (detached)
docker-compose up --build -d
# stop
docker-compose stop
```


### Stop and remove containers

```sh
docker-compose down
```

## API

### Documentation
Default endpoints
- ReDoc: http://localhost:5000/redoc
- Swagger: http://localhost:5000/swagger
