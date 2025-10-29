# PatientPortal.Tests.Unit

Unit tests for the PatientPortal service layer and supporting logic. Test coverage is primarily focused on service classes (health issues, visits, messaging, patients, staff, test results, patient-staff connections), with additional coverage for authentication extensions.

## Tech Stack

| Category | Stack |
| --- | --- |
| Runtime | .NET 8.0 |
| Test Framework | xUnit v3 |
| Test Runner | Microsoft Testing Platform (MTP) |
| Coverage | Microsoft.Testing.Extensions.CodeCoverage |
| In-Memory Database | EF Core InMemory |
| Mocking | NSubstitute |

## Quickstart

Run all tests:

```sh
dotnet test
```

Run all tests and generate an HTML coverage report:

```sh
dotnet test -- --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
reportgenerator -reports:tests/PatientPortal.Tests.Unit/bin/Debug/net8.0/TestResults/coverage.cobertura.xml -targetdir:CoverageReport
```

Open `CoverageReport/index.html` in your browser to view the report.

## Project Configuration

- MTP is enabled via these properties in the project file:

  ```xml
  <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
  <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
  ```

- No need for legacy test runner packages unless you require older Visual Studio support.

## Running Tests

From the repo root:

```sh
dotnet test
```

Run only this project:

```sh
dotnet test tests/PatientPortal.Tests.Unit/PatientPortal.Tests.Unit.csproj
```

Filter by class (requires fully qualified class name):

```sh
dotnet test -- --filter-class "PatientPortal.Tests.Unit.Services.StaffServiceTests"
```

Filter by trait:

```sh
dotnet test -- --filter-trait Category=Unit
```

List all options:

```sh
dotnet test -?
```

List all tests:
```sh
dotnet test -p:TestingPlatformCaptureOutput=false -- --list-tests
```

> [!TIP]
> `--list-tests` only works when the Platform Capture Output option is disabled.
> - [xunit Github issue](https://github.com/xunit/xunit/issues/3092#issuecomment-2556234421)
> - [Microsoft: Show Complete Platform Output](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-integration-dotnet-test#show-complete-platform-output)

## Example Test

```csharp
public class ExampleServiceTests
{
    [Fact]
    public void Operation_ValidInput_ReturnsExpected()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

## Coverage

Code coverage is collected via Microsoft.Testing.Extensions.CodeCoverage.

Generate a coverage XML file:

```sh
dotnet test -- --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml
```

Generate an HTML report (requires [ReportGenerator](https://github.com/danielpalme/ReportGenerator)):

```sh
reportgenerator -reports:tests/PatientPortal.Tests.Unit/bin/Debug/net8.0/TestResults/coverage.cobertura.xml -targetdir:CoverageReport
```

Open `CoverageReport/index.html` to view results.

## Further Reading

- [xUnit Getting Started](https://xunit.net/docs/getting-started/v3/getting-started)
- [Microsoft Testing Platform](https://xunit.net/docs/getting-started/v3/microsoft-testing-platform)
- [Code Coverage with MTP](https://xunit.net/docs/getting-started/v3/code-coverage-with-mtp)
- [MTP Command Line Options](https://learn.microsoft.com/dotnet/core/testing/unit-testing-platform-intro#options)
