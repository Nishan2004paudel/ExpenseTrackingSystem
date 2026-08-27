# Hisab Kitab

Hisab Kitab is a full-stack personal expense tracking application designed to help users manage their expenses, categories, budgets, and personal financial activities.

The system consists of an ASP.NET Core Web API backend and an Angular frontend. The backend is responsible for authentication, business logic, database operations, budget tracking, expense management, administration, and real-time notifications.

## Project Overview

Hisab Kitab provides users with a centralized platform to:

- Manage personal expenses
- Organize expenses using categories
- Set monthly budgets
- Monitor budget usage
- View expense and budget summaries
- Manage user profiles and settings
- Receive budget-related notifications
- Authenticate securely using JWT-based authentication
- Access administrative features based on user roles

The backend follows a layered approach where controllers handle HTTP requests, services contain business logic, and repositories handle database operations.

## Backend

The backend of Hisab Kitab is built using ASP.NET Core Web API.

It provides RESTful APIs for authentication, users, categories, expenses, budgets, dashboards, profiles, settings, administration, and notifications.

### Backend Technology Stack

- C#
- ASP.NET Core Web API
- .NET 8
- SQL Server
- Dapper
- JWT Authentication
- SignalR
- Swagger / OpenAPI
- SMTP / Email Services
- Google Authentication
- BCrypt Password Hashing
- Git for version control

## Backend Architecture

The backend is organized into several layers to keep responsibilities separated and make the application easier to maintain.

### Controllers

Controllers expose HTTP endpoints to the frontend and handle incoming API requests.

They are responsible for:

- Receiving requests
- Validating request models
- Obtaining the authenticated user's identity
- Calling the appropriate service
- Returning API responses

### Services

Services contain the main business logic of the application.

Examples include:

- `AuthService`
- `CategoryService`
- `ExpenseService`
- `BudgetService`
- `DashboardService`
- `ProfileService`
- `SettingService`
- `AdminService`
- `NotificationService`

Services validate business rules before performing database operations.

### Repositories

Repositories are responsible for communicating with the database.

Examples include:

- `UserRepository`
- `CategoryRepository`
- `ExpenseRepository`
- `BudgetRepository`
- `DashboardRepository`
- `RefreshTokenRepository`
- `AdminRepository`
- `NotificationRepository`

Dapper is used to execute SQL queries and map database results to C# models.

### DTOs

Data Transfer Objects are used to control the data exchanged between the API and frontend.

Examples include:

- Login DTOs
- Registration DTOs
- Expense DTOs
- Budget DTOs
- Category DTOs
- Dashboard DTOs
- Notification DTOs

DTOs prevent internal database models from being directly exposed through API endpoints.

## Authentication and Authorization

Hisab Kitab uses JWT-based authentication to secure API endpoints.

### JWT Authentication

After successful authentication, the backend generates an access token containing information about the authenticated user.

Protected endpoints use the `[Authorize]` attribute to ensure that only authenticated users can access them.

The JWT contains claims such as:

- User ID
- Token version
- User-related authentication information

The API validates:

- Token signature
- Issuer
- Audience
- Token lifetime
- Token version

### Token Version Validation

The application uses a token version mechanism to invalidate previously issued tokens.

When a JWT is validated, the backend compares the token's `tokenVersion` claim with the current token version stored for the user.

If the versions do not match, the token is rejected.

This allows previously issued access tokens to become invalid when necessary.

### Refresh Tokens

The backend also supports refresh tokens to allow users to obtain a new access token without logging in again.

Refresh tokens are stored and managed through the refresh token repository.

### Google Authentication

Google authentication is also supported as an authentication method.

The backend handles Google authentication settings and integrates the authentication flow with the application's user system.

## User Management

The backend provides functionality for managing users and their account information.

User-related functionality includes:

- Registration
- Login
- Logout
- Email verification
- Password reset
- Refresh token handling
- Profile management
- Account settings

Password authentication uses password hashing rather than storing plain-text passwords.

## Category Management

Categories allow users to organize their expenses.

Users can:

- Create categories
- View categories
- Update categories
- Delete categories

Categories belong to individual users.

The backend checks category ownership before allowing a user to create or modify expenses using a category.

## Expense Management

Expense management is one of the core features of Hisab Kitab.

Users can:

- Create expenses
- View individual expenses
- Update expenses
- Soft-delete expenses
- Filter expenses

An expense contains information such as:

- Expense ID
- User ID
- Category ID
- Amount
- Expense date
- Description

### Expense Validation

The backend validates expenses before saving them.

For example:

- The selected category must exist.
- The category must belong to the authenticated user.
- Expense dates cannot be in the future.
- Users cannot modify another user's expenses.
- Users cannot delete another user's expenses.

### Expense Filtering

Expenses can be filtered using parameters such as:

- Year
- Month
- Category

This allows the frontend to retrieve only the expenses relevant to a particular view.

## Budget Management

Users can create monthly budgets for their expenses.

Budgets can be associated with a specific category or used as a broader budget depending on the application's budget configuration.

### Budget Validation

The backend validates:

- Category existence
- Category ownership
- Budget amount
- Budget month
- Duplicate budgets
- User ownership

Budgets cannot be created or updated for past months.

### Budget Usage

The backend calculates budget usage by comparing:

- Budget amount
- Total expenses
- Remaining amount
- Percentage of budget used

For example:

```text
Budget Amount = 10,000
Expenses      = 7,500

Remaining     = 2,500
Percentage    = 75%