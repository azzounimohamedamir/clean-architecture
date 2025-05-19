# Clean Architecture with ASP.NET Core and PostgreSQL

This project is a comprehensive implementation of Clean Architecture with ASP.NET Core 7.0, implementing CQRS pattern with MediatR, AutoMapper for object mapping, and PostgreSQL as the database.

## Technologies & Patterns

- **.NET 7.0**
- **ASP.NET Core**
- **Entity Framework Core**
- **PostgreSQL**
- **MediatR** (CQRS implementation)
- **AutoMapper** (Object mapping)
- **FluentValidation** (Input validation)
- **Swagger/OpenAPI**

## Architecture Overview

### Domain Layer
- Contains enterprise/business logic
- Defines entities, value objects, and domain events
- No dependencies on other layers

### Application Layer
- Contains business logic
- Implements CQRS with MediatR
- Defines interfaces that are implemented by outside layers
- Uses AutoMapper for object mapping
- Implements FluentValidation for input validation

### Infrastructure Layer
- Contains implementations of interfaces defined in the Application layer
- Implements data access using Entity Framework Core
- Configures PostgreSQL database
- Handles external concerns

### WebApi Layer
- Handles HTTP requests and responses
- Dependency injection configuration
- API documentation (Swagger)
- Global error handling

## Features

### Product Management
- Create new products
- Retrieve all products
- Update existing products
- Delete products
- Input validation
- DTO mapping
- Error handling

## Project Structure

```
src/
├── Domain/
│   └── Entities/
│       └── Product.cs
├── Application/
│   ├── Common/
│   │   ├── Interfaces/
│   │   ├── Mappings/
│   │   └── Exceptions/
│   └── Products/
│       ├── Commands/
│       │   ├── CreateProduct/
│       │   ├── UpdateProduct/
│       │   └── DeleteProduct/
│       ├── Queries/
│       │   └── GetProducts/
│       └── DTOs/
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── ApplicationDbContext.cs
│   └── ConfigureServices.cs
└── WebApi/
    ├── Controllers/
    ├── Middleware/
    └── Program.cs
```

## Getting Started

### Prerequisites

1. .NET 7.0 SDK
2. PostgreSQL
3. Your favorite IDE (Visual Studio, VS Code, etc.)

### Database Configuration

Update the connection string in `src/WebApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cleanarchdb;Username=your_username;Password=your_password"
  }
}
```

### Running the Application

1. Clone the repository
2. Navigate to the solution directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Apply database migrations:
   ```bash
   cd src/WebApi
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at:
- API: https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

## API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | /api/products | Get all products |
| POST   | /api/products | Create a new product |
| PUT    | /api/products/{id} | Update an existing product |
| DELETE | /api/products/{id} | Delete a product |

## Error Handling

The application includes global error handling that:
- Catches all unhandled exceptions
- Returns appropriate HTTP status codes
- Provides meaningful error messages
- Logs errors for debugging
- Handles validation errors gracefully

## Best Practices Implemented

1. **Clean Architecture**
   - Separation of concerns
   - Dependency inversion
   - Domain-driven design principles

2. **CQRS Pattern**
   - Separate command and query responsibilities
   - Use of MediatR for handling commands and queries

3. **Repository Pattern**
   - Abstraction of data persistence
   - Centralized data access logic

4. **Input Validation**
   - FluentValidation for robust input validation
   - Validation pipeline behavior

5. **Object Mapping**
   - AutoMapper for clean DTO transformations
   - Mapping profiles for type conversion

6. **Error Handling**
   - Global exception handling
   - Consistent error responses
   - Proper HTTP status codes

7. **API Documentation**
   - Swagger/OpenAPI integration
   - Detailed endpoint documentation

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
