#!/usr/bin/env pwsh

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to check if dotnet is available
function Test-DotnetAvailable {
    try {
        dotnet --version | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

# Check if dotnet is available
if (-not (Test-DotnetAvailable)) {
    Write-Error "dotnet CLI is not available. Please install .NET SDK."
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running My-Ai Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Clean previous test results
if (Test-Path "TestResults") {
    Remove-Item "TestResults" -Recurse -Force
    Write-Host "Cleaned previous test results" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Building solution..." -ForegroundColor Green
dotnet build --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Running Backend Tests..." -ForegroundColor Green
dotnet test "My-Ai.Tests" --logger "console;verbosity=normal" --collect:"XPlat Code Coverage" --results-directory TestResults/Backend
$backendExitCode = $LASTEXITCODE

Write-Host ""
Write-Host "Running Frontend/Blazor Tests..." -ForegroundColor Green
dotnet test "My-Ai.BlazorTests" --logger "console;verbosity=normal" --collect:"XPlat Code Coverage" --results-directory TestResults/Frontend
$frontendExitCode = $LASTEXITCODE

# Check if reportgenerator is installed
Write-Host ""
Write-Host "Checking for coverage report generator..." -ForegroundColor Green
$reportGenInstalled = $false
try {
    reportgenerator --help | Out-Null
    $reportGenInstalled = $true
    Write-Host "ReportGenerator is available" -ForegroundColor Green
}
catch {
    Write-Host "Installing ReportGenerator..." -ForegroundColor Yellow
    try {
        dotnet tool install -g dotnet-reportgenerator-globaltool
        $reportGenInstalled = $true
        Write-Host "ReportGenerator installed successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to install ReportGenerator. Skipping coverage report." -ForegroundColor Red
    }
}

# Generate coverage report if possible
if ($reportGenInstalled) {
    Write-Host ""
    Write-Host "Generating Coverage Report..." -ForegroundColor Green
    
    $coverageFiles = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse
    if ($coverageFiles.Count -gt 0) {
        $coverageFilesString = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
        
        try {
            reportgenerator -reports:$coverageFilesString -targetdir:"TestResults\Coverage" -reporttypes:"Html;TextSummary"
            Write-Host "Coverage report generated in TestResults\Coverage" -ForegroundColor Yellow
            
            # Try to display summary
            $summaryFile = "TestResults\Coverage\Summary.txt"
            if (Test-Path $summaryFile) {
                Write-Host ""
                Write-Host "Coverage Summary:" -ForegroundColor Cyan
                Get-Content $summaryFile | Write-Host
            }
        }
        catch {
            Write-Host "Failed to generate coverage report: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "No coverage files found" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Execution Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($backendExitCode -eq 0) {
    Write-Host "✅ Backend Tests: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ Backend Tests: FAILED" -ForegroundColor Red
}

if ($frontendExitCode -eq 0) {
    Write-Host "✅ Frontend Tests: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ Frontend Tests: FAILED" -ForegroundColor Red
}

# Overall result
if ($backendExitCode -eq 0 -and $frontendExitCode -eq 0) {
    Write-Host ""
    Write-Host "🎉 All tests completed successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Host ""
    Write-Host "❌ Some tests failed!" -ForegroundColor Red
    exit 1
}