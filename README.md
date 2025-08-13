# PatientPortal

PatientPortal is a portfolio project — a full-stack Electronic Medical Records (EMR) portal built with ASP.NET Core 8.0. It demonstrates a realistic patient/provider workflow with a working database, role-based access, and a clean navigable UI.

<!-- > **Demo:** _Link coming soon_
>
> **Screenshot:** _Coming soon_ -->

## Features

- Patient and staff accounts with role-based access control
- Medical history, health issues, and visit records
- Test result tracking
- In-app messaging between patients and their care team
- Staff and patient management

## Repository Structure

| Path | Description |
| --- | --- |
| [`src/PatientPortal`](src/PatientPortal/README.MD) | Main EMR web application (ASP.NET Core 8.0) |
| [`tools/PatientPortal.SeedTool`](tools/PatientPortal.SeedTool/README.md) | CLI tool for seeding the database with mock data |
| [`tests/PatientPortal.Tests.Unit`](tests/PatientPortal.Tests.Unit/README.MD) | Service-layer unit test suite |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL Server 8.0

## Getting Started

See the [PatientPortal web app README](src/PatientPortal/README.md) for full setup and run instructions, including local development and Docker Compose.
