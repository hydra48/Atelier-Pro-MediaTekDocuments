﻿using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        private readonly Service service = null;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        public FrmMediatek(Service service)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.service = service;
        }

        /// <summary>
        /// Affichage de l'alerte si le service de l'utilisateur est "administratif" ou "administrateur"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmAlerteFinAbonnement_Shown(object sender, EventArgs e)
        {
            if (service.Libelle == "administratif" || service.Libelle == "administrateur")
            {
                FrmAlerteFinAbonnement frmAlerteFinAbonnement = new FrmAlerteFinAbonnement(controller);
                frmAlerteFinAbonnement.ShowDialog();
            }
            AutorisationsAcces(service);
        }

        /// <summary>
        /// Désactive les champs selon les habilitations du service de prêts
        /// </summary>
        /// <param name="service">Service associé à l'utilisateur</param>
        private void AutorisationsAcces(Service service)
        {
            if (service.Libelle == "prêts")
            {
                tabOngletsApplication.TabPages.Remove(tabCommandesLivres);
                tabOngletsApplication.TabPages.Remove(tabCommandesDvd);
                tabOngletsApplication.TabPages.Remove(tabCommandesRevues);

                grpLivresInfos.Enabled = false;
               

                grpDvdInfos.Enabled = false;
                

                grpRevuesInfos.Enabled = false;

                txbReceptionExemplaireNumero.Enabled = false;
                dtpReceptionExemplaireDate.Enabled = false;
                txbReceptionExemplaireImage.Enabled = false;
                btnReceptionExemplaireImage.Enabled = false;
                btnReceptionExemplaireValider.Enabled = false;
                
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
            
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        
        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                    
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            textlivreauteur.Text = livre.Auteur;
            textlivrecollec.Text = livre.Collection;
            textlivreimage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            textlivresnum.Text = livre.Id;
            textlivretitre.Text = livre.Titre;
            textlivregenre.Text = livre.Genre;
            textlivrepublic.Text = livre.Public;
            textlivrerayon.Text = livre.Rayon;
            string image = livre.Image;
            try
            {
                picturelivreimg.Image = Image.FromFile(image);
            }
            catch
            {
                picturelivreimg.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            textlivreauteur.Text = "";
            textlivrecollec.Text = "";
            textlivreimage.Text = "";
            txbLivresIsbn.Text = "";
            textlivresnum.Text = "";
            textlivregenre.Text = "";
            textlivrepublic.Text = "";
            textlivrerayon.Text = "";
            textlivretitre.Text = "";
            picturelivreimg.Image = null;
            
        }

        private void btnInfosLivreVider_Click(object sender, EventArgs e)
        {
            VideLivresInfos();
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }


        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
            

        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();


        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                    
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            textrealdvd.Text = dvd.Realisateur;
            txbdvdsyno.Text = dvd.Synopsis;
            textchemindvd.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txtnumdvd.Text = dvd.Id;
            textgenredvd.Text = dvd.Genre;
            textpublicdvd.Text = dvd.Public;
            textrayondvd.Text = dvd.Rayon;
            texttitredvd.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                picturedvdimg.Image = Image.FromFile(image);
            }
            catch
            {
                picturedvdimg.Image = null;
            }

        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            textrealdvd.Text = "";
            txbdvdsyno.Text = "";
            textchemindvd.Text = "";
            txbDvdDuree.Text = "";
            txtnumdvd.Text = "";
            textgenredvd.Text = "";
            textpublicdvd.Text = "";
            textrayondvd.Text = "";
            texttitredvd.Text = "";
            picturedvdimg.Image = null;
            
        }

        private void btnInfosDvdVider_Click(object sender, EventArgs e)
        {
            VideDvdInfos();
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                   
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

       

        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues">Liste des revues</param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            textrevueperiod.Text = revue.Periodicite;
            textrevueimg.Text = revue.Image;
            textrevuerelai.Text = revue.DelaiMiseADispo.ToString();
            textrevuenum.Text = revue.Id;
            textrevuegenre.Text = revue.Genre;
            textrevuepublic.Text = revue.Public;
            textrevuerayon.Text = revue.Rayon;
            textrevuetitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            textrevueperiod.Text = "";
            textrevueimg.Text = "";
            textrevuerelai.Text = "";
            textrevuenum.Text = "";
            textrevuegenre.Text = "";
            textrevuepublic.Text = "";
            textrevuerayon.Text = "";
            textrevuetitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }



               
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireRevueImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {

            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireRevueImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                     Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

         #endregion

        #region Onglet CommandesLivres
        private readonly BindingSource bdgCommandesLivre = new BindingSource();
        private List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        private List<Suivi> lesSuivis = new List<Suivi>();

        /// <summary>
        /// Ouverture de l'onglet Commandes de livres :
        /// appel des méthodes pour remplir le datagrid des commandes de livre et du combo "suivi"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            lesSuivis = controller.GetAllSuivis();
            gbxInfosCommandeLivre.Enabled = false;
            gbxEtapeSuivi.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbxInfosCommandeLivre_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfosCommandeLivreAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = true;
            gbxInfosCommandeLivre.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des informations de commande et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbxEtapeSuivi_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeLivre.Enabled = false;
            txbCommandesLivresNumRecherche.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des commandes et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEtapeSuiviAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
            gbxInfosCommandeLivre.Enabled = true;
            txbCommandesLivresNumRecherche.Enabled = true;
        }

        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesCommandesDocument">Liste des commandes d'un document</param>
        private void RemplirCommandesLivresListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesLivre.DataSource = lesCommandesDocument;
                dgvComLivre.DataSource = bdgCommandesLivre;
                dgvComLivre.Columns["id"].Visible = false;
                dgvComLivre.Columns["idLivreDvd"].Visible = false;
                dgvComLivre.Columns["idSuivi"].Visible = false;
                dgvComLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvComLivre.Columns["dateCommande"].DisplayIndex = 4;
                dgvComLivre.Columns["montant"].DisplayIndex = 1;
                dgvComLivre.Columns[5].HeaderCell.Value = "Date de commande";
                dgvComLivre.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvComLivre.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesLivre.DataSource = null;
            }
        }

        /// <summary>
        /// Mise à jour de la liste des commandes de livre
        /// </summary>
        private void AfficheReceptionCommandesLivre()
        {
            string idDocument = txbCommandesLivresNumRecherche.Text;
            lesCommandesDocument = controller.GetCommandesDocument(idDocument);
            RemplirCommandesLivresListe(lesCommandesDocument);
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesLivresNumRecherche.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCommandesLivresNumRecherche.Text));
                if (livre != null)
                {
                    AfficheReceptionCommandesLivre();
                    gbxInfosCommandeLivre.Enabled = true;
                    AfficheReceptionCommandesLivreInfos(livre);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }

        /// <summary>
        /// Affichage des informations du livre
        /// </summary>
        /// <param name="livre">Le livre</param>
        private void AfficheReceptionCommandesLivreInfos(Livre livre)
        {
            txbCommandeLivresTitre.Text = livre.Titre;
            txbCommandeLivresAuteur.Text = livre.Auteur;
            txbCommandeLivresIsbn.Text = livre.Isbn;
            txbCommandeLivresCollection.Text = livre.Collection;
            txbCommandeLivresGenre.Text = livre.Genre;
            txbCommandeLivresPublic.Text = livre.Public;
            txbCommandeLivresRayon.Text = livre.Rayon;
            txbCommandeLivresCheminImage.Text = livre.Image;
            string image = livre.Image;
            try
            {
                pictureBox1.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox1.Image = null;
            }
            AfficheReceptionCommandesLivre();
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des étapes de suivi correspondantes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEtapeSuivi_TextChanged(object sender, EventArgs e)
        {
            string etapeSuivi = lblEtapeSuivi.Text;
            RemplirCbxCommandeLivreLibelleSuivi(etapeSuivi);
        }

        /// <summary>
        /// Remplissage de la comboBox selon les étapes de suivi et le libelle correspondant
        /// </summary>
        /// <param name="etapeSuivi">Etapes de suivi possibles d'une commande de livre</param>
        private void RemplirCbxCommandeLivreLibelleSuivi(string etapeSuivi)
        {
            combocommandeetapelivre.Items.Clear();
            if (etapeSuivi == "livrée")
            {
                combocommandeetapelivre.Text = "";
                combocommandeetapelivre.Items.Add("réglée");
            }
            else if (etapeSuivi == "en cours")
            {
                combocommandeetapelivre.Text = "";
                combocommandeetapelivre.Items.Add("relancée");
                combocommandeetapelivre.Items.Add("livrée");
            }
            else if (etapeSuivi == "relancée")
            {
                combocommandeetapelivre.Text = "";
                combocommandeetapelivre.Items.Add("en cours");
                combocommandeetapelivre.Items.Add("livrée");
            }
        }

        /// <summary>
        /// Récupération de l'id de suivi d'une commande selon son libelle
        /// </summary>
        /// <param name="libelle">Libelle de l'étape de suivi d'une commande</param>
        /// <returns></returns>
        private string GetIdSuivi(string libelle)
        {
            List<Suivi> lesSuivisCommande = controller.GetAllSuivis();
            foreach (Suivi unSuivi in lesSuivisCommande)
            {
                if (unSuivi.Libelle == libelle)
                {
                    return unSuivi.Id;
                }
            }
            return null;
        }

        /// <summary>
        /// Affichage des informations de la commande sélectionnée 
        /// Masque le bouton "Modifier étape de suivi" si étape finale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeslivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvComLivre.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
            string libelle = row.Cells["Libelle"].Value.ToString();

            textlivrecommandenum.Text = id;
            textlivrecommandeexemplaire.Text = nbExemplaire.ToString();
            textlivrecommandemontant.Text = montant.ToString();
            datetimelivrecommande.Value = dateCommande;
            lblEtapeSuivi.Text = libelle;

            if (GetIdSuivi(libelle) == "00003")
            {
                combocommandeetapelivre.Enabled = false;
                btnetapesuivilivre.Enabled = false;
            }
            else
            {
                combocommandeetapelivre.Enabled = true;
                btnetapesuivilivre.Enabled = true;
            }

        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesLivre_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvComLivre.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesLivresListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'une commande de livre dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionCommandeLivreValider_Click(object sender, EventArgs e)
        {
            if (!textlivrecommandenum.Text.Equals("") && !textlivrecommandeexemplaire.Text.Equals("") && !textlivrecommandemontant.Text.Equals(""))
            {
                string id = textlivrecommandenum.Text;
                int nbExemplaire = int.Parse(textlivrecommandeexemplaire.Text);
                double montant = double.Parse(textlivrecommandemontant.Text);
                DateTime dateCommande = datetimelivrecommande.Value;
                string idLivreDvd = txbCommandesLivresNumRecherche.Text;
                string idSuivi = lesSuivis[0].Id;

                Commande commande = new Commande(id, dateCommande, montant);

                var idCommandeLivreExistante = controller.GetCommandes(id);
                var idCommandeLivreNonExistante = !idCommandeLivreExistante.Any();

                if (idCommandeLivreNonExistante)
                {
                    if (controller.CreerCommande(commande))
                    {
                        controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
                        MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                        AfficheReceptionCommandesLivre();
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de la commande existe déjà, veuillez saisir un nouveau numéro.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de livre dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierSuiviCommandeLivre_Click(object sender, EventArgs e)
        {
            string id = textlivrecommandenum.Text;
            int nbExemplaire = int.Parse(textlivrecommandeexemplaire.Text);
            double montant = double.Parse(textlivrecommandemontant.Text);
            DateTime dateCommande = datetimelivrecommande.Value;
            string idLivreDvd = txbCommandesLivresNumRecherche.Text;
            string idSuivi = GetIdSuivi(combocommandeetapelivre.Text);

            try
            {
                string libelle = combocommandeetapelivre.SelectedItem.ToString();

                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (MessageBox.Show("Voulez-vous modifier le suivi de la commande " + commandedocument.Id + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierSuiviCommandeDocument(commandedocument.Id, commandedocument.IdSuivi);
                    MessageBox.Show("L'étape de suivi de la commande " + id + " a bien été modifiée.", "Information");
                    AfficheReceptionCommandesLivre();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("La nouvelle étape de suivi de la commande doit être sélectionnée.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'une commande dans la base de données
        /// Si elle n'a pas encore été livrée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvComLivre.SelectedRows.Count > 0)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre.List[bdgCommandesLivre.Position];
                if (commandeDocument.Libelle == "en cours" || commandeDocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandeDocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.SupprimerCommandeDocument(commandeDocument);
                        AfficheReceptionCommandesLivre();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée, elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }

        #endregion

        #region Onglet CommandesDvd
        private readonly BindingSource bdgCommandesDvd = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Commandes de dvd :
        /// appel des méthodes pour remplir le datagrid des commandes de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            lesSuivis = controller.GetAllSuivis();
            dtpCommandeDvd.Value = DateTime.Now;
            gbxInfosCommandeDvd.Enabled = false;
            gbxEtapeSuiviDvd.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbxInfosCommandeDvd_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInfosCommandeDvdAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = true;
            gbxInfosCommandeDvd.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des informations de commande et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbxEtapeSuiviDvd_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeDvd.Enabled = false;
            txbCommandesDvdNumRecherche.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des commandes et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEtapeSuiviAnnulerDvd_Click(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
            gbxInfosCommandeDvd.Enabled = true;
            txbCommandesDvdNumRecherche.Enabled = true;
        }


        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesCommandesDocument">Liste des commandes d'un document</param>
        private void RemplirCommandesDvdListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesDvd.DataSource = lesCommandesDocument;
                dgvComsDvd.DataSource = bdgCommandesDvd;
                dgvComsDvd.Columns["id"].Visible = false;
                dgvComsDvd.Columns["idLivreDvd"].Visible = false;
                dgvComsDvd.Columns["idSuivi"].Visible = false;
                dgvComsDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvComsDvd.Columns["dateCommande"].DisplayIndex = 0;
                dgvComsDvd.Columns["montant"].DisplayIndex = 1;
                dgvComsDvd.Columns[5].HeaderCell.Value = "Date de commande";
                dgvComsDvd.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvComsDvd.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesDvd.DataSource = null;
            }
        }

        /// <summary>
        /// Mise à jour de la liste des commandes de dvd
        /// </summary>
        private void AfficheReceptionCommandesDvd()
        {
            string idDocument = txbCommandesDvdNumRecherche.Text;
            lesCommandesDocument = controller.GetCommandesDocument(idDocument);
            RemplirCommandesDvdListe(lesCommandesDocument);
        }

        /// <summary>
        /// Recherche les commandes concernant le dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesDvdNumRecherche.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandesDvdNumRecherche.Text));
                if (dvd != null)
                {
                    AfficheReceptionCommandesDvd();
                    gbxInfosCommandeDvd.Enabled = true;
                    AfficheReceptionCommandesDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }

        /// <summary>
        /// Affichage des informations du dvd
        /// </summary>
        /// <param name="dvd">Le dvd</param>
        private void AfficheReceptionCommandesDvdInfos(Dvd dvd)
        {
            txbCommandeDvdTitre.Text = dvd.Titre;
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdDuree.Text = dvd.Duree.ToString();
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdCheminImage.Text = dvd.Image;
            string image = dvd.Image;
            try
            {
                pictureBox2.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox2.Image = null;
            }
            AfficheReceptionCommandesDvd();
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des étapes de suivi correspondantes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSuiviEtapeDvd_TextChanged(object sender, EventArgs e)
        {
            string etapeSuivi = lblEtapeSuiviDvd.Text;
            RemplirCbxCommandeDvdLibelleSuivi(etapeSuivi);
        }

        /// <summary>
        /// Remplissage de la liste de suivi des commandes de dvd
        /// </summary>
        /// <param name="etapeSuivi">Etapes de suivi possibles d'une commande de Dvd</param>
        private void RemplirCbxCommandeDvdLibelleSuivi(string etapeSuivi)
        {
            combocommandesuividvd.Items.Clear();
            if (etapeSuivi == "livrée")
            {
                combocommandesuividvd.Text = "";
                combocommandesuividvd.Items.Add("réglée");
            }
            else if (etapeSuivi == "en cours")
            {
                combocommandesuividvd.Text = "";
                combocommandesuividvd.Items.Add("relancée");
                combocommandesuividvd.Items.Add("livrée");
            }
            else if (etapeSuivi == "relancée")
            {
                combocommandesuividvd.Text = "";
                combocommandesuividvd.Items.Add("en cours");
                combocommandesuividvd.Items.Add("livrée");
            }
        }

        /// <summary>
        /// Affiche les informations de la commande sélectionnée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvComsDvd.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
            string libelle = row.Cells["Libelle"].Value.ToString();

            textcommandenumdvd.Text = id;
            txbCommandexemplairedvd.Text = nbExemplaire.ToString();
            textcommandemontantdvd.Text = montant.ToString();
            dtpCommandeDvd.Value = dateCommande;

            lblEtapeSuiviDvd.Text = libelle;
            if (GetIdSuivi(libelle) == "00003")
            {
                combocommandesuividvd.Enabled = false;
                btnmodifcommandedvd.Enabled = false;
            }
            else
            {
                combocommandesuividvd.Enabled = true;
                btnmodifcommandedvd.Enabled = true;
            }
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesDvd_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvComsDvd.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'une nouvelle commande de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionCommandeDvdValider_Click(object sender, EventArgs e)
        {
            if (!textcommandenumdvd.Text.Equals("") && !txbCommandexemplairedvd.Text.Equals("") && !textcommandemontantdvd.Text.Equals(""))
            {
                string idLivreDvd = txbCommandesDvdNumRecherche.Text;
                string idSuivi = lesSuivis[0].Id;
                string id = textcommandenumdvd.Text;
                int nbExemplaire = int.Parse(txbCommandexemplairedvd.Text);
                double montant = double.Parse(textcommandemontantdvd.Text);
                DateTime dateCommande = dtpCommandeDvd.Value;

                Commande commande = new Commande(id, dateCommande, montant);

                var idCommandeExistante = controller.GetCommandes(id);
                var idCommandeNonExistante = !idCommandeExistante.Any();

                if (idCommandeNonExistante)
                {
                    if (controller.CreerCommande(commande))
                    {
                        controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
                        MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                        AfficheReceptionCommandesDvd();
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de la commande existe déjà, veuillez saisir un nouveau numéro.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionCommandeDvdModifierSuivi_Click(object sender, EventArgs e)
        {
            try
            {
                string idLivreDvd = txbCommandesDvdNumRecherche.Text;
                string idSuivi = GetIdSuivi(combocommandesuividvd.Text);
                string libelle = combocommandesuividvd.SelectedItem.ToString();
                string id = textcommandenumdvd.Text;
                int nbExemplaire = int.Parse(txbCommandexemplairedvd.Text);
                double montant = double.Parse(textcommandemontantdvd.Text);
                DateTime dateCommande = dtpCommandeDvd.Value;

                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (MessageBox.Show("Voulez-vous modifier le suivi de la commande " + commandedocument.Id + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierSuiviCommandeDocument(commandedocument.Id, commandedocument.IdSuivi);
                    AfficheReceptionCommandesDvd();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("La nouvelle étape de suivi de la commande doit être sélectionnée.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'une commande de dvd si elle n'a pas encore été livrée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvComsDvd.SelectedRows.Count > 0)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvd.List[bdgCommandesDvd.Position];
                if (commandeDocument.Libelle == "en cours" || commandeDocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandeDocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.SupprimerCommandeDocument(commandeDocument);
                        AfficheReceptionCommandesDvd();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }


        #endregion

        #region Onglet CommandesRevues

        private readonly BindingSource bdgAbonnementsRevue = new BindingSource();
        private List<Abonnement> lesAbonnementsRevue = new List<Abonnement>();

        /// <summary>
        /// Ouverture de l'onglet Commandes de revues :
        /// appel des méthodes pour remplir le datagrid des abonnements d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            groupcommanderevue.Enabled = false;
            dtpCommandeDvd.Value = DateTime.Now;
        }

        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesAbonnementsRevue">Liste des abonnements d'une revue</param>
        private void RemplirAbonnementsRevueListe(List<Abonnement> lesAbonnementsRevue)
        {
            if (lesAbonnementsRevue != null)
            {
                bdgAbonnementsRevue.DataSource = lesAbonnementsRevue;
                dgvAbonnRevue.DataSource = bdgAbonnementsRevue;
                dgvAbonnRevue.Columns["id"].Visible = false;
                dgvAbonnRevue.Columns["idRevue"].Visible = false;
                dgvAbonnRevue.Columns["titre"].Visible = false;
                dgvAbonnRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvAbonnRevue.Columns["dateCommande"].DisplayIndex = 0;
                dgvAbonnRevue.Columns["montant"].DisplayIndex = 1;
                dgvAbonnRevue.Columns[4].HeaderCell.Value = "Date de commande";
                dgvAbonnRevue.Columns[0].HeaderCell.Value = "Date de fin d'abonnement";
            }
            else
            {
                bdgAbonnementsRevue.DataSource = null;
            }
        }

        /// <summary>
        /// Affiche la liste des abonnements d'une revue
        /// </summary>
        private void AfficheReceptionAbonnementsRevue()
        {
            string idDocument = txbCommandesRevueNumRecherche.Text;
            lesAbonnementsRevue = controller.GetAbonnementRevue(idDocument);
            RemplirAbonnementsRevueListe(lesAbonnementsRevue);
        }

        /// <summary>
        /// Recherche d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesRevueNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesRevueNumRecherche.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbCommandesRevueNumRecherche.Text));
                if (revue != null)
                {
                    AfficheReceptionAbonnementsRevue();
                    groupcommanderevue.Enabled = true;
                    AfficheReceptionAbonnementsRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("Ce numéro de revue n'existe pas.");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de revue est obligatoire.");
            }
        }

        /// <summary>
        /// Affichage des informations d'une revue
        /// </summary>
        /// <param name="revue">La revue</param>
        private void AfficheReceptionAbonnementsRevueInfos(Revue revue)
        {
            txbCommandeRevueTitre.Text = revue.Titre;
            txbCommandeRevuePeriodicite.Text = revue.Periodicite;
            txbCommandeRevueDelai.Text = revue.DelaiMiseADispo.ToString();
            txbCommandeRevueGenre.Text = revue.Genre;
            txbCommandeRevuePublic.Text = revue.Public;
            txbCommandeRevueRayon.Text = revue.Rayon;
            txbCommandeRevueCheminImage.Text = revue.Image;
            string image = revue.Image;
            try
            {
                pictureBox4.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox4.Image = null;
            }
            AfficheReceptionAbonnementsRevue();
        }

        /// <summary>
        /// Affichage des informations de l'abonnement sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvAbonnRevue.Rows[e.RowIndex];
            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            DateTime dateFinAbonnement = (DateTime)row.Cells["DateFinAbonnement"].Value;

            textnumcommanderevue.Text = id;
            textmontantcommanderevue.Text = montant.ToString();
            datetimecommandeevue.Value = dateCommande;
            datetimefinabonnementrevue.Value = dateFinAbonnement;
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvAbonnRevue.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.Montant).ToList();
                    break;
                case "Date de fin d'abonnement":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateFinAbonnement).Reverse().ToList();
                    break;
            }
            RemplirAbonnementsRevueListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'un abonnement de revue dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionAbonnementRevueValider_Click(object sender, EventArgs e)
        {
            if (!textnumcommanderevue.Text.Equals("") && !textmontantcommanderevue.Text.Equals(""))
            {
                string idRevue = txbCommandesRevueNumRecherche.Text;
                string id = textnumcommanderevue.Text;
                double montant = double.Parse(textmontantcommanderevue.Text);
                DateTime dateCommande = datetimecommandeevue.Value;
                DateTime dateFinAbonnement = datetimefinabonnementrevue.Value;

                Commande commande = new Commande(id, dateCommande, montant);

                var idCommandeRevueExistante = controller.GetCommandes(id);
                var idCommandeRevueNonExistante = !idCommandeRevueExistante.Any();

                if (idCommandeRevueNonExistante)
                {
                    if (controller.CreerCommande(commande))
                    {
                        controller.CreerAbonnementRevue(id, dateFinAbonnement, idRevue);
                        MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                        AfficheReceptionAbonnementsRevue();
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de la commande existe déjà, veuillez saisir un nouveau numéro.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires", "Information");
            }
        }

        /// <summary>
        /// Retourne vrai si la date de parution est entre les 2 autres dates
        /// </summary>
        /// <param name="dateCommande">Date de la commande d'un abonnement à une revue</param>
        /// <param name="dateFinAbonnement">Date de fin d'abonnement à une revue</param>
        /// <param name="dateParution">Date de parution d'un exemplaire</param>
        /// <returns></returns>
        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (DateTime.Compare(dateCommande, dateParution) < 0 && DateTime.Compare(dateParution, dateFinAbonnement) < 0);
        }

        /// <summary>
        /// Vérifie si aucun exemplaire n'est rattaché à un abonnement de revue
        /// </summary>
        /// <param name="abonnement">L'abonnement</param>
        /// <returns></returns>
        public bool VerificationExemplaire(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplairesAbonnement = controller.GetExemplairesRevue(abonnement.IdRevue);
            bool datedeparution = false;
            foreach (Exemplaire exemplaire in lesExemplairesAbonnement.Where(exemplaires => ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, exemplaires.DateAchat)))
            {
                datedeparution = true;

            }
            return !datedeparution;
        }

        /// <summary>
        /// Suppression d'un abonnement de revue dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvAbonnRevue.SelectedRows.Count > 0)
            {
                Abonnement abonnement = (Abonnement)bdgAbonnementsRevue.Current;
                if (MessageBox.Show("Souhaitez-vous confirmer la suppression de l'abonnement " + abonnement.Id + " ?", "Confirmation de la suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    if (VerificationExemplaire(abonnement))
                    {
                        if (controller.SupprimerAbonnementRevue(abonnement))
                        {
                            AfficheReceptionAbonnementsRevue();
                        }
                        else
                        {
                            MessageBox.Show("Une erreur s'est produite.", "Erreur");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cet abonnement contient un ou plusieurs exemplaires, il ne peut donc pas être supprimé.", "Information");
                    }
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }


        #endregion

        private void label53_Click(object sender, EventArgs e)
        {

        }

        
    }
}
