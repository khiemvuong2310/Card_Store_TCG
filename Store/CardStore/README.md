# Card Store TCG Backend API

A comprehensive .NET Aspire-based backend API for managing a Trading Card Game (TCG) store. This API provides complete CRUD operations for cards, users, orders, and collections with JWT authentication, PostgreSQL database, and Redis caching.

## Features

### 🎯 Core Functionality
- **Card Management**: Full CRUD operations for TCG cards with search and filtering
- **User Management**: User registration, authentication, and profile management
- **Order Processing**: Complete order lifecycle management with status tracking
- **Collection Management**: User card collection tracking and valuation
- **JWT Authentication**: Secure token-based authentication system

### 🏗️ Architecture
- **Clean Architecture**: Separated concerns with distinct layers
- **Repository Pattern**: Abstracted data access layer
- **Service Layer**: Business logic encapsulation
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Request validation

### 🚀 Technology Stack
- **.NET 8 & .NET Aspire**: Modern distributed application framework
- **PostgreSQL**: Primary database with Entity Framework Core
- **Redis**: Caching and session management
- **JWT Bearer Authentication**: Secure authentication
- **Swagger/OpenAPI**: API documentation
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation
- **BCrypt**: Password hashing

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/validate-token` - Token validation

### Cards
- `GET /api/v1/cards` - Get all cards (with pagination)
- `GET /api/v1/cards/{id}` - Get card by ID
- `POST /api/v1/cards` - Create new card (Admin)
- `PUT /api/v1/cards/{id}` - Update card (Admin)
- `DELETE /api/v1/cards/{id}` - Delete card (Admin)
- `GET /api/v1/cards/search?q={query}` - Search cards
- `GET /api/v1/cards/by-set/{set}` - Get cards by set
- `GET /api/v1/cards/by-rarity/{rarity}` - Get cards by rarity
- `GET /api/v1/cards/by-price?minPrice={min}&maxPrice={max}` - Get cards by price range
- `GET /api/v1/cards/in-stock` - Get cards in stock

### Users
- `GET /api/v1/users/profile` - Get current user profile
- `PUT /api/v1/users/profile` - Update current user profile
- `DELETE /api/v1/users/profile` - Delete current user account
- `GET /api/v1/users/check-username/{username}` - Check username availability
- `GET /api/v1/users/check-email/{email}` - Check email availability

### Orders
- `GET /api/v1/orders` - Get current user's orders
- `GET /api/v1/orders/{id}` - Get specific order
- `POST /api/v1/orders` - Create new order
- `POST /api/v1/orders/calculate-total` - Calculate order total
- `POST /api/v1/orders/{id}/cancel` - Cancel order
- `GET /api/v1/orders/pending` - Get pending orders (Admin)
- `PUT /api/v1/orders/{id}/status` - Update order status (Admin)
- `GET /api/v1/orders/stats/revenue` - Get revenue statistics (Admin)

### Collections
- `GET /api/v1/collections` - Get user's collection
- `GET /api/v1/collections/summary` - Get collection summary
- `GET /api/v1/collections/cards/{cardId}` - Get specific card in collection
- `POST /api/v1/collections` - Add card to collection
- `PUT /api/v1/collections/cards/{cardId}` - Update card in collection
- `DELETE /api/v1/collections/cards/{cardId}` - Remove card from collection
- `GET /api/v1/collections/value` - Get collection value

## Project Structure

```
CardStore/
├── Controllers/              # API Controllers
│   ├── AuthController.cs
│   ├── CardsController.cs
│   ├── UsersController.cs
│   ├── OrdersController.cs
│   └── CollectionsController.cs
├── Services/                 # Business Logic Layer
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── ICardService.cs
│   ├── CardService.cs
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── IOrderService.cs
│   ├── OrderService.cs
│   ├── ICollectionService.cs
│   └── CollectionService.cs
├── Data/                     # Data Access Layer
│   ├── AppDbContext.cs
│   ├── Repositories/         # Repository Pattern
│   │   ├── IBaseRepository.cs
│   │   ├── BaseRepository.cs
│   │   ├── ICardRepository.cs
│   │   ├── CardRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── OrderRepository.cs
│   │   ├── ICollectionRepository.cs
│   │   └── CollectionRepository.cs
│   └── Configurations/       # EF Configurations
│       ├── CardConfiguration.cs
│       ├── UserConfiguration.cs
│       ├── OrderConfiguration.cs
│       ├── OrderItemConfiguration.cs
│       └── CollectionConfiguration.cs
├── Models/                   # Domain Models
│   ├── Card.cs
│   ├── User.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Collection.cs
│   └── ApiResponse.cs
├── DTOs/                     # Data Transfer Objects
│   ├── CardDto.cs
│   ├── CreateCardDto.cs
│   ├── UpdateCardDto.cs
│   ├── UserDto.cs
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   ├── OrderDto.cs
│   ├── CreateOrderDto.cs
│   └── CollectionDto.cs
├── Validators/               # FluentValidation Validators
│   ├── CreateCardDtoValidator.cs
│   ├── RegisterDtoValidator.cs
│   └── CreateOrderDtoValidator.cs
├── Middleware/               # Custom Middleware
│   ├── ExceptionMiddleware.cs
│   └── JwtMiddleware.cs
├── Attributes/               # Custom Attributes
│   └── AuthorizeAttribute.cs
├── Extensions/               # Extension Methods
│   ├── ServiceCollectionExtensions.cs
│   └── DatabaseExtensions.cs
├── Mappings/                 # AutoMapper Profiles
│   └── AutoMapperProfile.cs
└── Program.cs               # Application Entry Point
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker (for PostgreSQL and Redis)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Card_Store_TCG
   ```

2. **Run with .NET Aspire**
   ```bash
   cd Store
   dotnet run --project Store.AppHost
   ```

   This will automatically start:
   - PostgreSQL database
   - Redis cache
   - CardStore API

3. **Access the API**
   - API: `https://localhost:<port>`
   - Swagger Documentation: `https://localhost:<port>/swagger`
   - Aspire Dashboard: `https://localhost:15888`

### Configuration

The API uses the following default configuration in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-that-is-at-least-32-characters-long!",
    "Issuer": "CardStoreAPI",
    "Audience": "CardStoreClients"
  }
}
```

**⚠️ Security Note**: Change the JWT secret key in production!

### Database

The application automatically:
- Creates the PostgreSQL database on first run
- Applies Entity Framework migrations
- Seeds sample card data for development

### Authentication

The API uses JWT Bearer tokens for authentication:

1. Register a new user at `/api/v1/auth/register`
2. Login at `/api/v1/auth/login` to receive a JWT token
3. Include the token in the Authorization header: `Bearer <token>`

### Sample Data

The development environment includes sample cards:
- Blue-Eyes White Dragon (Legendary)
- Dark Magician (Epic)
- Exodia the Forbidden One (Mythic)
- Mystical Space Typhoon (Common)
- Mirror Force (Rare)

## Development

### Running Tests
```bash
dotnet test
```

### Adding Migrations
```bash
dotnet ef migrations add <MigrationName> --project CardStore
dotnet ef database update --project CardStore
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__cardstoredb`: PostgreSQL connection string
- `ConnectionStrings__redis`: Redis connection string

## Production Deployment

1. **Update JWT Settings**: Use a secure secret key
2. **Configure CORS**: Update allowed origins
3. **Database**: Configure production PostgreSQL
4. **Caching**: Configure production Redis
5. **Logging**: Configure structured logging
6. **Health Checks**: Monitor `/health` endpoint

## License

This project is licensed under the MIT License.