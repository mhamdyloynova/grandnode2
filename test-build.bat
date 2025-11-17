@echo off
REM Test Docker build script for Windows

echo ==========================================
echo   Testing LoynovaGrandNode2 Docker Build
echo ==========================================

echo Testing alternative Dockerfile...
docker build -f Dockerfile.alternative -t loynovagrandnode2:test .

if %ERRORLEVEL% neq 0 (
    echo.
    echo X Alternative Dockerfile build failed!
    echo Trying original Dockerfile...
    echo.
    
    docker build -f Dockerfile -t loynovagrandnode2:test .
    
    if %ERRORLEVEL% neq 0 (
        echo.
        echo X Original Dockerfile also failed!
        echo Please check the build logs above for errors.
        pause
        exit /b 1
    ) else (
        echo.
        echo Original Dockerfile build succeeded!
        echo Use: docker-compose up -d
    )
) else (
    echo.
    echo Alternative Dockerfile build succeeded!
    echo Use: docker-compose -f docker-compose.alternative.yml up -d
)

echo.
echo Build test completed successfully!
echo You can now run the full stack with docker-compose.

pause