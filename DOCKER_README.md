# LoynovaGrandNode2 Docker Setup

This repository contains Docker configuration for running GrandNode2 e-commerce platform with MongoDB database, including full API support.

## 🚀 Quick Start

### Prerequisites

- Docker Desktop 4.0+ installed
- Docker Compose V2
- At least 4GB of available RAM
- At least 5GB of free disk space

### Windows Users

```powershell
# Clone the repository
git clone <repository-url>
cd grandnode2Loynova

# Run the setup script
.\start.bat
```

### Linux/macOS Users

```bash
# Clone the repository
git clone <repository-url>
cd grandnode2Loynova

# Make the script executable and run
chmod +x start.sh
./start.sh
```

### Manual Setup

If you prefer to run commands manually:

```bash
# Copy environment template
cp .env.example .env

# Build and start services
docker compose up -d --build

# Check status
docker compose ps

# View logs
docker compose logs -f loynovagrandnode2
```

## 📋 Services Overview

### LoynovaGrandNode2 (Web Application)
- **Container Name**: `LoynovaGrandNode2`
- **Port**: 8080
- **URL**: http://localhost:8080
- **Features**: Full e-commerce platform with API support

### MongoDB Database
- **Container Name**: `loynova-mongodb`
- **Port**: 27017
- **Database**: `grandnodedb`
- **Admin User**: `admin`
- **Admin Password**: `LoynovaPass123!`

## 🔧 Configuration

### Environment Variables

Edit the `.env` file to customize your setup:

```env
# MongoDB Configuration
MONGO_ROOT_USERNAME=admin
MONGO_ROOT_PASSWORD=LoynovaPass123!
MONGO_DATABASE=grandnodedb

# API Configuration
FRONTEND_API_SECRET=LoynovaFrontendSecretKey2024!
BACKEND_API_SECRET=LoynovaBackendSecretKey2024!

# Port Configuration
MONGODB_PORT=27017
GRANDNODE_PORT=8080
```

### API Configuration

Both Frontend and Backend APIs are enabled by default:

#### Frontend API
- **Enabled**: Yes
- **Secret Key**: `LoynovaFrontendSecretKey2024!`
- **Token Expiry**: 1440 minutes (24 hours)
- **Endpoint**: `/api/frontend/`

#### Backend API  
- **Enabled**: Yes
- **Secret Key**: `LoynovaBackendSecretKey2024!`
- **Token Expiry**: 1440 minutes (24 hours)
- **Endpoint**: `/api/backend/`
- **System Model**: Enabled

## 🌐 Initial Setup

1. **Access the Application**
   - Open http://localhost:8080 in your browser
   - Complete the GrandNode2 installation wizard

2. **Database Configuration**
   - Connection string is pre-configured for Docker environment
   - Database: `grandnodedb`
   - MongoDB connection will be automatic

3. **Admin Account Setup**
   - Create your admin account during installation
   - Access admin panel at: http://localhost:8080/admin

## 🔍 API Usage

### Authentication

Both APIs use JWT token authentication. Get a token by sending a POST request:

```bash
# Frontend API Token
curl -X POST http://localhost:8080/api/frontend/authenticate \
  -H "Content-Type: application/json" \
  -d '{"username":"your-username","password":"your-password","secretkey":"LoynovaFrontendSecretKey2024!"}'

# Backend API Token  
curl -X POST http://localhost:8080/api/backend/authenticate \
  -H "Content-Type: application/json" \
  -d '{"username":"admin-username","password":"admin-password","secretkey":"LoynovaBackendSecretKey2024!"}'
```

### Making API Calls

Use the received token in the Authorization header:

```bash
curl -X GET http://localhost:8080/api/backend/products \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🛠️ Common Commands

### Container Management

```bash
# Start services
docker compose up -d

# Stop services
docker compose down

# Restart services
docker compose restart

# View all logs
docker compose logs -f

# View specific service logs
docker compose logs -f loynovagrandnode2
docker compose logs -f mongodb

# Shell access to GrandNode2 container
docker compose exec loynovagrandnode2 bash

