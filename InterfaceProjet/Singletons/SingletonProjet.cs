using InterfaceProjet.Classes;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Storage;

namespace InterfaceProjet.Singletons
{
    internal class SingletonProjet
    {
        private string connectionString;
        private ObservableCollection<Projet> listeProjet;
        private static SingletonProjet? instance = null;

        private SingletonProjet()
        {
            connectionString = Helpers.DatabaseHelper.ConnectionString;
            listeProjet = new ObservableCollection<Projet>();
        }

        public static SingletonProjet getInstance()
        {
            if (instance == null)
                instance = new SingletonProjet();
            return instance;
        }

        public ObservableCollection<Projet> Liste { get => listeProjet; }

        private string GetBaseQuery(string condition = "")
        {
            return $@"
                SELECT 
                    p.numero_projet, p.titre, p.date_debut, p.description, p.budget, p.nb_employes_requis, p.statut, p.date_creation,
                    p.id_client, c.nom as nom_client, c.telephone as telephone_client,
                    (SELECT COUNT(*) FROM assignations a WHERE a.numero_projet = p.numero_projet) as nb_employes_assignes,
                    (SELECT COALESCE(SUM(e.salaire_horaire * 40), 0) FROM assignations a JOIN employes e ON a.matricule_employe = e.matricule WHERE a.numero_projet = p.numero_projet) as total_salaires
                FROM projets p
                LEFT JOIN clients c ON p.id_client = c.id_client
                {condition}
            ";
        }

        private void ExecuteLoadQuery(string query, Action<SqliteCommand>? bindParameters = null)
        {
            listeProjet.Clear();
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = query;
                bindParameters?.Invoke(cmd);

                con.Open();
                using SqliteDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    int? idClient = r.IsDBNull(r.GetOrdinal("id_client")) ? (int?)null : r.GetInt32(r.GetOrdinal("id_client"));
                    string nomClient = r.IsDBNull(r.GetOrdinal("nom_client")) ? "Aucun client" : r.GetString(r.GetOrdinal("nom_client"));
                    string telClient = r.IsDBNull(r.GetOrdinal("telephone_client")) ? "N/A" : r.GetString(r.GetOrdinal("telephone_client"));
                    
                    Projet projet = new Projet(
                        numeroProjet: r.GetString(r.GetOrdinal("numero_projet")),
                        titre: r.GetString(r.GetOrdinal("titre")),
                        dateDebut: r.GetDateTime(r.GetOrdinal("date_debut")),
                        description: r.GetString(r.GetOrdinal("description")),
                        budget: r.GetDecimal(r.GetOrdinal("budget")),
                        nbEmployesRequis: r.GetInt32(r.GetOrdinal("nb_employes_requis")),
                        totalSalaires: r.GetDecimal(r.GetOrdinal("total_salaires")),
                        idClient: idClient,
                        nomClient: nomClient,
                        statut: r.GetString(r.GetOrdinal("statut")),
                        dateCreation: r.GetDateTime(r.GetOrdinal("date_creation"))
                    );

                    projet.NbEmployesAssignes = r.GetInt32(r.GetOrdinal("nb_employes_assignes"));
                    projet.TelephoneClient = telClient;

                    listeProjet.Add(projet);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite chargement projets : " + ex.Message);
            }
        }

        public void getProjetsEnCours() => ExecuteLoadQuery(GetBaseQuery("WHERE p.statut = 'En cours'"));
        public void getAllProjets() => ExecuteLoadQuery(GetBaseQuery());
        public void getProjetsTermines() => ExecuteLoadQuery(GetBaseQuery("WHERE p.statut = 'Terminé'"));

        public void rechercherProjets(string motCle)
        {
            string condition = "WHERE p.numero_projet LIKE @motCle OR p.titre LIKE @motCle OR c.nom LIKE @motCle";
            ExecuteLoadQuery(GetBaseQuery(condition), cmd => {
                cmd.Parameters.AddWithValue("@motCle", "%" + motCle + "%");
            });
        }

