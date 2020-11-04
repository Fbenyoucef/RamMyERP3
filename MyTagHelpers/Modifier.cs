/*
 * Type de fichier  : MyTagHelpers
 * Nom du fichier   : ModifierTagHelper.cs
 * Création         : 16/12/2019 - Mourad Yamani 
 * Modification     : 18/12/2019 - Mourad Yamani
 * Modification     : 23/08/2019 - Mourad Yamani
 */
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyErp.MyTagHelpers
{
    public class ModifierTagHelper : TagHelper
    {
        [HtmlAttributeName("Items")]
        //Le model
        public object Items { get; set; }
        //Id pour JavaScript
        public string Idjs { get; set; } = "";
        //le nom de la class CSS
        public string StyleInfo { get; set; } = "table";
        //le nom de la controlleur
        public string Controller { get; set; }

        // Initialiser le chemin de la photo
        public string Photo { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Afficher la carte (Photo)
            output.Content.AppendHtml(AfficherCartePhoto());

            //creation la table
            output.TagName = "div";
            output.Attributes.Add("class", "dataTables_wrapper dt-bootstrap4");
            output.Attributes.Add("id", "dataTable_wrapper");
            //sélectionner les attributs contient annotation lister et trié par position.
            var props = GetItemProperties()
                .Where(c =>c.CustomAttributes.Any(a => a.AttributeType.Name ==nameof(ModifierAttribute)))
                .OrderBy(s =>s.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name ==nameof(ModifierAttribute))
                    .NamedArguments.FirstOrDefault(t => t.MemberName== nameof(ModifierAttribute.Position)).TypedValue.Value);
            //Pour Generer l'entête de la table
            TableHeader(output, props);
            //pour generer le contenue de la table
            //TableBody(output, props);
        }        

        #region Afficher la carte
        // Afficher la carte (Photo)
        string AfficherCartePhoto()
        {
            // Initialiser la chaine Html
            string html =
                $"<div style = \"text-align: center;position: relative;\" class=\"pt-3 mt-3\">" +
                    "<img class=\"border carte-photo\" src=";

            if (Photo == null)
            {
                html = html + "\"/img/Societe/not_found.jpg\" />";
            }
            else
            {
                html = html + $"\"/img/Societe/{Photo}\" />";
            }

            html = html +
                $"<div class=\"updatepic\"><i class=\"fas fa-upload\" style=\"color: #000;text-align: center;margin: 27%;font-size: 3em;\"></i></div>" +
                "</div>";

            //+
            //    "<input asp-for=\"Logo\" type=\"file\" Class=\"upic\" />" +
            //    "<input id = \"files\" type=\"file\" Class=\"upic\" />";

            return html;
        }
        #endregion Afficher la carte

        private void TableHeader(TagHelperOutput output, IOrderedEnumerable<PropertyInfo> props)
        {
            output.Content.AppendHtml("<nav>");
            output.Content.AppendHtml("<div class=\"nav nav-tabs\" id=\"nav-tab\" role=\"tablist\">");
            string tab = null;
            foreach (var prop in props)
            {
                var key = prop.GetCustomAttributes<KeyAttribute>();
                if (key.Count() == 0)
                {
                    var name = GetNomOnglet(prop);
                    if (tab!=name)
                    {
                        string IdTab = name.Replace(' ', '_');
                        output.Content.AppendHtml(
                            $"<a class=\"nav-item nav-link active\" id=\"nav-tab-{IdTab}\" data-toggle=\"tab\" href=\"#nav-{IdTab}\" role=\"tab\" aria-controls=\"nav-{IdTab}\" aria-selected=\"true\">{name}</a>");
                    }
                    tab = name;
                }
            }
            output.Content.AppendHtml("</div>");
            output.Content.AppendHtml("</nav>");
        }

        private void TableBody(TagHelperOutput output, IOrderedEnumerable<PropertyInfo> props)
        {
            output.Content.AppendHtml("<div class=\"tab-content\" id=\"nav-tabContent\">");

            //foreach (var item in Items)
            //{
            //    string htmlact = "";
            //    output.Content.AppendHtml("<tr>");
            //    foreach (var prop in props)
            //    {
            //        //tester si le collone est ID avec l'annotation [Key].
            //        var key = prop.GetCustomAttributes<KeyAttribute>();
            //        var value = GetPropertyValue(prop, item);
            //        if (key.Count() == 0)
            //        {
            //            output.Content.AppendHtml($"<td>{value}</td>");
            //        }
            //        else
            //        {
            //            //Preparation les Action Modifier et afficher et supprimer
            //            string action=HtmlAction(value.ToString());
            //            htmlact=$"<th>{action}</th>";
            //        }
            //    }
            //    output.Content.AppendHtml(htmlact);
            //    output.Content.AppendHtml("</tr>");
            //}
            output.Content.AppendHtml("</tbody>");
        }

        private PropertyInfo[] GetItemProperties()
        {
            var listType = Items.GetType();
            Type itemType;
            if (Items!=null)
            {
                //itemType = listType.GetGenericArguments().First();
                return Items.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            return new PropertyInfo[] { };
        }

        private string GetNomOnglet(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ModifierAttribute>();

            if (attribute != null)
            {
                return attribute.NomOnglet;
            }
            return property.Name;
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

        private bool IsLister(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes<ListerAttribute>();
            if (attribute.Count()==1)
            {
                return true;
            }

            return false;
        }

        private object GetPropertyValue(PropertyInfo property, object instance)
        {
            var InfoListe = property.GetCustomAttributes<ListerAttribute>().First();
            var attributeDisp = property.GetCustomAttributes<DisplayFormatAttribute>();
            //gerer formate d'afficher DisplayFormat
            if (attributeDisp.Count() == 1)
            {
                string format = attributeDisp.First().DataFormatString;
                return string.Format(format, property.GetValue(instance));
            }

            var b = property.GetValue(instance);
            if (InfoListe != null && b != null && (b.GetType() == typeof(bool) || b.GetType() == typeof(string)))
            {
                
                string href = "";
                if (InfoListe.IsUrl)
                    href = $"href =\"{InfoListe.Url}{b.ToString()}\"";
                //les icons pour les champs bool true ou false
                if (InfoListe.IconeOk!=string.Empty && b!=null && b.GetType() == typeof(bool))
                {
                    if ((bool) b)
                    {
                        return HtmlIcon(InfoListe.IconeOk, href);
                    }
                    else
                    {
                        return HtmlIcon(InfoListe.IconeKo, href);
                    }
                }
                 
                //les icon pour afficher les icon au lieu text 
                if (InfoListe.IconeOk != null && b.GetType() == typeof(string))
                {
                    if (InfoListe.IconeOk != string.Empty && b.ToString()!=string.Empty)
                    {
                        return HtmlIcon(InfoListe.IconeOk, href);
                    }
                    else
                    {
                        return HtmlIcon(InfoListe.IconeKo, href);
                    }
                }
            }
            //pour afficher les colonne de type numéro
            if (InfoListe != null && b != null && (b.GetType() == typeof(Int32) && (b.GetType() == typeof(double) || b.GetType() == typeof(decimal) || b.GetType() == typeof(float))))
            {
                return $"<a class=\"right-elm\">{property.GetValue(instance)}</a>"; 

            }
            //pour afficher un phoo dans la table
            if (InfoListe.CheminPhoto != string.Empty && InfoListe.CheminPhoto!=null)
            {
                var img = property.GetValue(instance);
                if (img != null)
                {
                    return
                        $"<img class=\"rounded smalpic\" style=\"border-radius: 100% !important;\" src=\"{InfoListe.CheminPhoto}{img.ToString()}\"/>";
                }
            }

            var obj = property.GetValue(instance);
            if (obj != null && InfoListe.DisplayChamp!=null)
            {
                //pour afficher des colonne de table de referance
                if (obj.GetType().GetProperties().Length > 0 && obj.GetType().GetProperty(InfoListe.DisplayChamp) !=null)
                {
                    var x = obj.GetType().GetProperty(InfoListe.DisplayChamp).GetValue(obj,null);
                    return x;
                }
                //pour afficher les collection
                if (obj.GetType().GetProperties().Length > 0)
                {
                    string x = "";

                    foreach (var propertyInfo in (IEnumerable)obj)
                    {
                        foreach (var PInfo in propertyInfo.GetType().GetProperties())
                        {
                            if (PInfo.PropertyType == InfoListe.TypeObjet)
                            {
                                var obj2 = PInfo.GetValue(propertyInfo);
                                if (obj2.GetType().GetProperties().Length > 0 && obj2.GetType().GetProperty(InfoListe.DisplayChamp) != null)
                                {
                                    var value = obj2.GetType().GetProperty(InfoListe.DisplayChamp).GetValue(obj2, null);
                                    x+=$"<badg class=\"badge badge-primary\">{value}</badg>";
                                }
                            }
                        }
                    }
                    return x;

                }
            }

            return property.GetValue(instance);
        }

        string HtmlIcon(string ClassIcon, string url)
        {
            return $"<a class=\"centericon\" target=\"_blank\" {url}>" +
                   $"<i class=\"{ClassIcon} icon-large \" ></i>" +
                   "</a>";
        }

        string HtmlAction(string id)
        {
            string AfficherAction = "Afficher/" + id;
            string ModifierAction = "Modifier/" + id;
            string SupprimerAction = "Supprimer/" + id;
            return $"<div class=\"btn-group\" role=\"group\" aria-label=\"Action\" style =\"float: left;width: 115px;\" >" +
                    $"<a class=\"btn btn-info\" title=\"Afficher\" href=\"{AfficherAction}\">" +
                    "<i class=\"fas fa-info-circle mt-0\"></i>" +
                    "</a>" +
                    $"<a class=\"btn btn-primary\" title=\"Modifier\" href=\"{ModifierAction}\">" +
                    "<i class=\"fas fa-edit\"></i>" +
                    "</a>" +
                    $"<a class=\"btn btn-danger\" data-toggle=\"modal\" data-target=\"#Supprimer\" id=\"supprimer\" href=\"{SupprimerAction}\">" +
                    "<i class=\"fas fa-trash-alt\"></i>" +
                    "</a>" +
                    "</div>";
        }

    }

}

