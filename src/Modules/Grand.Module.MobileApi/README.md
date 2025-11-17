# GrandNode2 Mobile API Module

A comprehensive mobile-optimized REST API module for GrandNode2 e-commerce platform, designed specifically for mobile applications with JWT authentication and mobile-first response patterns.

## 🚀 Features

- **Pure JWT Authentication** - No anti-forgery tokens required, perfect for mobile apps
- **Mobile-Optimized DTOs** - Lightweight data structures designed for mobile bandwidth constraints
- **Guest User Support** - Full shopping experience without registration
- **Comprehensive Coverage** - Authentication, catalog, cart, checkout, and customer management
- **Interactive Documentation** - Modern Scalar documentation with beautiful UI
- **API Versioning** - Dedicated v3 API endpoints for mobile applications
- **Consistent Responses** - Standardized JSON response format with proper HTTP status codes

## 📋 Table of Contents

- [Installation](#installation)
- [Configuration](#configuration)
- [Authentication](#authentication)
- [API Endpoints](#api-endpoints)
- [Response Format](#response-format)
- [Error Handling](#error-handling)
- [Development](#development)
- [Testing](#testing)

## 🔧 Installation

1. **Enable the Module**
   
   Update your `appsettings.json` to enable the Mobile API module:
   ```json
   "FeatureManagement": {
     "Grand.Module.MobileApi": true
   }
   ```

2. **Configure JWT Settings**
   
   Add Mobile API configuration to `appsettings.json`:
   ```json
   "MobileApi": {
     "Enabled": true,
     "SecretKey": "your-mobile-api-secret-key-change-this-in-production-32chars-min",
     "ValidateIssuer": true,
     "ValidIssuer": "grandnode-mobile-api",
     "ValidateAudience": true,
     "ValidAudience": "grandnode-mobile-app",
     "ValidateLifetime": true,
     "ValidateIssuerSigningKey": true,
     "ExpiryInMinutes": 60,
     "RefreshTokenExpiryInMinutes": 1440,
     "AllowGuestUsers": true,
     "RequireHttps": false,
     "MaxRequestsPerMinute": 60,
     "EnableDocumentation": true,
     "DocumentationTitle": "GrandNode2 Mobile API",
     "DocumentationDescription": "Mobile-optimized REST API for GrandNode2 e-commerce platform"
   }
   ```

3. **Restart Application**
   
   Restart your GrandNode2 application to load the module.

## ⚙️ Configuration

### JWT Configuration Options

| Setting | Description | Default | Required |
|---------|-------------|---------|----------|
| `Enabled` | Enable/disable Mobile API | `true` | ✅ |
| `SecretKey` | JWT signing secret key (min 32 chars) | - | ✅ |
| `ValidateIssuer` | Validate JWT issuer | `true` | ✅ |
| `ValidIssuer` | Expected JWT issuer | `grandnode-mobile-api` | ✅ |
| `ValidateAudience` | Validate JWT audience | `true` | ✅ |
| `ValidAudience` | Expected JWT audience | `grandnode-mobile-app` | ✅ |
| `ExpiryInMinutes` | Access token expiry time | `60` | ✅ |
| `RefreshTokenExpiryInMinutes` | Refresh token expiry time | `1440` | ✅ |
| `AllowGuestUsers` | Allow guest user functionality | `true` | ❌ |
| `RequireHttps` | Require HTTPS for API calls | `false` | ❌ |
| `MaxRequestsPerMinute` | Rate limiting | `60` | ❌ |

### Security Considerations

- **Production Secret Key**: Use a strong, random 32+ character secret key in production
- **HTTPS**: Enable `RequireHttps: true` in production environments
- **Issuer/Audience**: Configure unique values for your application
- **Rate Limiting**: Adjust based on your application needs

## 🔐 Authentication

The Mobile API uses JWT Bearer token authentication with support for both registered and guest users.

### Authentication Flow

#### 1. Guest User Authentication
```http
POST /api/mobile/v3/auth/guest
Content-Type: application/json

{
  "deviceId": "unique-device-identifier"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Guest user created successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh-token-here",
    "expiresIn": 3600,
    "userType": "Guest",
    "customerId": "guest-customer-id"
  }
}
```

#### 2. User Registration
```http
POST /api/mobile/v3/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "confirmPassword": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

#### 3. User Login
```http
POST /api/mobile/v3/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

#### 4. Token Refresh
```http
POST /api/mobile/v3/auth/refresh
Content-Type: application/json
Authorization: Bearer {access-token}

{
  "refreshToken": "your-refresh-token"
}
```

### Using JWT Tokens

Include the JWT token in the Authorization header for all authenticated requests:

```http
GET /api/mobile/v3/catalog/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📊 API Endpoints

### Authentication Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/api/mobile/v3/auth/guest` | Create guest user | ❌ |
| `POST` | `/api/mobile/v3/auth/register` | Register new user | ❌ |
| `POST` | `/api/mobile/v3/auth/login` | User login | ❌ |
| `POST` | `/api/mobile/v3/auth/refresh` | Refresh JWT token | ✅ |
| `POST` | `/api/mobile/v3/auth/logout` | User logout | ✅ |

### Catalog Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/mobile/v3/catalog/products` | Get products with filtering | ✅ |
| `GET` | `/api/mobile/v3/catalog/products/{id}` | Get product details | ✅ |
| `GET` | `/api/mobile/v3/catalog/categories` | Get categories | ✅ |
| `GET` | `/api/mobile/v3/catalog/categories/{id}` | Get category details | ✅ |
| `GET` | `/api/mobile/v3/catalog/brands` | Get brands | ✅ |
| `GET` | `/api/mobile/v3/catalog/search` | Search products | ✅ |

### Cart Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/mobile/v3/cart` | Get cart contents | ✅ |
| `POST` | `/api/mobile/v3/cart/items` | Add item to cart | ✅ |
| `PUT` | `/api/mobile/v3/cart/items/{id}` | Update cart item | ✅ |
| `DELETE` | `/api/mobile/v3/cart/items/{id}` | Remove cart item | ✅ |
| `DELETE` | `/api/mobile/v3/cart/clear` | Clear cart | ✅ |
| `POST` | `/api/mobile/v3/cart/coupon` | Apply coupon | ✅ |

### Checkout Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/api/mobile/v3/checkout/start` | Start checkout process | ✅ |
| `POST` | `/api/mobile/v3/checkout/address` | Set checkout address | ✅ |
| `GET` | `/api/mobile/v3/checkout/shipping-methods` | Get shipping methods | ✅ |
| `POST` | `/api/mobile/v3/checkout/shipping-method` | Set shipping method | ✅ |
| `GET` | `/api/mobile/v3/checkout/payment-methods` | Get payment methods | ✅ |
| `POST` | `/api/mobile/v3/checkout/payment-method` | Set payment method | ✅ |
| `POST` | `/api/mobile/v3/checkout/place-order` | Place the order | ✅ |

### Customer Endpoints (Registered Users Only)
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/api/mobile/v3/customer/profile` | Get profile | ✅ |
| `PUT` | `/api/mobile/v3/customer/profile` | Update profile | ✅ |
| `POST` | `/api/mobile/v3/customer/change-password` | Change password | ✅ |
| `GET` | `/api/mobile/v3/customer/addresses` | Get addresses | ✅ |
| `POST` | `/api/mobile/v3/customer/addresses` | Add address | ✅ |
| `PUT` | `/api/mobile/v3/customer/addresses/{id}` | Update address | ✅ |
| `DELETE` | `/api/mobile/v3/customer/addresses/{id}` | Delete address | ✅ |
| `GET` | `/api/mobile/v3/customer/orders` | Get order history | ✅ |
| `GET` | `/api/mobile/v3/customer/orders/{id}` | Get order details | ✅ |

## 📋 Response Format

All API endpoints return a consistent JSON response format:

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {
    // Response data here
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Field 'email' is required",
      "field": "email"
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### HTTP Status Codes

| Code | Description | Usage |
|------|-------------|-------|
| `200` | OK | Successful GET, PUT operations |
| `201` | Created | Successful POST operations |
| `400` | Bad Request | Validation errors, invalid data |
| `401` | Unauthorized | Missing or invalid JWT token |
| `403` | Forbidden | Insufficient permissions |
| `404` | Not Found | Resource not found |
| `422` | Unprocessable Entity | Business logic errors |
| `429` | Too Many Requests | Rate limiting exceeded |
| `500` | Internal Server Error | Server errors |

## 🐛 Error Handling

The Mobile API provides comprehensive error handling with specific error codes:

### Error Codes

| Code | Description | HTTP Status |
|------|-------------|-------------|
| `VALIDATION_ERROR` | Request validation failed | 400 |
| `AUTHENTICATION_ERROR` | Authentication failed | 401 |
| `AUTHORIZATION_ERROR` | Insufficient permissions | 403 |
| `NOT_FOUND_ERROR` | Resource not found | 404 |
| `BUSINESS_LOGIC_ERROR` | Business rule violation | 422 |
| `RATE_LIMIT_ERROR` | Too many requests | 429 |
| `INTERNAL_SERVER_ERROR` | Server error | 500 |

### Example Error Responses

#### Validation Error
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Email is required",
      "field": "email"
    },
    {
      "code": "VALIDATION_ERROR", 
      "message": "Password must be at least 8 characters",
      "field": "password"
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

#### Authentication Error
```json
{
  "success": false,
  "message": "Invalid credentials",
  "errors": [
    {
      "code": "AUTHENTICATION_ERROR",
      "message": "Email or password is incorrect"
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 📚 Documentation

### Interactive API Documentation

The Mobile API includes beautiful interactive documentation powered by Scalar:

- **URL**: `http://your-domain.com/api/mobile/docs`
- **Features**:
  - Interactive request/response examples
  - JWT authentication testing
  - Mobile-optimized interface
  - Real-time API testing
  - Comprehensive endpoint documentation

### OpenAPI Specification

The complete OpenAPI v3 specification is available at:
- **JSON**: `http://your-domain.com/api/mobile/swagger/mobile-api/swagger.json`

## 🧪 Development

### Testing the API

1. **Start the Application**
   ```bash
   dotnet run
   ```

2. **Visit Documentation**
   ```
   http://localhost:5000/api/mobile/docs
   ```

3. **Health Check**
   ```bash
   curl http://localhost:5000/api/mobile/health
   ```

### Development Tips

- **JWT Secret**: Use a development secret key (min 32 characters)
- **HTTPS**: Disable for local development (`RequireHttps: false`)
- **Rate Limiting**: Increase limits for testing
- **Logging**: Enable detailed logging for debugging

### Example Mobile App Integration

```javascript
// Mobile app authentication example
class MobileApiClient {
  constructor(baseUrl) {
    this.baseUrl = baseUrl;
    this.accessToken = localStorage.getItem('accessToken');
    this.refreshToken = localStorage.getItem('refreshToken');
  }

  async createGuestUser(deviceId) {
    const response = await fetch(`${this.baseUrl}/api/mobile/v3/auth/guest`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ deviceId }),
    });
    
    const data = await response.json();
    if (data.success) {
      this.setTokens(data.data.accessToken, data.data.refreshToken);
    }
    return data;
  }

  async login(email, password) {
    const response = await fetch(`${this.baseUrl}/api/mobile/v3/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });
    
    const data = await response.json();
    if (data.success) {
      this.setTokens(data.data.accessToken, data.data.refreshToken);
    }
    return data;
  }

  async apiCall(endpoint, options = {}) {
    const url = `${this.baseUrl}${endpoint}`;
    const headers = {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.accessToken}`,
      ...options.headers,
    };

    const response = await fetch(url, {
      ...options,
      headers,
    });

    if (response.status === 401) {
      // Try to refresh token
      const refreshed = await this.refreshAccessToken();
      if (refreshed) {
        // Retry the original request
        headers['Authorization'] = `Bearer ${this.accessToken}`;
        return fetch(url, { ...options, headers });
      }
    }

    return response;
  }

  setTokens(accessToken, refreshToken) {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }
}
```

## 🚀 Production Deployment

### Security Checklist

- [ ] Use strong JWT secret key (32+ characters)
- [ ] Enable HTTPS (`RequireHttps: true`)
- [ ] Configure proper CORS settings
- [ ] Set appropriate rate limits
- [ ] Use secure issuer/audience values
- [ ] Enable request logging
- [ ] Configure proper error handling
- [ ] Test authentication flows
- [ ] Validate all endpoints
- [ ] Setup monitoring and alerting

### Performance Optimization

- Configure appropriate JWT expiry times
- Implement proper caching strategies
- Use connection pooling for database
- Monitor API response times
- Setup CDN for static content
- Configure proper logging levels

## 📝 Changelog

### Version 1.0.0
- Initial release
- JWT authentication with guest and registered user support
- Complete CRUD operations for catalog, cart, and checkout
- Mobile-optimized DTOs
- Scalar documentation integration
- Comprehensive error handling

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This module is part of GrandNode2 and follows the same MIT license.

## 🆘 Support

For support and questions:
- GrandNode2 Community: [https://grandnode.com/community](https://grandnode.com/community)
- Documentation: [https://docs.grandnode.com](https://docs.grandnode.com)
- Issues: Create an issue in the GrandNode2 repository

---

**Built with ❤️ for mobile developers using GrandNode2**