using Microsoft.AspNetCore.Razor.TagHelpers;
using RamMyERP3.Helpers.Entite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Helpers
{
    public class NaviguerTagHelper : TagHelper
    {
        // Initialiser la fonction
        public string Fonction { get; set; } = "";

        // Initialiser Id Valeur
        public string Id { get; set; }

        public List<bouton> BoutonsAction { get; set; }

        // Initialiser la valeur du Breadcrumb1
        public string Breadcrumb1 { get; set; }

        // Initialiser la valeur du Breadcrumb2
        public string Breadcrumb2 { get; set; }

        // Définir s'il faut afficher les boutons d'action
        public bool AfficherBoutonsAction { get; set; } = true;

        public string ParametresActionsRetour { get; set; }

        // Redéfinir la méthode process
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Afficher la barre de navigation
            string barreNavigation = AfficherBarreNavigation();

            output.Content.AppendHtml(barreNavigation);
        }

        // Générer la barre de navigation
        string AfficherBarreNavigation()
        {
            string html =
                // Afficher la barre de Navigation
                $"<div class=\"page-header\">" +
                    "<div class=\"card\">" +
                        "<div class=\"card-block caption-breadcrumb\">" +
                            // Afficher les boutons
                            "<div class=\"breadcrumb-header\">";



            // Gérer la fonction Lister
            if (Fonction == "Lister" && AfficherBoutonsAction)
            {
                html = html +
                    // Afficher le bouton Ajouter
                    $"<a class=\"btn btn-mat btn-primary\" href=\"CreationRam\" asp-route-id=2><i class=\"fa fa-plus\"></i> Ajouter</a>";
            }

            // Gérer la fonction Ajouter
            if (Fonction == "Ajouter" && AfficherBoutonsAction)
            {
                html = html +
                    // Afficher le bouton Retour
                    "<a class=\"btn btn-mat btn-secondary\"  href=\"Lister" + (!string.IsNullOrWhiteSpace(ParametresActionsRetour) ? "?" + ParametresActionsRetour : "") + "\"><i class=\"fa fa-reply\"></i> Retour</a>" +
                    // Afficher le bouton Valider
                    "<button form=\"Ajouter\" class=\"btn btn-mat btn-success bg-primary \" id=\"Ajout\" type=\"Ajouter\"><i class=\"fa fa-save\"></i> Valider</button>"; //+
                //// Afficher le bouton signature 
                //$"<a  id=\"Signature\" class=\"btn btn-mat btn-success\"  data-id = \"@item.ID\" data-toggle = \"modal\" data-target = \"#SignerRam\" href =\"../Modifier/{Id}\"><i class=\"fa fa-edit\"></i> Signer</a> ";

            }

            // Gérer la fonction Modifier
            if (Fonction == "Modifier" && AfficherBoutonsAction)
            {
                html = html +
                    // Afficher le bouton Retour
                    $"<a class=\"btn btn-mat btn-secondary\"  id=\"RetourDetail\" href=\"Details" + (!string.IsNullOrWhiteSpace(ParametresActionsRetour) ? "?" + ParametresActionsRetour : "") + "\"><i class=\"fa fa-reply\"></i> Retour</a>" +

                    // Afficher le bouton Valider
                    "<button form=\"Modifier\" class=\"btn btn-mat btn-success bg-primary \" id=\"Ajout\" type=\"submit\"><i class=\"fa fa-save\"></i> Valider</button>";

            }

            // Gérer la fonction Afficher
            if (Fonction == "Afficher" && AfficherBoutonsAction)
            {
                html = html +
                    //  href =\"./Lister"
                    // Afficher le bouton Retour
                    $"<a class=\"btn btn-mat btn-secondary\" id=\"Retour\" href=\"Lister" + (!string.IsNullOrWhiteSpace(ParametresActionsRetour) ? " ?" + ParametresActionsRetour : "") + "\"><i class=\"fa fa-reply\"></i> Retour</a>" +

                // Afficher le bouton Modifier
                //"<button id=\"Modification\" class=\"btn btn-mat btn-success bg-primary \" asp-controller=\"Rams\" asp-action=\"Modifier\" type=\"submit\"><i class=\"icofont icofont-ui-edit\"></i> Modifier</button>";

                $"<a  id=\"Modification\" class=\"btn btn-mat btn-success\" href=\"../Modifier/{Id}\"><i class=\"fa fa-edit\"></i> Modifier</a> " +
                // Afficher le bouton signature 
                $"<a  id=\"Signature\" class=\"btn btn-mat btn-success\"  data-id = \"@item.ID\" data-toggle = \"modal\" data-target = \"#SignerRam\" href =\"../Modifier/{Id}\"><i class=\"fa fa-edit\"></i> Signer</a> ";
            }

            if (AfficherBoutonsAction && BoutonsAction != null)
            {
                html = html + AfficherBoutons();
            }
            html = html +
                            "</div>" +
                            // Afficher les Breadcrumbs
                            "<div class=\"page-header-breadcrumb\">" +
                                "<ul class=\"breadcrumb-title\">" +
                                    // Afficher le logo Accueil
                                    "<li class=\"breadcrumb-item\">" +
                                        "<a href=\"/\"><i class=\"feather icon-home\"></i></a>" +
                                    "</li>" +

                                    // Afficher le lien parent
                                    "<li class=\"breadcrumb-item\">" +
                                        $"<a href=\"/" + Breadcrumb1 + "/" + Breadcrumb2 + "\">" + Breadcrumb1 + "</a>" +
                                    "</li>" +

                                    // Afficher le lien fils
                                    "<li class=\"breadcrumb-item\">" +
                                        "<a>" + Breadcrumb2 + "</a>" +
                                    "</li>" +
                                "</ul>" + "</div>" + "</div>" + "</div>" + "</div>";

            return html;
        }

        #region Afficher les Boutons D'action
        string AfficherBoutons()
        {
            if (BoutonsAction == null) return null;
            string html = "";
            foreach (var bouton in BoutonsAction)
            {
                string href = "";
                if (bouton.Url != null) href = $"href=\"{bouton.Url}\"";
                html += $"<a id=\"{bouton.Id}\" class=\"btn btn-mat {bouton.Style}\" {bouton.Data} title=\"{bouton.Titre}\" data-id=\"{bouton.Id}\" {bouton.Data} {href}>" +
                        $"<i class=\"{bouton.Icon}\"></i> " +
                        bouton.Titre +
                    "</a>";
            }
            return html;
        }

        #endregion Généré les Boutons D'action
    }
}

