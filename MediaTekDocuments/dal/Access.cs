using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Serilog;
using Serilog.Formatting.Json;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocumentsjean";
        /// <summary>
        /// nom de connexion à la bdd
        /// </summary>
        private static readonly string connectionName = "MediaTekDocuments.Properties.Settings.mediatek86ConnectionString";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";
        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// Récupération de la chaîne de connexion
        /// </summary>
        /// <param name="name">nom de la chaîne de connexion</param>
        /// <returns></returns>
        static string GetConnectionStringByName(string name)
        {
            string value = null;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            if (settings != null)
                value = settings.ConnectionString;
            return value;
        }
        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = GetConnectionStringByName(connectionName);

                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), "logs/log.txt",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Log.Fatal("Access.Access catch connectionString={0} erreur={1}", connectionName, e.Message);
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre");
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon");
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public");
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre");
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd");
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue");
            return lesRevues;
        }
        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + idDocument);
            return lesExemplaires;
        }
        /// <summary>
        /// Retourne les suivis d'un document
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            List<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi");
            return lesSuivis;
        }

        /// <summary>
        /// Retourne les commandes
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets CommandeDocument</returns>

        public List<Commande> GetCommandes(string idDocument)
        {
            List<Commande> lesCommandes = TraitementRecup<Commande>(GET, "commande/" + idDocument);
            return lesCommandes;
        }

        /// <summary>
        /// Retourne les commandes des documents
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets CommandeDocument</returns>

        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            List<CommandeDocument> lesCommandesDocument = TraitementRecup<CommandeDocument>(GET, "commandedocument/" + idDocument);
            return lesCommandesDocument;
        }

        /// <summary>
        /// Retourne les abonnements d'une revue
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Abonnement</returns>
        public List<Abonnement> GetAbonnementRevue(string idDocument)
        {
            List<Abonnement> lesAbonnementsRevue = TraitementRecup<Abonnement>(GET, "abonnement/" + idDocument);
            return lesAbonnementsRevue;
        }

        /// <summary>
        /// Retourne les abonnements arrivants à échéance dans 30 jours
        /// </summary>
        /// <returns></returns>
        public List<Abonnement> GetAbonnementsEcheance()
        {
            List<Abonnement> lesAbonnementsAEcheance = TraitementRecup<Abonnement>(GET, "abonnementsecheance");
            return lesAbonnementsAEcheance;
        }
         /// <summary>
        /// Ecriture d'une commande en base de données
        /// </summary>
        /// <param name="commande">Objet de type Commande à insérer</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerCommande(Commande commande)
        {
            String jsonCreerCommande = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());
            Console.WriteLine("jsonCreerCommande " + jsonCreerCommande);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Commande> liste = TraitementRecup<Commande>(POST, "commande/" + jsonCreerCommande);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerCommande catch jsonCreerCommande={0} erreur={1} ", jsonCreerCommande, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ecriture d'une commande de document en base de données
        /// </summary>
        /// <param name="id">Id de la commande de document à insérer</param>
        /// <param name="nbExemplaire">Nombre d'exemplaires de la commande de document</param>
        /// <param name="idLivreDvd">Id du livreDvd correspondant à la commande de document</param>
        /// <param name="idSuivi">Id de l'étape de suivi de la commande de document</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerCommandeDocument(string id, int nbExemplaire, string idLivreDvd, string idSuivi)
        {
            String jsonCreerCommandeDocument = "{ \"id\" : \"" + id + "\", \"nbExemplaire\" : \"" + nbExemplaire + "\", \"idLivreDvd\" : \"" + idLivreDvd + "\", \"idSuivi\" : \"" + idSuivi + "\"}";
            Console.WriteLine("jsonCreerCommandeDocument" + jsonCreerCommandeDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commandedocument/" + jsonCreerCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerCommandeDocument catch jsonCreerCommandeDocument={0} erreur={1} ", jsonCreerCommandeDocument, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de document en base de données
        /// </summary>
        /// <param name="id">Id de la commande de document à modifier</param>
        /// <param name="idSuivi">Id de l'étape de suivi</param>
        /// <returns>True si la modification a pu se faire</returns>
        public bool ModifierSuiviCommandeDocument(string id, string idSuivi)
        {
            String jsonModifierSuiviCommandeDocument = "{ \"id\" : \"" + id + "\", \"idSuivi\" : \"" + idSuivi + "\"}";
            Console.WriteLine("jsonModifierSuiviCommandeDocument" + jsonModifierSuiviCommandeDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(PUT, "commandedocument/" + id + "/" + jsonModifierSuiviCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierSuiviCommandeDocument catch jsonModifierSuiviCommandeDocument={0} erreur={1} ", jsonModifierSuiviCommandeDocument, ex.Message);

            }
            return false;
        }

        /// <summary>
        /// Suppression d'une commande de document en base de données
        /// </summary>
        /// <param name="commandesDocument">Objet de type CommandeDocument à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerCommandeDocument(CommandeDocument commandesDocument)
        {
            String jsonSupprimerCommandeDocument = "{\"id\":\"" + commandesDocument.Id + "\"}";
            Console.WriteLine("jsonSupprimerCommandeDocument=" + jsonSupprimerCommandeDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(DELETE, "commandedocument/" + jsonSupprimerCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerCommandeDocument catch jsonSupprimerCommandeDocument={0} erreur={1} ", jsonSupprimerCommandeDocument, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Ecriture d'un abonnement à une revue en base de données
        /// </summary>
        /// <param name="id">Id de l'abonnement à une revue à insérer</param>
        /// <param name="dateFinAbonnement">Date de fin d'abonnement à une revue</param>
        /// <param name="idRevue">Id de la revue concernée par l'abonnement</param>
        /// <returns>True si l'insertion a pu se faire</returns>
        public bool CreerAbonnementRevue(string id, DateTime dateFinAbonnement, string idRevue)
        {
            String jsonDateCommande = JsonConvert.SerializeObject(dateFinAbonnement, new CustomDateTimeConverter());
            String jsonCreerAbonnementRevue = "{\"id\":\"" + id + "\", \"dateFinAbonnement\" : " + jsonDateCommande + ", \"idRevue\" :  \"" + idRevue + "\"}";
            Console.WriteLine("jsonCreerAbonnementRevue" + jsonCreerAbonnementRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement/" + jsonCreerAbonnementRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerAbonnementRevue catch jsonCreerAbonnementRevue={0} erreur={1} ", jsonCreerAbonnementRevue, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Suppression d'un abonnement de revue en base de données
        /// </summary>
        /// <param name="abonnement">Objet de type Abonnement à supprimer</param>
        /// <returns>True si la suppression a pu se faire</returns>
        public bool SupprimerAbonnementRevue(Abonnement abonnement)
        {
            String jsonSupprimerAbonnementRevue = "{\"id\":\"" + abonnement.Id + "\"}";
            Console.WriteLine("jsonSupprimerAbonnementRevue=" + jsonSupprimerAbonnementRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Abonnement> liste = TraitementRecup<Abonnement>(DELETE, "abonnement/" + jsonSupprimerAbonnementRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerAbonnementRevue catch jsonSupprimerAbonnementRevue={0} erreur={1} ", jsonSupprimerAbonnementRevue, ex.Message);
            }
            return false;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire/" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// Récupère les informations d'identification de l'utilisateur
        /// </summary>
        /// <param name="login">Identifiant de l'utilisateur</param>
        /// <param name="password">Mot de passe de l'utilisateur</param>
        /// <returns>True si l'utilisateur est trouvé</returns>
        public Utilisateur GetUtilisateur(string login, string password)
        {
            try
            {
                List<Utilisateur> liste = TraitementRecup<Utilisateur>(GET, "utilisateur/" + login);
                if (liste == null || liste.Count == 0)
                {
                    return null;
                }
                Utilisateur utilisateur = liste[0];
                if (utilisateur.Password != password)
                {
                    return null;
                }
                return utilisateur;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.GetUtilisateur catch login={0} erreur={1}", login, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(String methode, String message)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + ", message = " + (String)retour["message"]);
                    Log.Error("Access.TraitementRecup catch code={0} erreur={1} ", code);
                }
            }
            catch (Exception e)
            {
                Log.Error("Access.TraitementRecup catch liste={0} erreur={1}", liste, e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}