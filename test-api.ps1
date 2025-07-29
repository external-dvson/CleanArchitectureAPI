# API Test Script
# This PowerShell script demonstrates the Clean Architecture API functionality

$baseUrl = "http://localhost:5216"

Write-Host "=== Clean Architecture API Test Script ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Yellow
Write-Host ""

# Test 1: Create a user
Write-Host "1. Creating a new user..." -ForegroundColor Cyan
$newUser = @{
    Username = "john_doe"
} | ConvertTo-Json

try {
    $user1 = Invoke-RestMethod -Uri "$baseUrl/api/users" -Method POST -Body $newUser -ContentType "application/json"
    Write-Host "User created successfully!" -ForegroundColor Green
    Write-Host "User ID: $($user1.id)" -ForegroundColor White
    Write-Host "Username: $($user1.username)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "Failed to create user: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 2: Create another user
Write-Host "2. Creating another user..." -ForegroundColor Cyan
$newUser2 = @{
    Username = "jane_smith"
} | ConvertTo-Json

try {
    $user2 = Invoke-RestMethod -Uri "$baseUrl/api/users" -Method POST -Body $newUser2 -ContentType "application/json"
    Write-Host "User created successfully!" -ForegroundColor Green
    Write-Host "User ID: $($user2.id)" -ForegroundColor White
    Write-Host "Username: $($user2.username)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "Failed to create user: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 3: Get all users
Write-Host "3. Retrieving all users..." -ForegroundColor Cyan
try {
    $allUsers = Invoke-RestMethod -Uri "$baseUrl/api/users" -Method GET
    Write-Host "Users retrieved successfully!" -ForegroundColor Green
    Write-Host "Total users: $($allUsers.Count)" -ForegroundColor White
    foreach ($user in $allUsers) {
        Write-Host "- ID: $($user.id), Username: $($user.username)" -ForegroundColor White
    }
    Write-Host ""
} catch {
    Write-Host "Failed to retrieve users: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "=== API Test Complete ===" -ForegroundColor Green
