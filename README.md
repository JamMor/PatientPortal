# PatientPortal

PatientPortal is an Electronic Medical Records (EMR) portal built with ASP.NET Core 8.0. It provides a fast, navigable interface for both patients and healthcare providers to store and manage medical history, health issues, visits, care teams, and messaging.

## Table of Contents

- [Technologies Used](#technologies-used)
- [Running the Project](#running-the-project)
  - [Running Locally](#running-locally)
  - [Running with Docker](#running-with-docker)
- [Testing](#testing)

---

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- Pomelo.EntityFrameworkCore.MySql
- Bogus (for test data generation)
- MySQL 8.0
- Docker and Docker Compose
- Bootstrap for front-end styling

## Running the Project

This project can be run locally or with Docker. The application runs on `http://localhost:5000` by default.

First clone the repository:

```bash
git clone <repository-url>
cd PatientPortal
```

### Environment Configuration

To run the project, you must make an `.env` file with the required environment variables whether running locally, or with Docker.

```env
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__Microsoft.Hosting.Lifetime=Information

AllowedHosts=*
DBInfo__ConnectionString=server=<server>;userid=<user_id>;password=<user_password>;port=<port>;database=<db_name>;SslMode=None
```

The `allowedHosts` variable can be set to `*` for local development, and changed as needed for production.

The `DBInfo__ConnectionString` variable must be set to your MySQL database connection string.

- `server`: The MySQL server address (e.g., `localhost` for local MySQL server, and `mysql_db` for the MySQL container in Docker).
- `user_id`: The MySQL user ID.
- `user_password`: The password for the MySQL user.
- `db_name`: The name of the MySQL database.
- `port`: The port number for the MySQL server (default is `3306`).

---

### Running Locally

#### Prerequisites

- .NET 8 SDK
- MySQL Server 8.0

#### 1. Configure Environment Variables

Put your [`.env` file](#environment-configuration) in the root directory of the project, `/src/PatientPortal/`.

#### 2. Restore and Build

> **Note**: The following commands assume you are in the root directory of the repo, but you can also navigate to the `src/PatientPortal/` directory to run them without specifying the project file every line.

```bash
dotnet restore src/PatientPortal/PatientPortal.csproj
dotnet build src/PatientPortal/PatientPortal.csproj
```

#### 3. Apply Migrations

```bash
dotnet ef database update --project src/PatientPortal/PatientPortal.csproj
```

#### 4. Run the Application

```bash
dotnet run --project src/PatientPortal/PatientPortal.csproj
```

The app will be available at `http://localhost:5000`.

---

### Running with Docker

Using Docker Compose, you can run both the ASP.NET Core app and MySQL database in containers.

#### 1. Environment Files

First save the former [`.env` file](#environment-configuration) in `src/PatientPortal/` as `.env.docker.dev`. This location and name can be changed in the compose file.

Make sure the MySql server uses the service name `mysql_db` as the server in your connection string. This too can be changed in the compose file if desired.

Your MySQL user ID *cannot* be `root` when using Docker, so choose a different user ID.

Next, create the env file for your MySQL container in `src/PatientPortal/` as `mysql.env`.

  ```env
  MYSQL_ROOT_PASSWORD=<root_password>
  MYSQL_USER=<user_id>
  MYSQL_PASSWORD=<user_password>
  MYSQL_DATABASE=<db_name>
  ```

  Make sure to use the same MySQL info in both files:

#### 2. **(Optional)** Initialize Database

If you want to initialize the database with data the first time, you must create an initialization script and mount it into the MySQL container. See the MySQL Docker image [for details](https://hub.docker.com/_/mysql#initializing-a-fresh-instance).

If you have the SDK installed, you can create a migration script from your current migrations with:

```bash
dotnet ef migrations script -o init.sql
```

Once done, you can create an en env file at the root of the `Docker` directory, `src/PatientPortal/Docker/.env`, that sets `DB_INIT_PATH` to the path of your script, so that compose can mount it into the MySQL container.

  ```env
  DB_INIT_PATH="<path-to-your-script>/init.sql"
  ```

#### 3. Build and Run with Docker Compose

From the `Docker` directory:

```bash
docker-compose up -d
```

This will build the ASP.NET Core image, start the MySQL container, and initialize the database using the migration script specified in `DB_INIT_PATH`.

The app will be available at `http://localhost:5000`.

## Testing

Unit tests for the PatientPortal service and logic components are located in the `tests/PatientPortal.Tests.Unit` directory.

More information can be found in the [PatientPortal Unit Tests README.md](tests/PatientPortal.Tests.Unit/README.MD).
