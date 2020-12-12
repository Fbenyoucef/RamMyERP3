using Microsoft.AspNetCore.Razor.TagHelpers;
using MyErp.MyTagHelpers;
using System.Collections.Generic;

namespace RamMyERP3.Helpers
{
    public class NavigateCustomeTagHelper : TagHelper
    {
        // Initialiser la fonction
        public string Fonction { get; set; } = "";

        public List<bouton> BoutonsAction { get; set; }

        // Initialiser la valeur du Breadcrumb1
        public string Breadcrumb1 { get; set; }

        // Initialiser la valeur du Breadcrumb2
        public string Breadcrumb2 { get; set; }

        // Définir s'il faut afficher les boutons d'action
        public bool AfficherBoutonsAction { get; set; } = true;

        public string ParametresActionsRetour { get; set; }

        // Définir s'il faut afficher tout les boutons d'action
        public bool AfficherAjaxBoutonAction { get; set; } = false;
        public string FonctionAjouterAjax { get; set; }
        public string FonctionEnregistrerAjax { get; set; }

        //<span class="icofont icofont-download-alt" style="color: white;"></span>
        public bool AfficherBoutonTelecharger { get; set; } = false;
        public string FonctionTelecharger { get; set; }


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
            if (Fonction == "Lister" && AfficherBoutonsAction && !AfficherAjaxBoutonAction)
            {
                html = html +
                    // Afficher le bouton Ajouter
                    $"<a class=\"btn btn-mat btn-primary\"  style=\"border-radius: 5px; width: 140px;\" href=\"Ajouter\"><i class=\"fa fa-plus\"></i> Ajouter</a>";
            }

            if (Fonction == "Lister" && AfficherAjaxBoutonAction)
            {
                html = html +
                       // Afficher le bouton Retour
                       "<a class=\"btn btn-mat btn-secondary checkclose\" id=\"Retourbtn\" style=\"border-radius: 5px; margin-right: 5px; ! important; width: 140px;\" href=\"../\"><i class=\"fa fa-reply\"></i>Retour</a>" +
                       // Afficher le bouton Ajouter
                       "<a class=\"btn btn-mat btn-primary\" style=\"border-radius: 5px;color: white; width: 140px; margin-right: 5px;\" onclick=\"" + FonctionAjouterAjax + "\"><i class=\"fa fa-plus\"></i> Ajouter</a>" +
                       // Afficher le bouton Valider
                       "<button class=\"btn btn-mat btn-success\" style=\"border-radius: 5px;color: white; width: 140px; margin-right: 5px;\" onclick=\"" + FonctionEnregistrerAjax + "\"><i class=\"fa fa-save\"></i> Enregistrer</button>";
            }

            // Gérer la fonction Ajouter
            if (Fonction == "Ajouter" && AfficherBoutonsAction)
            {
                html = html +
                    // Afficher le bouton Retour
                    "<a class=\"btn btn-mat btn-secondary checkclose\" id=\"Retourbtn\" style=\"border-radius: 5px; margin-right: 5px; ! important; width: 140px;\" href=\"Lister" + (!string.IsNullOrWhiteSpace(ParametresActionsRetour) ? "?" + ParametresActionsRetour : "") + "\"><i class=\"fa fa-reply\"></i> Retour    </a>" +
                    // Afficher le bouton Valider
                    "<button  form=\"Ajouter\" class=\"btn btn-mat btn-success\" style=\"border-radius: 5px; margin-right: 5px; width: 140px;\"  type=\"submit\"><i class=\"fa fa-save\"></i> Enregistrer</button>";
            }

            // Gérer la fonction Modifier
            if (Fonction == "Modifier" && AfficherBoutonsAction)
            {
                html = html +
                    // Afficher le bouton Retour
                    $"<a class=\"btn btn-mat btn-secondary checkclose\" id=\"Retourbtn\" style=\"border-radius: 5px; margin-right: 5px; ! important; width: 140px;\" href=\"../Lister" + (!string.IsNullOrWhiteSpace(ParametresActionsRetour) ? "?" + ParametresActionsRetour : "") + "\"><i class=\"fa fa-reply\"></i> Retour</a>" +

                    // Afficher le bouton Valider
                    "<button form=\"Modifier\" class=\"btn btn-mat btn-success\"  style=\"border-radius: 5px; width: 140px;\" type=\"submit\"><i class=\"fa fa-save\"></i> Enregistrer</button>";
            }

            // Gérer la fonction Afficher
            if (Fonction == "Afficher" && AfficherBoutonsAction)
            {
                html = html +
                    // Afficher le bouton Retour
                    $"<a class=\"btn btn-mat btn-secondary checkclose\" id=\"Retourbtn\"  style=\"border-radius: 5px; margin-right: 5px; width: 140px;\" href=\"../Lister" + (!string.IsNullOrWhiteSpace(ParametresActionsRetour) ? "?" + ParametresActionsRetour : "") + "\"><i class=\"fa fa-reply\"></i> Retour</a>";
            }

            if (AfficherBoutonTelecharger)
            {
                html = html +
                       "<a class=\"btn btn-mat btn-success\" style=\"border-radius: 5px;color: white; width: 140px; margin-right: 5px;\" onclick=\"" + FonctionTelecharger + "\"><i class=\"icofont icofont-download-alt\"></i> Telecharger</a>";
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
                                        "<a class=\"checkclose\" href=\"/\"><i class=\"feather icon-home\"></i></a>" +
                                    "</li>" +

                                    // Afficher le lien parent
                                    "<li class=\"breadcrumb-item\">" +
                                        $"<a class=\"checkclose\" href=\"/" + "Reference/Index" + "\">" + Breadcrumb1 + "</a>" +
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
