<#
.SYNOPSIS
    Generates DTOs (Create, Response, Filter) and Service Interface for a domain entity.

.DESCRIPTION
    Semi-automated DTO and Service Interface generation for M-TAU.Application layer.
    Creates 3 DTO records (Create, Response, Filter) + 1 Service Interface based on entity definition.
    Requires .NET entity class in M-TAU.Domain to be analyzed.

.PARAMETER Entity
    Name of the domain entity (e.g., "Product", "Order", "Message")

.PARAMETER Namespace
    Aggregate/Domain namespace folder (e.g., "Catalog" for M_TAU.Domain.Catalog.Product)

.PARAMETER Properties
    Comma-separated property definitions in format: "PropertyName:PropertyType:IsRequired"
    Example: "Title:string:true,Price:decimal:true,Description:string:false"

.PARAMETER BasePath
    Root path of the M-TAU solution (default: current directory)

.EXAMPLE
    .\code-generator.ps1 -Entity "Product" -Namespace "Catalog" -Properties "Title:string:true,Price:decimal:true,Description:string:false"

.EXAMPLE
    .\code-generator.ps1 -Entity "Order" -Namespace "Transaction" -Properties "BuyerId:Guid:true,ProductId:Guid:true,TotalAmount:decimal:true,Status:string:false"

.NOTES
    - Generates files in M-TAU.Application/Dtos/{Namespace}/ and M-TAU.Application/Services/
    - All DTOs use C# 10+ record syntax
    - All Service methods are async (Task/Task<T>)
    - Includes DataAnnotations for validation
    - Does NOT overwrite existing files
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Entity,

    [Parameter(Mandatory = $true)]
    [string]$Namespace,

    [Parameter(Mandatory = $true)]
    [string]$Properties,

    [Parameter(Mandatory = $false)]
    [string]$BasePath = (Get-Location)
)

# Colors for output
$InfoColor = "Cyan"
$SuccessColor = "Green"
$ErrorColor = "Red"
$WarningColor = "Yellow"

function Write-Info { Write-Host "[INFO] $args" -ForegroundColor $InfoColor }
function Write-Success { Write-Host "[✓] $args" -ForegroundColor $SuccessColor }
function Write-Error { Write-Host "[✗] $args" -ForegroundColor $ErrorColor }
function Write-Warning { Write-Host "[!] $args" -ForegroundColor $WarningColor }

# Paths
$AppPath = Join-Path $BasePath "M-TAU.Application"
$DtosPath = Join-Path $AppPath "Dtos\$Namespace"
$ServicesPath = Join-Path $AppPath "Services"

Write-Info "Starting DTO and Service Interface generation..."
Write-Info "Entity: $Entity | Namespace: $Namespace"

# Validate paths
if (-not (Test-Path $AppPath)) {
    Write-Error "M-TAU.Application not found at $AppPath"
    exit 1
}

# Create directories
if (-not (Test-Path $DtosPath)) {
    New-Item -ItemType Directory -Path $DtosPath -Force | Out-Null
    Write-Success "Created directory: $DtosPath"
}

# Parse properties
$PropList = @()
foreach ($prop in $Properties.Split(',')) {
    $parts = $prop.Trim().Split(':')
    if ($parts.Count -eq 3) {
        $PropList += @{
            Name = $parts[0].Trim()
            Type = $parts[1].Trim()
            Required = $parts[2].Trim() -eq "true"
        }
    }
}

if ($PropList.Count -eq 0) {
    Write-Error "No valid properties parsed. Use format: PropertyName:Type:IsRequired"
    exit 1
}

Write-Info "Parsed $($PropList.Count) properties"

# Generate CreateDto
$CreateDtoContent = @"
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.$Namespace;

