using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Controleur pour la fenêtre FrmMediatek
    /// Gère les interactions entre la vue et le modèle
    /// </summary>
    public class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return access.GetExemplairesRevue(idDocument);
        }

        /// <summary>
        /// getter sur les suivis
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// récèpère toutes les commandes d'un document
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>

        public List<Commande> GetCommandes(string idDocument)
        {
            return access.GetCommandes(idDocument);
        }

        /// <summary>
        /// récupère les commandes d'un document
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            return access.GetCommandesDocument(idDocument);
        }

        /// <summary>
        /// récupère les abonnements d'une revue
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Abonnement</returns>
        public List<Abonnement> GetAbonnementRevue(string idDocument)
        {
            return access.GetAbonnementRevue(idDocument);
        }
        /// <summary>
        /// récupère les abonnements qui prennent fin dans 30 jours
        /// </summary>
        /// <returns></returns>
        public List<Abonnement> GetAbonnementsEcheance()
        {
            return access.GetAbonnementsEcheance();
        }

        /// <summary>
        /// récupère les exemplaires d'un document
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            return access.GetExemplairesDocument(idDocument);
        }

        /// <summary>
        /// récupère les états
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> GetAllEtatsDocument()
        {
            return access.GetAllEtatsDocument();
        }

        /// <summary>
        /// récupère les documents
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Document</returns>
        public List<Document> GetAllDocuments(string idDocument)
        {
            return access.GetAllDocuments(idDocument);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="id">Id du document correspondant à l'exemplaire d'une revue à insérer</param>
        /// <param name="numero">Numéro de l'exemplaire d'une revue à insérer</param>
        /// <param name="dateAchat">Date d'achat de l'exemplaire d'une revue</param>
        /// <param name="photo">Photo de l'exemplaire de la revue</param>
        /// <param name="idEtat">Id de l'état d'usure de l'exemplaire d'une revue</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerExemplaireRevue(string id, int numero, DateTime dateAchat, string photo, string idEtat)
        {
            return access.CreerExemplaireRevue(id, numero, dateAchat, photo, idEtat);
        }

        /// <summary>
        /// Crée un document dans la bdd
        /// </summary>
        /// <param name="Id">Id du document à insérer</param>
        /// <param name="Titre">Titre du document</param>
        /// <param name="Image">Image du document</param>
        /// <param name="IdGenre">Id du genre du document</param>
        /// <param name="IdPublic">Id du public du document</param>
        /// <param name="IdRayon">Id du rayon du document</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerDocument(string Id, string Titre, string Image, string IdRayon, string IdPublic, string IdGenre)
        {
            return access.CreerDocument(Id, Titre, Image, IdRayon, IdPublic, IdGenre);
        }

        /// <summary>
        /// Modifie un document dans la bdd
        /// </summary>
        /// <param name="Id">Id du document à modifier</param>
        /// <param name="Titre">Titre du document</param>
        /// <param name="Image">Image du document</param>
        /// <param name="IdGenre">Id du genre du document</param>
        /// <param name="IdPublic">Id du public du document</param>
        /// <param name="IdRayon">Id du rayon du document</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifierDocument(string Id, string Titre, string Image, string IdGenre, string IdPublic, string IdRayon)
        {
            return access.ModifierDocument(Id, Titre, Image, IdGenre, IdPublic, IdRayon);
        }

        /// <summary>
        /// Supprime un document dans la bdd
        /// </summary>
        /// <param name="Id">Id du document à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerDocument(string Id)
        {
            return access.SupprimerDocument(Id);
        }

        /// <summary>
        /// Crée un livre dans la bdd
        /// </summary>
        /// <param name="Id">Id du livre à insérer</param>
        /// <param name="Isbn">Code Isbn du livre</param>
        /// <param name="Auteur">Auteur du livre</param>
        /// <param name="Collection">Collection du livre</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            return access.CreerLivre(Id, Isbn, Auteur, Collection);
        }

        /// <summary>
        /// Modifie un livre dans la bdd
        /// </summary>
        /// <param name="Id">Id du livre à modifier</param>
        /// <param name="Isbn">Code Isbn du livre</param>
        /// <param name="Auteur">Auteur du livre</param>
        /// <param name="Collection">Collection du livre</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifierLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            return access.ModifierLivre(Id, Isbn, Auteur, Collection);
        }

        /// <summary>
        /// Suppression d'un livre en bdd
        /// </summary>
        /// <param name="Id">Id du livre à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerLivre(string Id)
        {
            return access.SupprimerLivre(Id);
        }

        /// <summary>
        /// Créé un Dvd dans la bdd
        /// </summary>
        /// <param name="Id">Id du dvd à insérer</param>
        /// <param name="Synopsis">Synopsis du dvd</param>
        /// <param name="Realisateur">Réalisateur du dvd</param>
        /// <param name="Duree">Durée du dvd</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            return access.CreerDvd(Id, Synopsis, Realisateur, Duree);
        }

        /// <summary>
        /// Modifie un dvd dans la bdd
        /// </summary>
        /// <param name="Id">Id du dvd à modifier</param>
        /// <param name="Synopsis">Synopsis du dvd</param>
        /// <param name="Realisateur">Réalisateur du dvd</param>
        /// <param name="Duree">Durée du dvd</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifierDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            return access.ModifierDvd(Id, Synopsis, Realisateur, Duree);
        }

        /// <summary>
        /// Supprimme un dvd dans la bdd
        /// </summary>
        /// <param name="Id">Id du dvd à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerDvd(string Id)
        {
            return access.SupprimerDvd(Id);
        }

        /// <summary>
        /// Crée une revue dans la bdd
        /// </summary>
        /// <param name="Id">Id de la revue à insérer</param>
        /// <param name="Periodicite">Périodicité de la revue</param>
        /// <param name="DelaiMiseADispo">Délai de mise à disposition de la revue</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            return access.CreerRevue(Id, Periodicite, DelaiMiseADispo);
        }

        /// <summary>
        /// Modifie une revue dans la bdd
        /// </summary>
        /// <param name="Id">Id de la revue à modifier</param>
        /// <param name="Periodicite">Périodicité de la revue</param>
        /// <param name="DelaiMiseADispo">Délai de mise à disposition de la revue</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifierRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            return access.ModifierRevue(Id, Periodicite, DelaiMiseADispo);
        }

        /// <summary>
        /// Supprime une revue dans la bdd
        /// </summary>
        /// <param name="Id">Id de la revue à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerRevue(string Id)
        {
            return access.SupprimerRevue(Id);
        }

        /// <summary>
        /// Créé une commande dans la bdd
        /// </summary>
        /// <param name="commande">Objet de type Commande à insérer</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerCommande(Commande commande)
        {
            return access.CreerCommande(commande);
        }

        /// <summary>
        /// Créé une commande de document dans la bdd
        /// </summary>
        /// <param name="id">Id de la commande de document à insérer</param>
        /// <param name="nbExemplaire">Nombre d'exemplaires de la commande de document</param>
        /// <param name="idLivreDvd">Id du livreDvd correspondant à la commande de document</param>
        /// <param name="idSuivi">Id de l'étape de suivi de la commande de document</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerCommandeDocument(string id, int nbExemplaire, string idLivreDvd, string idSuivi)
        {
            return access.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
        }

        /// <summary>
        /// Modifie l'étape de suivi d'une commande dans la bdd
        /// </summary>
        /// <param name="id">Id de la commande de document à modifier</param>
        /// <param name="idSuivi">Id de l'étape de suivi</param>
        /// <returns>True si la modification a pu se faire</returns>
        internal bool ModifierSuiviCommandeDocument(string id, string idSuivi)
        {
            return access.ModifierSuiviCommandeDocument(id, idSuivi);
        }

        /// <summary>
        /// Supprimme une commande de document dans la bdd
        /// </summary>
        /// <param name="commandesDocument">Objet de type CommandeDocument à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerCommandeDocument(CommandeDocument commandesDocument)
        {
            return access.SupprimerCommandeDocument(commandesDocument);
        }

        /// <summary>
        /// Crée un abonnement de revue dans la bdd
        /// </summary>
        /// <param name="id">Id de l'abonnement à une revue à insérer</param>
        /// <param name="dateFinAbonnement">Date de fin d'abonnement à une revue</param>
        /// <param name="idRevue">Id de la revue concernée par l'abonnement</param>
        /// <returns>True si l'insertion pu se faire</returns>
        public bool CreerAbonnementRevue(string id, DateTime dateFinAbonnement, string idRevue)
        {
            return access.CreerAbonnementRevue(id, dateFinAbonnement, idRevue);
        }

        /// <summary>
        /// Supprimme un abonnement de revue dans la bdd
        /// </summary>
        /// <param name="abonnement">>Objet de type Abonnement à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerAbonnementRevue(Abonnement abonnement)
        {
            return access.SupprimerAbonnementRevue(abonnement);
        }

        /// <summary>
        /// Modifie l'état d'un exemplaire d'un document dans la bdd
        /// </summary>
        /// <param name="exemplaire">>Objet de type Exemplaire à modifier</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifierEtatExemplaireDocument(Exemplaire exemplaire)
        {
            return access.ModifierEtatExemplaireDocument(exemplaire);
        }

        /// <summary>
        /// Supprime un exemplaire d'un document dans la bdd
        /// </summary>
        /// <param name="exemplaire">>Objet de type Exemplaire à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerExemplaireDocument(Exemplaire exemplaire)
        {
            return access.SupprimerExemplaireDocument(exemplaire);
        }


    }
}
