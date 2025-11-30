using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace InterfaceEmploye.Singletons
{
    internal class SingletonEmploye
    {
        private string connectionString;
        private ObservableCollection<Employe> listeEmploye;
        private static SingletonEmploye instance = null;

        private SingletonEmploye()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
            listeEmploye = new ObservableCollection<Employe>();
        }

        public static SingletonEmploye getInstance()
        {
            if (instance == null)
                instance = new SingletonEmploye();
            return instance;
        }

        public ObservableCollection<Employe> Liste { get => listeEmploye; }

        // ============================================
        // MÉTHODE CORRIGÉE: getEmployesDisponibles
        // ============================================
        public void getEmployesDisponibles()
        {
            listeEmploye.Clear();

            try
            {
                Debug.WriteLine("=== Début getEmployesDisponibles ===");

                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_employes_disponibles";

                con.Open();
                Debug.WriteLine("Connexion ouverte");

                // ✅ CORRECTION: Enlever l'accolade en trop
                using MySqlDataReader r = cmd.ExecuteReader();

                int compteur = 0;
                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString("matricule"),
                        r.GetString("nom"),
                        r.GetString("prenom"),
                        DateTime.MinValue,   // date_naissance non dans la vue
                        r.GetString("email"),
                        "",                  // adresse
                        DateTime.MinValue,   // date_embauche
                        r.GetDecimal("taux_horaire"),
                        "",                  // photo_url
                        r.GetString("statut_employe")
                    );

                    listeEmploye.Add(employe);
                    compteur++;
                }

                Debug.WriteLine($"=== Fin getEmployesDisponibles: {compteur} employés chargés ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERREUR getEmployesDisponibles: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        // ============================================
        // MÉTHODE CORRIGÉE: getEmployesNonDisponibles
        // ============================================
        public void getEmployesNonDisponibles()
        {
            listeEmploye.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_employes_non_disponibles";

                con.Open();

                // ✅ CORRECTION: Enlever l'accolade en trop
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString("matricule"),
                        r.GetString("nom"),
                        r.GetString("prenom"),
                        DateTime.MinValue,
                        r.GetString("email"),
                        "",
                        DateTime.MinValue,
                        r.GetDecimal("taux_horaire"),
                        "",
                        r.GetString("statut_employe")
                    );

                    listeEmploye.Add(employe);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur getEmployesNonDisponibles: {ex.Message}");
            }
        }

        // ============================================
        // MÉTHODE: RechercherEmployesTout
        // ============================================
        public void RechercherEmployesTout(string motCle)
        {
            listeEmploye.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("RechercherEmployesTout", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_motCle", motCle);

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    // ✅ Gérer les colonnes NULL
                    string photoUrl = r.IsDBNull(r.GetOrdinal("photo_url"))
                        ? ""
                        : r.GetString("photo_url");

                    Employe employe = new Employe(
                        r.GetString("matricule"),
                        r.GetString("nom"),
                        r.GetString("prenom"),
                        r.GetDateTime("date_naissance"),
                        r.GetString("email"),
                        r.GetString("adresse"),
                        r.GetDateTime("date_embauche"),
                        r.GetDecimal("taux_horaire"),
                        photoUrl,
                        r.GetString("statut")
                    );

                    listeEmploye.Add(employe);
                }

                Debug.WriteLine($"Recherche '{motCle}': {listeEmploye.Count} résultat(s)");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL RechercherEmployesTout: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur RechercherEmployesTout: {ex.Message}");
            }
        }

        // ============================================
        // MÉTHODE: AjouterEmploye
        // ============================================
        public void AjouterEmploye(
            string nom,
            string prenom,
            DateTime dateNaissance,
            string email,
            string adresse,
            DateTime dateEmbauche,
            decimal tauxHoraire,
            string photoUrl,
            string statut)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AjouterEmploye", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_nom", nom);
                cmd.Parameters.AddWithValue("p_prenom", prenom);
                cmd.Parameters.AddWithValue("p_date_naissance", dateNaissance);
                cmd.Parameters.AddWithValue("p_email", email);
                cmd.Parameters.AddWithValue("p_adresse", adresse);
                cmd.Parameters.AddWithValue("p_date_embauche", dateEmbauche);
                cmd.Parameters.AddWithValue("p_taux_horaire", tauxHoraire);
                cmd.Parameters.AddWithValue("p_photo_url", photoUrl ?? "");
                cmd.Parameters.AddWithValue("p_statut", statut);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine("Employé ajouté avec succès !");

                // Recharger la liste
                getEmployesDisponibles();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL AjouterEmploye: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // MÉTHODE: ModifierEmploye
        // ============================================
        public void ModifierEmploye(
            string matricule,
            string nom,
            string prenom,
            string email,
            string adresse,
            decimal tauxHoraire,
            string photoUrl,
            string statut)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("ModifierEmploye", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_matricule", matricule);
                cmd.Parameters.AddWithValue("p_nom", nom);
                cmd.Parameters.AddWithValue("p_prenom", prenom);
                cmd.Parameters.AddWithValue("p_email", email);
                cmd.Parameters.AddWithValue("p_adresse", adresse);
                cmd.Parameters.AddWithValue("p_taux_horaire", tauxHoraire);
                cmd.Parameters.AddWithValue("p_photo_url", photoUrl ?? "");
                cmd.Parameters.AddWithValue("p_statut", statut);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Employé {matricule} modifié avec succès !");

                // Recharger la liste
                getEmployesDisponibles();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL ModifierEmploye: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // MÉTHODE: SupprimerEmploye
        // ============================================
        public void SupprimerEmploye(string matricule)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("SupprimerEmploye", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_matricule", matricule);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Employé {matricule} supprimé avec succès !");

                // Recharger la liste
                getEmployesDisponibles();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL SupprimerEmploye: {ex.Message}");
                throw;
            }
        }

        // ============================================
        // MÉTHODE: getNombreEmployes
        // ============================================
        public int getNombreEmployes()
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "SELECT COUNT(*) FROM employes";

                con.Open();
                object res = cmd.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
                else
                    return 0;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Erreur MySQL getNombreEmployes: {ex.Message}");
                return 0;
            }
        }
    }
}