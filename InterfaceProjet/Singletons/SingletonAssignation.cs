using InterfaceProjet.Classes;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace InterfaceProjet.Singletons
{
    internal class SingletonAssignation
    {
        private string connectionString;
        private ObservableCollection<Assignation> listeAssignation;
        private static SingletonAssignation? instance = null;

        private SingletonAssignation()
        {
            connectionString = Helpers.DatabaseHelper.ConnectionString;
            listeAssignation = new ObservableCollection<Assignation>();
        }

        public static SingletonAssignation getInstance()
        {
            if (instance == null)
                instance = new SingletonAssignation();
            return instance;
        }

        public ObservableCollection<Assignation> Liste { get => listeAssignation; }

        public void AjouterAssignationEmploye(string matriculeEmploye, string numeroProjet, decimal heuresTravaillees)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO assignations (numero_projet, matricule_employe) VALUES (@numeroProjet, @matriculeEmploye)";
                cmd.Parameters.AddWithValue("@matriculeEmploye", matriculeEmploye);
                cmd.Parameters.AddWithValue("@numeroProjet", numeroProjet);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur SQLite AjouterAssignationEmploye : {ex.Message}");
            }
        }

        public ObservableCollection<Assignation> getAssignationsParProjet(string numeroProjet)
        {
            listeAssignation.Clear();
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    SELECT a.numero_projet, a.matricule_employe, a.date_assignation,
                           e.nom, e.prenom, e.salaire_horaire
                    FROM assignations a
                    JOIN employes e ON a.matricule_employe = e.matricule
                    WHERE a.numero_projet = @numeroProjet
                    ORDER BY a.date_assignation DESC";
                cmd.Parameters.AddWithValue("@numeroProjet", numeroProjet);

                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString(r.GetOrdinal("matricule_employe")),
                        r.GetString(r.GetOrdinal("nom")),
                        r.GetString(r.GetOrdinal("prenom")),
                        DateTime.MinValue,
                        "email@domaine.com",
                        string.Empty,
                        DateTime.MinValue,
                        r.GetDecimal(r.GetOrdinal("salaire_horaire")),
                        string.Empty,
                        "Assigné"
                    );

                    Assignation assignation = new Assignation(
                        r.GetString(r.GetOrdinal("matricule_employe")),
                        r.GetString(r.GetOrdinal("numero_projet")),
                        40,
                        40 * employe.TauxHoraire,
                        employe
                    );
                    
                    // Génération d'un identifiant local pour compatibilité avec le modèle d'affichage
                    assignation.IdAssignation = new Random().Next(1, 10000); 
                    assignation.DateAssignation = r.GetDateTime(r.GetOrdinal("date_assignation"));

                    listeAssignation.Add(assignation);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur SQLite GetAssignationsParProjet : {ex.Message}");
            }
            return listeAssignation;
        }

        public void ModifierHeuresAssignation(int idAssignation, decimal nouvellesHeures)
        {
            // Non requis dans le schéma SQLite local avec clé composite
        }

        public void SupprimerAssignation(int idAssignation)
        {
            // Redirection vers la suppression par clé composite
        }

        // Suppression par clé composite (numéro projet, matricule employé)
        public void SupprimerAssignation(string numeroProjet, string matriculeEmploye)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "DELETE FROM assignations WHERE numero_projet = @num AND matricule_employe = @mat";
                cmd.Parameters.AddWithValue("@num", numeroProjet);
                cmd.Parameters.AddWithValue("@mat", matriculeEmploye);
                con.Open();
                cmd.ExecuteNonQuery();   
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur SQLite SupprimerAssignation : {ex.Message}");
            }
        }

        public int getNombreAssignationsProjet(string numeroProjet)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM assignations WHERE numero_projet = @numeroProjet";
                cmd.Parameters.AddWithValue("@numeroProjet", numeroProjet);
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

        public void LibererEmployesProjet(string numeroProjet)
        {
            getAssignationsParProjet(numeroProjet);
            var copie = listeAssignation.ToList();
            foreach (var a in copie)
            {
                SupprimerAssignation(a.NumeroProjet, a.MatriculeEmploye);
                listeAssignation.Remove(a);
            }
        }

        public bool EmployeDejaAssigne(string matricule)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM assignations a
                    JOIN projets p ON a.numero_projet = p.numero_projet
                    WHERE a.matricule_employe = @matricule AND p.statut = 'En cours'";
                cmd.Parameters.AddWithValue("@matricule", matricule);
                con.Open();
                object? res = cmd.ExecuteScalar();
                return res != null && Convert.ToInt32(res) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}