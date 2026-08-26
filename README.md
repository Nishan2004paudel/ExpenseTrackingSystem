# Expense Tracker

Expense Tracker is a full-stack personal finance application with a .NET backend API and an Angular frontend client. The system supports authentication, expense and budget management, category tracking, profile management, notifications, and admin controls.

## Project Overview

This repository includes:

- Backend API: ASP.NET Core application for business logic, database access, authentication, and real-time notifications.
- Frontend app: Angular 22 application in the expensetrackerserverfrontend folder for user interaction and dashboard experience.

## Backend

The backend provides a secure API for:

- User registration, login, logout, email verification, password reset, and JWT refresh
- Expense and category management
- Budget tracking and budget usage monitoring
- User profile and settings management
- Admin operations
- Real-time notifications using SignalR

### Key backend capabilities

- JWT-based authentication and refresh flow
- Role-based access control
- ASP.NET Core controllers and services
- Entity Framework-based persistence
- SignalR hub for user-specific live notifications
- Notification creation on budget threshold events

## Frontend

The frontend is an Angular application located in the expensetrackerserverfrontend folder.

### Frontend features

- Login, registration, forgot password, reset password, email verification flows
- Protected and guest-only routes using Angular guards
- Dashboard, profile, settings, categories, budgets, and expenses pages
- Admin panel for elevated access
- Auth state management with Angular signals
- HTTP interceptor for attaching the access token to API requests
- Real-time notification connection using SignalR

### Frontend architecture

- Angular 22 + TypeScript
- Routing with lazy-loaded feature modules/components
- Services for auth, profile, budget, category, expense, dashboard, settings, admin, and notifications
- SignalR integration to connect to /notificationHub and listen for ReceiveNotification events
- Token handling with refresh flow and in-memory access token storage

### Frontend startup

From the frontend folder:

```bash
npm install
npm start
```

The app runs with Angular CLI and is intended to connect to the backend API environment configured in the environment settings.

## Notification Flow

The app includes a notification pipeline that works across backend and frontend:

1. A user creates or updates an expense.
2. The backend calculates budget usage and checks thresholds.
3. When the budget crosses a configured threshold, a notification is created.
4. The notification is saved to the database.
5. SignalR pushes the message to the user-specific group.
6. The Angular frontend receives the notification and can display it in the UI.

## Project Structure

```text
ExpenseTracker/
├── README.md
├── backend-project-files/
│   └── ASP.NET Core API
└── expensetrackerserverfrontend/
    ├── src/
    ├── public/
    ├── angular.json
    ├── package.json
    └── ...
```

## Current Status

The application is progressing with both the API and frontend integrated:

- Authentication and user flows are implemented on the backend and frontend
- Expense, budget, category, and settings features are present in the frontend app
- Notification infrastructure is implemented end-to-end with SignalR
- Notification UI still remains a future enhancement

## Run Instructions

### Backend

Run the ASP.NET server from the backend project and ensure the database connection and environment settings are configured correctly.

### Frontend

Open the frontend project and run:

```bash
cd expensetrackerserverfrontend
npm install
npm start
```

## Summary

This project combines a secure .NET backend with a modern Angular frontend to deliver a complete expense tracking experience with live notifications, user authentication, budget monitoring, and administrative controls.

