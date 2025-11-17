# GrandNode2 Application Launcher
Write-Host "Starting GrandNode2 Application..." -ForegroundColor Green
Write-Host "Application will be available at: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Admin Panel: http://localhost:5000/admin" -ForegroundColor Cyan
Write-Host "API Docs (Scalar): http://localhost:5000/scalar/v1" -ForegroundColor Cyan
Write-Host "Mobile API Docs: http://localhost:5000/scalar/v3" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host ""

Set-Location -Path "$PSScriptRoot\src\Web\Grand.Web"
dotnet run --urls http://localhost:5000
