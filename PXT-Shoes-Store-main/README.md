# PXT Shoes Shop

## Introduction
PXT Shoes Shop is a C# application built with Visual Studio 2022 that provides an online shopping platform for shoes. The system supports user authentication, shopping cart management, and order processing, including payment integration with VNPay. It has two main user roles: **Admin** and **User**.

PXT Shoes Shop is developed based on the **VinaShoeShop** template, enhancing its features and performance for a seamless shopping experience.

## Features
### User Functionality
- **Registration & Login**: Secure user authentication system.
- **Product Browsing**: View available shoes with details and pricing.
- **Shopping Cart**: Add, remove, and update products in the cart.
- **Checkout & Payment**: Complete purchases using VNPay.
- **Order History**: Track previous orders and status updates.

### Admin Functionality
- **Product Management**: Add, edit, delete, and manage shoe inventory.
- **Order Management**: View and update order status.
- **User Management**: Manage registered users and their roles.
- **Sales Report**: Generate and view reports on sales and inventory.

## Technologies Used
- **Programming Language**: C#
- **Framework**: .NET Core
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: JWT-based authentication
- **Payment Gateway**: VNPay
- **IDE**: Visual Studio 2022

## Installation
### Prerequisites
- Install **Visual Studio 2022** with .NET Core SDK.
- Install **SQL Server** and configure a database instance.

### Setup Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/pxt-shoes-shop.git
   cd pxt-shoes-shop
   ```
2. Configure the database connection in `appsettings.json`:
   ```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "Server=YOUR_SERVER;Database=PXT_Shoes;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
       }
   }
   ```
3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

## Usage
- **User Login**: Register and log in to browse products and place orders.
- **Admin Access**: Log in as an admin to manage inventory, users, and orders.
- **Payment Processing**: Select VNPay at checkout for seamless transactions.

## Contribution
Feel free to contribute by submitting pull requests or reporting issues.

## License
This project is licensed under the MIT License.

---
For any queries, contact **@hidr0**.
