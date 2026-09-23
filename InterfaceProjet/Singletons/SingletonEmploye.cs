using InterfaceProjet.Classes;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace InterfaceEmploye.Singletons
{
    internal class SingletonEmploye
    {
        private string connectionString;
        private ObservableCollection<Employe> listeEmploye;
        private static SingletonEmploye? instance = null;

        private SingletonEmploye()
        {
            connectionString = InterfaceProjet.Helpers.DatabaseHelper.ConnectionString;
            listeEmploye = new ObservableCollection<Employe>();
        }

        public static SingletonEmploye getInstance()
        {
            if (instance == null)
                instance = new SingletonEmploye();
            return instance;
        }

        public ObservableCollection<Employe> Liste { get => listeEmploye; }

        public void GetEmployesDisponibles()
        {
            listeEmploye.Clear();
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    SELECT * FROM employes 
                    WHERE matricule NOT IN (
                        SELECT a.matricule_employe FROM assignations a 
                        JOIN projets p ON a.numero_projet = p.numero_projet 
                        WHERE p.statut = 'En cours'
                    )";
                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString(r.GetOrdinal("matricule")),
                        r.GetString(r.GetOrdinal("nom")),
                        r.GetString(r.GetOrdinal("prenom")),
                        r.GetDateTime(r.GetOrdinal("date_embauche")),
                        "employe@entreprise.com",
                        "Adresse principale",
                        r.GetDateTime(r.GetOrdinal("date_embauche")),
                        r.GetDecimal(r.GetOrdinal("salaire_horaire")),
                        string.Empty,
                        "Disponible"
                    );
                    listeEmploye.Add(employe);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERREUR getEmployesDisponibles : {ex.Message}");
            }
        }

        public void getEmployesNonDisponibles()
        {
            listeEmploye.Clear();
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    SELECT * FROM employes 
                    WHERE matricule IN (
                        SELECT a.matricule_employe FROM assignations a 
                        JOIN projets p ON a.numero_projet = p.numero_projet 
                        WHERE p.statut = 'En cours'
                    )";
                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString(r.GetOrdinal("matricule")),
                        r.GetString(r.GetOrdinal("nom")),
                        r.GetString(r.GetOrdinal("prenom")),
                        DateTime.MinValue,
                        "employe@entreprise.com",
                        string.Empty,
                        DateTime.MinValue,
                        r.GetDecimal(r.GetOrdinal("salaire_horaire")),
                        string.Empty,
                        "Occupé"
                    );
                    listeEmploye.Add(employe);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur getEmployesNonDisponibles : {ex.Message}");
            }
        }

        public void RechercherEmployesTout(string motCle)
        {
            listeEmploye.Clear();
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM employes WHERE nom LIKE @motCle OR prenom LIKE @motCle OR matricule LIKE @motCle";
                cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");

                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString(r.GetOrdinal("matricule")),
                        r.GetString(r.GetOrdinal("nom")),
                        r.GetString(r.GetOrdinal("prenom")),
                        r.GetDateTime(r.GetOrdinal("date_embauche")),
                        "employe@entreprise.com",
                        "Adresse principale",
                        r.GetDateTime(r.GetOrdinal("date_embauche")),
                        r.GetDecimal(r.GetOrdinal("salaire_horaire")),
                        string.Empty,
                        "Statut"
                    );
                    listeEmploye.Add(employe);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur RechercherEmployesTout : {ex.Message}");
            }
        }

        public void AjouterEmploye(string nom, string prenom, DateTime dateNaissance, string email, string adresse, DateTime dateEmbauche, decimal tauxHoraire, string photoUrl, string statut)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO employes (matricule, nom, prenom, date_embauche, salaire_horaire) 
                    VALUES (@matricule, @nom, @prenom, @date, @salaire)";
                cmd.Parameters.AddWithValue("@matricule", "EMP-" + new Random().Next(1000, 9999));
                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@prenom", prenom);
                cmd.Parameters.AddWithValue("@date", dateEmbauche);
                cmd.Parameters.AddWithValue("@salaire", tauxHoraire);

                con.Open();
                cmd.ExecuteNonQuery();
                GetEmployesDisponibles();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur SQLite AjouterEmploye : {ex.Message}");
            }
        }

        public void ModifierEmploye(string matricule, string nom, string prenom, string email, string adresse, decimal tauxHoraire, string photoUrl, string statut)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE employes SET nom = @nom, prenom = @prenom, salaire_horaire = @salaire WHERE matricule = @matricule";
                cmd.Parameters.AddWithValue("@matricule", matricule);
                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@prenom", prenom);
                cmd.Parameters.AddWithValue("@salaire", tauxHoraire);

                con.Open();
                cmd.ExecuteNonQuery();
                GetEmployesDisponibles();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur SQLite ModifierEmploye : {ex.Message}");
            }
        }

        public void SupprimerEmploye(string matricule)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                con.Open();
                using (var cmdAssign = con.CreateCommand())
                {
                    cmdAssign.CommandText = "DELETE FROM assignations WHERE matricule_employe = @matricule";
                    cmdAssign.Parameters.AddWithValue("@matricule", matricule);
                    cmdAssign.ExecuteNonQuery();
                }
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM employes WHERE matricule = @matricule";
                    cmd.Parameters.AddWithValue("@matricule", matricule);
                    cmd.ExecuteNonQuery();
                }
                GetEmployesDisponibles();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur SQLite SupprimerEmploye : {ex.Message}");
            }
        }

        public int getNombreEmployes()
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM employes";
                con.Open();
                object? res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}