        public void ajouterProjetAvecProcedure(string titre, DateTime dateDebut, string description, decimal budget, int nbEmployesRequis, int idClient)
        {
            try
            {
                // Génération d'un identifiant unique de projet
                string numProjet = "PRJ-" + new Random().Next(1000, 9999);
                
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO projets (numero_projet, titre, date_debut, description, budget, nb_employes_requis, id_client)
                    VALUES (@num, @titre, @date_debut, @description, @budget, @nb_employes_requis, @id_client)";
                
                cmd.Parameters.AddWithValue("@num", numProjet);
                cmd.Parameters.AddWithValue("@titre", titre);
                cmd.Parameters.AddWithValue("@date_debut", dateDebut);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@budget", budget);
                cmd.Parameters.AddWithValue("@nb_employes_requis", nbEmployesRequis);
                cmd.Parameters.AddWithValue("@id_client", idClient);

                con.Open();
                cmd.ExecuteNonQuery();
                getAllProjets();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite ajouterProjet : " + ex.Message);
                throw;
            }
        }

        public void modifierProjetSansClient(string numeroProjet, string titre, DateTime dateDebut, string description, decimal budget, int nbEmployesRequis, string statut)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    UPDATE projets 
                    SET titre = @titre, date_debut = @date_debut, description = @description, 
                        budget = @budget, nb_employes_requis = @nb_employes_requis, statut = @statut
                    WHERE numero_projet = @numero_projet";

                cmd.Parameters.AddWithValue("@numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("@titre", titre);
                cmd.Parameters.AddWithValue("@date_debut", dateDebut);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@budget", budget);
                cmd.Parameters.AddWithValue("@nb_employes_requis", nbEmployesRequis);
                cmd.Parameters.AddWithValue("@statut", statut.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
                getAllProjets();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite modifierProjet : " + ex.Message);
                throw;
            }
        }

        public void supprimerProjet(string numeroProjet)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                con.Open();
                
                // Suppression préalable des assignations liées pour préserver l'intégrité
                using (var cmdAssign = con.CreateCommand())
                {
                    cmdAssign.CommandText = "DELETE FROM assignations WHERE numero_projet = @num";
                    cmdAssign.Parameters.AddWithValue("@num", numeroProjet);
                    cmdAssign.ExecuteNonQuery();
                }

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM projets WHERE numero_projet = @num";
                    cmd.Parameters.AddWithValue("@num", numeroProjet);
                    cmd.ExecuteNonQuery();
                }
                
                getAllProjets();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite supprimerProjet : " + ex.Message);
                throw;
            }
        }

        public void TerminerProjet(string numeroProjet)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE projets SET statut = 'Terminé' WHERE numero_projet = @num";
                cmd.Parameters.AddWithValue("@num", numeroProjet);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite TerminerProjet : " + ex.Message);
                throw;
            }
            finally
            {
                getProjetsEnCours();
            }
        }

        public void AssocierClientAuProjet(string numeroProjet, int idClient)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE projets SET id_client = @id WHERE numero_projet = @num";
                cmd.Parameters.AddWithValue("@num", numeroProjet);
                cmd.Parameters.AddWithValue("@id", idClient);

                con.Open();
                cmd.ExecuteNonQuery();
                getAllProjets();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite AssocierClientAuProjet : " + ex.Message);
                throw; 
            }
        }

        public List<Projet> ExporterProjetsCsv(StorageFile cheminFichier)
        {
            // Retourne la collection de projets en mémoire pour l'exportation
            return new List<Projet>(listeProjet);
        }

        public decimal GetBudgetRestant(string numeroProjet)
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                    SELECT p.budget - COALESCE(SUM(e.salaire_horaire * 40), 0)
                    FROM projets p
                    LEFT JOIN assignations a ON p.numero_projet = a.numero_projet
                    LEFT JOIN employes e ON a.matricule_employe = e.matricule
                    WHERE p.numero_projet = @num";
                
                cmd.Parameters.AddWithValue("@num", numeroProjet);

                con.Open();
                object? res = cmd.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    return Convert.ToDecimal(res);
                return 0m;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite GetBudgetRestant : " + ex.Message);
                return 0m;
            }
        }

        public int getNombreProjets()
        {
            try
            {
                using SqliteConnection con = new SqliteConnection(connectionString);
                using SqliteCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM projets";

                con.Open();
                object? res = cmd.ExecuteScalar();

                if (res != null && res != DBNull.Value)
                    return Convert.ToInt32(res);
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur SQLite getNombreProjets : " + ex.Message);
                return 0;
            }
        }
    }
}