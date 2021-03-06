# =================================================================================================================================
# Copyright (c) RapidField LLC. Licensed under the MIT License. See LICENSE.txt in the project root for license information.
# =================================================================================================================================

$ArtifactsDirectoryName = "artifacts"
$ConfigurationModulePath = Join-Path -Path $PSScriptRoot -ChildPath "Configuration.psm1"
$ConfigurationTypeLocal = "Local"
$ConfigurationTypeProduction = "Production"
$ProjectRootDirectory = (Get-Item $PSScriptRoot).Parent.FullName
$PrototypeDirectoryName = "prototype"
$PrototypeServiceApplicationNamespace = "RapidField.SolidInstruments.Prototype.ServiceApplication"
$PrototypeServiceApplicationTargetFramework = "netcoreapp2.1"
$PrototypeWebApplicationNamespace = "RapidField.SolidInstruments.Prototype.WebApplication"
$SolutionConfigurationDebug = "Debug"
$SolutionConfigurationRelease = "Release"
$SolutionFileName = "RapidField.SolidInstruments.sln"
$SolutionPath = Join-Path -Path $ProjectRootDirectory -ChildPath $SolutionFileName
$SourceDirectoryName = "src"

Import-Module $ConfigurationModulePath

# Build
# =================================================================================================================================

function Build {
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $SolutionConfiguration
    )

    $BuildVersion = GetBuildVersion
    Write-Host -ForegroundColor DarkCyan "Building $SolutionPath using $SolutionConfiguration configuration."
    Write-Host -ForegroundColor DarkCyan "Build version: $BuildVersion"
    dotnet build $SolutionPath --configuration $SolutionConfiguration --no-restore --verbosity minimal /p:BuildVersion=$BuildVersion

    If ($LASTEXITCODE -ne 0) {
        Throw "The build failed for $SolutionPath using $SolutionConfiguration configuration."
    }

    $SourceDirectoryPath = Join-Path -Path $ProjectRootDirectory -ChildPath $SourceDirectoryName
    $ArtifactsDirectoryPath = Join-Path -Path $ProjectRootDirectory -ChildPath "$ArtifactsDirectoryName\$SolutionConfiguration"

    If (-not (Test-Path $ArtifactsDirectoryPath)) {
        New-Item -ItemType Directory -Path $ArtifactsDirectoryPath -Force
    }

    Get-ChildItem -Path $SourceDirectoryPath -Directory | ForEach-Object {
        $ProjectOutputPath = Join-Path -Path $_.FullName -ChildPath "bin\$SolutionConfiguration"

        If (Test-Path $ProjectOutputPath) {
            Write-Host -ForegroundColor DarkCyan "Copying artifacts from $ProjectOutputPath."
            Get-ChildItem -Path $ProjectOutputPath -File | Copy-Item -Container -Destination $ArtifactsDirectoryPath -Force
        }
    }

    Write-Host -ForegroundColor DarkCyan "`n>>> Finished building. <<<`n"
}

function BuildDebug {
    Build -SolutionConfiguration $SolutionConfigurationDebug
}

function BuildRelease {
    Build -SolutionConfiguration $SolutionConfigurationRelease
}

# Clean
# =================================================================================================================================

function Clean {
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $SolutionConfiguration
    )

    Write-Host -ForegroundColor DarkCyan "Cleaning $SolutionPath using $SolutionConfiguration configuration."
    dotnet clean $SolutionPath --configuration $SolutionConfiguration --verbosity minimal

    If ($LASTEXITCODE -ne 0) {
        Throw "Cleaning failed for $SolutionPath using $SolutionConfiguration configuration."
    }

    $SourceDirectoryPath = Join-Path -Path $ProjectRootDirectory -ChildPath $SourceDirectoryName

    Get-ChildItem -Path $SourceDirectoryPath -Directory | ForEach-Object {
        $ProjectBinPath = Join-Path -Path $_.FullName -ChildPath "bin\$SolutionConfiguration"
        $ProjectObjPath = Join-Path -Path $_.FullName -ChildPath "obj"

        If (Test-Path $ProjectBinPath) {
            Write-Host -ForegroundColor DarkCyan "Removing $ProjectBinPath."
            Remove-Item -Path $ProjectBinPath -Recurse -Confirm:$false -Force
        }

        If (Test-Path $ProjectObjPath) {
            Write-Host -ForegroundColor DarkCyan "Removing $ProjectObjPath."
            Remove-Item -Path $ProjectObjPath -Recurse -Confirm:$false -Force
        }
    }

    $ArtifactsDirectoryPath = Join-Path -Path $ProjectRootDirectory -ChildPath "$ArtifactsDirectoryName\$SolutionConfiguration"

    If (Test-Path $ArtifactsDirectoryPath) {
        Write-Host -ForegroundColor DarkCyan "Removing artifacts from $ArtifactsDirectoryPath."
        Remove-Item -Path $ArtifactsDirectoryPath -Recurse -Confirm:$false -Force
    }

    Write-Host -ForegroundColor DarkCyan "`n>>> Finished cleaning. <<<`n"
}

function CleanDebug {
    Clean -SolutionConfiguration $SolutionConfigurationDebug
}

function CleanRelease {
    Clean -SolutionConfiguration $SolutionConfigurationRelease
}

# Deploy
# =================================================================================================================================

