# Claude Backend Instructions — Budget Tracker

> Stack: .NET 9, C#, ASP.NET Core Web API, Entity Framework Core, PostgreSQL, JWT Authentication
> These instructions extend the root CLAUDE.md. Both apply when working in this folder.

---

## 1. Project Structure

```
src/
├── Api/
│   ├── Controllers/            # HTTP endpoints only — no business logic
│   ├── Middleware/             # Global error handling, logging middleware
│   └── Program.cs              # App entry point, DI registration, pipeline config
│
├── Application/
│   ├── Features/
│   │   ├── Auth/               # Login, Register, RefreshToken
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   └── DTOs/
│   │   ├── Accounts/           # CRUD for user accounts (bank, cash, e-wallet)
│   │   ├── Cards/              # CRUD for credit/debit cards
│   │   ├── Transactions/       # Create/list transactions (expense, income, installment)
│   │   ├── Installments/       # Installment plans and progress tracking
│   │   └── Reports/            # Aggregated report queries (daily/weekly/monthly/yearly)
│   ├── Common/
│   │   ├── Behaviors/          # MediatR pipeline: ValidationBehavior, LoggingBehavior
│   │   └── Interfaces/         # ICurrentUserService, IDateTimeService, etc.
│   └── Mappings/               # AutoMapper profiles (or manual mapping)
│
├── Domain/
│   ├── Entities/               # User, Account, Card, Transaction, InstallmentPlan, InstallmentPayment
│   ├── Enums/                  # TransactionType, CardType, CardCategory, AccountType, Currency
│   └── Exceptions/             # DomainException, NotFoundException, ValidationException
│
└── Infrastructure/
    ├── Persistence/
    │   ├── AppDbContext.cs
    │   ├── Configurations/     # IEntityTypeConfiguration per entity
    │   └── Repositories/       # Repository implementations
    └── Services/               # CurrentUserService, DateTimeService, TokenService
```

---

## 2. Architecture

- Follow **Clean Architecture** strictly.
- Dependency direction: `Api` → `Application` → `Domain`. `Infrastructure` implements interfaces defined in `Application`.
- **Domain layer** has zero external dependencies.
- **Application layer** depends only on Domain — never reference EF Core from Application.
- **Controllers** are thin — receive HTTP input, call MediatR, return result. No business logic.
- Use **CQRS via MediatR** for all feature operations.

---

## 3. Domain Entities

Key entities for Budget Tracker:

- **User** — id, email, passwordHash, createdAt
- **Account** — id, userId, name, type (BankAccount/Cash/EWallet), currency, balance, isArchived
- **Card** — id, userId, name, cardCategory (Credit/Debit), cardType (Visa/Mastercard/AmEx/Discover), last4Digits, expiryDate, currency, color, creditLimit (nullable, credit only), linkedAccountId (nullable, debit only)
- **Transaction** — id, userId, accountId (nullable), cardId (nullable), type (Expense/Income/Installment), amount, category, description, date, recurrence, installmentPlanId (nullable)
- **InstallmentPlan** — id, userId, cardId, name, category, totalAmount, monthlyPayment, numberOfMonths, startDate
- **InstallmentPayment** — id, installmentPlanId, monthNumber, dueDate, paidDate (nullable), isPaid

---

## 4. Naming Conventions

| Element | Convention | Example |
|--------|-----------|---------|
| Classes | PascalCase | `AccountService`, `GetAccountsQuery` |
| Interfaces | `I` prefix | `IAccountRepository` |
| Methods | PascalCase | `GetAccountByIdAsync` |
| Parameters/locals | camelCase | `accountId`, `cancellationToken` |
| Private fields | `_` prefix | `_dbContext`, `_logger` |
| DTOs | `Dto` suffix | `AccountDto`, `CreateCardRequestDto` |
| Commands | `Command` suffix | `CreateAccountCommand` |
| Queries | `Query` suffix | `GetAccountByIdQuery` |
| Handlers | `Handler` suffix | `CreateAccountCommandHandler` |
| Enums | PascalCase values | `TransactionType.Expense` |

---

## 5. Entity Framework Core

- One `AppDbContext`. Use `IEntityTypeConfiguration<T>` for all entity config — no data annotations on entities.
- Register via `modelBuilder.ApplyConfigurationsFromAssembly(...)`.
- Migration naming: descriptive names → `AddAccountTable`, `AddInstallmentPaymentsTable`.
- Use `AsNoTracking()` for all read-only queries.
- Project with `.Select()` — don't load full entity graphs unnecessarily.
- Never expose `IQueryable` outside the repository layer.

---

## 6. API Design

- Controllers: HTTP concerns only. `[ApiController]`, `[Route("api/[controller]")]` on all controllers.
- Return `ActionResult<T>` consistently with correct HTTP status codes.
- Use DTOs for all input and output — never expose domain entities through the API.
- Validate all requests via **FluentValidation** registered in the MediatR pipeline behavior.
- Global error handling via middleware — no try/catch in controllers.

### Error Response Format
```json
{
  "status": 400,
  "error": "Validation failed",
  "details": ["Amount must be greater than zero", "Category is required"]
}
```

---

## 7. Authentication

- JWT Bearer with refresh token strategy.
- Access token: 15 minutes. Refresh token: 7 days, stored hashed in DB.
- Authorization policies defined in `Program.cs`, applied via `[Authorize(Policy = "...")]`.
- No authorization logic inside business services.

---

## 8. Dependency Injection

- Register services in `Program.cs` or via extension methods (`services.AddApplicationServices()`).
- `AddScoped` for repositories, DbContext.
- `AddSingleton` for stateless, thread-safe services.
- `AddTransient` for lightweight stateless utilities.
- Never use service locator pattern.

---

## 9. Async/Await

- All I/O operations must be async.
- Always pass and propagate `CancellationToken` from controller → handler → repository.
- Never use `.Result` or `.Wait()`. Never use `async void`.

---

## 10. Logging

- Use built-in `ILogger<T>`.
- `Information`: significant events (user login, transaction created).
- `Warning`: unexpected but recoverable.
- `Error`: failures needing attention.
- `Debug`: development only — do not leave in production code.
- Never log passwords, tokens, or PII.

---

## 11. Configuration

- All configurable values in `appsettings.json`. Never hardcode connection strings or secrets.
- Use `IOptions<T>` pattern for typed config binding.
- Secrets in environment variables or `appsettings.Development.json` (git-ignored).

---

## 12. Code Quality

- Every public method performing I/O must be async and return `Task` or `Task<T>`.
- No magic strings or numbers — use constants or enums.
- No empty catch blocks — always log or re-throw.
- No committed TODOs without an explicit issue reference.
- Keep methods short and focused. If a method exceeds ~30 lines, consider splitting.

---

*Last updated: 2026-02-21*
