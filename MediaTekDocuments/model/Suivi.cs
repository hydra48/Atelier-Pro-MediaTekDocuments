using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi (étapes de suivi d'une commande)
    /// </summary>
    public class Suivi
    {
        /// <summary>
        /// Récupère ou définit l'id du Suivi d'une commande
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Récupère ou définit le libelle du Suivi d'une commande
        /// </summary>
        public string Libelle { get; set; }

        /// <summary>
        /// Initialisation d'un objet Suivi
        /// </summary>
        /// <param name="id">Id du Suivi d'une commande</param>
        /// <param name="libelle">Libelle du Suivi d'une commande</param>
        public Suivi(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }
    }
}
