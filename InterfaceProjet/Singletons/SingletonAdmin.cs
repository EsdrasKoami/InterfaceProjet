using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceAdmin.Singletons
{
    internal class SingletonAdmin
    {
        private string connectionString;
        private ObservableCollection<Administrateur> listeAdmin;
        private static SingletonAdmin instance = null;
        public Administrateur AdministrateurConnecte { get; private set; } = null;


        private SingletonAdmin()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
            listeAdmin = new ObservableCollection<Administrateur>();
        }

        public static SingletonAdmin getInstance()
        {
            if (instance == null)
                instance = new SingletonAdmin();
            return instance;
        }

        // Propriété pour accéder à la liste
        public ObservableCollection<Administrateur> ListeAdmin
        {
            get { return listeAdmin; }
        }

        //Nombre d'administrateur
        public int ObtenirNombreAdministrateurs()
        {
            int count = 0;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM administrateurs";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du comptage des administrateurs: {ex.Message}");
            }

            return count;
        }

        /// Charge tous les administrateurs de la BD dans la liste
       
        public void ChargerAdministrateurs()
        {
            listeAdmin.Clear();

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"SELECT id_admin, nom_utilisateur, mot_de_passe_hash, 
                                     date_creation, derniere_connexion 
                                     FROM administrateurs";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Administrateur admin = new Administrateur(
                            reader.GetString("nom_utilisateur"),
                            reader.GetString("mot_de_passe_hash")
                        )
                        {
                            IdAdmin = reader.GetInt32("id_admin"),
                            DateCreation = reader.GetDateTime("date_creation"),
                            DerniereConnexion = reader.IsDBNull(reader.GetOrdinal("derniere_connexion"))
                                ? null
                                : (DateTime?)reader.GetDateTime("derniere_connexion")
                        };

                        listeAdmin.Add(admin);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du chargement des administrateurs: {ex.Message}");
            }
        }

     
        /// Crée un nouvel administrateur (max 1 seul autorisé)
        
        public void CreerAdministrateur(string nomUtilisateur, string motDePasseHash)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("CreerAdministrateur", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_nom_utilisateur", nomUtilisateur);
                    cmd.Parameters.AddWithValue("@p_mot_de_passe_hash", motDePasseHash);

                    cmd.ExecuteNonQuery();
                }

                // Recharger la liste après ajout
                ChargerAdministrateurs();
            }
            catch (MySqlException ex)
            {
                if (ex.Message.Contains("existe deja"))
                {
                    throw new Exception("Un compte administrateur existe déjà. Impossible d'en créer un autre.");
                }
                throw new Exception($"Erreur lors de la création de l'administrateur: {ex.Message}");
            }
        }

       
        /// Vérifie les credentials d'un administrateur
     
        public Administrateur VerifierAdministrateur(string nomUtilisateur, string motDePasseHash)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("VerifierAdministrateur", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_nom_utilisateur", nomUtilisateur);
                    cmd.Parameters.AddWithValue("@p_mot_de_passe_hash", motDePasseHash);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Administrateur admin = new Administrateur(
                            reader.GetString("nom_utilisateur"),
                            motDePasseHash
                        )
                        {
                            IdAdmin = reader.GetInt32("id_admin"),
                            DateCreation = reader.GetDateTime("date_creation"),
                            DerniereConnexion = reader.IsDBNull(reader.GetOrdinal("derniere_connexion"))
                                ? null
                                : (DateTime?)reader.GetDateTime("derniere_connexion")
                        };

                        reader.Close();

                        // Mettre à jour la dernière connexion
                        MettreAJourDerniereConnexion(admin.IdAdmin);

                        return admin;
                    }

                    reader.Close();
                    return null; // Authentification échouée
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la vérification de l'administrateur: {ex.Message}");
            }
        }

    
        /// Met à jour la dernière connexion d'un administrateur
       
        private void MettreAJourDerniereConnexion(int idAdmin)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"UPDATE administrateurs 
                                     SET derniere_connexion = NOW() 
                                     WHERE id_admin = @id";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", idAdmin);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log l'erreur mais ne la propage pas
                Console.WriteLine($"Erreur lors de la mise à jour de la dernière connexion: {ex.Message}");
            }
        }

   
        /// Vérifie si un administrateur existe déjà
        
        public bool AdministrateurExiste()
        {
            return ObtenirNombreAdministrateurs() > 0;
        }



        // Méthode pour vérifier la connexion et définir l'administrateur actuel
        public bool ConnecterAdministrateur(string nomUtilisateur, string motDePasseHash)
        {
            Administrateur admin = VerifierAdministrateur(nomUtilisateur, motDePasseHash);
            if (admin != null)
            {
                AdministrateurConnecte = admin;
                return true;
            }

            AdministrateurConnecte = null;
            return false;
        }

        // Méthode pour savoir si quelqu'un est connecté
        public bool EstConnecte()
        {
            return AdministrateurConnecte != null;
        }

        // Méthode pour déconnecter
        public void Deconnecter()
        {
            AdministrateurConnecte = null;
        }
    }

}
