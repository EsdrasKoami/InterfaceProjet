using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceEmploye.Singletons
{
    internal class SingletonEmploye
    {
        
            string connectionString;
            ObservableCollection<Employe> listeEmploye;
            static SingletonEmploye instance = null;
            private SingletonEmploye()
            {
                connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
                listeEmploye = new ObservableCollection<Employe>();
            }
            //retourne l’instance du singleton
            public static SingletonEmploye getInstance()
            {
                if (instance == null)
                    instance = new SingletonEmploye();
                return instance;
            }
            //Propriété qui retourne la liste des Employes
            public ObservableCollection<Employe> Liste { get => listeEmploye; }

        public void getEmployesDisponibles()
        {
            listeEmploye.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_employes_disponibles";

                con.Open();

                using MySqlDataReader r = cmd.ExecuteReader();
        {
                    while (r.Read())
                    {
                        Employe employe = new Employe(
                            r.GetString("matricule"),
                            r.GetString("nom"),
                            r.GetString("prenom"),
                            DateTime.MinValue,   // date_naissance non dans la vue
                            r.GetString("email"),
                            "",                  // adresse
                            DateTime.MinValue,   // date_embauche3k
                            r.GetDecimal("taux_horaire"),
                            "",                  // photo_url
                            r.GetString("statut_employe")
                        );

                        listeEmploye.Add(employe);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur : " + ex.Message);
            }
        }
        public void getEmployesNonDisponibles()
        {
            listeEmploye.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_employes_non_disponibles";

                con.Open();

                using MySqlDataReader r = cmd.ExecuteReader();
        {
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

                        // Si tu veux, tu peux aussi récupérer le projet assigné
                        string projet = r.GetString("numero_projet");

                        listeEmploye.Add(employe);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur : " + ex.Message);
            }
        }

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
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_nom", nom);
                cmd.Parameters.AddWithValue("p_prenom", prenom);
                cmd.Parameters.AddWithValue("p_date_naissance", dateNaissance);
                cmd.Parameters.AddWithValue("p_email", email);
                cmd.Parameters.AddWithValue("p_adresse", adresse);
                cmd.Parameters.AddWithValue("p_date_embauche", dateEmbauche);
                cmd.Parameters.AddWithValue("p_taux_horaire", tauxHoraire);
                cmd.Parameters.AddWithValue("p_photo_url", photoUrl);
                cmd.Parameters.AddWithValue("p_statut", statut);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine("Employé ajouté avec succès !");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur : " + ex.Message);
            }
        }



        public int getNombreEmployes()
            {
                MySqlConnection con = new MySqlConnection("Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;");
                try
                {
                    MySqlCommand commande = new MySqlCommand();
                    commande.Connection = con;
                    commande.CommandText = "select count(*) from Employe";
                    con.Open();
                    var res = commande.ExecuteScalar();
                    con.Close();
                    if (res is not null)
                        return Convert.ToInt32(res);
                    else
                        return 0;
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine(ex.Message);
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                    return 0;
                }
            }
            //ajoute un Employe dans la liste
            public void ajouterEmployeAvecProcedure(string numeroEmploye, string titre, DateTime dateDebut,
                                            string description, decimal budget, int nbEmployesRequis,
                                            int idClient)
            {
                try
                {
                    using MySqlConnection con = new MySqlConnection(connectionString);
                    using MySqlCommand cmd = new MySqlCommand("AjouterEmploye", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Paramètres de la procédure
                    cmd.Parameters.AddWithValue("@p_numero_Employe", numeroEmploye);
                    cmd.Parameters.AddWithValue("@p_titre", titre);
                    cmd.Parameters.AddWithValue("@p_date_debut", dateDebut);
                    cmd.Parameters.AddWithValue("@p_description", description);
                    cmd.Parameters.AddWithValue("@p_budget", budget);
                    cmd.Parameters.AddWithValue("@p_nb_employes_requis", nbEmployesRequis);
                    cmd.Parameters.AddWithValue("@p_id_client", idClient);

                    con.Open();
                    cmd.ExecuteNonQuery();

                getEmployesDisponibles();       
                getEmployesNonDisponibles();
            }
                catch (MySqlException ex)
                {
                    Debug.WriteLine("Erreur MySQL : " + ex.Message);
                }
            }
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
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Paramètres de la procédure
                cmd.Parameters.AddWithValue("p_matricule", matricule);
                cmd.Parameters.AddWithValue("p_nom", nom);
                cmd.Parameters.AddWithValue("p_prenom", prenom);
                cmd.Parameters.AddWithValue("p_email", email);
                cmd.Parameters.AddWithValue("p_adresse", adresse);
                cmd.Parameters.AddWithValue("p_taux_horaire", tauxHoraire);
                cmd.Parameters.AddWithValue("p_photo_url", photoUrl);
                cmd.Parameters.AddWithValue("p_statut", statut);

                // Exécution de la procédure
                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Employé {matricule} modifié avec succès !");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur : " + ex.Message);
            }
        }



        public void SupprimerEmploye(string matricule)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("SupprimerEmploye", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Assure-toi que le paramètre correspond au nom dans la procédure MySQL
                cmd.Parameters.AddWithValue("p_matricule", matricule);

                con.Open();
                cmd.ExecuteNonQuery();

                // Recharge les listes pour mettre à jour l'UI
                getEmployesDisponibles();
                getEmployesNonDisponibles();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur : " + ex.Message);
            }
        }

        public void RechercherEmployesTout(string motCle)
        {
            listeEmploye.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("RechercherEmployesTout", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_motCle", motCle);

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Employe employe = new Employe(
                        r.GetString("matricule"),
                        r.GetString("nom"),
                        r.GetString("prenom"),
                        r.GetDateTime("date_naissance"),
                        r.GetString("email"),
                        r.GetString("adresse"),
                        r.GetDateTime("date_embauche"),
                        r.GetDecimal("taux_horaire"),
                        r.GetString("photo_url"),
                        r.GetString("statut")
                    );

                    listeEmploye.Add(employe);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur : " + ex.Message);
            }
        }

        
           

        }
    
}
