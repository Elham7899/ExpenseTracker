# 💰 Expense Tracker API

A **clean, modular, and scalable Expense Tracking System** built with **.NET 8** following **Clean Architecture** principles.  
Designed to help users manage their expenses efficiently while showcasing enterprise-level architecture, maintainability, and testability.

## 🚀 Tech Stack

- **.NET 8**, **C#**
- **Entity Framework Core**
- **AutoMapper**, **MediatR**
- **SQL Server**
- **JWT Authentication**
- **Swagger / OpenAPI**
- **xUnit** (Unit & Integration Testing)

## 🧩 Architecture Overview

📦 ExpenseTracker
┣ 📂 Domain → Core entities (User, Category, Transaction)
┣ 📂 Application → DTOs, Interfaces, Business Logic, Services
┣ 📂 Infrastructure → EF Core Repositories, Migrations, Database Context
┣ 📂 API → Controllers, Endpoints, Swagger
┗ 📂 Tests → Unit & Integration Tests

### 🧠 Key Highlights

- 🧱 Implements **Clean Architecture** with strict separation of concerns  
- 🔄 Uses **AutoMapper** for clean and maintainable data transformation  
- 🔐 Secured with **JWT-based authentication**  
- 🧾 Includes **Swagger/OpenAPI** for complete API documentation  
- 🧪 Designed for **testability** and **scalability**

## 🧪 Getting Started
 1️⃣ Clone the Repository
git clone https://github.com/Elham7899/ExpenseTracker.git
2️⃣ Configure the Database
Update your connection string in appsettings.json under the Infrastructure or API project.
3️⃣ Apply Migrations
dotnet ef database update
4️⃣ Run the Application
dotnet run
Visit Swagger UI at:
https://localhost:5001/swagger

✅ Next Future Enhancements

🌐 Adding front-end client (React or Blazor)
📧 Introduce email notifications for expense thresholds
📊 Add analytics dashboards with charts and insights
🕵️ Role-based access and audit logging
☁️ Docker and CI/CD pipeline setup for deployment

📚 Learning Outcomes

This project demonstrates:
Clean Architecture and layered design
Dependency injection and interface-driven development
Secure API design using JWT authentication
Modern .NET API development practices

🤝 Contributing

Contributions are welcome!
Feel free to open an issue or submit a pull request to discuss improvements or new ideas.

🧾 License
This project is open source and available under the MIT License

👤 Author
Elham
https://github.com/Elham7899
https://www.linkedin.com/in/elham-ghorbanzade-909823200/
