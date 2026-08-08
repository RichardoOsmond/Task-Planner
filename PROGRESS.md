# Task Planner — Progress & Roadmap

A living record of what's built and what's next. (The in-app task tracker kept trimming, so this file is the source of truth.)

## Completed

- **Design & planning** — ERD, mobile + desktop wireframes, security diagrams, API endpoint spec.
- **Project setup** — VS 2026, .NET 10 Web API, PostgreSQL, EF Core, Git repo + .gitignore.
- **Task CRUD API** — TaskItem model, migration, full CRUD with filtering and completion toggle.
- **Goal CRUD API + relationships** — self-referencing sub-goals, Task↔Goal relationships, cascade/promote delete.
- **Cycle prevention** — app-side ancestor-walk check + DB self-parent CHECK constraint.
- **Referenced-resource ownership validation** — validate goalId/parentGoalId ownership on create/update.
- **Authentication (JWT + Identity)** — User entity, ASP.NET Identity, register/login, token issuance.
- **Token-based authorization** — `[Authorize]` + per-user ownership from the token on all Task/Goal endpoints.
- **Activity logging** — Activity table + event logging on task/goal create/complete.
- **Analytics endpoints** — summary (done-today, completion rate, active goals, timezone-aware streak), productivity (day/week/month bucketing over selectable ranges), activity heatmap. Streak covered by xUnit unit tests.

## Next

- **AI task-planning feature** — Goal → LLM-generated task plan, server-side, review-before-save.
- **Frontend (PWA)** — responsive web app (web + installable), consumes the API, builds from the wireframes.
- **Deployment** — host backend + PostgreSQL + frontend; live URL (consider Azure for Students).
- **Hire-ready polish** — automated tests (expand coverage), CI/CD (GitHub Actions), README with screenshots + architecture.

## Backlog / future (deferred on purpose)

- Accurate goal-completion detection (transition + sub-goal rollup).
- Concurrency-safe cycle prevention (serializable isolation / trigger).
- Offline editing / sync (PWA).
- Cycle rejection as one structured API error.
- Timezone detection from the frontend (populate `User.TimeZoneId`).
- Streak scale-hardening (hybrid counter + gaps-and-islands) — only if scale demands.
- Integration tests for analytics endpoints (EF in-memory + seeded data).
- Extract DB logic into a service layer.
- User preferences feature (default chart range, etc.).
- Practice safe live DB migrations with real users.
