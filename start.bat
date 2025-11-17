@echo off
REM GrandNode2 Docker Startup Script for Windows

echo ==========================================
echo   LoynovaGrandNode2 Docker Setup
echo ==========================================

REM Check if Docker is installed
docker --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo X Docker is not installed. Please install Docker Desktop first.
    pause
    exit /b 1
)

REM Check if Docker Compose is available
docker compose version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo X Docker Compose is not available. Please install Docker Desktop first.
    pause
    exit /b 1
)

REM Create .env file if it doesn't exist
if not exist .env (
    echo Creating .env file from template...
    copy .env.example .env
    echo .env file created. Please review and modify the values as needed.
)

REM Create data directories
echo Creating data directories...
mkdir data\mongodb 2>nul
mkdir data\grandnode 2>nul

REM Build and start the containers
echo Building and starting containers...
docker compose down --remove-orphans
docker compose build --no-cache
docker compose up -d

echo.
echo Waiting for services to start...
timeout /t 30 /nobreak >nul

REM Check if services are running
echo Checking service health...
docker compose ps | findstr "Up" >nul
if %ERRORLEVEL% equ 0 (
    echo Services are starting up!
) else (
    echo Some services may still be initializing. Check logs with:
    echo    docker compose logs loynovagrandnode2
    echo    docker compose logs mongodb
)

echo.
echo ==========================================
echo   LoynovaGrandNode2 is starting!
echo ==========================================
echo Web Application: http://localhost:8080
echo MongoDB: mongodb://localhost:27017
echo MongoDB Admin: admin / LoynovaPass123!
echo.
echo Useful commands:
echo    View logs:      docker compose logs -f loynovagrandnode2
echo    Stop services:  docker compose down
echo    Restart:        docker compose restart
echo    Shell access:   docker compose exec loynovagrandnode2 bash
echo.
echo API Configuration:
echo    Frontend API:   Enabled
echo    Backend API:    Enabled
echo    API Key (Frontend): LoynovaFrontendSecretKey2024!
echo    API Key (Backend):  LoynovaBackendSecretKey2024!
echo.

REM Wait for application to be fully ready
echo Waiting for GrandNode2 to be fully ready...
set /a timeout=300
set /a counter=0

:wait_loop
if %counter% geq %timeout% goto timeout_reached

REM Try to connect to the application
curl -s -f http://localhost:8080 >nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo GrandNode2 is ready!
    echo Open http://localhost:8080 in your browser to complete the installation.
    goto end
)

timeout /t 5 /nobreak >nul
set /a counter+=5
echo    Still waiting... (%counter%/%timeout% seconds)
goto wait_loop

:timeout_reached
echo Timeout reached. GrandNode2 may still be starting.
echo Check the logs: docker compose logs loynovagrandnode2

:end
echo.
echo Press any key to exit...
pause >nul