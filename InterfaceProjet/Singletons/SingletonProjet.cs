using InterfaceProjet.Classes;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Singletons
{
    internal class SingletonProjet
    {
        string connectionString;
        ObservableCollection<Projet> listeProjet;
        static SingletonProjet instance = null;
        private SingletonProjet()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
           listeProjet = new ObservableCollection<Projet>();
        }
        //retourne l’instance du singleton
        public static SingletonProjet getInstance()
        {
            if (instance == null)
                instance = new SingletonProjet();
            return instance;
        }
        //Propriété qui retourne la liste des Projets
        public ObservableCollection<Projet> Liste { get =>listeProjet; }

        // retourne  les projets en cours 
        public void getProjetsEnCours()
        {
            listeProjet.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_projets_en_cours";

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    Projet projet = new Projet(
                        numeroProjet: r.GetString("numero_projet"),
                        titre: r.GetString("titre"),
                        dateDebut: r.GetDateTime("date_debut"),
                        description: r.GetString("description"),
                        budget: r.GetDecimal("budget"),
                        nbEmployesRequis: r.GetInt32("nb_employes_requis"),
                        totalSalaires: r.GetDecimal("total_salaires"),
                        idClient: r.GetInt32("id_client"),
                        nomClient: r.GetString("nom_client"),
                        statut: r.GetString("statut"),
                        dateCreation: r.GetDateTime("date_creation")
                    );

                    // remplissage des champs ajoutés dans ton modèle
                    projet.NbEmployesAssignes = r.GetInt32("nb_employes_assignes");
                    projet.TelephoneClient = r.GetString("telephone_client");


                 

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getProjetsEnCours : " + ex.Message);
            }
        }



        public void getAllProjets() // Charge la liste avec tous les projets
        {
            listeProjet.Clear(); // Vide la liste avant de la recharger

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand commande = con.CreateCommand();
                commande.CommandText = "SELECT * FROM vue_tous_projets";

                con.Open();

                using MySqlDataReader r = commande.ExecuteReader();

                while (r.Read())
                {
                    Projet projet = new Projet(
                        r.GetString("numero_projet"),
                        r.GetString("titre"),
                        r.GetDateTime("date_debut"),
                        r.GetString("description"),
                        r.GetDecimal("budget"),
                        r.GetInt32("nb_employes_requis"),
                        r.GetDecimal("total_salaires"),
                        0, // IdClient si non disponible dans la vue
                        r.GetString("nom_client"),
                        r.GetString("statut"),
                        r.GetDateTime("date_creation")
                        
                    );

                    listeProjet.Add(projet);
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

        public int getNombreProjets()
        {
            MySqlConnection con = new MySqlConnection("Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;");
            try
            {
                MySqlCommand commande = new MySqlCommand();
                commande.Connection = con;
                commande.CommandText = "select count(*) from Projet";
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
        //ajoute un Projet dans la liste
        public void ajouterProjetAvecProcedure(string numeroProjet, string titre, DateTime dateDebut,
                                        string description, decimal budget, int nbEmployesRequis,
                                        int idClient)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AjouterProjet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Paramètres de la procédure
                cmd.Parameters.AddWithValue("@p_numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("@p_titre", titre);
                cmd.Parameters.AddWithValue("@p_date_debut", dateDebut);
                cmd.Parameters.AddWithValue("@p_description", description);
                cmd.Parameters.AddWithValue("@p_budget", budget);
                cmd.Parameters.AddWithValue("@p_nb_employes_requis", nbEmployesRequis);
                cmd.Parameters.AddWithValue("@p_id_client", idClient);

                con.Open();
                cmd.ExecuteNonQuery();

                getAllProjets(); // Recharge la liste après l'ajout
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }

        //modifie un Projet
        public void modifierProjet(string numeroProjet, string titre, DateTime dateDebut,
                           string description, decimal budget, int nbEmployesRequis,
                           int idClient, string statut)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("ModifierProjet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Paramètres de la procédure
                cmd.Parameters.AddWithValue("@p_numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("@p_titre", titre);
                cmd.Parameters.AddWithValue("@p_date_debut", dateDebut);
                cmd.Parameters.AddWithValue("@p_description", description);
                cmd.Parameters.AddWithValue("@p_budget", budget);
                cmd.Parameters.AddWithValue("@p_nb_employes_requis", nbEmployesRequis);
                cmd.Parameters.AddWithValue("@p_id_client", idClient);
                cmd.Parameters.AddWithValue("@p_statut", statut);

                con.Open();
                cmd.ExecuteNonQuery();

                getAllProjets(); // Recharge la liste des projets après modification
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }

        //supprime un Projet en fonction de son id
        public void supprimerProjet(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("SupprimerProjet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@p_numero_projet", numeroProjet);

                con.Open();
                cmd.ExecuteNonQuery();

                getAllProjets(); // Recharge la liste après suppression
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }
        public void rechercherProjets(string motCle)
        {
            listeProjet.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                // Recherche dans le titre, le numéro et le nom du client
                cmd.CommandText = @"SELECT * FROM vue_tous_projets 
                            WHERE numero_projet LIKE @motCle 
                               OR titre LIKE @motCle 
                               OR nom_client LIKE @motCle";

                cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    Projet projet = new Projet(
                        r.GetString("numero_projet"),
                        r.GetString("titre"),
                        r.GetDateTime("date_debut"),
                        r.GetString("description"),
                        r.GetDecimal("budget"),
                        r.GetInt32("nb_employes_requis"),
                        r.GetDecimal("total_salaires"),
                        0,
                           r.GetString("nom_client"),
                        r.GetString("statut"),
                        r.GetDateTime("date_creation")
                     
                    );

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }


        public void TerminerProjet(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "TerminerProjet";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_numero_projet", numeroProjet);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
            finally
            {
                
                getProjetsEnCours(); 
            }
        }
        public void getProjetsTermines()
        {
            listeProjet.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_projets_termines";

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    Projet projet = new Projet(
                        numeroProjet: r.GetString("numero_projet"),
                        titre: r.GetString("titre"),
                        dateDebut: r.GetDateTime("date_debut"),
                        description: r.GetString("description"),
                        budget: r.GetDecimal("budget"),
                        nbEmployesRequis: r.GetInt32("nb_employes_requis"),
                        totalSalaires: r.GetDecimal("total_salaires"),
                        idClient: r.GetInt32("id_client"),
                        nomClient: r.GetString("nom_client"),
                        statut: r.GetString("statut"),
                        dateCreation: r.GetDateTime("date_creation")
                    );

                    projet.NbEmployesAssignes = r.GetInt32("nb_employes_assignes");
                    projet.TelephoneClient = r.GetString("telephone_client");

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getProjetsTermines : " + ex.Message);
            }
        }

        public void ExporterProjetsCsv(string cheminFichier)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

              
                cmd.CommandText = @"
            SELECT 
                numero_projet,
                titre,
                nom_client,
                date_debut,
                budget,
                total_salaires,
                statut
            FROM vue_tous_projets
            ORDER BY date_debut;";

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();
                using StreamWriter writer = new StreamWriter(cheminFichier, false, Encoding.UTF8);

                // En-tête CSV
                writer.WriteLine("NumeroProjet;Titre;NomClient;DateDebut;Budget;TotalSalaires;BudgetRestant;Statut");

                while (r.Read())
                {
                    string numero = r.GetString("numero_projet");
                    string titre = r.GetString("titre");
                    string nomClient = r.GetString("nom_client");
                    DateTime dateDebut = r.GetDateTime("date_debut");
                    decimal budget = r.GetDecimal("budget");
                    decimal totalSalaires = r.GetDecimal("total_salaires");
                    string statut = r.GetString("statut");

                    // Calcul du budget restant
                    decimal budgetRestant = budget - totalSalaires;

                    
                    string safeTitre = titre.Replace(";", ",");
                    string safeNomClient = nomClient.Replace(";", ",");

                    writer.WriteLine(
                        $"{numero};{safeTitre};{safeNomClient};{dateDebut:yyyy-MM-dd};{budget};{totalSalaires};{budgetRestant};{statut}"
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur export CSV : " + ex.Message);
            }
        }

        public decimal GetBudgetRestant(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "SELECT BudgetRestant(@num)";
                cmd.Parameters.AddWithValue("@num", numeroProjet);

                con.Open();
                object? res = cmd.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToDecimal(res);
                else
                    return 0m;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL GetBudgetRestant : " + ex.Message);
                return 0m;
            }
        }




    }
}
