# BookFinder API

## Project Overview
BookFinder API is a Web API built with .NET 8 as part of a technical assessment for the Junior Backend Developer position. The project integrates with the Open Library API to retrieve information about books and authors, processes this data, and persists it to a SQL Server database, which is orchestrated via Docker Compose.

In addition to the main functionality, the API implements a complete authentication and authorization system via JWT, CRUD for local data and pagination.

## Features
- Third-Party API Integration: Search for books by author and publication year in the Open Library.
- Docker Database: Consistent and easy-to-configure development environment with SQL Server running in a Docker container.
- Authentication and Authorization: Secure registration and login system with JSON Web Tokens (JWT). Protected endpoints that require authentication.
- Full CRUD: Create, Read, Update, and Delete operations for authors saved in the database.
- Pagination: Endpoints that return data lists are paginated to ensure API performance and scalability.
- Clean Architecture: The project is organized into layers of responsibility (API, Domain, Infrastructure), following market best practices.


## Technologies Used
- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server (Docker)
- Docker Compose
- BCrypt.Net-Next (for password hashing)

## Prerequisites
Before you begin, make sure you have the following tools installed:
- .NET 8 SDK
- Docker Desktop


## How to Set Up and Run
Follow these steps to have your project running locally in minutes.

1. Clone the repository:
```bash
git clone https://github.com/josiasdev/BookFinderApi
cd BookFinderApi
```

2. Check the Configuration Files:
The docker-compose.yml and BookFinder.Api/appsettings.json files are already pre-configured. The default database password is BookFinder2509. If you want to change it, remember to change it in both files:
- docker-compose.yml: in the SA_PASSWORD environment variable.
- BookFinder.Api/appsettings.json: in the ConnectionString.

3. Start the SQL Server Container:
This command will download the SQL Server image (first-time only) and start the container in the background.

```bash
docker-compose up -d
```
Wait about 1 minute for the SQL Server service inside the container to fully initialize.


4. Apply Database Migrations:
This command will create the BookFinderDB database and all necessary tables within the container.
```bash
dotnet ef database update --startup-project BookFinder.Api
```

5. Run the API:
```bash
dotnet run --project BookFinder.Api/BookFinder.Api.csproj
```

6. Access the Documentation and Test:
The API will be running. Access the interactive Swagger documentation to test all endpoints:
- URL: https://localhost:PORT/swagger (the port is usually 7xxx, check your terminal output).


## Project Structure
The project uses a layered architecture to separate responsibilities:
- BookFinder.Domain: Contains the database entities (Author, Book, User) and the DTOs (Data Transfer Objects), which define the application's data "contracts."
- BookFinder.Infrastructure: Responsible for data access (DbContext) and communication with external services (integration with the Open Library API, token generation service).
- BookFinder.Api: The presentation layer. Contains the Controllers, which expose the HTTP endpoints, and the application entry point (Program.cs).

## API Endpoints

Below is a list of the main available endpoints.

## API Endpoints

Below is a list of the main available endpoints.

| HTTP Method | Endpoint | Description | Requires Authentication? |
| :---------- | :--------------------------------------- | :------------------------------------------------------ | :------------------- |
| `POST` | `Auth/register` | Registers a new user. | No |
| `POST` | `/Auth/login` | Authenticates a user and returns a JWT token. | No |
| `GET` | `/Books` | Lists all saved authors (paginated). | **Yes** |
| `POST` | `/Books/search-and-save/{authorName}` | Searches for an author in Open Library and saves the data. | **Yes** |
| `GET` | `/Books/author/{id}` | Searches for a specific author by their ID. | **Yes** |
| `PUT` | `/Books/author/{id}` | Updates an author's name. | **Yes** |
| `DELETE` | `/Books/author/{id}` | Deletes an author and their books. | **Yes** |
| `GET` | `/Books/count` | Returns the total number of books in the database. | **Yes** |
| `GET` | `/{year}` | Searches for books in Open Library by publication year (paginated). | **Yes** |


## Third-Party API Link
- API: Open Library API
- Documentation: https://openlibrary.org/developers/api
