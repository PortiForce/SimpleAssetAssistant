param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path

$InfrastructureProject = Join-Path $RepoRoot `
  "src\Backend\Infra\Portiforce.SAA.Infrastructure.EF\Portiforce.SAA.Infrastructure.EF.csproj"

$StartupProject = Join-Path $RepoRoot `
  "src\Backend\Api\Portiforce.SAA.Api\Portiforce.SAA.Api.csproj"

if (-not (Test-Path $InfrastructureProject)) { throw "Infrastructure project not found: $InfrastructureProject" }
if (-not (Test-Path $StartupProject)) { throw "Startup project not found: $StartupProject" }

dotnet ef migrations add $Name `
  --project $InfrastructureProject `
  --startup-project $StartupProject `
  --context AssetAssistantDbContext `
  --output-dir Migrations