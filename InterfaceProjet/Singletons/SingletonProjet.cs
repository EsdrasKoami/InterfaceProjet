using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;

namespace InterfaceProjet.Singletons
{
    internal class SingletonProjet
    {
        private string connectionString;
        private ObservableCollection<Projet> listeProjet;
        private static SingletonProjet instance = null;

        // ============================================
        // Constructeur privé (Singleton)
        // ============================================
        private SingletonProjet()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
            listeProjet = new ObservableCollection<Projet>();
        }

        // ============================================
        // Instance du Singleton
        // ============================================
        public static SingletonProjet getInstance()
        {
            if (instance == null)
                instance = new SingletonProjet();
            return instance;
        }

        // ============================================
        // Propriété: Liste des projets
        // ============================================
        public ObservableCollection<Projet> Liste { get => listeProjet; }

        // ============================================
        // MÉTHODE: Obtenir tous les projets en cours
        // ============================================
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
                    // Gestion du NULL pour id_client
                    int? idClient = null;
                    if (!r.IsDBNull(r.GetOrdinal("id_client")))
                    {
                        idClient = r.GetInt32("id_client");
                    }

                    // Gestion du NULL pour nom_client
                    string nomClient = "Aucun client";
                    if (!r.IsDBNull(r.GetOrdinal("nom_client")))
                    {
                        nomClient = r.GetString("nom_client");
                    }

                    // Gestion du NULL pour telephone_client
                    string telClient = "N/A";
                    if (!r.IsDBNull(r.GetOrdinal("telephone_client")))
                    {
                        telClient = r.GetString("telephone_client");
                    }

                    Projet projet = new Projet(
                        numeroProjet: r.GetString("numero_projet"),
                        titre: r.GetString("titre"),
                        dateDebut: r.GetDateTime("date_debut"),
                        description: r.GetString("description"),
                        budget: r.GetDecimal("budget"),
                        nbEmployesRequis: r.GetInt32("nb_employes_requis"),
                        totalSalaires: r.GetDecimal("total_salaires"),
                        idClient: idClient,
                        nomClient: nomClient,
                        statut: r.GetString("statut"),
                        dateCreation: r.GetDateTime("date_creation")
                    );

