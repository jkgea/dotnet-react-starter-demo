# dotnet-react-starter

A minimal full-stack application template for learning GitHub Copilot capabilities.

> **📋 Note:** This is a **demo/starter template** designed to showcase how organizations can configure and leverage GitHub Copilot (instructions, prompts, skills, and agents) within a real-world project structure. The development standards, patterns, and configurations included here are **illustrative examples** — they do not represent any proprietary or organization-specific standards. Feel free to use this as a template and adapt the standards to your own team's needs.

## Overview

- **Backend:** .NET 8 Web API with Entity Framework Core
- **Frontend:** React 18 + TypeScript + Vite + Tailwind CSS
- **Testing:** xUnit + Moq (backend), Jest + React Testing Library (frontend)

This template is a companion to the [GitHub Copilot Customization Workshop](https://copilot-academy.github.io/workshops/copilot-customization). Please refer to the workshop materials for detailed instructions on how to configure and use GitHub Copilot agents, prompts, and skills in the context of this project.

## Getting Started with Codespaces (Recommended)

The fastest way to get started — no local setup required.

1. Click the green **"Use this template"** button → **Create a new repository**
2. In your new repo, click **Code** → **Codespaces** → **Create codespace on main**
3. Wait for the environment to build (installs .NET 8 SDK, Node.js 20, and all dependencies automatically)
4. Once ready, start the backend and frontend:

```bash
# Terminal 1 — Backend
cd backend
dotnet run --project src/Api
```

```bash
# Terminal 2 — Frontend
cd frontend
npm run dev
```

- **Backend API:** Port `5000` (Swagger UI at `/swagger`)
- **Frontend:** Port `5173` (auto-opens in preview)

> Both ports are auto-forwarded. Codespaces will show notifications when they're ready.

## Local Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/Api --startup-project src/Api
dotnet run --project src/Api
```

The API will be available at `http://localhost:5000` with Swagger UI at `/swagger`.
The default database connection targets a local SQL Server instance. Configure
`ConnectionStrings__DefaultConnection` for your environment before applying the migration.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`.

## Project Structure

```
├── .devcontainer/             # Codespaces / Dev Container config
├── backend/
│   ├── src/Api/               # Web API project
│   └── tests/Api.Tests/       # Unit tests
├── frontend/                  # React + TypeScript application
├── legacy/                    # Classic ASP files (for migration demo)
└── .github/
    ├── copilot-instructions.md  # Organization standards
    ├── agents/                   # Copilot agents
    ├── prompts/                  # Reusable prompts
    └── skills/                   # Copilot skills
```

## API Endpoints

| Method | Endpoint     | Description              |
|--------|--------------|--------------------------|
| GET    | /v1/health   | Health check endpoint    |
| GET    | /v1/user-preferences | Get display and notification preferences |
| PUT    | /v1/user-preferences | Save display and notification preferences |

## Development Standards

See `.github/copilot-instructions.md` for complete development standards including:
- API design patterns (envelope responses, naming conventions)
- C# coding standards (async/await, DTOs, repositories, services)
- Testing standards (naming, AAA pattern, organization)
- Frontend standards (TypeScript, Tailwind)

## Recreating the Project

To recreate this project from scratch, use a frontier model such as Opus 4.6 with GitHub Copilot in agent mode. Supply the prompt in docs/BUILD_INSTRUCTIONS.md to generate the project structure, code files, and configurations.

## License

MIT
