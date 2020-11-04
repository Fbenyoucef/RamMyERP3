/*
 * Type de fichier  : MyTagHelpers
 * Nom du fichier   : Lister.cs
 * Création         : 16/12/2019 - Mourad Yamani 
 * Modification     : 18/12/2019 - Mourad Yamani
 * Modification     : 23/08/2019 - Mourad Yamani
 * Modification     : 31/12/2019 - Mehdi Califano (Mettre au propre)
 * Modification     : 01/01/2020 - Mehdi Califano (Intégrer la suppression d'un élément)
 * Modification     : 24/02/2020 - Mourad YAMANI Changement du template adminty
 */

using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MyErp.MyTagHelpers
{
    public class ListerTagHelper : TagHelper
    {
        // Initialiser les items
        [HtmlAttributeName("Items")]
        public IEnumerable<object> Items { get; set; }
        [HtmlAttributeName("TypeClass")]
        public Type TypeClass { get; set; }
        public List<bouton> BoutonsAction { get; set; }
        //
        public bool? BtnAfficher { get; set; }
        public bool? BtnModifier { get; set; }
        public bool? BtnSupprimer { get; set; }
        public string[] DonnesInfo { get; set; }
        // Redéfinir la méthode process
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Afficher le datatable
            AfficherTable(output);
            // Gérer la suppression des éléments du datatable
            if (BtnSupprimer == null) output.Content.AppendHtml(Supprimer());
        }

        #region Afficher le datatable
        // Afficher le datatable
        private void AfficherTable(TagHelperOutput output)
        {
            // Initialiser l'affichage de la table
            string html = "<table class=\"table table-bordered tablepress tablepress-id-demo dataTable\" id=\"dataTable\">";

            output.Content.AppendHtml(html);

            // Récupérer les attributs contenant les annotations lister et triés par position
            var proprietes = GetItemProperties();
            //.Where(c => c.CustomAttributes.Any(a => a.AttributeType.Name == nameof(ListerAttribute)))
            //.OrderBy(s => s.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == nameof(ListerAttribute))
            //    .NamedArguments.FirstOrDefault(t => t.MemberName == nameof(ListerAttribute.Position)).TypedValue.Value);

            // Afficher l'entête de la table
            AfficherTableEntete(output, proprietes);

            // Afficher le contenu de la table
             AfficherTableContenu(output, proprietes);
        }

        // Afficher l'entête du datatable
        private void AfficherTableEntete(TagHelperOutput output, List<PropertyInfo> proprietes)
        {
            // Afficher les colonnes
            output.Content.AppendHtml("<thead>");
            output.Content.AppendHtml("<tr>");

            // Boucler sur les colonnes
            foreach (var propriete in proprietes)
            {
                //var InfoListe = propriete.GetCustomAttributes<ListerAttribute>().First();
                //var key = propriete.GetCustomAttributes<KeyAttribute>();
                //if (key.Count() == 0 && !InfoListe.Cle && !InfoListe.Cacher)
                //{
                var name = GetPropertyName(propriete);
                output.Content.AppendHtml($"<th>{name}</th>");
                // }
            }

            // Afficher la colonne Action
            output.Content.AppendHtml("<th></th>");
            output.Content.AppendHtml("</tr>");
            output.Content.AppendHtml("</thead>");
        }

        // Afficher le contenu du datable
        private void AfficherTableContenu(TagHelperOutput output, List<PropertyInfo> proprietes)
        {
            // Afficher le contenu
            output.Content.AppendHtml("<tbody>");

            // Boucler sur les lignes
            foreach (var item in Items)
            {
                string htmlAction = "";
                output.Content.AppendHtml("<tr>");

                // Boucler sur les colonnes
                foreach (var propriete in proprietes)
                {
                   // var InfoListe = propriete.GetCustomAttributes<ListerAttribute>().First();
                    var key = propriete.GetCustomAttributes<KeyAttribute>();

                    // Tester si la propriété a un ID avec l'annotation [Key]
                    var value = GetPropertyValue(propriete, item);
                    //if (key.Count() == 0 && !InfoListe.Cle && !InfoListe.Cacher)
                    //{
                    //    output.Content.AppendHtml($"<td>{value}</td>");
                    //}
                    //else if (key.Count() > 0 || InfoListe.Cle)
                    //{
                    //    //var Data = JsonConvert.SerializeObject(item, jsonSerializerSettings);
                    // Préparer les actions Afficher, Modifier, Supprimer
                    output.Content.AppendHtml($"<td>{value}</td>");
                    htmlAction = $"<th>{HtmlAction(value.ToString(), SetData(item))}</th>";
                    //}
                }

                output.Content.AppendHtml(htmlAction);
                output.Content.AppendHtml("</tr>");
            }
            output.Content.AppendHtml("</tbody>");
            output.Content.AppendHtml("</table>");
            //output.Content.AppendHtml("</div></div></div></div></div></div></div></div>");
        }
        #endregion Afficher le datatable

        #region Gérer les propriétés
        private List<PropertyInfo> GetItemProperties()
        {
            // Récupérer la liste des type de donnée
          //  var listeType = TypeClass.GetType();

            Type itemType;

            // Tester s'il s'agit d'un type générique
       
                //itemType = listeType.GetGenericArguments().First();
                return TypeClass.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
          

            return null;
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

        private object GetPropertyValue(PropertyInfo property, object instance)
        {
            //var InfoListe = property.GetCustomAttributes<ListerAttribute>().First();
            //var attributeDisp = property.GetCustomAttributes<DisplayFormatAttribute>();
            var b = property.GetValue(instance);
          //  bool isNumeric = IsNumeric(b);
           // var key = property.GetCustomAttributes<KeyAttribute>();

            // Gérer le format d'affichage DisplayFormat
            //if (attributeDisp.Count() == 1)
            //{
            //    string format = attributeDisp.First().DataFormatString;
            //    if (key.Count() == 0 && !InfoListe.Cle)
            //        return string.Format(format, b);
            //    if (isNumeric)
            //        return $"<a class=\"right-elm\">{string.Format(format, b)}</a>";
            //    else
            //        return string.Format(format, b);
            //}

            //if (InfoListe != null && b != null && (b.GetType() == typeof(bool) || b.GetType() == typeof(string)))
            //{
            //    string href = "";
            //    if (InfoListe.IsUrl) href = $"href =\"{InfoListe.Url}{b.ToString()}\"";

            //    // Afficher les icons pour les champs bool true ou false
            //    if (InfoListe.IconeOk != string.Empty && b != null && b.GetType() == typeof(bool))
            //    {
            //        if ((bool)b)
            //        {
            //            return HtmlIcon(InfoListe.IconeOk, href);
            //        }
            //        else
            //        {
            //            return HtmlIcon(InfoListe.IconeKo, href);
            //        }
            //    }

            //    //les icon pour afficher les icon au lieu text 
            //    if (InfoListe.IconeOk != null && b.GetType() == typeof(string))
            //    {
            //        if (InfoListe.IconeOk != string.Empty && b.ToString() != string.Empty)
            //        {
            //            return HtmlIcon(InfoListe.IconeOk, href);
            //        }
            //        else
            //        {
            //            return HtmlIcon(InfoListe.IconeKo, href);
            //        }
            //    }
            //}


            // Afficher les colonnes de type nombre
            //if (InfoListe != null && b != null && isNumeric && key.Count() == 0 && InfoListe.Cle == false)
            //{
            //    return $"<a class=\"right-elm\">{b}</a>";
            //}

            // Afficher les colonne de type photo
            //if (InfoListe.CheminPhoto != string.Empty && InfoListe.CheminPhoto != null)
            //{
            //    var img = property.GetValue(instance);
            //    if (img != null && img.ToString() != string.Empty)
            //    {
            //        return $"<img class=\"img-radius img-40\" src=\"{InfoListe.CheminPhoto}{img.ToString()}\"/>";
            //    }
            //}

            var obj = property.GetValue(instance);
            //if (obj != null && InfoListe.DisplayChamp != null)
            //{
            //    //pour afficher des colonne de table de referance
            //    if (obj.GetType().GetProperties().Length > 0 && obj.GetType().GetProperty(InfoListe.DisplayChamp) != null)
            //    {
            //        var x = obj.GetType().GetProperty(InfoListe.DisplayChamp).GetValue(obj, null);
            //        return x;
            //    }
            //    //pour afficher les collection
            //    if (obj.GetType().GetProperties().Length > 0)
            //    {
            //        string x = "";

            //        foreach (var propertyInfo in (IEnumerable)obj)
            //        {
            //            foreach (var PInfo in propertyInfo.GetType().GetProperties())
            //            {
            //                if (PInfo.PropertyType == InfoListe.TypeObjet)
            //                {
            //                    var obj2 = PInfo.GetValue(propertyInfo);
            //                    if (obj2.GetType().GetProperties().Length > 0 && obj2.GetType().GetProperty(InfoListe.DisplayChamp) != null)
            //                    {
            //                        var value = obj2.GetType().GetProperty(InfoListe.DisplayChamp).GetValue(obj2, null);
            //                        x += $"<badg class=\"badge badge-primary\">{value}</badg>";
            //                    }
            //                }
            //            }
            //        }
            //        return x;

            //    }
            //}
            var test = property.GetValue(instance);
            return property.GetValue(instance);
        }
        #endregion Gérer les propriétés

        string HtmlIcon(string ClassIcon, string url)
        {
            return $"<a class=\"centericon\" target=\"_blank\" {url}>" +
                   $"<i class=\"{ClassIcon} icon-large \" ></i>" +
                   "</a>";
        }
        string HtmlAction(string id, string data)
        {
            string AfficherAction = "Afficher/" + id;
            string ModifierAction = "Modifier/" + id;
            string SupprimerAction = "Supprimer/" + id;
            string action = $"<div class=\"btn-group\" role=\"group\" aria-label=\"Action\" style =\"float: left;\" >";

            if (BtnAfficher == null)
            {
                action += $"<a class=\"btn btn-info btn-mat\" title=\"Afficher\" data-id=\"{id}\" {data} href=\"{AfficherAction}\">" +
                          "<i class=\"fas fa-info-circle mt-0\"></i>" +
                          "</a>";
            }
            if (BtnModifier == null)
            {
                action += $"<a class=\"btn btn-primary btn-mat\" title=\"Modifier\" data-id=\"{id}\" {data} href=\"{ModifierAction}\">" +
                          "<i class=\"fas fa-edit\"></i>" +
                          "</a>";
            }
            if (BtnSupprimer == null)
            {
                action += $"<a class=\"btn btn-danger btn-mat\" data-toggle=\"modal\" data-id=\"{id}\" {data} data-target=\"#Supprimer\" id=\"supprimer\" href=\"{SupprimerAction}\">" +
                          "<i class=\"fas fa-trash-alt\"></i>" +
                          "</a>";
            }
            action += GenereBoutons(id, data);
            action += "</div>";

            return action;
        }

        #region Supprimer les éléments du datatable
        // Supprimer les éléments du datatable
        string Supprimer()
        {
            // Supprimer un élément
            string html =
                $"<div class=\"modal fade\" id=\"Supprimer\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"exampleModalLabel\" aria-hidden=\"true\">" +
                    "<div class=\"modal-dialog\" role=\"document\">" +
                        "<div class=\"modal-content\">" +
                            "<div class=\"modal-header\">" +
                                "<h5 class=\"modal-title\" id=\"exampleModalLabel\">Supprimer l'élément sélectionné</h5>" +
                                "<button class=\"close\" type=\"button\" data-dismiss=\"modal\" aria-label=\"Close\">" +
                                    "<span aria-hidden=\"true\">×</span>" +
                                "</button>" +
                            "</div>" +

                            // Afficher le texte du formulaire
                            "<div class=\"modal-body\">Voulez-vous supprimer cet élément ?</div>" +

                            // Afficher le footer du formulaire
                            "<div class=\"modal-footer\">" +
                                // Afficher le bouton Annuler
                                "<button class=\"btn btn-secondary btn-mat\" type=\"button\" data-dismiss=\"modal\">Annuler</button>" +

                                // Afficher le bouton Supprimer
                                "<a class=\"btn btn-danger btn-mat\" id=\"confirmer\">Supprimer</a>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>";

            return html;
        }
        #endregion Supprimer les éléments du datatable
        #region Généré les Boutons D'action
        string GenereBoutons(string id, string data)
        {
            if (BoutonsAction == null) return null;
            string html = "";
            foreach (var bouton in BoutonsAction)
            {
                string href = "";
                if (bouton.Url != null) href = $"href=\"{bouton.Url}/{id}\"";
                html += $"<a id=\"{bouton.Id}\" class=\"btn {bouton.Style} btn-mat\" {bouton.Data} title=\"{bouton.Titre}\" data-id=\"{id}\" {data} {href}>" +
                        $"<i class=\"{bouton.Icon} mt-0\"></i>" +
                        "</a>";
            }
            return html;
        }
        #endregion Généré les Boutons D'action

        string SetData(object obj)
        {
            if (DonnesInfo == null || DonnesInfo.Length == 0) return null;
            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (var data in DonnesInfo)
            {
                list.Add(data, obj.GetType().GetProperty(data).GetValue(obj, null).ToString());
            }
            string Json = JsonConvert.SerializeObject(list);
            return "data-obj='" + Json + "'";
        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

    }

    public class bouton
    {
        public string Titre { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Id { get; set; }
        public string Style { get; set; }
        public string Data { get; set; }
    }
}