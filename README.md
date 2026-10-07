# HMS — Hotels Management System

ASP.NET Core Web API on .NET 10, Clean Architecture, EF Core + SQL Server, JWT auth with email verification.

## Projects

| Project | Role | References |
|---|---|---|
| `HMS.Domain` | Entities, enums, exceptions | none |
| `HMS.Application` | DTOs, interfaces, services, validators, mappings | Domain |
| `HMS.Infrastructure` | EF Core, repositories, JWT, password hashing, email | Domain, Application |
| `HMS.API` | Controllers, middleware, Program.cs | Application, Infrastructure |
| `HMS.Tests` | xUnit unit tests | Domain, Application |

## Before you run: three things to fill in

Open `HMS.API/appsettings.Development.json`.

**1. Connection string** — change `Server=localhost` if you use a named instance, e.g. `Server=.\\SQLEXPRESS` or `Server=(localdb)\\MSSQLLocalDB`.

**2. JWT secret** — must be at least 32 characters or the app throws at startup on purpose. Generate one in PowerShell:

```powershell
[Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }))
```

**3. Email (Gmail)** — `Username`/`Sender` are your Gmail address. `Password` is **not** your Gmail password; it is a 16-character **App Password**:

1. Enable 2-Step Verification on the Google account
2. Go to https://myaccount.google.com/apppasswords
3. Create one, paste it with the spaces removed

`SmtpServer` stays `smtp.gmail.com` and `Port` stays `587` — those are Google's server, not your address.

Set `AppBaseUrl` to whatever URL the API actually runs on (see `Properties/launchSettings.json`, default `https://localhost:7235`). This is the base of the verification link in the email, so it must be reachable from the device that opens the email. `localhost` works only on the machine running the API.

### Keeping secrets out of the repo

Preferred for local dev — values go in your user profile, not in the project:

```powershell
cd HMS.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Secret" "<your generated secret>"
dotnet user-secrets set "EmailSettings:Username" "you@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "<16-char app password>"
dotnet user-secrets set "EmailSettings:Sender" "you@gmail.com"
```

## Build and run

```powershell
dotnet restore
dotnet build
```

Create the database (Package Manager Console, Default project = `HMS.Infrastructure`, startup = `HMS.API`):

```powershell
Add-Migration InitialCreate -Project HMS.Infrastructure -StartupProject HMS.API -OutputDir Data/Migrations
Update-Database -Project HMS.Infrastructure -StartupProject HMS.API
```

CLI equivalent from the solution root:

```bash
dotnet ef migrations add InitialCreate --project HMS.Infrastructure --startup-project HMS.API --output-dir Data/Migrations
dotnet ef database update --project HMS.Infrastructure --startup-project HMS.API
```

Then run `HMS.API`. Swagger opens at `/swagger`.

On startup the seeder applies pending migrations and creates the admin account plus a demo hotel with three rooms.

## Seeded admin

```
admin@hms.com  /  Admin@12345
```

Change this before the project goes anywhere real.

## Roles

| | Admin | Manager | Guest |
|---|---|---|---|
| Create/delete hotel | yes | no | no |
| Update hotel | any | own only | no |
| Create/update/delete room | any | own hotel | no |
| Create managers | yes | no | no |
| View hotels/rooms | yes | yes | yes |
| Create reservation | yes | no | own only |
| View all reservations | yes | yes | no |
| View/cancel a reservation | any | any | own only |

`Admin` and `Manager` cannot be self-assigned. Registration always produces a `Guest`. An Admin creates managers through `POST /api/hotels/{hotelId}/managers`, which creates the `Manager` record and its login account together.

Manager ownership is resolved as `User.ManagerId → Manager.HotelId`, read from the database on every protected call. A `hotelId` sent by the client is only ever compared against that value, never trusted on its own.

## Auth flow

```
POST /api/auth/register      → Guest account, EmailConfirmed = false, email sent
   (email arrives with a link to /confirm-email.html?email=...&token=...)
click link → page calls POST /api/auth/verify-email → EmailConfirmed = true
POST /api/auth/login         → JWT (refused with 403 while unverified)
```

The token in the register/login response is the **JWT** for API calls. The verification token is a different value and only ever arrives by email.

If SMTP fails, registration still succeeds and the verification token is written to the console as a `DEV FALLBACK` warning so the flow stays testable. `POST /api/auth/resend-verification` issues a fresh one.

Tokens: 32 random bytes, stored only as a SHA-256 hash, valid 24 hours, cleared on use.

## Using Authorize in Swagger

1. `POST /api/auth/login`, copy `data.token`
2. Click **Authorize** (top right)
3. Enter `Bearer <token>` — the word `Bearer`, a space, then the token
4. Authorize → Close

Swagger now attaches that header to every request until you refresh the page. A `401` means no or expired token; a `403` means the token is valid but the role or ownership check failed.

## Status codes

`400` validation or business rule · `401` missing/invalid token · `403` wrong role, not your resource, or email unverified · `404` missing · `409` duplicate email, personal number or phone

All responses use `{ success, message, data, errors }`.

## Tests

```bash
dotnet test
```

## Notes

- Package versions in the `.csproj` files are best guesses and may need bumping — if restore complains, let NuGet resolve the latest compatible version.
- `Microsoft.AspNetCore.OpenApi` is deliberately not referenced. Swashbuckle alone handles Swagger here; having both pulls in conflicting `Microsoft.OpenApi` versions.
- The Swagger security setup uses the `Microsoft.OpenApi` 2.x API (`OpenApiSecuritySchemeReference`, no `.Models` namespace), which is what Swashbuckle 10.x expects.
