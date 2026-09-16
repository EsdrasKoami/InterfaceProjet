# InterfaceProjet — Système de Gestion de Projets d'Entreprise

> **Plateforme desktop Windows native** pour la gestion opérationnelle de projets, d'employés, de clients et d'assignations, construite sur WinUI 3 / Windows App SDK avec accès MySQL via procédures stockées et triggers.  
> L'architecture repose sur un **Singleton Repository Pattern** couplé à un modèle de navigation par Frame WinUI, offrant un cycle de vie de données centralisé et une UI réactive pilotée par `ObservableCollection<T>`.

---

## Table des matières

- [Architecture & Choix d'Ingénierie](#architecture--choix-dingénierie)
- [Stack Technique & Dépendances](#stack-technique--dépendances)
- [Modèle de données & Entités](#modèle-de-données--entités)
- [Carte des modules](#carte-des-modules)
- [Flux de données (Data Flow)](#flux-de-données-data-flow)
- [Guide d'installation et d'exécution locale](#guide-dinstallation-et-dexécution-locale)
- [Démonstration](#démonstration)
- [Feuille de route](#feuille-de-route)

---

## Architecture & Choix d'Ingénierie

### Patron Singleton — Repository Layer (`/Singletons`)

Chaque agrégat métier est encapsulé dans un singleton qui sert à la fois de **cache mémoire** (`ObservableCollection<T>`) et de **point d'accès unique à la base de données** :

| Singleton | Agrégat géré | Responsabilités clés |
|---|---|---|
| `SingletonProjet` | `Projet` | CRUD complet, filtrage par statut, export CSV, budget restant via fonction SQL |
| `SingletonEmploye` | `Employe` | CRUD, filtrage disponibilité/indisponibilité, recherche full-text via stored procedure |
| `SingletonClient` | `Client` | CRUD, recherche multi-champs, liaison optionnelle aux projets |
| `SingletonAssignation` | `Assignation` | Affectation employé ↔ projet, mise à jour des heures, libération en masse |
| `SingletonAdmin` | `Administrateur` | Authentification, session courante, gestion unicité du compte admin |

**Décisions clés :**

- **Lazy initialization** : les instances ne sont créées qu'à la première invocation de `getInstance()` — aucune ressource DB n'est allouée au démarrage.
- **ObservableCollection<T> comme bus de données UI** : toute modification persistée en base déclenche un rechargement immédiat de la collection, propageant automatiquement les mises à jour vers les contrôles XAML liés (`ItemsSource`).
- **Procédures stockées MySQL pour toutes les mutations** : `AjouterProjet`, `SupprimerProjet`, `TerminerProjet`, `AjouterEmploye`, `ModifierEmploye`, `SupprimerEmploye`, `AjouterClient`, `ModifierClient`, `SupprimerClient`, `AjouterAssignation`, `MajHeuresAssignation`, `CreerAdministrateur`, `VerifierAdministrateur`, `AssocierClientAuProjet`. Les SELECT exploitent des **vues SQL** dédiées (`vue_projets_en_cours`, `vue_tous_projets`, `vue_projets_termines`, `vue_employes_disponibles`, `vue_employes_non_disponibles`, `vue_tous_clients`).
- **Triggers SQL pour la génération de clés** : `numero_projet` et `id_client` sont générés côté serveur, découplant la logique de séquençage de la couche applicative.
- **Nullable reference types activés** (`<Nullable>enable</Nullable>`) : `IdClient` (int?) sur `Projet` et `DerniereConnexion` (DateTime?) sur `Administrateur` reflètent explicitement les relations optionnelles du schéma.

### Navigation — Shell Pattern (`MainWindow` + `Frame`)

`MainWindow` joue le rôle de **shell de navigation** : il héberge un `NavigationView` (sidebar) et un `Frame` central dans lequel les `Page` WinUI sont empilées.

- **Bootstrapping conditionnel** : au lancement, `VerifierEtNaviguer()` interroge `SingletonAdmin` — si aucun administrateur n'existe, la page de création de compte (`PageAdmin`) est forcée avant toute navigation.
- **Guard clause sur chaque item de menu** : la navigation est bloquée tant qu'aucun administrateur n'est enregistré en base.
- **Template switching contextuel** (`PageProjets`) : selon que `SingletonAdmin.EstConnecte()` est vrai, le `DataTemplate` XAML basculé dynamiquement (`ProjetTemplateAdmin` / `ProjetTemplateUser`) masque ou expose les actions CRUD.

### Sécurité — Hashing SHA-256 (`Cryptage`)

```csharp
// Helpers/Cryptage.cs
public static string GenererSHA256(string texte)
{
    using var sha256 = SHA256.Create();
    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texte));
    return Convert.ToHexString(bytes).ToLower();
}
```

Les mots de passe ne transitent jamais en clair : le hash SHA-256 est calculé côté client avant l'appel à `SingletonAdmin.ConnecterAdministrateur()`. La procédure stockée `VerifierAdministrateur` compare uniquement les hashes.

### Persistance des préférences — `ApplicationData.LocalSettings`

Le thème applicatif (Dark / Light / System) est persisté dans le magasin isolé Windows (`ApplicationData.Current.LocalSettings`) sous la clé `"AppTheme"` et rechargé dans `ChargerThemeSauvegarde()` à chaque démarrage, avant même que le premier Frame ne soit rendu.

### Gestion des relations optionnelles

La relation `Projet ↔ Client` est volontairement nullable (`int? IdClient`, `string NomClient` avec fallback `"Aucun client"`). La méthode `AUnClient()` sur `Projet` encapsule ce prédicat sans exposer la nullabilité aux couches supérieures.

### Export CSV

L'export est déclenché depuis `MainWindow` via `FileSavePicker` (API WinRT) avec interop native (`WindowNative.GetWindowHandle`). La sérialisation délègue à `Projet.ToString()` qui implémente un format CSV semi-colon-separated reproductible.

---

## Stack Technique & Dépendances

| Composant | Technologie | Version |
|---|---|---|
| Runtime cible | .NET | 8.0 |
| UI Framework | WinUI 3 / Windows App SDK | 1.8.251106002 |
| SDK Windows | Windows 10 / 11 | min 10.0.17763 (RS5), cible 10.0.19041 |
| Connecteur BDD | MySql.Data (Oracle) | 9.5.0 |
| Base de données | MySQL (serveur distant) | — |
| Packaging | MSIX (single-project) | MSBuild tooling |
| Langage | C# | 12 (avec nullable refs) |
| Plateformes | x86 · x64 · ARM64 | — |
| Build Tools | Microsoft.Windows.SDK.BuildTools | 10.0.26100.7175 |

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
│  │ DateCreation     │                                                 │
│  │ DerniereConnexion│  ← nullable                                     │
│  └──────────────────┘                                                 │
└─────────────────────────────────────────────────────────────────────┘
```

**Propriétés calculées exposées au XAML** (computed properties, pas de méthodes) :

| Classe | Propriété | Calcul |
|---|---|---|
| `Employe` | `NomComplet` | `$"{Prenom} {Nom}"` |
| `Employe` | `AgeTexte` | Calcul sur `DateNaissance` |
| `Employe` | `TauxHoraireFormate` | `$"{TauxHoraire:F2} $/h"` |
| `Projet` | `BudgetAfficher` | `$"{Budget}$"` |
| `Projet` | `BudgetRestant()` | `Budget - TotalSalaires` |
| `Projet` | `PourcentageBudgetUtilise()` | `(TotalSalaires / Budget) * 100` |
| `Assignation` | `TauxHoraireCalcule()` | `SalaireAPayer / HeuresTravaillees` |

---

## Carte des modules

```
InterfaceProjet/
│
├── App.xaml / App.xaml.cs          ← Point d'entrée WinUI, bootstrapping de MainWindow
│
├── MainWindow.xaml / .cs           ← Shell : NavigationView + Frame central
│   └── Comportements :
│       ├── ChargerThemeSauvegarde()  ← Lecture LocalSettings au démarrage
│       ├── VerifierEtNaviguer()      ← Guard: admin requis avant navigation
│       ├── navView_ItemInvoked()     ← Routage par tag (accueil/employe/clients/…)
│       ├── MenuExporter_Click()      ← Export CSV via FileSavePicker
│       └── GererDeconnexion()        ← Déconnexion avec confirmation
│
├── Classes/                         ← Domain Model (POCO enrichis)
│   ├── Employe.cs
│   ├── Client.cs
│   ├── Projet.cs
│   ├── Assignation.cs
│   └── Administrateur.cs
│
├── Singletons/                      ← Repository + Cache Layer
│   ├── SingletonProjet.cs           ← 605 lignes, 10 opérations DB
│   ├── SingletonEmploye.cs          ← CRUD + recherche stored proc
│   ├── SingletonClient.cs           ← CRUD + recherche inline
│   ├── SingletonAssignation.cs      ← Assignation, heures, libération
│   └── SingletonAdmin.cs            ← Auth session, unicité admin
│
├── Helpers/
│   └── Cryptage.cs                  ← Utilitaire SHA-256 stateless
│
└── Pages/                           ← View Layer (34 fichiers XAML+CS)
    ├── PageAccueil                  ← Dashboard projets en cours + recherche
    ├── PageProjets                  ← CRUD projets (template admin/user)
    ├── PageEmployes                 ← Listing + filtrage disponibilité
    ├── PageClients                  ← CRUD clients
    ├── PageConnexion                ← Login + hashing SHA-256
    ├── PageParametres               ← Thème + déconnexion
    ├── PageAdmin                    ← Création compte admin (first-run)
    ├── PageAjoutPojet               ← Formulaire création projet
    ├── PageAjoutEmploye             ← Formulaire création employé
    ├── PageAjoutClient              ← Formulaire création client
    ├── PageAssignationEmploye       ← Affecter employé à un projet
    ├── PageAssignationClient        ← Associer client à un projet
    ├── ProjetDetailsDialog          ← Dialog détails + assignations
    ├── ModifierProjetDialog         ← Dialog édition projet (inline)
    ├── ModifierEmployeDialog        ← Dialog édition employé (inline)
    └── ModifierClientDialog         ← Dialog édition client (inline)
```

---

## Flux de données (Data Flow)

```
┌──────────────────────────────────────────────────────────────────┐
│                         UI LAYER (Pages XAML)                     │
│                                                                    │
│  User Action ──► Event Handler (code-behind)                       │
│       │                    │                                       │
│       │          Input Validation (IsNullOrWhiteSpace,             │
│       │          type checks, state guards)                        │
│       │                    │                                       │
│       ▼                    ▼                                       │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │              SINGLETON REPOSITORY LAYER                   │      │
│  │                                                           │      │
│  │  READ path:                                               │      │
│  │  getInstance() → SELECT vue_* / stored proc               │      │
│  │       → MySqlDataReader → new Domain Object()             │      │
│  │       → listeXxx.Add() → ObservableCollection<T>         │      │
│  │       → UI auto-update via data binding                   │      │
│  │                                                           │      │
│  │  WRITE path:                                              │      │
│  │  getInstance() → MySqlCommand (stored procedure)          │      │
│  │       → Parameters.AddWithValue(typed)                    │      │
│  │       → ExecuteNonQuery()                                 │      │
│  │       → Reload collection (getAllXxx / getXxxEnCours)     │      │
│  │                                                           │      │
│  │  SCALAR path:                                             │      │
│  │  ExecuteScalar() → COUNT(*) / BudgetRestant(@num)         │      │
│  └─────────────────────────────────────────────────────────┘      │
│                            │                                        │
│                            ▼                                        │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │              MYSQL DATABASE (remote)                      │      │
│  │                                                           │      │
│  │  Tables     : projets, employes, clients,                 │      │
│  │               assignations, administrateurs               │      │
│  │  Vues       : vue_projets_en_cours, vue_tous_projets,     │      │
│  │               vue_projets_termines,                       │      │
│  │               vue_employes_disponibles,                   │      │
│  │               vue_employes_non_disponibles,               │      │
│  │               vue_tous_clients                            │      │
│  │  Stored Procs: AjouterProjet, SupprimerProjet,            │      │
│  │               TerminerProjet, AssocierClientAuProjet,     │      │
│  │               AjouterEmploye, ModifierEmploye,            │      │
│  │               SupprimerEmploye, RechercherEmployesTout,   │      │
│  │               AjouterClient, ModifierClient,              │      │
│  │               SupprimerClient, AjouterAssignation,        │      │
│  │               MajHeuresAssignation, CreerAdministrateur,  │      │
│  │               VerifierAdministrateur                      │      │
│  │  Fonctions  : BudgetRestant(p_numero_projet)              │      │
│  │  Triggers   : génération auto de numero_projet, id_client │      │
│  └─────────────────────────────────────────────────────────┘      │
└──────────────────────────────────────────────────────────────────┘
```

### Authentification Flow

```
PageConnexion
    │
    ├── 1. Validation champs (non-vide)
    ├── 2. Cryptage.GenererSHA256(password)        ← hash côté client
    ├── 3. SingletonAdmin.ConnecterAdministrateur(user, hash)
    │       └── VerifierAdministrateur (stored proc) → retourne row ou null
    │           └── MettreAJourDerniereConnexion() ← UPDATE NOW()
    ├── 4. AdministrateurConnecte = admin          ← session en mémoire
    └── 5. Frame.Navigate(PageAccueil)
```

---

## Guide d'installation et d'exécution locale

### Prérequis

| Outil | Version minimale |
|---|---|
| Windows | 10 version 1809 (build 17763) ou supérieur |
| Visual Studio | 2022 17.x avec workload **Windows App SDK** |
| .NET SDK | 8.0 |
| Extension VS | **Windows App SDK C# Templates** |
| Accès réseau | Connectivité vers `cours.cegep3r.info:3306` |

### Cloner le dépôt

```bash
git clone https://github.com/<org>/InterfaceProjet.git
cd InterfaceProjet
```

### Ouvrir et restaurer les packages NuGet

```bash
# Via CLI .NET
dotnet restore InterfaceProjet/InterfaceProjet.csproj

# Ou ouvrir InterfaceProjet.sln dans Visual Studio
# → clic droit Solution → Restore NuGet Packages
```

### Configuration de la connexion base de données

La chaîne de connexion est définie dans chaque Singleton. Pour pointer vers un autre environnement, modifier la constante dans les fichiers concernés :

```
Singletons/SingletonProjet.cs      → ligne 25
Singletons/SingletonEmploye.cs     → ligne 18
Singletons/SingletonClient.cs      → ligne 22
Singletons/SingletonAssignation.cs → ligne 20
Singletons/SingletonAdmin.cs       → ligne 22
```

Format :
```
Server=<HOST>;Database=<DB_NAME>;Uid=<USER>;Pwd=<PASSWORD>;
```

> **Recommandation de sécurité** : Externaliser la connexion dans un fichier de configuration chiffré ou via `Windows Credential Manager` pour les environnements de production. Ne pas committer les credentials en clair.

### Compiler et exécuter

```bash
# Debug x64 (recommandé pour le développement)
dotnet build InterfaceProjet/InterfaceProjet.csproj -c Debug -r win-x64

# Lancer depuis Visual Studio
# → Sélectionner la plateforme cible (x64 recommandé)
# → F5 ou Ctrl+F5
```

### Premier lancement

Au premier démarrage, si aucun administrateur n'existe en base, l'application redirige automatiquement vers `PageAdmin` pour la création du compte unique. Ce mécanisme est géré par `SingletonAdmin.AdministrateurExiste()` dans `MainWindow.VerifierEtNaviguer()`.

### Publier en MSIX (production)

```bash
dotnet publish InterfaceProjet/InterfaceProjet.csproj \
  -c Release \
  -r win-x64 \
  -p:PublishReadyToRun=true \
  -p:PublishTrimmed=true
```

---

## Démonstration

> **[ Insérer ici une capture d'écran ou un GIF animé de l'application ]**

```
docs/
├── screenshot-accueil.png       ← Dashboard projets en cours
├── screenshot-employes.png      ← Gestion des employés
├── screenshot-projets-admin.png ← Vue admin avec actions CRUD
└── demo.gif                     ← Flux complet d'assignation
```

---

## Feuille de route

| Priorité | Amélioration | Impact |
|---|---|---|
| 🔴 Haute | Externaliser la connexion DB (fichier config / secrets manager) | Sécurité, déploiement multi-env |
| 🔴 Haute | Remplacer le Singleton pattern par un `IRepository<T>` injectable (DI) | Testabilité, découplage |
| 🟠 Moyenne | Ajouter une couche de validation métier centralisée (FluentValidation) | Cohérence, maintenabilité |
| 🟠 Moyenne | Pagination des listes (actuellement chargement complet en mémoire) | Performance, scalabilité |
| 🟡 Basse | Internationalisation (i18n) des libellés UI | Portabilité linguistique |
| 🟡 Basse | Logging structuré (Serilog) en remplacement des `Debug.WriteLine` | Observabilité production |
| 🟡 Basse | Tests unitaires sur les Singletons avec une BD en mémoire (SQLite) | Qualité, CI/CD |

---

## Structure du dépôt

```
InterfaceProjet/
├── InterfaceProjet.sln
└── InterfaceProjet/
    ├── InterfaceProjet.csproj
    ├── App.xaml[.cs]
    ├── MainWindow.xaml[.cs]
    ├── app.manifest
    ├── Package.appxmanifest
    ├── Assets/
    ├── Classes/
    │   ├── Administrateur.cs
    │   ├── Assignation.cs
    │   ├── Client.cs
    │   ├── Employe.cs
    │   └── Projet.cs
    ├── Helpers/
    │   └── Cryptage.cs
    ├── Singletons/
    │   ├── SingletonAdmin.cs
    │   ├── SingletonAssignation.cs
    │   ├── SingletonClient.cs
    │   ├── SingletonEmploye.cs
    │   └── SingletonProjet.cs
    └── Pages/
        ├── PageAccueil.xaml[.cs]
        ├── PageAdmin.xaml[.cs]
        ├── PageAjoutClient.xaml[.cs]
        ├── PageAjoutEmploye.xaml[.cs]
        ├── PageAjoutPojet.xaml[.cs]
        ├── PageAssignationClient.xaml[.cs]
        ├── PageAssignationEmploye.xaml[.cs]
        ├── PageClients.xaml[.cs]
        ├── PageConnexion.xaml[.cs]
        ├── PageEmployes.xaml[.cs]
        ├── PageModifierClient.xaml[.cs]
        ├── PageParametres.xaml[.cs]
        ├── PageProjets.xaml[.cs]
        ├── ProjetDetailsDialog.xaml[.cs]
        ├── ModifierClientDialog.xaml[.cs]
        ├── ModifierEmployeDialog.xaml[.cs]
        └── ModifierProjetDialog.xaml[.cs]
```

---


