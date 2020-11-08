/*
 * Type de fichier  : MyTagHelpers
 * Nom du fichier   : Lister.cs
 * Création         : 16/12/2019 - Mourad Yamani 
 * Modification     : 18/12/2019 - Mourad Yamani
 * Modification     : 23/08/2019 - Mourad Yamani
 * Modification     : 31/12/2019 - Mehdi Califano (Mettre au propre)
 * Modification     : 01/01/2020 - Mehdi Califano (Intégrer la suppression d'un élément)
 */

using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MyErp.MyTagHelpers
{
    public class TableDynamiqueTagHelper : TagHelper
    {
        // Initialiser les items
        public IEnumerable<object> Items { get; set; }
        // Redéfinir la méthode process

        public async override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var proprietes = GetItemProperties();
            // Afficher le datatable
            AfficherTableEntete(output, proprietes);

        }



        private void AfficherTableEntete(TagHelperOutput output, PropertyInfo[] proprietes)
        {
            string html = "<table class=\"table table-bordered tablepress tablepress-id-demo dataTable\" id=\"dataTable\">";
            output.Content.AppendHtml(html);
            // Afficher les colonnes
            output.Content.AppendHtml("<thead>");
            output.Content.AppendHtml("<tr>");

            // Boucler sur les colonnes
            foreach (var propriete in proprietes)
            {
                var InfoListe = propriete.GetCustomAttributes<ListerAttribute>().First();
                var key = propriete.GetCustomAttributes<KeyAttribute>();
                if (key.Count() == 0 && !InfoListe.Cle && !InfoListe.Cacher)
                {
                    var name = GetPropertyName(propriete);
                    output.Content.AppendHtml($"<th>{name}</th>");
                }
            }

            // Afficher la colonne Action
            output.Content.AppendHtml("<th></th>");
            output.Content.AppendHtml("</tr>");
            output.Content.AppendHtml("</thead>");
            output.Content.AppendHtml("</table>");
        }
        private string GetPropertyName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                return attribute.Name;
            }

            return property.Name;
        }

        private PropertyInfo[] GetItemProperties()
        {
            // Récupérer la liste des type de donnée
            var listeType = Items.GetType();

            Type itemType;

            // Tester s'il s'agit d'un type générique
            if (listeType.IsGenericType)
            {
                itemType = listeType.GetGenericArguments().First();
                var x = itemType.GetProperties();
                return x;
            }

            return new PropertyInfo[] { };
        }
    }

}