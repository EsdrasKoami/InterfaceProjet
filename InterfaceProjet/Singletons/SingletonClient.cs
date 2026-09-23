using InterfaceProjet.Classes;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace InterfaceClient.Singletons
{
    internal class SingletonClient
    {
        private string connectionString;
        private ObservableCollection<Client> listeClient;
        private static SingletonClient? instance = null;

        private SingletonClient()
        {
            connectionString = InterfaceProjet.Helpers.DatabaseHelper.ConnectionString;
            listeClient = new ObservableCollection<Client>();
        }

        public static SingletonClient getInstance()
        {
            if (instance == null)
                instance = new SingletonClient();
            return instance;
        }

        public ObservableCollection<Client> Liste { get => listeClient; }

        public void getAllClients()
        {
            listeClient.Clear();
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM clients";

                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Client client = new Client(
                        r.GetInt32(r.GetOrdinal("id_client")),
                        r.GetString(r.GetOrdinal("nom")),
                        r.GetString(r.GetOrdinal("adresse")),
                        r.GetString(r.GetOrdinal("telephone")),
                        r.GetString(r.GetOrdinal("email")),
                        r.GetDateTime(r.GetOrdinal("date_creation"))
                    );
                    listeClient.Add(client);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite getAllClients : " + ex.Message);
            }
        }

        public int getNombreClients()
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM clients";

                con.Open();
                object? res = cmd.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite getNombreClients : " + ex.Message);
                return 0;
            }
        }

        public void AjouterClientAvecProcedure(string nom, string adresse, string telephone, string email)
        {
            try
            {
                using var con = new SqliteConnection(connectionString);
                using var cmd = con.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO clients (nom, adresse, telephone, email) 
                    VALUES (@nom, @adresse, @telephone, @email)";

                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@adresse", adresse);
                cmd.Parameters.AddWithValue("@telephone", telephone);
                cmd.Parameters.AddWithValue("@email", email);

                con.Open();
                cmd.ExecuteNonQuery();
                getAllClients();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite AjouterClient : " + ex.Message);
            }
        }

        public void ModifierClient(int idClient, string nom, string adresse, string telephone, string email)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    UPDATE clients 
                    SET nom = @nom, adresse = @adresse, telephone = @telephone, email = @email 
                    WHERE id_client = @id";

                cmd.Parameters.AddWithValue("@id", idClient);
                cmd.Parameters.AddWithValue("@nom", nom);
                cmd.Parameters.AddWithValue("@adresse", adresse);
                cmd.Parameters.AddWithValue("@telephone", telephone);
                cmd.Parameters.AddWithValue("@email", email);

                con.Open();
                cmd.ExecuteNonQuery();
                getAllClients();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite ModifierClient : " + ex.Message);
            }
        }

        public void SupprimerClient(int idClient)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                
                con.Open();
                
                using (var cmdProj = con.CreateCommand())
                {
                    cmdProj.CommandText = "UPDATE projets SET id_client = NULL WHERE id_client = @id";
                    cmdProj.Parameters.AddWithValue("@id", idClient);
                    cmdProj.ExecuteNonQuery();
                }

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM clients WHERE id_client = @id";
                    cmd.Parameters.AddWithValue("@id", idClient);
                    cmd.ExecuteNonQuery();
                }
                
                getAllClients();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite SupprimerClient : " + ex.Message);
            }
        }

        public void RechercherClients(string motCle)
        {
            try
            {
                listeClient.Clear();
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    SELECT * FROM clients
                    WHERE nom LIKE @motCle OR adresse LIKE @motCle OR telephone LIKE @motCle OR email LIKE @motCle";
                
                cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");

                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Client client = new Client(
                        r.GetInt32(r.GetOrdinal("id_client")),
                        r.GetString(r.GetOrdinal("nom")),
                        r.GetString(r.GetOrdinal("adresse")),
                        r.GetString(r.GetOrdinal("telephone")),
                        r.GetString(r.GetOrdinal("email")),
                        r.GetDateTime(r.GetOrdinal("date_creation"))
                    );
                    listeClient.Add(client);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite RechercherClients : " + ex.Message);
            }
        }

        public void ModifierClientAvecProcedure(int idClient, string nom, string adresse, string telephone, string email)
        {
            ModifierClient(idClient, nom, adresse, telephone, email);
        }
    }
}
