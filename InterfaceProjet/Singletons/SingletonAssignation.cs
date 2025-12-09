using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace InterfaceProjet.Singletons
{
    internal class SingletonAssignation
    {
        private string connectionString;
        private ObservableCollection<Assignation> listeAssignation;
        private static SingletonAssignation instance = null;

       
        private SingletonAssignation()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
            listeAssignation = new ObservableCollection<Assignation>();
        }

        public static SingletonAssignation getInstance()
        {
            if (instance == null)
                instance = new SingletonAssignation();
            return instance;
        }

       
        public ObservableCollection<Assignation> Liste { get => listeAssignation; }


        public void AjouterAssignationEmploye(
      string matriculeEmploye,
      string numeroProjet,
      decimal heuresTravaillees)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AjouterAssignation", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_matricule_employe", MySqlDbType.VarChar).Value = matriculeEmploye;
                cmd.Parameters.Add("p_numero_projet", MySqlDbType.VarChar).Value = numeroProjet;
                cmd.Parameters.Add("p_heures_travaillees", MySqlDbType.Decimal).Value = heuresTravaillees;

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL AjouterAssignationEmploye: {ex.Message}");
                throw;
            }
        }



        // Obtenir les assignations d'un projet
        public ObservableCollection<Assignation> getAssignationsParProjet(string numeroProjet)
        {
            listeAssignation.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = @"
            SELECT 
                a.id_assignation,
                a.matricule_employe,
                a.numero_projet,
                a.heures_travaillees,
                a.salaire_a_payer,
                a.date_assignation,
                e.nom,
                e.prenom,
                e.taux_horaire,
                e.email,
                e.statut
            FROM assignations a
            JOIN employes e ON a.matricule_employe = e.matricule
            WHERE a.numero_projet = @numeroProjet
            ORDER BY a.date_assignation DESC";

                cmd.Parameters.AddWithValue("@numeroProjet", numeroProjet);

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString("matricule_employe"),
                        r.GetString("nom"),
                        r.GetString("prenom"),
                        DateTime.MinValue,
                        r.GetString("email"),
                        "",
                        DateTime.MinValue,
                        r.GetDecimal("taux_horaire"),
                        "",
                        r.GetString("statut")
                    );

                    Assignation assignation = new Assignation(
                        r.GetString("matricule_employe"),
                        r.GetString("numero_projet"),
                        r.GetDecimal("heures_travaillees"),
                        r.GetDecimal("salaire_a_payer"),
                        employe
                    );

                    assignation.IdAssignation = r.GetInt32("id_assignation");
                    assignation.DateAssignation = r.GetDateTime("date_assignation");

                    listeAssignation.Add(assignation);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL GetAssignationsParProjet: {ex.Message}");
            }

            return listeAssignation;
        }



        // MÉTHODE: Modifier les heures d'une assignation

        public void ModifierHeuresAssignation(int idAssignation, decimal nouvellesHeures)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("MajHeuresAssignation", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_id_assignation", idAssignation);
                cmd.Parameters.AddWithValue("p_nouvelles_heures", nouvellesHeures);

                con.Open();
                cmd.ExecuteNonQuery();

                
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($" Erreur MySQL ModifierHeuresAssignation: {ex.Message}");
                throw;
            }
        }


        // Supprimer une assignation

        public void SupprimerAssignation(int idAssignation)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "DELETE FROM assignations WHERE id_assignation = @id";
                cmd.Parameters.AddWithValue("@id", idAssignation);

                con.Open();
                cmd.ExecuteNonQuery();   
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL SupprimerAssignation: {ex.Message}");
                throw;
            }
        }



        // Obtenir le nombre d'assignations d'un projet

        public int getNombreAssignationsProjet(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "SELECT COUNT(*) FROM assignations WHERE numero_projet = @numeroProjet";
                cmd.Parameters.AddWithValue("@numeroProjet", numeroProjet);

                con.Open();
                object res = cmd.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
                else
                    return 0;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($" Erreur MySQL getNombreAssignationsProjet: {ex.Message}");
                return 0;
            }
        }

        public void LibererEmployesProjet(string numeroProjet)
        {
            // On recharge les assignations de ce projet
            getAssignationsParProjet(numeroProjet);

            // On copie la liste pour éviter les problèmes de modification pendant l’itération
            var copie = listeAssignation.ToList();

            foreach (var a in copie)
            {
                SupprimerAssignation(a.IdAssignation);
                listeAssignation.Remove(a);
            }
        }



        //  Vérifier si un employé est déjà assigné à un projet en cours

        public bool EmployeDejaAssigne(string matricule)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = @"
                    SELECT COUNT(*) 
                    FROM assignations a
                    JOIN projets p ON a.numero_projet = p.numero_projet
                    WHERE a.matricule_employe = @matricule
                      AND p.statut = 'En cours'";

                cmd.Parameters.AddWithValue("@matricule", matricule);

                con.Open();
                object res = cmd.ExecuteScalar();

                return res != null && Convert.ToInt32(res) > 0;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($" Erreur MySQL EmployeDejaAssigne: {ex.Message}");
                return false;
            }
        }
    }
}