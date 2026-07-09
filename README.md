# FeedLens

> A video platform where you control your own recommendation algorithm.

Most video platforms use a black-box algorithm optimised for their ad revenue — not for you. You don't know why something was recommended, you can't tune it, and it's designed to keep you watching regardless of whether that's actually what you want.

FeedLens is different. The recommendation engine is transparent. You set your interest preferences. You see why every video was suggested. The algorithm works for you, not the platform.

---

## Status

🚧 **In active development** — first iteration in progress.

| Layer | Status |
|---|---|
| .NET 8 API | 🔄 In progress |
| Angular Frontend | ⏳ Planned |
| Python ML Service | ⏳ Planned |

---

## Planned Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 17, TypeScript |
| Backend API | .NET 8, ASP.NET Core, JWT Auth |
| ML Service | Python 3.11, FastAPI, scikit-learn |
| Database | SQL Server (Azure SQL free tier) |
| Cache | Redis (Upstash) |
| Video Storage | AWS S3 |
| Real-time | Azure SignalR |
| Background Jobs | Hangfire |
| CI/CD | GitHub Actions |
| Frontend Hosting | Vercel |
| API Hosting | Render |

---

## First Iteration Features

- JWT authentication — register, login, logout
- User profile with algorithm preference settings
- Video upload with title, description, category and tags
- Video search
- Like / unlike videos

---

## Project Structure
feedlens/
├── backend/        .NET 8 Web API
├── frontend/       Angular 17 SPA (coming soon)
├── ml-service/     Python FastAPI ML engine (coming soon)
└── docs/           Architecture notes and decisions

---

## Author

**Yousuf** — Senior .NET / Full-Stack Engineer