# Politicz.PoliticiansAndParties

Politicz.PoliticiansAndParties is a reference project that demonstrates the usage of Dapper, Fluent Validations, and Docker in an ASP.NET Core back-end application. The project aims to provide a source of politicians and political parties.

## Features

- CRUD operations for politicians and political parties

## Technologies Used

- ASP.NET Core 7.0
- Dapper
- Fluent Validation
- Microsoft SQL Server
- Docker

## Environment variables
    - PoliticalPartiesApi_Database__DefaultConnection
    - PoliticalPartiesApi_Database__MasterConnection
    - PoliticalPartiesApi_Database__Name
    - PoliticalPartiesApi_Auth0__Domain
    - PoliticalPartiesApi_Auth0__Audience

## Getting Started

To get started with Politicz.PoliticiansAndParties, follow these steps:

1. Clone the repository: `git clone https://github.com/Politi-cz/politicz-politicians-and-parties.git`
2. Build the Docker image: `docker build -t politiczpoliticiansparties .`
3. Run the Docker container with all necessary environment variables: `docker run -p 8080:80 politiczpoliticiansparties`

## Contributing

Contributions to Politicz.PoliticiansAndParties are welcome and encouraged! To contribute, follow these steps:

1. Fork the repository
2. Create a new branch for your changes: `git checkout -b my-feature-branch`
3. Make your changes and commit them: `git commit -am 'Add some feature'`
4. Push your changes to your fork: `git push origin my-feature-branch`
5. Submit a pull request to the main repository

## Contact

If you have any questions or suggestions, please feel free to contact us. We'd love to hear from you!
