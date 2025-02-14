# RealEstatePortal API

## Overview
RealEstatePortal API is a robust and scalable backend service built with ASP.NET for managing real estate listings. This API provides functionalities for property management, user authentication, and advanced search capabilities, enabling seamless interactions for buyers, sellers, and agents.

## Features
- **Property Management**: Advanced operations for real estate business logic.
- **User Authentication & Authorization**: Secure user authentication with role-based access control.
- **Advanced Search**: Filter properties based on various criteria such as location, price, and size.
- **RESTful API**: Follows REST principles for easy integration with frontend applications.
- **Database Management**: Uses SQL Server for efficient data storage and retrieval.
- **Error Handling & Logging**: Implements structured error handling and logging mechanisms.

- **Rate Limiter**: Implements rate limiting to protect the API from overuse and ensure consistent performance.
- **Minimal API (Carter)**: Utilizes Carter to build minimal APIs, simplifying routing and improving overall API performance.
- **AutoMapper**: Automates the mapping of DTOs (Data Transfer Objects) to entity models and vice versa, reducing boilerplate code.
- **API Versioning**: Supports API versioning to manage updates and changes without breaking existing clients.
- **Reflection**: Leverages reflection for dynamic object mapping and service discovery, providing flexibility in code execution.
- **Repository Pattern**: Implements the repository pattern for data abstraction, ensuring cleaner, testable, and maintainable code.
- **ElasticSearch**: Integrates ElasticSearch for advanced full-text search capabilities, enabling fast and efficient property listings search.
- **ResourceDesigner**: Automates the design and transformation of resources to ensure consistency and maintainability across the API.
- **Swagger**: Automatically generates API documentation with Swagger UI, allowing developers to easily test and explore the API endpoints.
- **Redis**: Implements Redis for caching frequently accessed data, improving response time, and enhancing scalability.


## Getting Started

### Prerequisites
- .NET 6.0 or later
- SQL Server
- Visual Studio or VS Code
- Postman (optional, for testing API endpoints)

### Installation & Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/alitaami/RealEstatePortal_Api.git
   cd RealEstatePortal_Api
   ```

2. **Open the Solution**
   - Open `RealEstatePortal_Api.sln` in Visual Studio.

3. **Restore Dependencies**
   - Run the following command in the terminal:
     ```bash
     dotnet restore
     ```

4. **Database Setup**
   - Ensure SQL Server is running.
   - Restore the provided database backup file (`EstateProjectDbBackup.sql`).
   - Update the connection string in `WebApplicationBuilderExtensions.AddAppDbContext`:
   ```
    options.UseSqlServer("Data Source =YOUR_SERVER; Initial Catalog=EstateProject; Integrated Security=true;Trust Server Certificate=true;");
   ```

6. **Apply Migrations (If Needed)**
   ```bash
   add-migration init
   update-database
   ```

7. **Run the Application**
   ```bash
   dotnet run
   ```
   The API should now be running at `https://localhost:5001` (or the configured port).

## Project Structure
```
RealEstatePortal_Api/
│── Common/             # Shared utilities and helper classes
│── Data/               # Database context and migrations
│── Entities/           # Models and entity definitions
│── Services/           # Business logic and service implementations
│── WebFramework/       # Middleware, extensions, and API configurations
│── EstateAgentApi/     # Main Web API project
│── appsettings.json    # Configuration settings
│── Program.cs         # Entry point for the application
```

## API Endpoints
I have documented all endpoints in Swagger; 
So if you build the project you will be able to see all Endpoints.
 
## Contributing
Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m "Add new feature"`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a Pull Request.

## License
This project is licensed under the MIT License. See `LICENSE` for more details.

## Contact
For any inquiries, please reach out via GitHub Issues.