# Shell access to MongoDB container
docker compose exec mongodb mongosh
```

### Database Management

```bash
# Connect to MongoDB
docker compose exec mongodb mongosh -u admin -p LoynovaPass123! --authenticationDatabase admin

# Backup database
docker compose exec mongodb mongodump --host localhost:27017 -u admin -p LoynovaPass123! --authenticationDatabase admin --db grandnodedb --out /backup

# Restore database
docker compose exec mongodb mongorestore --host localhost:27017 -u admin -p LoynovaPass123! --authenticationDatabase admin --db grandnodedb /backup/grandnodedb
```

### Application Management

```bash
# Check application health
curl http://localhost:8080/health

# View application logs
docker compose logs -f loynovagrandnode2

# Monitor resource usage
docker compose top

# Get container information
docker compose ps
docker inspect LoynovaGrandNode2
```

## 📁 Volume Management

Persistent data is stored in Docker volumes:

- **mongodb_data**: MongoDB database files
- **app_data**: GrandNode2 application data (uploads, logs, etc.)

### Backup Volumes

```bash
# Backup MongoDB volume
docker run --rm -v grandnode2loynova_mongodb_data:/data -v $(pwd):/backup ubuntu tar czf /backup/mongodb_backup.tar.gz -C /data .

# Backup App data volume
docker run --rm -v grandnode2loynova_app_data:/data -v $(pwd):/backup ubuntu tar czf /backup/app_data_backup.tar.gz -C /data .
```

### Restore Volumes

```bash
# Restore MongoDB volume
docker run --rm -v grandnode2loynova_mongodb_data:/data -v $(pwd):/backup ubuntu tar xzf /backup/mongodb_backup.tar.gz -C /data

# Restore App data volume
docker run --rm -v grandnode2loynova_app_data:/data -v $(pwd):/backup ubuntu tar xzf /backup/app_data_backup.tar.gz -C /data
```

## 🔧 Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Change ports in docker-compose.yml or .env file
   # For port 8080: Change "8080:8080" to "8081:8080"
   # For port 27017: Change "27017:27017" to "27018:27017"
   ```

2. **Memory Issues**
   ```bash
   # Ensure Docker has at least 4GB RAM allocated
   # Check Docker Desktop settings
   ```

3. **Database Connection Failed**
   ```bash
   # Check MongoDB container status
   docker compose ps
   docker compose logs mongodb
   
   # Verify network connectivity
   docker compose exec loynovagrandnode2 ping mongodb
   ```

4. **Application Won't Start**
   ```bash
   # Check logs for detailed error information
   docker compose logs loynovagrandnode2
   
   # Rebuild without cache
   docker compose down
   docker compose build --no-cache
   docker compose up -d
   ```

### Health Checks

```bash
# Check if all services are healthy
docker compose ps

# Test MongoDB connection
docker compose exec mongodb mongosh --eval "db.adminCommand('ping')"

# Test GrandNode2 application
curl -f http://localhost:8080 || echo "Application not ready yet"
```

## 🔒 Security Notes

1. **Change Default Passwords**: Update all default passwords in production
2. **API Keys**: Use strong, unique API keys in production
3. **Network Security**: Consider using Docker networks for isolation
4. **SSL/TLS**: Configure HTTPS for production deployment
5. **Firewall**: Restrict access to necessary ports only

## 📈 Production Deployment

For production deployment:

1. Use environment-specific configuration files
2. Enable HTTPS/SSL
3. Configure proper backup strategies
4. Set up monitoring and logging
5. Use Docker Swarm or Kubernetes for orchestration
6. Configure load balancing
7. Implement proper security measures

## 🆘 Support

If you encounter issues:

1. Check the logs: `docker compose logs`
2. Verify system requirements
3. Ensure all ports are available
4. Check Docker Desktop status
5. Review environment configuration

## 📚 Additional Resources

- [GrandNode2 Documentation](https://grandnode.com)
- [Docker Documentation](https://docs.docker.com)
- [MongoDB Documentation](https://docs.mongodb.com)
- [ASP.NET Core Docker](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker)

---

**LoynovaGrandNode2** - E-commerce platform powered by ASP.NET Core and MongoDB