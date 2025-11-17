#!/bin/bash
# GrandNode2 Docker Startup Script for Linux/macOS

echo "=========================================="
echo "  LoynovaGrandNode2 Docker Setup"
echo "=========================================="

# Check if Docker and Docker Compose are installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Create .env file if it doesn't exist
if [ ! -f .env ]; then
    echo "📋 Creating .env file from template..."
    cp .env.example .env
    echo "✅ .env file created. Please review and modify the values as needed."
fi

# Create data directories
echo "📁 Creating data directories..."
mkdir -p data/mongodb data/grandnode

# Set executable permissions for MongoDB init script
chmod +x mongo-init/01-init-grandnode.sh

# Build and start the containers
echo "🚀 Building and starting containers..."
docker-compose down --remove-orphans
docker-compose build --no-cache
docker-compose up -d

echo ""
echo "⏳ Waiting for services to start..."
sleep 30

# Check if services are healthy
echo "🔍 Checking service health..."

if docker-compose ps | grep -q "Up (healthy)"; then
    echo "✅ Services are starting up!"
else
    echo "⚠️  Some services may still be initializing. Check logs with:"
    echo "   docker-compose logs loynovagrandnode2"
    echo "   docker-compose logs mongodb"
fi

echo ""
echo "=========================================="
echo "  🎉 LoynovaGrandNode2 is starting!"
echo "=========================================="
echo "📱 Web Application: http://localhost:8080"
echo "🗄️  MongoDB: mongodb://localhost:27017"
echo "🔧 MongoDB Admin: admin / LoynovaPass123!"
echo ""
echo "📋 Useful commands:"
echo "   View logs:      docker-compose logs -f loynovagrandnode2"
echo "   Stop services:  docker-compose down"
echo "   Restart:        docker-compose restart"
echo "   Shell access:   docker-compose exec loynovagrandnode2 bash"
echo ""
echo "🔧 API Configuration:"
echo "   Frontend API:   Enabled"
echo "   Backend API:    Enabled"
echo "   API Key (Frontend): LoynovaFrontendSecretKey2024!"
echo "   API Key (Backend):  LoynovaBackendSecretKey2024!"
echo ""

# Wait for application to be fully ready
echo "⏳ Waiting for GrandNode2 to be fully ready..."
timeout=300
counter=0

while [ $counter -lt $timeout ]; do
    if curl -s -f http://localhost:8080 > /dev/null 2>&1; then
        echo "✅ GrandNode2 is ready!"
        echo "🌐 Open http://localhost:8080 in your browser to complete the installation."
        break
    fi
    sleep 5
    counter=$((counter + 5))
    echo "   Still waiting... ($counter/$timeout seconds)"
done

if [ $counter -ge $timeout ]; then
    echo "⚠️  Timeout reached. GrandNode2 may still be starting."
    echo "   Check the logs: docker-compose logs loynovagrandnode2"
fi