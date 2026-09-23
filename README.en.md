# InterfaceProjet — Enterprise Project Management System (Proman)

[🇫🇷 Français](README.md) | [🇬🇧 English](README.en.md)

> **Native Windows desktop platform** for operational management of projects, employees, clients, and assignments, built on **WinUI 3 / Windows App SDK (.NET 8)** with a local embedded **SQLite** database (`portfolio.db`).  
> The architecture relies on a **Singleton Repository Pattern** combined with a Shell navigation model (`NavigationView` + `Frame`), providing a centralized data lifecycle and a reactive user interface synchronized via `ObservableCollection<T>`.

---

## Table of Contents

- [Key Highlights & Features](#key-highlights--features)
- [Architecture & Engineering Decisions](#architecture--engineering-decisions)
- [Tech Stack & Dependencies](#tech-stack--dependencies)
- [Domain Model & Entities](#domain-model--entities)
- [Module Map](#module-map)
- [Data Flow](#data-flow)
- [Installation & Quick Start Guide](#installation--quick-start-guide)
- [Accounts & Demo Data](#accounts--demo-data)
- [Roadmap](#roadmap)

---

## Key Highlights & Features

- **100% Standalone & Portable (Zero-Config)**: Powered by SQLite and *Self-Contained Unpackaged* mode, the application runs out of the box without requiring external database servers or MSIX certificates.
- **Automatic Initialization & Seeding**: On first run, the SQLite database (`portfolio.db`) is automatically created and populated with realistic sample data.
- **Integrated Security**: SHA-256 password hashing for administrator accounts before database storage.
- **Modern & Responsive UI**: Conforms to Windows 11 Fluent Design (WinUI 3), with dynamic theme switching (Light / Dark) and reactive XAML two-way data bindings.
- **Data Export**: Native project export to CSV format via the Windows File Picker (`FileSavePicker`).

---

## Architecture & Engineering Decisions

### 1. Singleton Pattern — Repository Layer (`/Singletons`)

Each business aggregate is encapsulated within a singleton repository that acts as both an **in-memory cache** (`ObservableCollection<T>`) and a **gateway to SQLite**:

| Singleton | Managed Aggregate | Key Responsibilities |
|---|---|---|
| `SingletonProjet` | `Projet` | Full CRUD, status filtering, remaining budget calculation, CSV export |
| `SingletonEmploye` | `Employe` | CRUD, availability filtering (assigned / available), multi-field search |
| `SingletonClient` | `Client` | CRUD, client search, project association |
| `SingletonAssignation` | `Assignation` | Employee ↔ Project assignment, tracking worked hours and salary costs |
| `SingletonAdmin` | `Administrateur` | Admin account management, SHA-256 verification, authenticated session state |

**Key Design Decisions:**
- **Lazy Initialization**: Repositories are only instantiated upon the first call to `getInstance()`.
- **ObservableCollection<T> as UI Data Bus**: Any database modification immediately reloads the memory collection, automatically notifying bound XAML controls (`ItemsSource`).
- **Parameterized SQL Queries**: Native protection against SQL injection vulnerabilities using `SqliteParameter`.
- **Nullable Reference Types Enabled** (`<Nullable>enable</Nullable>`): Explicit handling of optional relations (e.g. `IdClient` as `int?` on `Projet`).

---

### 2. Navigation — Shell Pattern (`MainWindow` + `Frame`)

`MainWindow` orchestrates the application view hierarchy:
- **Conditional Bootstrapping (`VerifierEtNaviguer`)**: If no administrator exists in the database, the app automatically redirects to `PageAdmin` to initialize the root account.
- **Navigation Guard**: Modal dialog prompts the user to create an administrator account before granting navigation access.
- **Contextual View Switching**: Depending on authentication state (`SingletonAdmin.EstConnecte()`), administrative actions (edit, delete, create forms) are dynamically exposed or hidden.

---

### 3. Persistence & Helpers (`/Helpers`)

- **`DatabaseHelper.cs`**: Handles SQLite connection string (`portfolio.db`), engine initialization via `SQLitePCL.Batteries_V2.Init()`, table migrations, and sample data seeding.
- **`LocalSettingsHelper.cs`**: Key-value JSON storage in `appsettings.json` adjacent to the executable. Replaces `ApplicationData.Current.LocalSettings` (which requires MSIX package identity) to preserve user theme preferences across restarts.
- **`Cryptage.cs`**: Provides stateless SHA-256 cryptographic hashing to secure passwords prior to storage and authentication.

```csharp
// Helpers/Cryptage.cs
public static string GenererSHA256(string texte)
{
    using var sha256 = SHA256.Create();
    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texte));
    return Convert.ToHexString(bytes).ToLower();
}
```

---

## Tech Stack & Dependencies

| Component | Technology | Version / Details |
|---|---|---|
| **Target Runtime** | .NET | 8.0 (`net8.0-windows10.0.19041.0`) |
| **UI Framework** | WinUI 3 / Windows App SDK | 1.8.251106002 |
| **Minimum Windows SDK** | Windows 10 / 11 | min 10.0.17763.0, target 10.0.19041.0 |
| **Database Engine** | Embedded SQLite | `Microsoft.Data.Sqlite` 8.0.8 |
| **SQLite Initializer** | SQLitePCLRaw | Batteries V2 (`SQLitePCL.Batteries_V2`) |
| **Deployment Mode** | Unpackaged (Standalone) | `WindowsPackageType=None`, `WindowsAppSdkSelfContained=true` |
| **Target Architecture** | Windows x64 | `win-x64` |
| **Language** | C# | 12 (with nullable reference types) |

---

## Domain Model & Entities

```
┌─────────────────────────────────────────────────────────────────────┐
│  DOMAIN MODEL  (namespace InterfaceProjet.Classes)                   │
│                                                                       │
│  ┌──────────────┐       0..1    ┌──────────────────┐                 │
│  │   Client     │◄──────────── │      Projet       │                 │
│  │──────────────│              │──────────────────│                  │
│  │ IdClient  int│              │ NumeroProjet str  │                  │
│  │ Nom       str│              │ Titre         str │                  │
│  │ Adresse   str│              │ DateDebut    date │                  │
│  │ Telephone str│              │ Budget      dec   │                  │
│  │ Email     str│              │ TotalSalaires dec │                  │
│  │ DateCreat date│             │ IdClient    int?  │                  │
│  └──────────────┘              │ NomClient    str  │                  │
│                                │ Statut       str  │                  │
│  ┌──────────────────┐          │ NbEmployesRequis  │                  │
│  │   Employe        │          │ NbEmployesAssignes│                  │
│  │──────────────────│          └──────────────────┘                  │
│  │ Matricule    str │                    │ 1                          │
│  │ Nom          str │                    │                            │
│  │ Prenom       str │               0..N │                            │
│  │ DateNaissance dat│          ┌──────────────────┐                  │
│  │ Email        str │          │   Assignation    │                  │
│  │ Adresse      str │◄────────►│──────────────────│                  │
│  │ DateEmbauche dat │  0..N    │ IdAssignation int │                  │
│  │ TauxHoraire  dec │          │ MatriculeEmploye  │                  │
│  │ PhotoUrl     str │          │ NumeroProjet  str │                  │
│  │ Statut       str │          │ HeuresTravaillees │                  │
│  └──────────────────┘          │ SalaireAPayer dec │                  │
│                                │ DateAssignation   │                  │
│  ┌──────────────────┐          └──────────────────┘                  │
│  │  Administrateur  │                                                 │
│  │──────────────────│                                                 │
│  │ IdAdmin      int │                                                 │
│  │ NomUtilisateur   │                                                 │
│  │ MotDePasseHash   │  ← SHA-256                                      │
│  │ Sel          str │                                                 │
│  │ Role         str │                                                 │
│  └──────────────────┘                                                 │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Module Map

```
InterfaceProjet/
│
├── App.xaml / App.xaml.cs          ← WinUI lifecycle, DB initialization & global unhandled exception handling
│
├── MainWindow.xaml / .cs           ← Shell : Custom titlebar, NavigationView, central Frame
│   ├── ChargerThemeSauvegarde()    ← Theme retrieval from appsettings.json
│   ├── VerifierEtNaviguer()        ← Navigation guard: initial admin redirection
│   ├── navView_ItemInvoked()       ← Routing across pages
│   └── MenuExporter_Click()        ← CSV export using FileSavePicker
│
├── Classes/                         ← Domain entities (POCO)
│   ├── Administrateur.cs
│   ├── Assignation.cs
│   ├── Client.cs
│   ├── Employe.cs
│   └── Projet.cs
│
├── Helpers/                         ← Cross-cutting services
│   ├── Cryptage.cs                 ← Cryptographic hashing (SHA-256)
│   ├── DatabaseHelper.cs           ← Connection, schema creation & sample data seeding
│   └── LocalSettingsHelper.cs      ← JSON key-value settings persistence
│
├── Singletons/                      ← Repositories & in-memory caches
│   ├── SingletonProjet.cs           ← Project CRUD, status management, CSV export
│   ├── SingletonEmploye.cs          ← Employee CRUD, availability filters
│   ├── SingletonClient.cs           ← Client CRUD and associations
│   ├── SingletonAssignation.cs      ← Task assignments & cost tracking
│   └── SingletonAdmin.cs            ← Authentication & session management
│
└── Pages/                           ← Views & code-behind controllers
    ├── PageAccueil                  ← Active projects dashboard with search
    ├── PageProjets                  ← Complete project catalog and management
    ├── PageEmployes                 ← Employee directory with availability toggle
    ├── PageClients                  ← Client management directory
    ├── PageConnexion                ← Authentication screen
    ├── PageParametres               ← Settings (Light/Dark theme, logout)
    ├── PageAdmin                    ← Root administrator setup screen
    ├── PageAjoutPojet               ← New project creation form
    ├── PageAjoutEmploye             ← Employee onboarding form
    ├── PageAjoutClient              ← New client creation form
    ├── PageAssignationEmploye       ← Employee assignment workflow
    ├── PageAssignationClient        ← Client-to-project association workflow
    ├── ProjetDetailsDialog          ← Detailed project modal dialog
    ├── ModifierProjetDialog         ← Inline project editor
    ├── ModifierEmployeDialog        ← Inline employee editor
    └── ModifierClientDialog         ← Inline client editor
```

---

## Data Flow

```
┌──────────────────────────────────────────────────────────────────┐
│                         UI LAYER (XAML Pages)                     │
│                                                                    │
│  User Action ──► Event Handlers (Code-Behind)                      │
│       │                         │                                  │
│       │                 Input Validation                           │
│       ▼                         ▼                                  │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │              SINGLETON REPOSITORY LAYER                   │      │
│  │                                                           │      │
│  │  READ:                                                    │      │
│  │  getInstance() → SqliteCommand (SELECT ...)               │      │
│  │       → SqliteDataReader → Entity Instantiation           │      │
│  │       → ObservableCollection<T>.Add(...)                 │      │
│  │       → Automatic UI update (DataBinding)                 │      │
│  │                                                           │      │
│  │  WRITE:                                                   │      │
│  │  getInstance() → SqliteCommand (INSERT / UPDATE / DELETE) │      │
│  │       → Parameters.AddWithValue(...)                      │      │
│  │       → ExecuteNonQuery()                                 │      │
│  │       → Automatic refresh of in-memory collection         │      │
│  └─────────────────────────────────────────────────────────┘      │
│                            │                                        │
│                            ▼                                        │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │              SQLITE DATABASE (portfolio.db)              │      │
│  │                                                           │      │
│  │  Tables     : clients, projets, employes,                 │      │
│  │               assignations, administrateurs               │      │
│  │  Location   : Executable Directory (AppContext)           │      │
│  └─────────────────────────────────────────────────────────┘      │
└──────────────────────────────────────────────────────────────────┘
```

---

## Installation & Quick Start Guide

### Prerequisites

- **Operating System**: Windows 10 (version 1809 build 17763 or higher) or Windows 11.
- **.NET SDK**: Version 8.0 or higher ([Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)).

---

### Quick Launch (1-Click)

Ready-to-use launch scripts are provided at the root of the repository:

- **Windows (Command Prompt / Batch)**: Double-click `lancer.bat`
- **Windows (PowerShell)**:
  ```powershell
  .\lancer.ps1
  ```
- **Bash / WSL**:
  ```bash
  ./launch.sh
  ```

These scripts automatically:
1. Terminate any running instance to prevent file locking issues.
2. Publish a `Release` self-contained build for `win-x64`.
3. Launch the generated standalone executable immediately.

---

### Manual CLI Build

```powershell
# 1. Navigate to the project directory
cd InterfaceProjet

# 2. Restore NuGet dependencies
dotnet restore

# 3. Publish as a standalone self-contained app
dotnet publish -c Release -r win-x64 --self-contained true --nologo

# 4. Run the executable
.\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\InterfaceProjet.exe
```

---

## Accounts & Demo Data

On first startup, the local SQLite database is populated with:
- **3 Default Clients**: Jean Dupont, Marie Curie, Alice Wonderland.
- **Sample Projects & Employees** ready for testing assignment and filtering capabilities.
- **Administrator Setup**: On initial launch, the app prompts you to register your administrator credentials on `PageAdmin`.

---

## Roadmap

| Status | Feature / Enhancement | Impact |
|---|---|---|
| ✅ Completed | Migration to embedded SQLite (`portfolio.db`) | Complete portability and zero external infrastructure |
| ✅ Completed | Standalone Unpackaged publication (Self-Contained) | Seamless distribution without MSIX packaging requirements |
| ✅ Completed | Local JSON settings store (`LocalSettingsHelper`) | Persistent theme toggle (Dark/Light) |
| 🟠 In Progress | Progressive replacement of singletons with Dependency Injection (`Microsoft.Extensions.DependencyInjection`) | Decoupling & automated unit testability |
| 🟡 Planned | Large list pagination for projects and employees | Memory & rendering optimization |
| 🟡 Planned | PDF report generation for project budgets and employee summaries | Advanced reporting |
