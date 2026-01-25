param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path

$InfrastructureProject = Join-Path $RepoRoot `
  "src\Infrastructure\Portiforce.SimpleAssetAssistant.Infrastructure.EF\Portiforce.SimpleAssetAssistant.Infrastructure.EF.csproj"

$StartupProject = Join-Path $RepoRoot `
  "src\Presentation\Portiforce.SimpleAssetAssistant.Presentation.WebApi\Portiforce.SimpleAssetAssistant.Presentation.WebApi.csproj"

if (-not (Test-Path $InfrastructureProject)) { throw "Infrastructure project not found: $InfrastructureProject" }
if (-not (Test-Path $StartupProject)) { throw "Startup project not found: $StartupProject" }

dotnet ef migrations add $Name `
  --project $InfrastructureProject `
  --startup-project $StartupProject `
  --context AssetAssistantDbContext `
  --output-dir Migrations