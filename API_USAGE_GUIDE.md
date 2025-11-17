# LoynovaGrandNode2 API Usage Guide

## ✅ Enabled APIs

The Docker setup has successfully enabled both Admin and Frontend APIs for the LoynovaGrandNode2 container.

### 🔧 API Configuration

- **Frontend API**: ✅ Enabled
- **Backend API**: ✅ Enabled  
- **API Module**: ✅ Loaded
- **Authentication**: JWT Token-based

### 📍 API Endpoints

**Base URL**: http://localhost:8080

#### Backend/Admin API Endpoints (v1)
```
POST /api/Token/Create               - Get JWT token for backend/admin
GET  /api/Product                    - List all products  
GET  /api/Product/{id}              - Get product by ID
POST /api/Product                   - Create new product
PATCH /api/Product/{id}             - Update product (JSON Patch)
DELETE /api/Product/{id}            - Delete product
GET  /api/Category                  - List categories
GET  /api/Customer                  - List customers
GET  /api/Brand                     - List brands
GET  /api/Currency                  - List currencies
GET  /api/Country                   - List countries
```

#### Frontend API Endpoints (v2)  
```
POST /TokenWeb/Guest                - Get guest token
POST /TokenWeb/Login                - Login and get JWT token
POST /TokenWeb/Refresh              - Refresh JWT token
GET  /TokenWeb/Antiforgery          - Get antiforgery token
```

### 🔑 API Authentication

#### Backend/Admin API Authentication
```bash
curl -X POST http://localhost:8080/api/Token/Create \
  -H "Content-Type: application/json" \
  -d '{
    "Email": "admin@example.com",
    "SecretKey": "LoynovaBackendSecretKey2024!"
  }'
```

#### Frontend API Authentication (Guest)
```bash
curl -X POST http://localhost:8080/TokenWeb/Guest \
  -H "Content-Type: application/json"
```

#### Frontend API Authentication (Login)
```bash
curl -X POST http://localhost:8080/TokenWeb/Login \
  -H "Content-Type: application/json" \
  -d '{
    "Email": "customer@example.com",
    "Password": "customer_password"
  }'
```

### 📝 Using API Tokens

After authentication, use the returned JWT token in the Authorization header:

```bash
# Example: Get products via Backend API
curl -X GET http://localhost:8080/api/Product \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json"

# Example: Get specific product by ID
curl -X GET http://localhost:8080/api/Product/{product-id} \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -H "Content-Type: application/json"
```

### 🔐 API Keys

- **Frontend Secret Key**: `LoynovaFrontendSecretKey2024!`
- **Backend Secret Key**: `LoynovaBackendSecretKey2024!`

### 📊 Token Information

- **Token Expiry**: 1440 minutes (24 hours)
- **Refresh Token Expiry**: 1440 minutes (24 hours)
- **Validation**: JWT signature validation enabled

### 🧪 Testing APIs

#### 1. First, complete GrandNode2 installation:
- Open http://localhost:8080
- Complete the installation wizard
- Create admin account

#### 2. Test Backend API:
```bash
# Get authentication token (replace with your admin email)
curl -X POST http://localhost:8080/api/Token/Create \
  -H "Content-Type: application/json" \
  -d '{
    "Email": "YOUR_ADMIN_EMAIL",
    "SecretKey": "LoynovaBackendSecretKey2024!"
  }'
```

#### 3. Test Frontend API (Guest):
```bash
# Get guest token
curl -X POST http://localhost:8080/TokenWeb/Guest \
  -H "Content-Type: application/json"
```

#### 4. Use the token for API calls:
```bash
# Example: Get all products
curl -X GET http://localhost:8080/api/Product \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json"
```

### 🌐 API Documentation

**Scalar Interactive Documentation** (Recommended):
- **Backend API (v1)**: http://localhost:8080/scalar/v1
- **Frontend API (v2)**: http://localhost:8080/scalar/v2

**OpenAPI JSON Specifications**:
- **Backend API Spec**: http://localhost:8080/openapi/v1.json
- **Frontend API Spec**: http://localhost:8080/openapi/v2.json

**Note**: Scalar documentation is available when the application runs in Development mode.

### 📋 Common API Operations

#### Products (Backend API)
- `GET /api/Product` - List all products
- `GET /api/Product/{id}` - Get product by ID  
- `POST /api/Product` - Create new product
- `PATCH /api/Product/{id}` - Update product (JSON Patch)
- `DELETE /api/Product/{id}` - Delete product

#### Categories (Backend API)
- `GET /api/Category` - List categories
- `GET /api/Category/{id}` - Get category by ID

#### Customers (Backend API)
- `GET /api/Customer` - List customers
- `GET /api/Customer/{id}` - Get customer details

#### Brands (Backend API)
- `GET /api/Brand` - List brands
- `GET /api/Brand/{id}` - Get brand by ID

#### Currencies (Backend API)
- `GET /api/Currency` - List currencies
- `GET /api/Currency/{id}` - Get currency by ID

#### Countries (Backend API)
- `GET /api/Country` - List countries
- `GET /api/Country/{id}` - Get country by ID

### 🔍 Troubleshooting

1. **401 Unauthorized**: Check if JWT token is valid and not expired
2. **403 Forbidden**: Verify API key is correct
3. **404 Not Found**: Ensure API endpoints exist and module is loaded
4. **500 Server Error**: Check container logs for detailed errors

### 📚 Additional Resources

- Container logs: `docker compose -f docker-compose.alternative.yml logs loynovagrandnode2`
- API status: Check http://localhost:8080/health (if health endpoint is configured)
- Admin panel: http://localhost:8080/admin

---

**Success!** 🎉 Your LoynovaGrandNode2 APIs are now fully operational and ready for integration.