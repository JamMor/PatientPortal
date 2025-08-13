# PatientPortal.SeedTool

A standalone .NET console tool for seeding the PatientPortal project's database with mock/test data. Intended for local development, testing, and portfolio demonstration purposes. This tool enables you to quickly create a "working" database so visitors can explore the PatientPortal project with realistic data.

> [!IMPORTANT]
> The tool reads the database connection string from the main project's `appsettings.json` by default, but you can override it via the `--connection-string` option.

## Tech Stack

| Category | Stack |
| --- | --- |
| Runtime | .NET 8.0 (C#) |
| CLI Parsing | System.CommandLine 2.0.3 |
| Fake Data | Bogus 35.6 |
| ORM | Entity Framework Core 8.0.16 |
| Identity | ASP.NET Core Identity (EntityFrameworkCore 8.0) |
| Configuration | Microsoft.Extensions.Configuration.Json 8.0.0 |

## Quick Start

```sh
# Clone the repository
git clone https://github.com/JamMor/PatientPortal.git
cd PatientPortal

# Restore dependencies
dotnet restore

# Run the seed tool
dotnet run --project tools/PatientPortal.SeedTool -- [options]
```

## Configuration

By default, the tool reads the database connection string from the main PatientPortal project's `appsettings.json` file. You can override this by providing the `--connection-string` option.

| Variable/Option | Required | Purpose | Example Value |
| --- | --- | --- | --- |
| `--connection-string` or `CONNECTION_STRING` | No | Database connection string (overrides config file) | `Server=...;...` |

## Running the Project

### Local Development, Testing, and Demo

```sh
dotnet restore
dotnet run --project tools/PatientPortal.SeedTool -- [options]
```

### CLI Options

You can control what data is seeded using the following options:

| Option | Description |
| --- | --- |
| `--presets` | Seed preset staff members and patients |
| `--staff <number>` | Number of staff members to seed (must be positive integer) |
| `--patients <number>` | Number of patients to seed (must be positive integer) |
| `--messages` | Seed conversations and messages for all messaging links under the conversation threshold |
| `--connection-string` | Database connection string (optional, overrides value from main project's appsettings.json) |

You can combine options as needed. If no options are specified, the tool will exit without making changes.

#### Examples

Seed 10 staff and 50 patients:

```sh
dotnet run --project tools/PatientPortal.SeedTool --staff 10 --patients 50
```

Seed only preset demo users:

```sh
dotnet run --project tools/PatientPortal.SeedTool --presets
```

Seed messages for all links:

```sh
dotnet run --project tools/PatientPortal.SeedTool --messages
```

Use a custom connection string:

```sh
dotnet run --project tools/PatientPortal.SeedTool --staff 5 --connection-string "Server=localhost;..."
```

## Usage Notes

> [!IMPORTANT]
> This tool is intended for local, test, and demo/portfolio use only.

To populate a deployed instance with seeded data, export a dump from your local database and import it on the target server. When using Docker, you can seed a fresh MySQL container by mounting a `.sql` dump into `/docker-entrypoint-initdb.d` — the MySQL image will execute it on first startup. See the [MySQL Docker image docs](https://hub.docker.com/_/mysql#initializing-a-fresh-instance) for details.

## License

This project is unlicensed and intended for personal or internal use.

## Author

JamMor
