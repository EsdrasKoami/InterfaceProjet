# InterfaceProjet — Système de Gestion de Projets d'Entreprise (Proman)

[🇫🇷 Français](README.md) | [🇬🇧 English](README.en.md)

> **Plateforme desktop Windows native** pour la gestion opérationnelle de projets, d'employés, de clients et d'assignations, construite sur **WinUI 3 / Windows App SDK (.NET 8)** avec base de données embarquée **SQLite** locale (`portfolio.db`).  
> L'architecture repose sur un **Singleton Repository Pattern** couplé à un modèle de navigation par Shell (`NavigationView` + `Frame`), offrant un cycle de vie de données centralisé et une interface réactive synchronisée via `ObservableCollection<T>`.

---

## Table des matières

- [Points Forts & Caractéristiques](#points-forts--caractéristiques)
- [Architecture & Choix d'Ingénierie](#architecture--choix-dingénierie)
- [Stack Technique & Dépendances](#stack-technique--dépendances)
- [Modèle de données & Entités](#modèle-de-données--entités)
- [Carte des modules](#carte-des-modules)
- [Flux de données (Data Flow)](#flux-de-données-data-flow)
- [Guide d'installation et d'exécution](#guide-dinstallation-et-dexécution)
- [Comptes & Données de Démonstration](#comptes--données-de-démonstration)
- [Feuille de route](#feuille-de-route)

---

## Points Forts & Caractéristiques

- **100% Autonome & Portable (Zéro-Config)** : Grâce à SQLite et au mode *Self-Contained Unpackaged*, l'application fonctionne immédiatement sans serveur de base de données externe ni certificat MSIX.
- **Initialisation & Seeding Automatiques** : Au premier démarrage, la base de données SQLite (`portfolio.db`) est créée et peuplée automatiquement avec des données de test réalistes.
- **Sécurité Intégrée** : Hachage SHA-256 des mots de passe administrateur avant stockage.
- **Interface Moderne & Réactive** : Respect des standards graphiques Windows 11 (WinUI 3), gestion dynamique du thème (Clair / Sombre) et liaisons bidirectionnelles XAML.
- **Export de Données** : Export des projets au format CSV natif via le sélecteur de fichiers Windows (`FileSavePicker`).

---

## Architecture & Choix d'Ingénierie

### 1. Patron Singleton — Couche Repository (`/Singletons`)

Chaque agrégat métier est encapsulé dans un singleton qui assure le rôle de **cache mémoire** (`ObservableCollection<T>`) et de **passerelle d'accès aux données SQLite** :

| Singleton | Agrégat géré | Responsabilités clés |
|---|---|---|
| `SingletonProjet` | `Projet` | CRUD complet, filtrage par statut, calcul du budget restant, export CSV |
| `SingletonEmploye` | `Employe` | CRUD, filtrage de disponibilité (assigné / non assigné), recherche multi-champs |
| `SingletonClient` | `Client` | CRUD, recherche client, association aux projets |
| `SingletonAssignation` | `Assignation` | Affectation employé ↔ projet, suivi des heures et calcul des coûts salariaux |
| `SingletonAdmin` | `Administrateur` | Gestion des administrateurs, hachage SHA-256, état de session connecté |

**Décisions clés :**
- **Lazy Initialization** : L'instanciation de chaque repository s'effectue au premier appel de `getInstance()`.
- **ObservableCollection<T> comme bus de données UI** : Toute opération de mise à jour persiste les données en base puis recharge la collection mémoire, ce qui notifie instantanément les contrôles XAML (`ItemsSource`).
- **Requêtes SQL paramétrées sécurisées** : Prévention native contre les injections SQL grâce à `SqliteParameter`.
- **Types références nullables activés** (`<Nullable>enable</Nullable>`) : `IdClient` (int?) sur `Projet` et les relations facultatives sont déclarées explicitement pour une robustesse accrue.

---

### 2. Navigation — Modèle Shell (`MainWindow` + `Frame`)

`MainWindow` structure l'expérience utilisateur :
- **Bootstrapping conditionnel (`VerifierEtNaviguer`)** : Si aucun compte administrateur n'est présent dans la base de données, l'application redirige automatiquement l'utilisateur vers `PageAdmin` pour initialiser le premier compte.
- **Garde de navigation** : Un dialogue modal invite l'utilisateur à créer son compte administrateur s'il tente de naviguer sans compte configuré.
- **Affichage contextuel** : Selon l'état de connexion (`SingletonAdmin.EstConnecte()`), les contrôles d'administration (boutons d'édition, suppression, formulaires d'ajout) sont masqués ou activés dynamiquement.

---

### 3. Persistance & Helpers (`/Helpers`)

- **`DatabaseHelper.cs`** : Gère la chaîne de connexion SQLite (`portfolio.db`), l'initialisation du moteur SQLite via `SQLitePCL.Batteries_V2.Init()`, la création des tables et le peuplement initial (*seed data*).
- **`LocalSettingsHelper.cs`** : Système de persistance clé-valeur dans un fichier local `appsettings.json`. Il remplace `ApplicationData.Current.LocalSettings` (inopérant en mode Unpackaged / sans MSIX) et conserve notamment le choix du thème (Dark / Light).
- **`Cryptage.cs`** : Fournit le calcul d'empreinte SHA-256 pour sécuriser les mots de passe avant insertion ou vérification.

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

## Stack Technique & Dépendances

| Composant | Technologie | Version / Détail |
|---|---|---|
| **Runtime cible** | .NET | 8.0 (`net8.0-windows10.0.19041.0`) |
| **Framework UI** | WinUI 3 / Windows App SDK | 1.8.251106002 |
| **SDK Windows minimal** | Windows 10 / 11 | min 10.0.17763.0, cible 10.0.19041.0 |
| **Moteur BDD** | SQLite embarqué | `Microsoft.Data.Sqlite` 8.0.8 |
| **Initialiseur SQLite** | SQLitePCLRaw | Batteries V2 (`SQLitePCL.Batteries_V2`) |
| **Mode Déploiement** | Unpackaged (Standalone) | `WindowsPackageType=None`, `WindowsAppSdkSelfContained=true` |
| **Plateforme** | Windows x64 | `win-x64` |
| **Langage** | C# | 12 (avec types nullables activés) |

---

## Modèle de données & Entités

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

## Carte des modules

```
InterfaceProjet/
│
├── App.xaml / App.xaml.cs          ← Cycle de vie WinUI, initialisation DB et capture d'exceptions
│
├── MainWindow.xaml / .cs           ← Shell : Barre de titre personnalisée, NavigationView, Frame central
│   ├── ChargerThemeSauvegarde()    ← Lecture du thème dans appsettings.json
│   ├── VerifierEtNaviguer()        ← Guard : redirection admin initial
│   ├── navView_ItemInvoked()       ← Routage vers les différentes pages
│   └── MenuExporter_Click()        ← Export CSV via FileSavePicker
│
├── Classes/                         ← Objets du domaine (POCO)
│   ├── Administrateur.cs
│   ├── Assignation.cs
│   ├── Client.cs
│   ├── Employe.cs
│   └── Projet.cs
│
├── Helpers/                         ← Services transversaux
│   ├── Cryptage.cs                 ← Hachage cryptographique SHA-256
│   ├── DatabaseHelper.cs           ← Connexion, schéma SQLite et insertion du jeu d'essai
│   └── LocalSettingsHelper.cs      ← Stockage JSON des paramètres d'application
│
├── Singletons/                      ← Repositories métier + Caches mémoires
│   ├── SingletonProjet.cs           ← CRUD Projets, statuts, export
│   ├── SingletonEmploye.cs          ← CRUD Employés, gestion statuts et filtres
│   ├── SingletonClient.cs           ← CRUD Clients, liaisons
│   ├── SingletonAssignation.cs      ← Gestion des affectations et des heures
│   └── SingletonAdmin.cs            ← Connexion, inscription et session admin
│
└── Pages/                           ← Vues XAML et contrôleurs d'interface
    ├── PageAccueil                  ← Dashboard des projets en cours avec recherche
    ├── PageProjets                  ← Liste complète et gestion des projets
    ├── PageEmployes                 ← Catalogue des employés avec filtre de disponibilité
    ├── PageClients                  ← Répertoire des clients
    ├── PageConnexion                ← Écran d'authentification
    ├── PageParametres               ← Réglages (thème clair/sombre, déconnexion)
    ├── PageAdmin                    ← Création du compte administrateur
    ├── PageAjoutPojet               ← Formulaire de création de projet
    ├── PageAjoutEmploye             ← Formulaire de recrutement d'employé
    ├── PageAjoutClient              ← Formulaire d'ajout client
    ├── PageAssignationEmploye       ← Affectation d'employés aux projets
    ├── PageAssignationClient        ← Association d'un client à un projet
    ├── ProjetDetailsDialog          ← Boîte de dialogue détaillée d'un projet
    ├── ModifierProjetDialog         ← Édition inline d'un projet
    ├── ModifierEmployeDialog        ← Édition inline d'un employé
    └── ModifierClientDialog         ← Édition inline d'un client
```

---

## Flux de données (Data Flow)

```
┌──────────────────────────────────────────────────────────────────┐
│                         UI LAYER (Pages XAML)                     │
│                                                                    │
│  Action utilisateur ──► Gestionnaire d'événements (Code-Behind)   │
│       │                         │                                  │
│       │                 Validation des saisies                     │
│       ▼                         ▼                                  │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │              SINGLETON REPOSITORY LAYER                   │      │
│  │                                                           │      │
│  │  LECTURE :                                                │      │
│  │  getInstance() → SqliteCommand (SELECT ...)               │      │
│  │       → SqliteDataReader → Instanciation de l'Entité      │      │
│  │       → ObservableCollection<T>.Add(...)                 │      │
│  │       → Mise à jour automatique de la vue (DataBinding)   │      │
│  │                                                           │      │
│  │  ÉCRITURE :                                               │      │
│  │  getInstance() → SqliteCommand (INSERT / UPDATE / DELETE) │      │
│  │       → Parameters.AddWithValue(...)                      │      │
│  │       → ExecuteNonQuery()                                 │      │
│  │       → Rechargement automatique de la collection mémoire │      │
│  └─────────────────────────────────────────────────────────┘      │
│                            │                                        │
│                            ▼                                        │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │              SQLITE DATABASE (portfolio.db)              │      │
│  │                                                           │      │
│  │  Tables     : clients, projets, employes,                 │      │
│  │               assignations, administrateurs               │      │
│  │  Localisation : Dossier de l'exécutable (AppContext)      │      │
│  └─────────────────────────────────────────────────────────┘      │
└──────────────────────────────────────────────────────────────────┘
```

---

## Guide d'installation et d'exécution

### Prérequis

- **Système d'exploitation** : Windows 10 (version 1809 build 17763 ou supérieure) ou Windows 11.
- **SDK .NET** : Version 8.0 ou supérieure ([Télécharger .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)).

---

### Lancement Rapide (1-Clic)

Des scripts prêts à l'emploi sont mis à disposition à la racine du dépôt :

- **Sous Windows (Invite de commandes)** : Double-cliquez sur `lancer.bat`
- **Sous Windows (PowerShell)** :
  ```powershell
  .\lancer.ps1
  ```
- **Sous Bash / WSL** :
  ```bash
  ./launch.sh
  ```

Ces scripts effectuent automatiquement :
1. La fermeture d'une éventuelle instance en cours pour éviter les verrous de fichiers.
2. La publication autonome (`self-contained`) en mode `Release` pour `win-x64`.
3. Le démarrage immédiat de l'exécutable généré.

---

### Compilation manuelle via le CLI .NET

```powershell
# 1. Se positionner dans le sous-dossier du projet
cd InterfaceProjet

# 2. Restaurer les dépendances NuGet
dotnet restore

# 3. Compiler et publier en mode autonome
dotnet publish -c Release -r win-x64 --self-contained true --nologo

# 4. Lancer l'application
.\bin\Release\net8.0-windows10.0.19041.0\win-x64\publish\InterfaceProjet.exe
```

---

## Comptes & Données de Démonstration

Au premier lancement, la base SQLite est initialisée avec :
- **3 Clients par défaut** : Jean Dupont, Marie Curie, Alice Wonderland.
- **Projets & Employés exemples** prêts pour tester les assignations et les filtres.
- **Compte Administrateur** : Lors du tout premier lancement, l'application vous invite à créer vos identifiants administrateur dans `PageAdmin`.

---

## Feuille de route

| Statut | Amélioration | Impact |
|---|---|---|
| ✅ Terminé | Migration vers SQLite embarqué (`portfolio.db`) | Portabilité totale et zéro dépendance externe |
| ✅ Terminé | Packaging autonome Unpackaged (Self-Contained) | Déploiement simplifié sans installation MSIX |
| ✅ Terminé | Stockage JSON local des préférences (`LocalSettingsHelper`) | Thème sombre/clair persistant hors package |
| 🟠 En cours | Remplacement progressif des Singletons par injection de dépendances (`Microsoft.Extensions.DependencyInjection`) | Découplage et testabilité unitaire |
| 🟡 Prévu | Pagination des grandes listes de projets/employés | Optimisation de la mémoire |
| 🟡 Prévu | Export PDF des bilans de projets et rapports d'assignations | Reporting avancé |
