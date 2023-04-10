# Political parties and politicians
Microservice provides basic CRUD operations for manipulation with Politicians and Political Parties
## Make sure
- Ports 5000, 5001 and 1433 are not occupied
- Docker desktop is installed and running

## Environment variables
These environment variables must be set in order to run the application

- PoliticalPartiesApi_ConnectionStrings__DefaultConnection
- PoliticalPartiesApi_ConnectionStrings__MasterConnection
- PoliticalPartiesApi_Database
- PoliticalPartiesApi_Auth0__Domain
- PoliticalPartiesApi_Auth0__Audience
- PoliticalPartiesApi_Auth0__Permissions__0

To achieve that you could create .env file and then pass it to `docker compose` which sets these variables

structure of `.env` file should be following
```
PoliticalPartiesApi_DefaultConnection=defaultString
PoliticalPartiesApi_MasterConnection=masterString
PoliticalPartiesApi_Database=dbexamplename
PoliticalPartiesApi_Auth0_Domain=authDomain
PoliticalPartiesApi_Auth0_Audience=audience
PoliticalPartiesApi_Auth0_Permissions_0=example:permission
```
## Running
- Open terminal
- Cd into folder with docker-compose.yml
- Execute command `docker compose --env-file path-to-.env-file up`

## Usage
- For HTTPS use [https://localhost:5001](https://localhost:5001)
- For HTTP use [http://localhost:5000](http://localhost:5000)

## Swagger
Available endpoints should be in swagger on url [http://localhost:5000/swagger](http://localhost:5000/swagger) (For HTTPS use port 5001)

