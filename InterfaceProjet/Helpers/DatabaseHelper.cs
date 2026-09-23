using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;

namespace InterfaceProjet.Helpers
{
    public static class DatabaseHelper
    {
        public static string DbPath => Path.Combine(AppContext.BaseDirectory, "portfolio.db");
        public static string ConnectionString => $"Data Source={DbPath}";

        public static void InitializeDatabase()
        {
            try
            {
                bool dbExists = File.Exists(DbPath);
                
                SQLitePCL.Batteries_V2.Init();

                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();

                    CreateTables(connection);

                    if (!dbExists || ObtenirNombreClients(connection) == 0)
                    {
                        Debug.WriteLine("Création de la base de données SQLite et des tables...");
                        SeedData(connection);
                        Debug.WriteLine("Base de données initialisée avec succès avec les données de test.");
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    string logFile = Path.Combine(AppContext.BaseDirectory, "error_db.txt");
                    File.WriteAllText(logFile, $"Erreur DB : {ex.ToString()}");
                }
                catch { }
                Debug.WriteLine($"Erreur lors de l'initialisation de la base de données : {ex.Message}");
            }
        }

        private static int ObtenirNombreClients(SqliteConnection connection)
        {
            try
            {
                using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM clients", connection))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        private static void CreateTables(SqliteConnection connection)
        {
            string createClients = @"
                CREATE TABLE IF NOT EXISTS clients (
                    id_client INTEGER PRIMARY KEY AUTOINCREMENT,
                    nom TEXT NOT NULL,
                    adresse TEXT NOT NULL,
                    telephone TEXT NOT NULL,
                    email TEXT NOT NULL,
                    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP
                );";

            string createProjets = @"
                CREATE TABLE IF NOT EXISTS projets (
                    numero_projet TEXT PRIMARY KEY,
                    titre TEXT NOT NULL,
                    date_debut DATETIME NOT NULL,
                    description TEXT NOT NULL,
                    budget DECIMAL NOT NULL,
                    nb_employes_requis INTEGER NOT NULL,
                    id_client INTEGER,
                    statut TEXT DEFAULT 'En cours',
                    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (id_client) REFERENCES clients(id_client)
                );";

            string createEmployes = @"
                CREATE TABLE IF NOT EXISTS employes (
                    matricule TEXT PRIMARY KEY,
                    nom TEXT NOT NULL,
                    prenom TEXT NOT NULL,
                    date_embauche DATETIME NOT NULL,
                    salaire_horaire DECIMAL NOT NULL
                );";

            string createAssignations = @"
                CREATE TABLE IF NOT EXISTS assignations (
                    numero_projet TEXT,
                    matricule_employe TEXT,
                    date_assignation DATETIME DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (numero_projet, matricule_employe),
                    FOREIGN KEY (numero_projet) REFERENCES projets(numero_projet),
                    FOREIGN KEY (matricule_employe) REFERENCES employes(matricule)
                );";

            string createAdmins = @"
                CREATE TABLE IF NOT EXISTS administrateurs (
                    id_admin INTEGER PRIMARY KEY AUTOINCREMENT,
                    nom_utilisateur TEXT UNIQUE NOT NULL,
                    mot_de_passe_hash TEXT NOT NULL,
                    sel TEXT NOT NULL,
                    role TEXT DEFAULT 'Admin'
                );";

            ExecuteCommand(connection, createClients);
            ExecuteCommand(connection, createProjets);
            ExecuteCommand(connection, createEmployes);
            ExecuteCommand(connection, createAssignations);
            ExecuteCommand(connection, createAdmins);
        }

        private static void SeedData(SqliteConnection connection)
        {
            string seedClients = @"
                INSERT INTO clients (nom, adresse, telephone, email) VALUES 
                ('Jean Dupont', '123 Rue de la Paix, Paris', '0601020304', 'jean.dupont@email.com'),
                ('Marie Curie', '45 Avenue des Sciences, Lyon', '0611223344', 'marie.curie@email.com'),
                ('Alice Wonderland', '99 Rue des Rêves, Marseille', '0699887766', 'alice@email.com');";

            string seedProjets = @"
                INSERT INTO projets (numero_projet, titre, date_debut, description, budget, nb_employes_requis, id_client, statut) VALUES 
                ('PRJ-001', 'Refonte Site Web', '2026-10-01', 'Création du nouveau site e-commerce', 15000.00, 3, 1, 'En cours'),
                ('PRJ-002', 'Application Mobile', '2026-11-15', 'Développement app iOS/Android', 25000.00, 5, 2, 'En cours'),
                ('PRJ-003', 'Audit Sécurité', '2026-09-01', 'Audit complet du système', 5000.00, 2, 3, 'Terminé');";

            string seedEmployes = @"
                INSERT INTO employes (matricule, nom, prenom, date_embauche, salaire_horaire) VALUES 
                ('EMP001', 'Martin', 'Paul', '2020-01-15', 35.50),
                ('EMP002', 'Bernard', 'Lucie', '2021-03-10', 40.00),
                ('EMP003', 'Dubois', 'Marc', '2022-06-20', 30.00);";

            string seedAssignations = @"
                INSERT INTO assignations (numero_projet, matricule_employe) VALUES 
                ('PRJ-001', 'EMP001'),
                ('PRJ-001', 'EMP002'),
                ('PRJ-003', 'EMP003');";

            string seedAdmins = @"
                INSERT INTO administrateurs (nom_utilisateur, mot_de_passe_hash, sel, role) VALUES 
                ('admin', 'hash_factice', 'sel_factice', 'SuperAdmin');";

            ExecuteCommand(connection, seedClients);
            ExecuteCommand(connection, seedProjets);
            ExecuteCommand(connection, seedEmployes);
            ExecuteCommand(connection, seedAssignations);
            ExecuteCommand(connection, seedAdmins);
        }

        private static void ExecuteCommand(SqliteConnection connection, string commandText)
        {
            using (var command = new SqliteCommand(commandText, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
