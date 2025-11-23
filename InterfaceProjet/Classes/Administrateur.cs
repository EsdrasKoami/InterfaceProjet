using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    internal class Administrateur
    {
         int  idAdmin;
            string nomUtilisateur;
           string motDePasse;
        DateTime dateCreation;
        DateTime dateDerniereConnexion;

        public Administrateur(int idAdmin, string nomUtilisateur, string motDePasse, DateTime dateCreation, DateTime dateDerniereConnexion)
        {
            this.idAdmin = idAdmin;
            this.nomUtilisateur = nomUtilisateur;
            this.motDePasse = motDePasse;
            this.dateCreation = dateCreation;
            this.dateDerniereConnexion = dateDerniereConnexion;
        }

        public int IdAdmin { get => idAdmin; set => idAdmin = value; }
        public string NomUtilisateur { get => nomUtilisateur; set => nomUtilisateur = value; }
        public string MotDePasse { get => motDePasse; set => motDePasse = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }
        public DateTime DateDerniereConnexion { get => dateDerniereConnexion; set => dateDerniereConnexion = value; }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
