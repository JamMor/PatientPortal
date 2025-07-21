# PatientPortal.SeedTool

A .NET CLI tool for seeding the PatientPortal project's database. Designed for developers to quickly initialize or reset local and test environments.

## Features

- Seeds PatientPortal database with initial/test data
- Command-line interface for easy automation
- Supports .NET 8.0 environments

## Tech Stack

| Category | Stack |
| --- | --- |
| Backend | .NET 8.0 (C#) |
| Tooling | .NET CLI |

## Quick Start

```sh
# Clone the repository
git clone https://github.com/JamMor/PatientPortal.git
cd PatientPortal/tools/PatientPortal.SeedTool

# Restore dependencies
dotnet restore

# Run the seed tool
dotnet run
```

## Environment Variables

| Variable Name | Required | Purpose | Example Value |
| --- | --- | --- | --- |
| `CONNECTION_STRING` | Yes | Database connection string | `Server=...;...` |

## Running the Project

### Local Development

```sh
dotnet restore
dotnet run
```

## Available Scripts / CLI Commands

| Command | Description |
| --- | --- |
| `dotnet run` | Runs the seed tool |
| `dotnet publish` | Builds for production |

## Testing

_No tests implemented yet._

## Project Structure / Architecture Overview

- Project is located in `tools/PatientPortal.SeedTool`
- Designed to interact with the PatientPortal database

## Documentation

- No additional documentation at this time.

## Deployment Notes

- Intended for local and test environment use only

## Roadmap / Future Improvements

- TBD

## License

This project is unlicensed and intended for personal or internal use.

## Author

JamMor
