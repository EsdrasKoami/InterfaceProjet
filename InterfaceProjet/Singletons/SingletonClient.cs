
using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceClient.Singletons
{
    //internal class SingletonClient
    //{
    //    string connectionString;
    //    ObservableCollection<Client> listeClient;
    //    static SingletonClient instance = null;
    //    private SingletonClient()
    //    {
    //        connectionString = "Server=@cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
    //        listeClient = new ObservableCollection<Client>();
    //    }
    //    //retourne l’instance du singleton
    //    public static SingletonClient getInstance()
    //    {
    //        if (instance == null)
    //            instance = new SingletonClient();
    //        return instance;
    //    }
    //    //Propriété qui retourne la liste des Clients
    //    public ObservableCollection<Client> Liste { get => listeClient; }

    //    public void getAllClients()
    //    {
    //        listeClient.Clear();

    //        try
    //        {
    //            using MySqlConnection con = new MySqlConnection(connectionString);
    //            using MySqlCommand cmd = con.CreateCommand();
    //            cmd.CommandText = "SELECT * FROM vue_tous_clients";

    //            con.Open();

    //            using MySqlDataReader r = cmd.ExecuteReader();
    //            while (r.Read())
    //            {
    //                // Utilisation de l'ordre des paramètres du constructeur
    //                Client client = new Client(
    //                    r.GetInt32("id_client"),
    //                    r.GetString("nom"),
    //                    r.GetString("adresse"),
    //                    r.GetString("telephone"),
    //                    r.GetString("email"),
    //                    r.GetDateTime("date_creation")
    //                );

    //                listeClient.Add(client);
    //            }
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine("Erreur MySQL : " + ex.Message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine("Erreur : " + ex.Message);
    //        }
    //    }


    //    public int getNombreClients()
    //    {
    //        MySqlConnection con = new MySqlConnection("Server=@cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;");
    //        try
    //        {
    //            MySqlCommand commande = new MySqlCommand();
    //            commande.Connection = con;
    //            commande.CommandText = "select count(*) from Client";
    //            con.Open();
    //            var res = commande.ExecuteScalar();
    //            con.Close();
    //            if (res is not null)
    //                return Convert.ToInt32(res);
    //            else
    //                return 0;
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine(ex.Message);
    //            if (con.State == System.Data.ConnectionState.Open)
    //                con.Close();
    //            return 0;
    //        }
    //    }
    //    //ajoute un Client dans la liste
    //    public void ajouterClientAvecProcedure(int idClient, string nom, string adresse, string telephone, string email, DateTime dateCreation)
    //    {
    //        try
    //        {
    //            using MySqlConnection con = new MySqlConnection(connectionString);
    //            using MySqlCommand cmd = new MySqlCommand("AjouterClient", con);
    //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

    //            // Paramètres de la procédure
    //            cmd.Parameters.AddWithValue("@p_numero_Client", idClient);
    //            cmd.Parameters.AddWithValue("@p_titre", nom);
    //            cmd.Parameters.AddWithValue("@p_date_debut", adresse);
    //            cmd.Parameters.AddWithValue("@p_description",  telephone);
    //            cmd.Parameters.AddWithValue("@p_budget", email);
    //            cmd.Parameters.AddWithValue("@p_nb_employes_requis", dateCreation);
              

    //            con.Open();
    //            cmd.ExecuteNonQuery();

    //            getAllClients(); // Recharge la liste après l'ajout
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine("Erreur MySQL : " + ex.Message);
    //        }
    //    }

    //    //modifie un Client
    //    public void modifierClient(string numeroClient, string titre, DateTime dateDebut,
    //                       string description, decimal budget, int nbEmployesRequis,
    //                       int idClient, string statut)
    //    {
    //        try
    //        {
    //            using MySqlConnection con = new MySqlConnection(connectionString);
    //            using MySqlCommand cmd = new MySqlCommand("ModifierClient", con);
    //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

    //            // Paramètres de la procédure
    //            cmd.Parameters.AddWithValue("@p_numero_Client", numeroClient);
    //            cmd.Parameters.AddWithValue("@p_titre", titre);
    //            cmd.Parameters.AddWithValue("@p_date_debut", dateDebut);
    //            cmd.Parameters.AddWithValue("@p_description", description);
    //            cmd.Parameters.AddWithValue("@p_budget", budget);
    //            cmd.Parameters.AddWithValue("@p_nb_employes_requis", nbEmployesRequis);
    //            cmd.Parameters.AddWithValue("@p_id_client", idClient);
    //            cmd.Parameters.AddWithValue("@p_statut", statut);

    //            con.Open();
    //            cmd.ExecuteNonQuery();

    //            getAllClients(); // Recharge la liste des Clients après modification
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine("Erreur MySQL : " + ex.Message);
    //        }
    //    }

    //    //supprime un Client en fonction de son id
    //    public void supprimerClient(string numeroClient)
    //    {
    //        try
    //        {
    //            using MySqlConnection con = new MySqlConnection(connectionString);
    //            using MySqlCommand cmd = new MySqlCommand("SupprimerClient", con);
    //            cmd.CommandType = System.Data.CommandType.StoredProcedure;

    //            cmd.Parameters.AddWithValue("@p_numero_Client", numeroClient);

    //            con.Open();
    //            cmd.ExecuteNonQuery();

    //            getAllClients(); // Recharge la liste après suppression
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine("Erreur MySQL : " + ex.Message);
    //        }
    //    }
    //    public void rechercherClients(string motCle)
    //    {
    //        listeClient.Clear();

    //        try
    //        {
    //            using MySqlConnection con = new MySqlConnection(connectionString);
    //            using MySqlCommand cmd = con.CreateCommand();

    //            // Recherche dans le titre, le numéro et le nom du client
    //            cmd.CommandText = @"SELECT * FROM vue_tous_Clients 
    //                        WHERE numero_Client LIKE @motCle 
    //                           OR titre LIKE @motCle 
    //                           OR nom_client LIKE @motCle";

    //            cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");

    //            con.Open();
    //            using MySqlDataReader r = cmd.ExecuteReader();

    //            while (r.Read())
    //            {
    //                Client Client = new Client(
    //                    r.GetString("numero_Client"),
    //                    r.GetString("titre"),
    //                    r.GetDateTime("date_debut"),
    //                    r.GetString("description"),
    //                    r.GetDecimal("budget"),
    //                    r.GetInt32("nb_employes_requis"),
    //                    r.GetDecimal("total_salaires"),
    //                    0,
    //                       r.GetString("nom_client"),
    //                    r.GetString("statut"),
    //                    r.GetDateTime("date_creation")

    //                );

    //                listeClient.Add(Client);
    //            }
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine("Erreur MySQL : " + ex.Message);
    //        }
    //    }

    //    public void getClientsEnCours()
    //    {
    //        listeClient.Clear(); // Vide la liste avant de la recharger

    //        try
    //        {
    //            using MySqlConnection con = new MySqlConnection(connectionString);
    //            using MySqlCommand cmd = con.CreateCommand();
    //            cmd.CommandText = "SELECT * FROM vue_Clients_en_cours";

    //            con.Open();
    //            using MySqlDataReader r = cmd.ExecuteReader();

    //            while (r.Read())
    //            {
    //                // Crée l'objet avec le constructeur actuel
    //                Client Client = new Client(
    //                    numeroClient: r.GetString("numero_Client"),
    //                    titre: r.GetString("titre"),
    //                    dateDebut: r.GetDateTime("date_debut"),
    //                    description: r.GetString("description"),
    //                    budget: r.GetDecimal("budget"),
    //                    nbEmployesRequis: r.GetInt32("nb_employes_requis"),
    //                    totalSalaires: r.GetDecimal("total_salaires"),
    //                    idClient: 0,
    //                    nomClient: r.GetString("nom_client"),
    //                    statut: r.GetString("statut"),
    //                    dateCreation: DateTime.Now
    //                );

    //                // Remplissage des nouveaux champs via les propriétés
    //                Client.NbEmployesAssignes = r.GetInt32("nb_employes_assignes");
    //                Client.TelephoneClient = r.GetString("telephone_client");

    //                listeClient.Add(Client);
    //            }
    //        }
    //        catch (MySqlException ex)
    //        {
    //            Debug.WriteLine("Erreur MySQL : " + ex.Message);
    //        }
    //    }




    //}
}
