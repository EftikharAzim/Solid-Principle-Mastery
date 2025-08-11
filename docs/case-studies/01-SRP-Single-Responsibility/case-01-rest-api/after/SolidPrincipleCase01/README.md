# Case Study 1: Single Responsibility Principle (SRP) - REST API

## 📋 Problem Statement

Build a REST API where:
- Users can login and logout
- Users can upload a CSV file with keywords  
- For each keyword, perform a Google search via network call
- Store search results in database

**Challenge:** Design this system following SOLID principles, specifically focusing on **Single Responsibility Principle (SRP)**.

## 🎯 Learning Objectives

- ✅ Understand what SRP means: "A class should have only one reason to change"
- ✅ Learn to identify different responsibilities in a system
- ✅ Apply Controller → Service → Repository pattern
- ✅ Experience the benefits of proper separation of concerns

## 🚫 Common Violations (What NOT to Do)

**❌ Bad Design:** One massive class handling everything
```csharp
public class UserFileController  // VIOLATES SRP!
{
    // HTTP handling + Password validation + Database access + 
    // File parsing + Google API calls + Email notifications
    // = 6+ different reasons to change!
}
```

**Problems with Bad Design:**
- Hard to test individual parts
- Changes to one feature break other features  
- Multiple developers can't work on it simultaneously
- Debugging is nightmare - where's the bug?

## ✅ Our SRP-Compliant Solution

### Architecture Overview
```
HTTP Request → Controller → Service → Repository → Database
```

### Responsibility Separation

| Class | Single Responsibility | Reason to Change |
|-------|---------------------|------------------|
| `UserController` | Handle HTTP requests for user operations | API endpoints change |
| `FileController` | Handle HTTP requests for file operations | File endpoints change |
| `PasswordValidationService` | Validate user credentials | Authentication rules change |
| `FileValidationService` | Validate and parse CSV files | File format rules change |
| `GoogleSearchService` | Perform external search operations | Search API changes |
| `InMemoryUserRepository` | Store/retrieve user data | Data storage method changes |
| `InMemorySearchResultRepository` | Store/retrieve search results | Search data storage changes |

## 🏗️ Project Structure

```
SolidPrinciplesCase01/
├── Program.cs                 # DI configuration & app setup
├── Controllers/               # HTTP layer (in Program.cs)
│   ├── UserController         # User login/logout endpoints
│   └── FileController         # File upload endpoints
├── Services/                  # Business logic (in Program.cs)
│   ├── PasswordValidationService    # Authentication logic
│   ├── FileValidationService       # File processing logic
│   └── GoogleSearchService         # External API calls
├── Repositories/             # Data access (in Program.cs)
│   ├── InMemoryUserRepository      # User data storage
│   └── InMemorySearchResultRepository # Search results storage
├── Models/                   # Data models (in Program.cs)
├── keywords.csv              # Test data
└── README.md                 # This file
```

## 🚀 How to Run

### Prerequisites
- .NET 8.0 SDK
- Any text editor or IDE

