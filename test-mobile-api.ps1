# Mobile API Test Script
# This script tests the basic functionality of the GrandNode2 Mobile API

Write-Host "🚀 Testing GrandNode2 Mobile API..." -ForegroundColor Green

# Base API URL
$baseUrl = "http://localhost:8080/api/mobile/v3"

# Test 1: Health Check
Write-Host "`n📊 Testing Health Check..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "http://localhost:8080/api/mobile/health" -Method GET
    Write-Host "✅ Health Check: SUCCESS" -ForegroundColor Green
    Write-Host "Status: $($healthResponse.Status)" -ForegroundColor Cyan
    Write-Host "Version: $($healthResponse.Version)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Health Check: FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Create Guest User
Write-Host "`n👤 Testing Guest User Creation..." -ForegroundColor Yellow
$guestBody = @{
    deviceId = "test-device-12345"
} | ConvertTo-Json

try {
    $guestResponse = Invoke-RestMethod -Uri "$baseUrl/auth/guest" -Method POST -ContentType "application/json" -Body $guestBody
    Write-Host "✅ Guest User: SUCCESS" -ForegroundColor Green
    
    if ($guestResponse.success) {
        $guestToken = $guestResponse.data.accessToken
        Write-Host "Token received: $($guestToken.Substring(0, 50))..." -ForegroundColor Cyan
        
        # Test 3: Get Products with Guest Token
        Write-Host "`n🛍️ Testing Product List (Guest)..." -ForegroundColor Yellow
        try {
            $headers = @{
                "Authorization" = "Bearer $guestToken"
                "Content-Type" = "application/json"
            }
            
            $productsResponse = Invoke-RestMethod -Uri "$baseUrl/catalog/products" -Method GET -Headers $headers
            Write-Host "✅ Product List: SUCCESS" -ForegroundColor Green
            
            if ($productsResponse.success) {
                Write-Host "Products found: $($productsResponse.data.products.Count)" -ForegroundColor Cyan
            }
        } catch {
            Write-Host "❌ Product List: FAILED" -ForegroundColor Red
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        }
        
        # Test 4: Get Cart (Guest)
        Write-Host "`n🛒 Testing Cart Access (Guest)..." -ForegroundColor Yellow
        try {
            $cartResponse = Invoke-RestMethod -Uri "$baseUrl/cart" -Method GET -Headers $headers
            Write-Host "✅ Cart Access: SUCCESS" -ForegroundColor Green
            
            if ($cartResponse.success) {
                Write-Host "Cart items: $($cartResponse.data.items.Count)" -ForegroundColor Cyan
                Write-Host "Total: $($cartResponse.data.total)" -ForegroundColor Cyan
            }
        } catch {
            Write-Host "❌ Cart Access: FAILED" -ForegroundColor Red
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "❌ Guest User Creation Failed: $($guestResponse.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Guest User: FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Test User Registration
Write-Host "`n📝 Testing User Registration..." -ForegroundColor Yellow
$registerBody = @{
    email = "testuser@example.com"
    password = "TestPassword123!"
    confirmPassword = "TestPassword123!"
    firstName = "Test"
    lastName = "User"
} | ConvertTo-Json

try {
    $registerResponse = Invoke-RestMethod -Uri "$baseUrl/auth/register" -Method POST -ContentType "application/json" -Body $registerBody
    Write-Host "✅ User Registration: SUCCESS" -ForegroundColor Green
    
    if ($registerResponse.success) {
        $userToken = $registerResponse.data.accessToken
        Write-Host "User token received: $($userToken.Substring(0, 50))..." -ForegroundColor Cyan
        
        # Test 6: Get Products with User Token
        Write-Host "`n🛍️ Testing Product List (Registered User)..." -ForegroundColor Yellow
        try {
            $userHeaders = @{
                "Authorization" = "Bearer $userToken"
                "Content-Type" = "application/json"
            }
            
            $userProductsResponse = Invoke-RestMethod -Uri "$baseUrl/catalog/products" -Method GET -Headers $userHeaders
            Write-Host "✅ Product List (User): SUCCESS" -ForegroundColor Green
            
            if ($userProductsResponse.success) {
                Write-Host "Products found: $($userProductsResponse.data.products.Count)" -ForegroundColor Cyan
            }
        } catch {
            Write-Host "❌ Product List (User): FAILED" -ForegroundColor Red
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "❌ User Registration Failed: $($registerResponse.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ User Registration: FAILED" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 Mobile API Testing Complete!" -ForegroundColor Green
Write-Host "`n📚 Documentation: http://localhost:8080/api/mobile/docs" -ForegroundColor Cyan
Write-Host "🏠 Main Application: http://localhost:8080/" -ForegroundColor Cyan