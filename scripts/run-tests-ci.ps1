#!/usr/bin/env pwsh

param(
    [string]$OutputFormat = "trx",
    [string]$CoverageFormat = "cobertura",
    [switch]$CI = $false
)

# Set error action preference
$ErrorActionPreference = "Stop"

if ($CI) {
    Write-Host "Running in CI mode" -ForegroundColor Yellow
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running My-Ai Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Clean previous test results
if (Test-Path "TestResults") {
    Remove-Item "TestResults" -Recurse -Force
    Write-Host "Cleaned previous test results" -ForegroundColor Yellow
}

# Build solution
Write-Host "Building solution..." -ForegroundColor Green
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    throw "Build failed!"
}

# Run backend tests
Write-Host "Running Backend Tests..." -ForegroundColor Green
$backendArgs = @(
    "test", "My-Ai.Tests",
    "--configuration", "Release",
    "--no-build",
    "--logger", "trx;LogFileName=backend-tests.trx",
    "--collect", "XPlat Code Coverage",
    "--results-directory", "TestResults/Backend"
)

if ($CI) {
    $backendArgs += "--logger"
    $backendArgs += "GitHubActions"
}

dotnet @backendArgs
$backendExitCode = $LASTEXITCODE

# Run frontend tests
Write-Host "Running Frontend Tests..." -ForegroundColor Green
$frontendArgs = @(
    "test", "My-Ai.BlazorTests",
    "--configuration", "Release", 
    "--no-build",
    "--logger", "trx;LogFileName=frontend-tests.trx",
    "--collect", "XPlat Code Coverage",
    "--results-directory", "TestResults/Frontend"
)

if ($CI) {
    $frontendArgs += "--logger"
    $frontendArgs += "GitHubActions"
}

dotnet @frontendArgs
$frontendExitCode = $LASTEXITCODE

# Generate coverage report if not in CI (CI handles this separately)
if (-not $CI) {
    Write-Host "Generating Coverage Report..." -ForegroundColor Green
    
    $coverageFiles = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse
    if ($coverageFiles.Count -gt 0) {
        $coverageFilesString = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
        
        try {
            dotnet tool install -g dotnet-reportgenerator-globaltool 2>$null
            reportgenerator -reports:$coverageFilesString -targetdir:"TestResults\Coverage" -reporttypes:"Html;TextSummary"
            Write-Host "Coverage report generated in TestResults\Coverage" -ForegroundColor Yellow
        }
        catch {
            Write-Host "Failed to generate coverage report: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

# Exit with appropriate code
if ($backendExitCode -eq 0 -and $frontendExitCode -eq 0) {
    Write-Host "🎉 All tests passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "❌ Some tests failed!" -ForegroundColor Red
    exit 1
}