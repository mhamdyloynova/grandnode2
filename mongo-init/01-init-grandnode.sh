#!/bin/bash
# MongoDB initialization script for GrandNode2

echo "Creating GrandNode2 database and user..."

# Create application database and user
mongosh <<EOF
use admin
db.auth('${MONGO_INITDB_ROOT_USERNAME}', '${MONGO_INITDB_ROOT_PASSWORD}')

// Create the application database
use ${MONGO_INITDB_DATABASE}

// Create application user (optional - you can use root user for simplicity)
db.createUser({
  user: 'grandnodeuser',
  pwd: 'GrandNodePass123!',
  roles: [
    {
      role: 'readWrite',
      db: '${MONGO_INITDB_DATABASE}'
    }
  ]
})

// Create initial collections (optional)
db.createCollection('logs')
db.createCollection('customers')
db.createCollection('products')

print('GrandNode2 database initialization completed')
EOF