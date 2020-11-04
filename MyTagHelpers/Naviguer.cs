/*
 * Type de fichier  : MyTagHelpers
 * Nom du fichier   : Naviguer.cs
 * Création         : 31/12/2019 - Mehdi Califano 
 * Modification     : 01/28/2019 - Mourad Yamani
 */

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace MyErp.MyTagHelpers
{
    public class NaviguerTagHelper : TagHelper
    {
        // Initialiser la fonction Lister
        public bool Lister { get; set; } = false;

        // Initialiser la fonction Ajouter
        public bool Ajouter { get; set; } = false;

        // Initialiser la fonction Modifier
        public bool Modifier { get; set; } = false;

        // Initialiser la fonction Afficher
        public bool Afficher { get; set; } = false;
        // Initialiser Id Valeur
        public string Id { get; set; }
        public List<bouton> BoutonsAction { get; set; }

        // Redéfinir la méthode process
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Afficher la barre de navigation
            string barre = AfficherBarreNavigation() +
                            GenereBoutons();
            output.Content.AppendHtml(barre);
        }

        // Générer la barre de navigation
        string AfficherBarreNavigation()
        {
            string html="";

            // Gérer la fonction Lister
            if (Lister)
            {
                html =
                    $"<a class=\"btn btn-mat btn-primary\" href=\"Ajouter\"> <i class=\"fas fa-plus\"></i> Ajouter</a> ";
                
            }

            // Gérer la fonction Ajouter
            if (Ajouter)
            {
                html = // Afficher le bouton Retour
                    "<a class=\"btn btn-mat btn-secondary\" href=\"Lister\"><i class=\"fas fa-reply\"></i> Retour</a> " +
                    // Afficher le bouton Valider
                    "<button form=\"Ajouter\" class=\"btn btn-mat btn-success\" type=\"submit\"><i class=\"fas fa-save\"></i> Valider</button> ";
            }

            // Gérer la fonction Modifier
            if (Modifier)
            {
                html = // Afficher le bouton Retour
                    "<a class=\"btn btn-mat btn-secondary\" href=\"../Lister\"><i class=\"fas fa-reply\"></i> Retour</a> " +
                    // Afficher le bouton Valider
                    "<button form=\"modifier\" class=\"btn btn-mat btn-success\" type=\"submit\"><i class=\"fas fa-save\"></i> Valider</button> ";

            }

            // Gérer la fonction Modifier
            if (Afficher)
            {
                html = // Afficher le bouton Retour
                    "<a class=\"btn btn-mat btn-secondary\" href=\"../Lister\"><i class=\"fas fa-reply\"></i> Retour</a> " +
                    // Afficher le bouton Modifier
                    $"<a form=\"modifier\" class=\"btn btn-mat btn-success\" href=\"../Modifier/{Id}\"><i class=\"fas fa-edit\"></i> Modifier</a> ";
                    //                < !--Afficher le bouton Modifier-- >
                    //< a class="button btn btn-primary margin-10" asp-action="Modifier" asp-route-id="@Model.Id"><span class="fas fa-edit"></span> Modifier</a>
            }

            return html;
        }

        #region Généré les Boutons D'action
        string GenereBoutons()
        {
            if (BoutonsAction == null) return null;
            string html = "";
            foreach (var bouton in BoutonsAction)
            {
                string href = "";
                if (bouton.Url != null) href = $"href=\"{bouton.Url}\"";
                html += $"<a id=\"{bouton.Id}\" class=\"btn btn-mat {bouton.Style}\" {bouton.Data} title=\"{bouton.Titre}\" data-id=\"{bouton.Id}\" {bouton.Data} {href}>" +
                        $"<i class=\"{bouton.Icon}\"></i> " +
                        bouton.Titre+
                    "</a>";
            }
            return html;
        }
        #endregion Généré les Boutons D'action
    }
}

