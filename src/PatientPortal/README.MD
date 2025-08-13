# PatientPortal

ASP.NET Core 8.0 Electronic Medical Records (EMR) web application. Provides a navigable interface for patients and healthcare providers to manage medical history, health issues, visits, care teams, and messaging.

## Tech Stack

| Category | Stack |
| --- | --- |
| Backend | ASP.NET Core 8.0 (C#) |
| ORM | Entity Framework Core 8.0 |
| Database | MySQL 8.0 |
| Database Driver | Pomelo.EntityFrameworkCore.MySql 8.0.3 |
| Authentication | ASP.NET Core Identity |
| Frontend UI | Bootstrap v4.3.1 |
| Containerization | Docker, Docker Compose |

## Running the Project

This project can be run locally or with Docker. The application runs on `http://localhost:5000` by default.

First clone the repository:

```bash
git clone https://github.com/JamMor/PatientPortal.git
cd PatientPortal
```

### Environment Configuration

To run the project, you must create a `.env` file with the required environment variables, whether running locally or with Docker.

```env
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__Microsoft.Hosting.Lifetime=Information

AllowedHosts=*
DBInfo__ConnectionString=server=<server>;userid=<user_id>;password=<user_password>;port=<port>;database=<db_name>;SslMode=None
```

`AllowedHosts` can be set to `*` for local development and changed as needed for production.

The `DBInfo__ConnectionString` variables:

- `server`: MySQL server address (`localhost` for local, `mysql_db` when using the Docker Compose setup)
- `user_id`: MySQL user ID
- `user_password`: MySQL user password
- `db_name`: MySQL database name
- `port`: MySQL port (default: `3306`)

---

### Running Locally

#### Prerequisites

- .NET 8 SDK
- MySQL Server 8.0

#### 1. Configure Environment Variables

Place your [`.env` file](#environment-configuration) in `src/PatientPortal/`.

#### 2. Restore and Build

> [!NOTE]
> The following commands assume you are in the root directory of the repo. You can also navigate to `src/PatientPortal/` and omit the project path from each command.

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

Save the [`.env` file](#environment-configuration) in `src/PatientPortal/` as `.env.docker.dev`. The location and name can be changed in the compose file.

Make sure the MySQL server uses the service name `mysql_db` in your connection string (or update the compose file accordingly).

Your MySQL user ID *cannot* be `root` when using Docker.

Next, create a MySQL container env file at `src/PatientPortal/mysql.env`:

```env
MYSQL_ROOT_PASSWORD=<root_password>
MYSQL_USER=<user_id>
MYSQL_PASSWORD=<user_password>
MYSQL_DATABASE=<db_name>
```

Make sure the MySQL credentials match between both files.

#### 2. (Optional) Initialize Database

To initialize the database with the migrations on first run, create a SQL initialization script and mount it into the MySQL container. See the MySQL Docker image docs for [details](https://hub.docker.com/_/mysql#initializing-a-fresh-instance).

Generate a migration script from your current migrations:

```bash
dotnet ef migrations script -o init.sql
```

Then create a `Docker/.env` file that sets the path to your script:

```env
DB_INIT_PATH="<path-to-your-script>/init.sql"
```

> [!IMPORTANT]
> If wanting to initialize with a dump of an existing database, export a `.sql` dump and mount it instead. See further explantion with the [SeedTool](#seeding-mock-data) below.

#### 3. Build and Run with Docker Compose

From the `Docker` directory:

```bash
docker-compose up -d
```

This will build the ASP.NET Core image, start the MySQL container, and initialize the database using the migration script specified in `DB_INIT_PATH`.

The app will be available at `http://localhost:5000`.

---

## Seeding Mock Data

To populate the database with mock data for local development or demo purposes, use the [PatientPortal.SeedTool](../../tools/PatientPortal.SeedTool/README.md).

## Testing

Unit tests are located in [tests/PatientPortal.Tests.Unit](../../tests/PatientPortal.Tests.Unit/README.MD).