/// <summary>
/// $Entity Create DTO for creating a new $Entity instance.
/// </summary>
public record ${Entity}CreateDto(
"@

$createProps = @()
foreach ($prop in $PropList) {
    if ($prop.Name -notin @("Id", "CreatedAt", "UpdatedAt")) {
        $attr = ""
        if ($prop.Required) {
            $attr += "    [Required]`n    "
        }
        if ($prop.Type -eq "string") {
            $attr += "[StringLength(255)]`n    "
        }
        elseif ($prop.Type -eq "decimal") {
            $attr += "[Range(0.01, double.MaxValue)]`n    "
        }
        
        $nullable = if (-not $prop.Required -and $prop.Type -eq "string") { "?" } else { "" }
        $default = if (-not $prop.Required) { " = null" } else { "" }
        $createProps += "$($attr)$($prop.Type)$nullable $($prop.Name)$default"
    }
}
$CreateDtoContent += ($createProps -join ",`n")
$CreateDtoContent += "`n);"

$createDtoFile = Join-Path $DtosPath "${Entity}CreateDto.cs"
if (Test-Path $createDtoFile) {
    Write-Warning "File already exists: $createDtoFile (skipping)"
} else {
    $CreateDtoContent | Out-File -FilePath $createDtoFile -Encoding UTF8
    Write-Success "Generated: ${Entity}CreateDto.cs"
}

# Generate ResponseDto
$ResponseDtoContent = @"
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.$Namespace;

/// <summary>
/// $Entity Response DTO for reading $Entity data.
/// </summary>
public record ${Entity}ResponseDto(
    [Required]
    Guid Id,
"@

$responseProps = @()
foreach ($prop in $PropList) {
    if ($prop.Name -notin @("Id", "CreatedAt", "UpdatedAt")) {
        $attr = ""
        if ($prop.Required) {
            $attr += "    [Required]`n    "
        }
        $responseProps += "$($attr)$($prop.Type)$($prop.Name)"
    }
}
$ResponseDtoContent += (($responseProps -join ",`n    ") + ",`n    DateTime CreatedAt,`n    DateTime? UpdatedAt = null`n)")

$responseDtoFile = Join-Path $DtosPath "${Entity}ResponseDto.cs"
if (Test-Path $responseDtoFile) {
    Write-Warning "File already exists: $responseDtoFile (skipping)"
} else {
    $ResponseDtoContent | Out-File -FilePath $responseDtoFile -Encoding UTF8
    Write-Success "Generated: ${Entity}ResponseDto.cs"
}

# Generate FilterDto
$FilterDtoContent = @"
using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.$Namespace;

/// <summary>
/// $Entity Filter DTO for filtering/searching $Entity records.
/// </summary>
public record ${Entity}FilterDto(
"@

$filterProps = @()
foreach ($prop in $PropList) {
    if ($prop.Name -notin @("Id", "CreatedAt", "UpdatedAt")) {
        $nullable = "?"
        $filterProps += "    $($prop.Type)$nullable $($prop.Name) = null"
    }
}
$FilterDtoContent += ($filterProps -join ",`n")
$FilterDtoContent += ",`n    [Range(1, int.MaxValue)]`n    int? PageNumber = null,`n    [Range(1, 100)]`n    int? PageSize = null`n);"

$filterDtoFile = Join-Path $DtosPath "${Entity}FilterDto.cs"
if (Test-Path $filterDtoFile) {
    Write-Warning "File already exists: $filterDtoFile (skipping)"
} else {
    $FilterDtoContent | Out-File -FilePath $filterDtoFile -Encoding UTF8
    Write-Success "Generated: ${Entity}FilterDto.cs"
}

# Generate Service Interface
$ServiceContent = @"
using M_TAU.Application.Dtos.$Namespace;

namespace M_TAU.Application.Services;

/// <summary>
/// I${Entity}Service provides application-layer operations for $Entity management.
/// </summary>
public interface I${Entity}Service
{
    Task<${Entity}ResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<${Entity}ResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<${Entity}ResponseDto>> FilterAsync(${Entity}FilterDto filter, CancellationToken cancellationToken = default);
    Task<${Entity}ResponseDto> CreateAsync(${Entity}CreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, ${Entity}CreateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
"@

$serviceFile = Join-Path $ServicesPath "I${Entity}Service.cs"
if (Test-Path $serviceFile) {
    Write-Warning "File already exists: $serviceFile (skipping)"
} else {
    $ServiceContent | Out-File -FilePath $serviceFile -Encoding UTF8
    Write-Success "Generated: I${Entity}Service.cs"
}

Write-Success "Generation complete!"
Write-Info "Generated files:"
Write-Info "  - ${Entity}CreateDto.cs"
Write-Info "  - ${Entity}ResponseDto.cs"
Write-Info "  - ${Entity}FilterDto.cs"
Write-Info "  - I${Entity}Service.cs"
Write-Info ""
Write-Info "Next steps:"
Write-Info "  1. Review generated files for accuracy"
Write-Info "  2. Add custom domain-specific operations to I${Entity}Service"
Write-Info "  3. Create mappers to convert between DTOs and domain entities"
Write-Info "  4. Implement I${Entity}Service in M-TAU.Application"
Write-Info "  5. Register service in DI container"