                    projet.NbEmployesAssignes = r.GetInt32("nb_employes_assignes");
                    projet.TelephoneClient = telClient;

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getProjetsEnCours : " + ex.Message);
            }
        }

        // ============================================
        // MÉTHODE: Obtenir tous les projets
        // ============================================
        public void getAllProjets()
        {
            listeProjet.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM vue_tous_projets";

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    // ✅ Gestion sécurisée: vérifier si la colonne existe d'abord
                    int? idClient = null; 
                    try
                    {
                        int ordinal = r.GetOrdinal("id_client");
                        if (!r.IsDBNull(ordinal))
                        {
                            idClient = r.GetInt32(ordinal);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // La colonne id_client n'existe pas dans la vue
                        Debug.WriteLine("Colonne 'id_client' introuvable dans vue_tous_projets");
                    }

                    string nomClient = "Aucun client";
                    try
                    {
                        int ordinal = r.GetOrdinal("nom_client");
                        if (!r.IsDBNull(ordinal))
                        {
                            nomClient = r.GetString(ordinal);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Debug.WriteLine("Colonne 'nom_client' introuvable dans vue_tous_projets");
                    }

                    Projet projet = new Projet(
                        r.GetString("numero_projet"),
                        r.GetString("titre"),
                        r.GetDateTime("date_debut"),
                        r.GetString("description"),
                        r.GetDecimal("budget"),
                        r.GetInt32("nb_employes_requis"),
                        r.GetDecimal("total_salaires"),
                        idClient,
                        nomClient,
                        r.GetString("statut"),
                        r.GetDateTime("date_creation")
                    );

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getAllProjets : " + ex.Message);
            }
        }

        // ============================================
        // MÉTHODE: Obtenir tous les projets terminés
        // ============================================
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
                    int? idClient = null;
                    if (!r.IsDBNull(r.GetOrdinal("id_client")))
                    {
                        idClient = r.GetInt32("id_client");
                    }

                    string nomClient = "Aucun client";
                    if (!r.IsDBNull(r.GetOrdinal("nom_client")))
                    {
                        nomClient = r.GetString("nom_client");
                    }

                    string telClient = "N/A";
                    if (!r.IsDBNull(r.GetOrdinal("telephone_client")))
                    {
                        telClient = r.GetString("telephone_client");
                    }

                    Projet projet = new Projet(
                        numeroProjet: r.GetString("numero_projet"),
                        titre: r.GetString("titre"),
                        dateDebut: r.GetDateTime("date_debut"),
                        description: r.GetString("description"),
                        budget: r.GetDecimal("budget"),
                        nbEmployesRequis: r.GetInt32("nb_employes_requis"),
                        totalSalaires: r.GetDecimal("total_salaires"),
                        idClient: idClient,
                        nomClient: nomClient,
                        statut: r.GetString("statut"),
                        dateCreation: r.GetDateTime("date_creation")
                    );

                    projet.NbEmployesAssignes = r.GetInt32("nb_employes_assignes");
                    projet.TelephoneClient = telClient;

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getProjetsTermines : " + ex.Message);
            }
        }

        // ============================================
        // MÉTHODE: Rechercher des projets
        // ============================================
        public void rechercherProjets(string motCle)
        {
            listeProjet.Clear();

            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = @"SELECT * FROM vue_tous_projets 
                                    WHERE numero_projet LIKE @motCle 
                                       OR titre LIKE @motCle 
                                       OR nom_client LIKE @motCle";

                cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    // Gestion sécurisée des colonnes optionnelles
                    int? idClient = null;
                    try
                    {
                        int ordinal = r.GetOrdinal("id_client");
                        if (!r.IsDBNull(ordinal))
                        {
                            idClient = r.GetInt32(ordinal);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Debug.WriteLine("Colonne 'id_client' introuvable");
                    }

                    string nomClient = "Aucun client";
                    try
                    {
                        int ordinal = r.GetOrdinal("nom_client");
                        if (!r.IsDBNull(ordinal))
                        {
                            nomClient = r.GetString(ordinal);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Debug.WriteLine("Colonne 'nom_client' introuvable");
                    }

                    Projet projet = new Projet(
                        r.GetString("numero_projet"),
                        r.GetString("titre"),
                        r.GetDateTime("date_debut"),
                        r.GetString("description"),
                        r.GetDecimal("budget"),
                        r.GetInt32("nb_employes_requis"),
                        r.GetDecimal("total_salaires"),
                        idClient,
                        nomClient,
                        r.GetString("statut"),
                        r.GetDateTime("date_creation")
                    );

                    listeProjet.Add(projet);
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL rechercherProjets : " + ex.Message);
            }
        }

        // ============================================
        // MÉTHODE: Ajouter un projet (avec client obligatoire)
        // ============================================
        public void ajouterProjetAvecProcedure(
            string titre,
            DateTime dateDebut,
            string description,
            decimal budget,
            int nbEmployesRequis,
            int idClient)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AjouterProjet", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Plus de paramètre p_numero_projet, le trigger s'en occupe
                cmd.Parameters.AddWithValue("p_titre", titre);
                cmd.Parameters.AddWithValue("p_date_debut", dateDebut);
                cmd.Parameters.AddWithValue("p_description", description);
                cmd.Parameters.AddWithValue("p_budget", budget);
                cmd.Parameters.AddWithValue("p_nb_employes_requis", nbEmployesRequis);
                cmd.Parameters.AddWithValue("p_id_client", idClient);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Projet '{titre}' créé avec succès pour le client {idClient}.");

                // Recharger la liste
                getAllProjets();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL ajouterProjetAvecProcedure : " + ex.Message);
                throw;
            }
        }

        // ============================================
        // MÉTHODE: Modifier un projet (SANS toucher au client)
        // ============================================
        public void modifierProjetSansClient(string numeroProjet, string titre, DateTime dateDebut,
                                             string description, decimal budget, int nbEmployesRequis,
                                             string statut)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = @"
                    UPDATE projets 
                    SET titre = @titre,
                        date_debut = @date_debut,
                        description = @description,
                        budget = @budget,
                        nb_employes_requis = @nb_employes_requis,
                        statut = @statut
                    WHERE numero_projet = @numero_projet";

                cmd.Parameters.AddWithValue("@numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("@titre", titre);
                cmd.Parameters.AddWithValue("@date_debut", dateDebut);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@budget", budget);
                cmd.Parameters.AddWithValue("@nb_employes_requis", nbEmployesRequis);
                cmd.Parameters.AddWithValue("@statut", statut.Trim());


                con.Open();
                Debug.WriteLine($"STATUT = '{statut}'");
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    Debug.WriteLine($"Projet {numeroProjet} modifié avec succès.");
                  

                }

                getAllProjets();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL modifierProjetSansClient : " + ex.Message);
                throw;
            }
        }

        // ============================================
        // MÉTHODE: Supprimer un projet
        // ============================================
        public void supprimerProjet(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("SupprimerProjet", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@p_numero_projet", numeroProjet);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Projet {numeroProjet} supprimé avec succès.");
                getAllProjets();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL supprimerProjet : " + ex.Message);
                throw;
            }
        }

        // ============================================
        // MÉTHODE: Terminer un projet
        // ============================================
        public void TerminerProjet(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "TerminerProjet";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_numero_projet", numeroProjet);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Projet {numeroProjet} terminé avec succès.");
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL TerminerProjet : " + ex.Message);
                throw;
            }
            finally
            {
                getProjetsEnCours();
            }
        }

        // ============================================
        // MÉTHODE: Associer un client à un projet
        // ============================================
        public void AssocierClientAuProjet(string numeroProjet, int idClient)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AssocierClientAuProjet", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("p_id_client", idClient);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Client {idClient} associé au projet {numeroProjet} avec succès.");

                // Recharger la liste
                getAllProjets();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL AssocierClientAuProjet : " + ex.Message);
                throw; 
            }
        }

        // ============================================
        // MÉTHODE: Exporter les projets en CSV
        // ============================================
        public List<Projet> ExporterProjetsCsv(StorageFile cheminFichier)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = @"
                    SELECT 
                        numero_projet,
                        titre,
                        IFNULL(nom_client, 'Aucun client') AS nom_client,
                        date_debut,
                        budget,
                        total_salaires,
                        statut
                    FROM vue_tous_projets
                    ORDER BY date_debut;";

                con.Open();
                using MySqlDataReader r = cmd.ExecuteReader();
          

                while (r.Read())
                {
                    string numero = r.GetString("numero_projet");
                    string titre = r.GetString("titre");
                    string nomClient = r.GetString("nom_client");
                    DateTime dateDebut = r.GetDateTime("date_debut");
                    decimal budget = r.GetDecimal("budget");
                    decimal totalSalaires = r.GetDecimal("total_salaires");
                    string statut = r.GetString("statut");

                    decimal budgetRestant = budget - totalSalaires;


                 
                }

                return new List<Projet>(listeProjet);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur export CSV : " + ex.Message);
                throw;
            }
        }

        

        // ============================================
        // MÉTHODE: Obtenir le budget restant d'un projet
        // ============================================
        public decimal GetBudgetRestant(string numeroProjet)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "SELECT BudgetRestant(@num)";
                cmd.Parameters.AddWithValue("@num", numeroProjet);

                con.Open();
                object res = cmd.ExecuteScalar();

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


        // ============================================
        // MÉTHODE: Obtenir le nombre total de projets
        // ============================================
        public int getNombreProjets()
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = con.CreateCommand();

                cmd.CommandText = "SELECT COUNT(*) FROM projets";

                con.Open();
                object res = cmd.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
                else
                    return 0;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL getNombreProjets : " + ex.Message);
                return 0;
            }
        }
    }
}