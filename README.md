# PatientPortal

PatientPortal is an Electronic Medical Records (EMR) portal built with ASP.NET Core 3.1. It is designed to provide a simple, fast, and more navigable interface for both patients and healthcare providers. Store and manage detailed patient information, including medical history, health issues, visit summaries, care team, and messaging history.

## Technologies Used

- ASP.NET Core 3.1
- Entity Framework Core 5.0
- MySQL 8.0
- Docker and Docker Compose
- Bootstrap for front-end styling

## Table of Contents

- [Technologies Used](#technologies-used)
- [Running the Project](#running-the-project)
  - [Running Locally](#running-locally)
  - [Running with Docker](#running-with-docker)

---

## Running the Project

This project can either be run from a pre-built Docker image, or built and run directly on your local machine using the steps outlined below. The application runs on `https://localhost:5000` by default.

First clone the repository:

```bash
git clone <repository-url>
cd PatientPortal
```

---

### Running Locally

This section describes how to build and run the project locally on your machine. You must also set up a MySQL database to store the patient data.

#### Local Prerequisites

- .NET Core SDK 3.1
- MySQL Server 8.0.23

#### 1. Create an `.env` file

The app will pull these environment variables from a `.env` in the root directory that overrides the standard `appsettings.json`.

```env
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__Microsoft.Hosting.Lifetime=Information

AllowedHosts=*
DBInfo__ConnectionString=server=<server>;userid=<user_id>;password=<user_password>;port=<port>;database=<db_name>;SslMode=None
```

Replace the placeholders in the `.env.docker.dev` and `mysql.env` files with your MySQL database credentials:

- `server`: The MySQL server address (e.g., `localhost` for local MySQL server).
- `user_id`: The MySQL user ID.
- `user_password`: The password for the MySQL user.
- `db_name`: The name of the MySQL database.
- `port`: The port number for the MySQL server (default is `3306`).

#### 2. Restore dependencies and build the project

```bash
dotnet restore
dotnet build
```

#### 3. Create the database and apply migrations

```bash
dotnet ef database update
```

#### 4. Run the application

```bash
dotnet run
```

---

### Running with Docker

Using Docker Compose, you can spin up the application and a MySQL database. For more information on the operation of the MySQL image, see the [MySQL Docker Hub page](https://hub.docker.com/_/mysql).

#### Docker Prerequisites

- Docker
- .NET Core SDK 3.1 (if you want to generate the migration file)

#### 1. Create environment files

Create these two `.env` files in the root directory:

- `.env.docker.dev`

```env
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__Microsoft.Hosting.Lifetime=Information

AllowedHosts=*
DBInfo__ConnectionString=server=<server>;userid=<user_id>;password=<user_password>;port=3306;database=<db_name>;SslMode=None
```

- `mysql.env`

```env
MYSQL_ROOT_PASSWORD=<root_password>
MYSQL_USER=<user_id>
MYSQL_PASSWORD=<user_password>
MYSQL_DATABASE=<db_name>
```

Replace the placeholders in the `.env.docker.dev` and `mysql.env` files with your MySQL database credentials:

- `server` is the name of your MySQL service in the Docker Compose file (e.g., `mysql_db`).
- `root_password` is the root password for your MySQL database.
- `user_id` is the user ID for your MySQL database (**not** `root`).
- `user_password` is the password for your MySQL database.
- `db_name` is the name of your MySQL database.
- `port`: The port number for the MySQL server (default is `3306`).

#### 2. Create a migration script to initialize the database

```bash
dotnet ef migrations script -o init.sql
```

This command will create a migration file named `init.sql` in the root directory. It will be mounted as a volume to the container.

**Note:** Currently, this requires having the .NET SDK installed by the user. Future versions will have the migration script created during the Docker build process and included in the image.

#### 3. Create an `.env` file in the Docker directory

```env
DB_INIT_PATH="<path_to_your_init_sql_file>"
```

Replace `<path_to_your_init_sql_file>` with the path to the `init.sql` file created in the previous step. This path is used to mount the SQL file as a volume in the MySQL Docker container, which will be used to initialize the database.

#### 4. Build and run the application using Docker Compose

```bash
docker-compose -f docker-compose.dev.yml up -d
```

This command will build the Docker images and start the containers defined in the `docker-compose.dev.yml` file.