### Setup & Run
```bash
# Navigate to project directory
cd SolidPrinciplesCase01

# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

**Application URLs:**
- 🌐 **Swagger UI:** https://localhost:7000/swagger
- 📡 **API Base:** https://localhost:7000/api

## 🧪 Testing Guide

### Test Data Setup
Create `keywords.csv` in project root:
```csv
asp.net core
solid principles
dependency injection
clean architecture
design patterns
unit testing
microservices
rest api
```

### API Endpoints Testing

#### 1. User Login 👤
**Endpoint:** `POST /api/users/login`

**Test Data:**
```json
{
  "username": "demo",
  "password": "password"
}
```

**Expected Response:**
```json
{
  "token": "jwt_token_for_demo_123456789",
  "message": "Login successful"
}
```

**Available Test Users:**
- Username: `demo`, Password: `password`
- Username: `admin`, Password: `admin123`  
- Username: `testuser`, Password: `test123`

#### 2. View All Users 👥
**Endpoint:** `GET /api/users`

**Expected Response:**
```json
[
  {
    "id": 1,
    "username": "demo",
    "email": "demo@example.com",
    "createdAt": "2024-01-15T10:30:00Z"
  }
]
```

#### 3. Upload CSV File 📄
**Endpoint:** `POST /api/files/upload`

1. Use Swagger UI "Choose file" button
2. Select your `keywords.csv` file
3. Click "Execute"

**Expected Response:**
```json
{
  "message": "File processed successfully! Found 24 search results.",
  "keywordCount": 8,
  "resultCount": 24,
  "processedKeywords": [
    "asp.net core",
    "solid principles",
    "dependency injection"
  ]
}
```

#### 4. View All Search Results 🔍
**Endpoint:** `GET /api/files/results`

Returns all search results stored in memory.

#### 5. Search Results by Keyword 🎯
**Endpoint:** `GET /api/files/results/{keyword}`

Example: `GET /api/files/results/asp.net%20core`

Returns results for specific keyword only.

## 🔍 What Happens Behind the Scenes

### File Upload Process Flow
```
1. FileController receives HTTP request
   ↓
2. FileValidationService validates CSV format
   ↓  
3. FileValidationService extracts keywords
   ↓
4. GoogleSearchService searches each keyword (mock)
   ↓
5. SearchResultRepository stores all results
   ↓
6. FileController returns success response
```

### Login Process Flow  
```
1. UserController receives login request
   ↓
2. PasswordValidationService validates credentials
   ↓
3. UserRepository retrieves user data
   ↓
4. PasswordValidationService generates token
   ↓
5. UserController returns token response
```

## 💡 SRP Benefits You'll Experience

### ✅ Easy Testing
Each service can be tested independently:
```csharp
// Test only password validation logic
var mockRepo = new Mock<IUserRepository>();
var service = new PasswordValidationService(mockRepo.Object);
var result = await service.ValidatePasswordAsync("demo", "password");
```

### ✅ Easy Debugging
- Login failing? Check `PasswordValidationService` logs
- File parsing error? Check `FileValidationService` logs  
- Search not working? Check `GoogleSearchService` logs

### ✅ Easy Changes
- Want Bing instead of Google? Only change `GoogleSearchService`
- Want SQL instead of memory? Only change Repository implementations
- Want different file formats? Only change `FileValidationService`

## 🎓 Key Learnings

### Before Understanding SRP
> "I need one big class that does everything!"

**Problems:**
- 200+ line methods
- Hard to test
- Changes break other features
- Can't work in teams

### After Understanding SRP  
> "I need focused classes, each with one clear purpose!"

**Benefits:**
- 20-line focused classes
- Easy to test each part
- Changes are isolated
- Multiple developers can collaborate

### The "Aha!" Moment
**Instead of:** 1 complex problem
**You get:** Several simple problems

**Controller-Service-Repository Pattern:**
- **Controllers:** Handle HTTP stuff only
- **Services:** Handle business logic only  
- **Repositories:** Handle data stuff only

## 🔄 Real-World Production Changes

### Development (Current)
```csharp
// In-memory storage (resets on restart)
services.AddScoped<IUserRepository, InMemoryUserRepository>();
services.AddScoped<ISearchService, GoogleSearchService>();
```

### Production (Future)
```csharp
// Persistent database
services.AddScoped<IUserRepository, SqlServerUserRepository>();

// Real search service
services.AddScoped<ISearchService, BingSearchService>();
```

**The Magic:** Controllers and Services don't change at all! Only repository implementations change.

## 🚨 Common Beginner Questions

### Q: "Why so many classes? Isn't this over-engineering?"
**A:** More classes = Less complexity per class. Would you rather debug:
- 1 class with 8 responsibilities? 
- 8 classes with 1 responsibility each?

### Q: "When do I create a new service vs extending existing?"
**A:** Ask: "Does this have a different reason to change?" If yes, new service.
