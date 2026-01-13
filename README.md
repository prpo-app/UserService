# UserService
C# API microservice for BookWorm application - handles user authentication and authorization.

## Features
- User registration with secure password hashing
- User authentication with JWT tokens
- Password validation and security
- RESTful API for user management
- Integration support for other microservices

## Tech Stack
- **Backend**: ASP.NET Core
- **Database**: PostgreSQL
- **Authentication**: JSON Web Tokens (JWT)
- **Password Hashing**: BCrypt.Net
- **Documentation**: Swagger
- **Other**: Docker, Entity Framework Core

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Docker (optional, for containerized setup)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/UserService.git
   cd UserService
   ```

2. Set up the database:
   - Update the connection string in `appsettings.json` to match your PostgreSQL setup.

3. Configure JWT settings:
   - Update the JWT configuration in `appsettings.json` with your secret key, issuer, and audience.

4. Run the application:
   ```bash
   dotnet run
   ```

5. Access the application:
   - API: `http://localhost:<port>`
   - Swagger UI: `http://localhost:<port>/swagger/index.html`

### Running Tests
```bash
dotnet test
```

### Docker Setup (Optional)
1. Build the Docker image:
   ```bash
   docker build -t userservice .
   ```

2. Run the container:
   ```bash
   docker run -p <port>:<port> userservice
   ```

3. Access the application at `http://localhost:<port>`.

## API Endpoints

### Public Endpoints
- `POST /user/register`: Register a new user with username and password.
- `POST /user/login`: Authenticate a user and receive a JWT token.
