# TDD-Based Web App

Full-stack project built(in development) trying to use and apply **Test-Driven Development (TDD)** principles, also counting with Clean Architecture, and SOLID.

## Overview

The application consists of:

- **Backend**: A **.NET 9** Web API for user authentication and profile management.
- **Frontend**: A **React (Vite)** web app for user registration, login, and profile editing.
- **Database**: **SQLite** for simplicity and lightweight storage.
- **Authentication**: **JWT-based authentication** with password hashing via `BCrypt.Net`.

---

## Tech Stack

### **Backend**
- **Language**: C#
- **Framework**: ASP.NET Core 9
- **Authentication**: JWT with `System.IdentityModel.Tokens.Jwt`
- **Database**: SQLite (via Entity Framework Core)
- **Testing**: xUnit, Moq

### **Frontend**
- **Library**: React (Vite)
- **State Management**: Redux
- **CSS**: CSS Modules
- **API Handling**: Fetch API for HTTP requests
- **Testing**: Jest & React Testing Library

### **Development Principles**
- **TDD (Test-Driven Development)**: Writing tests before implementing features.
- **SOLID Principles**: Ensuring maintainable and scalable code.
- **Clean Code**: Readable, modular, and well-documented.

---

## Features

### **Backend Features**
- User **Registration** with **BCrypt password hashing**.
- User **Login** with **JWT authentication**.
- **Profile Management** (view & update user details).
- Secure **JWT token generation** and validation.
- Unit tests for **Controllers, Services, and Repositories**.

### **Frontend Features**
- **Register & Login** forms with real-time validation.
- **JWT Token Storage** in `localStorage` for authentication.
- **Authenticated Routes** (Protected user profile page).
- **Logout Functionality**.
- User Profile Update.
- Tested components.