function Deploy {
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $PackageDeploymentUri
    )

    Write-Host -ForegroundColor DarkCyan "Deploying artifacts to $PackageDeploymentUri."
    $ArtifactsDirectoryPath = Join-Path -Path $ProjectRootDirectory -ChildPath "$ArtifactsDirectoryName\$SolutionConfigurationRelease"

    If (Test-Path $ArtifactsDirectoryPath) {
        ForEach ($ArtifactFile In (Get-ChildItem -Path $ArtifactsDirectoryPath -File)) {
            $ArtifactFilePath = $ArtifactFile.FullName

            If ($ArtifactFilePath.EndsWith(".nupkg")) {
                dotnet nuget push $ArtifactFilePath --source $PackageDeploymentUri
            }
        }
    }
    Else {
        Write-Host -ForegroundColor DarkCyan "No artifacts exist. Suppressing deployment."
    }

    Write-Host -ForegroundColor DarkCyan "`n>>> Finished deploying. <<<`n"
}

function DeployLocal {
    $PackageDeploymentUri = GetConfigurationValue -Type $ConfigurationTypeLocal -Key "PackageDeploymentUri"

    If (-not (Test-Path $PackageDeploymentUri)) {
        New-Item -ItemType Directory -Path $PackageDeploymentUri -Force
    }

    Deploy -PackageDeploymentUri $PackageDeploymentUri
}

function DeployProduction {
    $PackageDeploymentUri = GetConfigurationValue -Type $ConfigurationTypeProduction -Key "PackageDeploymentUri"
    Deploy -PackageDeploymentUri $PackageDeploymentUri
}

# Restore
# =================================================================================================================================

function RestoreDependencies {
    Write-Host -ForegroundColor DarkCyan "Restoring dependencies for $SolutionPath."
    dotnet restore $SolutionPath --verbosity minimal

    If ($LASTEXITCODE -ne 0) {
        Throw "One or more dependencies could not be restored for $SolutionPath."
    }

    Write-Host -ForegroundColor DarkCyan "`n>>> Finished restoring dependencies. <<<`n"
}

# Start
# =================================================================================================================================

function StartPrototypeServiceApplication {
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $SolutionConfiguration
    )

    Write-Host -ForegroundColor DarkCyan "Starting the prototype service application using $SolutionConfiguration configuration."
    $BinaryFilePath = Join-Path -Path $ProjectRootDirectory -ChildPath "$PrototypeDirectoryName\$PrototypeServiceApplicationNamespace\bin\$SolutionConfiguration\$PrototypeServiceApplicationTargetFramework\$PrototypeServiceApplicationNamespace.dll"
    Write-Host -ForegroundColor DarkCyan "Using binary path: $BinaryFilePath"
    Start-Process -FilePath "dotnet" -ArgumentList "$BinaryFilePath" -WindowStyle Minimized
    Write-Host -ForegroundColor DarkCyan "`n>>> Finished starting the application. <<<`n"
}

function StartPrototypeServiceApplicationDebug {
    StartPrototypeServiceApplication -SolutionConfiguration $SolutionConfigurationDebug
}

function StartPrototypeServiceApplicationRelease {
    StartPrototypeServiceApplication -SolutionConfiguration $SolutionConfigurationRelease
}

function StartPrototypeWebApplication {
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $SolutionConfiguration
    )

    Write-Host -ForegroundColor DarkCyan "Starting the prototype web application using $SolutionConfiguration configuration."
    $ProjectFilePath = Join-Path -Path $ProjectRootDirectory -ChildPath "$PrototypeDirectoryName\$PrototypeWebApplicationNamespace\$PrototypeWebApplicationNamespace.csproj"
    Write-Host -ForegroundColor DarkCyan "Using project path: $ProjectFilePath"
    Start-Process -FilePath "dotnet" -ArgumentList "run --project ""$ProjectFilePath"" --configuration $SolutionConfiguration" -WindowStyle Minimized
    Write-Host -ForegroundColor DarkCyan "`n>>> Finished starting the application. <<<`n"
}

function StartPrototypeWebApplicationDebug {
    StartPrototypeWebApplication -SolutionConfiguration $SolutionConfigurationDebug
}

function StartPrototypeWebApplicationRelease {
    StartPrototypeWebApplication -SolutionConfiguration $SolutionConfigurationRelease
}

# Stop
# =================================================================================================================================

function StopAllApplications {
    Write-Host -ForegroundColor DarkCyan "Stopping all applications."
    Stop-Process -Name "dotnet" -ErrorAction SilentlyContinue -Force
    Write-Host -ForegroundColor DarkCyan "`n>>> Finished stopping all applications. <<<`n"
}

# Test
# =================================================================================================================================

function Test {
    Param (
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $SolutionConfiguration
    )

    ForEach ($TestDirectoryPath In (Get-ChildItem -Path "test" -Directory)) {
        Write-Host -ForegroundColor DarkCyan "Running tests for $TestDirectoryPath using $SolutionConfiguration configuration."
        dotnet test $TestDirectoryPath.FullName --configuration $SolutionConfiguration --no-build --no-restore --verbosity minimal

        If ($LASTEXITCODE -ne 0) {
            Throw "One or more tests failed for $TestDirectoryPath using $SolutionConfiguration configuration."
        }

        Write-Host -ForegroundColor DarkCyan "`n>>> Finished running tests. <<<`n"
    }
}

function TestDebug {
    Test -SolutionConfiguration $SolutionConfigurationDebug
}

function TestRelease {
    Test -SolutionConfiguration $SolutionConfigurationRelease
}