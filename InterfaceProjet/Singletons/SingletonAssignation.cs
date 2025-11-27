
using InterfaceProjet.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Singletons
{
    internal class SingletonAssignation
    {

        string connectionString;
        ObservableCollection<Assignation> listeAssignation;
        static SingletonAssignation instance = null;
        private SingletonAssignation()
        {
            connectionString = "Server=cours.cegep3r.info;Database=a2025_420335-345ri_greq20;Uid=6233629;Pwd=6233629;";
            listeAssignation = new ObservableCollection<Assignation>();
        }
        //retourne l’instance du singleton
        public static SingletonAssignation getInstance()
        {
            if (instance == null)
                instance = new SingletonAssignation();
            return instance;
        }
        //Propriété qui retourne la liste des Assignations
        public ObservableCollection<Assignation> Liste { get => listeAssignation; }



        public void AjouterAssignationEmploye(string matriculeEmploye,
                                  string numeroProjet,
                                  decimal heuresTravaillees)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AjouterAssignation", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                
                cmd.Parameters.AddWithValue("p_matricule_employe", matriculeEmploye);
                cmd.Parameters.AddWithValue("p_numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("p_heures_travaillees", heuresTravaillees);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine("Assignation ajoutée avec succès !");

                // Si tu veux mettre à jour la liste locale :
                // ex : Recharger les assignations du projet
                // getAssignationsParProjet(numeroProjet);   (à écrire plus tard)
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

        public void AssocierClientAuProjet(string numeroProjet, int idClient)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AssocierClientAuProjet", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                
                cmd.Parameters.AddWithValue("p_numero_projet", numeroProjet);
                cmd.Parameters.AddWithValue("p_id_client", idClient);

                con.Open();
                cmd.ExecuteNonQuery();

                Debug.WriteLine($"Client {idClient} associé au projet {numeroProjet} avec succès.");

                SingletonProjet.getInstance().getAllProjets();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL (AssocierClientAuProjet) : " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur (AssocierClientAuProjet) : " + ex.Message);
            }
        }



      
        public void ajouterAssignationAvecProcedure(string numeroAssignation, string titre, DateTime dateDebut,
                                        string description, decimal budget, int nbAssignationsRequis,
                                        int idClient)
        {
            try
            {
                using MySqlConnection con = new MySqlConnection(connectionString);
                using MySqlCommand cmd = new MySqlCommand("AjouterAssignation", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // Paramètres de la procédure
                cmd.Parameters.AddWithValue("@p_numero_Assignation", numeroAssignation);
                cmd.Parameters.AddWithValue("@p_titre", titre);
                cmd.Parameters.AddWithValue("@p_date_debut", dateDebut);
                cmd.Parameters.AddWithValue("@p_description", description);
                cmd.Parameters.AddWithValue("@p_budget", budget);
                cmd.Parameters.AddWithValue("@p_nb_Assignations_requis", nbAssignationsRequis);
                cmd.Parameters.AddWithValue("@p_id_client", idClient);

                con.Open();
                cmd.ExecuteNonQuery();

                
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Erreur MySQL : " + ex.Message);
            }
        }
       


  
       
    }
}
