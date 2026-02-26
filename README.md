# InsurTIX Book Management API

A robust .NET Core Web API built for managing a bookstore's inventory. The application interfaces with an XML-based data store and provides a full CRUD RESTful API, alongside dynamic HTML report generation.

## 🏗 Architecture & Design Decisions

To ensure clean, maintainable, and scalable code, the project is structured using the **N-Tier Architecture**:

- **Controllers:** Handle incoming HTTP requests, basic validation, and return standard HTTP status codes.
- **Services (Business Logic):** `BookService` handles data transformation (e.g., formatting authors as comma-separated strings) and HTML report generation.
- **Repositories (Data Access):** `XmlBookRepository` isolates all file I/O operations.
- **DTOs:** Used to separate the internal XML data representation from the data contract exposed to the client.

### 🛡 Thread Safety & Concurrency

Since Web APIs handle multiple requests concurrently, accessing a single XML file directly can lead to `IOException` (file locked). To prevent this, the `XmlBookRepository` utilizes a `SemaphoreSlim` to ensure thread-safe read/write operations.

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or your specific version)
- VS Code / Visual Studio

### Running the Project

1. Clone the repository.
2. Navigate to the project directory: `cd InsurTix.Api`
3. Run the application: `dotnet run`
4. Open your browser and navigate to the Swagger UI to interact with the endpoints:
   `http://localhost:<port>/swagger`

### 📊 HTML Report

To view the generated HTML inventory report, simply navigate to:
`http://localhost:<port>/api/books/report`
