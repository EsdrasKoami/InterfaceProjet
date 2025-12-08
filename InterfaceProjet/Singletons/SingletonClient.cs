
using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceClient.Singletons
{
    internal class SingletonClient
    {
        string connectionString;
        ObservableCollection<Client> listeClient;
        static SingletonClient instance = null;
        private SingletonClient()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
            listeClient = new ObservableCollection<Client>();
        }
        //retourne l’instance du singleton
        public static SingletonClient getInstance()
        {
            if (instance == null)
                instance = new SingletonClient();
            return instance;
        }
        //Propriété qui retourne la liste des Clients
        public ObservableCollection<Client> Liste { get => listeClient; }

        public void getAllClients()
        {
            listeClient.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_tous_clients";

                con.Open();

                using MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    // Utilisation de l'ordre des paramètres du constructeur
                    Client client = new Client(
                        r.GetInt32("id_client"),
                        r.GetString("nom"),
                        r.GetString("adresse"),
                        r.GetString("telephone"),
                        r.GetString("email"),
                        r.GetDateTime("date_creation")
                    );

                    listeClient.Add(client);
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


        public int getNombreClients()
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand commande = con.CreateCommand();
                commande.CommandText = "SELECT COUNT(*) FROM clients";

                con.Open();
                object? res = commande.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
                else
                    return 0;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getNombreClients : " + ex.Message);
                return 0;
            }
        }

        //ajoute un Client dans la liste
        public void AjouterClientAvecProcedure(string nom, string adresse, string telephone, string email)
        {
            try
            {
                using var con = new MySqlConnection(connectionString);
                using var cmd = new MySqlCommand("AjouterClient", con);

                cmd.CommandType = CommandType.StoredProcedure;

                // On n'envoie PAS id_client , le trigger va le générer
                cmd.Parameters.AddWithValue("@p_nom", nom);
                cmd.Parameters.AddWithValue("@p_adresse", adresse);
                cmd.Parameters.AddWithValue("@p_telephone", telephone);
                cmd.Parameters.AddWithValue("@p_email", email);

                con.Open();
                cmd.ExecuteNonQuery();
                getAllClients();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }

        // Modifier un client existant
        public void ModifierClient(int idClient, string nom, string adresse, string telephone, string email)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                using (MySqlCommand cmd = new MySqlCommand("ModifierClient", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Paramètres de la procédure
                    cmd.Parameters.AddWithValue("@p_id_client", idClient);
                    cmd.Parameters.AddWithValue("@p_nom", nom);
                    cmd.Parameters.AddWithValue("@p_adresse", adresse);
                    cmd.Parameters.AddWithValue("@p_telephone", telephone);
                    cmd.Parameters.AddWithValue("@p_email", email);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        Debug.WriteLine("Aucun client trouvé avec cet ID.");
                    }
                }

                // Recharge la liste après modification
                getAllClients();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }


        public void SupprimerClient(int idClient)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                using (MySqlCommand cmd = new MySqlCommand("SupprimerClient", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Paramètre de la procédure
                    cmd.Parameters.Add("@p_id_client", MySqlDbType.Int32).Value = idClient;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                
                getAllClients();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }


        public void RechercherClients(string motCle)
        {
            try
            {
                listeClient.Clear();

                using (MySqlConnection con = new MySqlConnection(connectionString))
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT 
                                                        id_client,
                                                        nom,
                                                        adresse,
                                                        telephone,
                                                        email,
                                                        date_creation
                                                    FROM clients
                                                    WHERE nom LIKE @motCle
                                                       OR adresse LIKE @motCle
                                                       OR telephone LIKE @motCle
                                                       OR email LIKE @motCle", con))
                {
                    cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");

                    con.Open();

                    using MySqlDataReader r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        Client client = new Client(
                            r.GetInt32("id_client"),
                            r.GetString("nom"),
                            r.GetString("adresse"),
                            r.GetString("telephone"),
                            r.GetString("email"),
                            r.GetDateTime("date_creation")
                        );

                        listeClient.Add(client);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }

        public void ModifierClientAvecProcedure(int idClient, string nom, string adresse, string telephone, string email)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                using (MySqlCommand cmd = new MySqlCommand("ModifierClient", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                   
                    cmd.Parameters.AddWithValue("@p_id_client", idClient);
                    cmd.Parameters.AddWithValue("@p_nom", nom);
                    cmd.Parameters.AddWithValue("@p_adresse", adresse);
                    cmd.Parameters.AddWithValue("@p_telephone", telephone);
                    cmd.Parameters.AddWithValue("@p_email", email);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

               
                getAllClients();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }





    }
}
