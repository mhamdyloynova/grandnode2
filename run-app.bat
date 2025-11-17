@echo off
echo Starting GrandNode2 Application...
echo Application will be available at: http://localhost:5000
echo Admin Panel: http://localhost:5000/admin
echo API Docs (Scalar): http://localhost:5000/scalar/v1
echo Mobile API Docs: http://localhost:5000/scalar/v3
echo.
echo Press Ctrl+C to stop the application
echo.

cd /d "%~dp0src\Web\Grand.Web"
dotnet run --urls http://localhost:5000
pause
