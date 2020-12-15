/*
 * Type de fichier  : Controllers
 * Nom du fichier   : ReferenceController.cs
 * Création         : 04/11/2020 - Youcef Chabane 
 * Modification     : 01/12/2020 - Youcef Chabane rajouter la méthode Supprimer
 */

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyErp.MyTagHelpers;
using Newtonsoft.Json;
using RamMyERP3.DataContext;
using RamMyERP3.Helpers.Entite;
using RamMyERP3.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RamMyERP3.Controllers
{
    public class ReferenceController : Controller
    {
        #region Attributs
        private readonly MyContext _context;
        private readonly IHttpContextAccessor _userContext;
        #endregion

        #region Index
        /// <summary>
        /// Afficher la liste des fonctions et les tables de références 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(ListerTable());
        }
        #endregion

        #region constructeur
        /// <summary>
        /// le constructeur du ReferenceController  
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="userContext">Contient les informations de l'utilisateur connecté</param>
        public ReferenceController(MyContext context, IHttpContextAccessor userContext)
        {
            _context = context;
            _userContext = userContext;
        }
        #endregion

        #region Details
        /// <summary>
        /// Afficher les données d'une table de référence
        /// </summary>
        /// <param name="tableName">le nom de la table de référence</param>
        /// <returns></returns>
        public ViewResult DetailsReferenceTable(string tableName)
        {
            List<ProprieteInfos> listPrpInfos = new List<ProprieteInfos>();
            //Récupérer le Type d'une classe qui hérite de IReferenceTable
            var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                             where typeof(IReferenceTable).IsAssignableFrom(type)
                             select type).FirstOrDefault();
            string displayTableName = string.Empty;
            //Récupérer la fonction de cette table
            var fonction = typeTable.GetCustomAttribute(typeof(FonctionAttribute));
            if (fonction.GetType().GetProperties().Count() > 1)
                displayTableName = fonction.GetType().GetProperties()[1].GetValue(fonction).ToString();
            else
                displayTableName = tableName;
            //Récupérer la liste des données pour cette table de référence en utilisant la réflexion
            var ListeData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList().OrderBy(e => e.POSITION);

            var ListeDataLiee = new Dictionary<string, List<IReferenceTable>>();
            //Récupérer les custom attribute de type FonctionAttribute
            var customattr = typeTable.GetCustomAttribute<FonctionAttribute>();
            if (customattr.TableLiee != null)
            {
                //Récupérer le nom de la table liée
                var idPropetieLiee = typeTable.GetProperties()
                        .FirstOrDefault(p => p.Name.ToLower() == string.Format("{0}id", customattr.TableLiee).ToLower())?.Name;
                //Récupérer la liste des données pour cette table liée en utilisant la réflexion
                if (!string.IsNullOrEmpty(idPropetieLiee))
                {
                    ListeDataLiee.Add(idPropetieLiee, ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(customattr.TableLiee).GetValue(_context)).ToList());
                }
            }
            //Préparer la liste des Propriétés d'un model (table de référence) avec les attributes de chaque propriété
            foreach (var item in typeTable.GetProperties())
            {
                ProprieteInfos prpInfos = new ProprieteInfos();
                prpInfos.Nom = item.Name;
                prpInfos.Type = item.PropertyType;
                prpInfos.NomAfficher = GetPropertyName(item);
                prpInfos.NumericOrString = IsNumericOrStringType(item.PropertyType);
                var listerAttr = item.GetCustomAttributes<ListerAttribute>().FirstOrDefault();
                if (listerAttr != null)
                {
                    prpInfos.Visibilite = listerAttr.Cacher;
                    prpInfos.IsReadOnly = listerAttr.IsReadOnly;
                }
                listPrpInfos.Add(prpInfos);
            }
            //Préparer les données à envoyer a la vue 
            ReferenceModel referenceModel = new ReferenceModel();
            referenceModel.TypeClass = listPrpInfos;
            referenceModel.ListeValeur = new List<object>(ListeData);
            referenceModel.ListeTablesLiees = ListeDataLiee;
            // Affecter le titre de la vue
            ViewData["title"] = "Home Page";
            ViewData["tableName"] = tableName;
            ViewData["displayTableName"] = displayTableName;
            // Afficher la vue
            return View(referenceModel);
        }
        /// <summary>
        /// Retourner "Numeric" ou "String" tout depend le type de l'object
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string IsNumericOrStringType(Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return "Numeric";
                case TypeCode.String:
                    return "String";
                default:
                    return "Autres";
            }
        }
        /// <summary>
        /// Récupérer le nom de la property de l'attribute "Display"
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private string GetPropertyName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                return attribute.Name;
            }
            return property.Name;
        }
        #endregion

        #region ListerTable
        /// <summary>
        /// Lister les fonctions et les tables de références de chaque fonction
        /// </summary>
        /// <returns></returns>
        public ReferenceDomaine ListerTable()
        {
            ReferenceDomaine reference = new ReferenceDomaine();
            string nomTable = string.Empty;
            //récupérer tous les classes qui héritent de l'interface "IReferenceTable"
            var results = from type in AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(assembly => assembly.GetTypes())
                          where typeof(IReferenceTable).IsAssignableFrom(type)
                          select type;
            //créer des listes de tables de références pour chaque fonction
            foreach (var item in results)
            {
                TableReferenceDTO model = new TableReferenceDTO();
                model.TableName = item.Name;
                if (item.CustomAttributes.Count() == 0)
                    continue;
                var fonction = item.GetCustomAttribute(typeof(FonctionAttribute));
                var nomFonction = fonction.GetType().GetProperties()[0].GetValue(fonction).ToString();
                if (fonction.GetType().GetProperties().Count() > 1)
                    model.DisplayTableName = fonction.GetType().GetProperties()[1].GetValue(fonction).ToString();
                else
                    model.DisplayTableName = item.Name;
                try
                {
                    reference.ReferenceTable[nomFonction].Add(model);
                }
                catch
                {
                    reference.ReferenceTable.Add(nomFonction, new List<TableReferenceDTO> { model });
                }
            }
            return reference;
        }
        #endregion

        #region Enregister
        /// <summary>
        /// Ajouter un ou plusieurs enregistrements, ou modifier les données 
        /// </summary>
        /// <param name="listeData"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpPost]
        public object Enregister(string listeData, string tableName)
        {
            string userName = string.Empty;
            try
            {
                //récupérer le Type à travers le nom de la table
                var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                                 where typeof(IReferenceTable).IsAssignableFrom(type)
                                 select type).FirstOrDefault();
                //créer une liste générique de type  "typeTable"
                var listGeneric = CreateGenericList(typeTable);
                Type protocolType = (listGeneric.GetType());

                //Désérialiser la liste des données qu'on reçoit du datatable
                IEnumerable<IReferenceTable> data = (IEnumerable<IReferenceTable>)JsonConvert.DeserializeObject(listeData, protocolType,
                    new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy HH:mm:ss" });
                //Lister les données de la table avant la modification
                List<IReferenceTable> originaleData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();

                //TODO :: Youcef intégrer identity
                // userName = _userContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                userName = "y.chabane@my-kiwi.fr";

                //Récupérer la liste des données changées
                var listeModifications = data.Where(d => CompareListe(typeTable, d, originaleData, userName)).ToList();

                //Envoyer les données à la base de données Mysql
                if (listeModifications != null && listeModifications.Any())
                    EnregistrerDonnees(listeModifications);
                //Retourner un message "succès" a la vue
                return Json(new
                {
                    success = true,
                    titre = "",
                    responseText = "Modification enregistrer avec succès",
                    redirect = nameof(DetailsReferenceTable)
                });
            }
            catch (Exception ex)
            {
                //s'il y a des erreurs, on envoit le message d'erreur  
                return Json(new
                {
                    success = false,
                    titre = tableName.ToUpper(),
                    responseText = ex.Message,
                    redirect = nameof(DetailsReferenceTable)
                });
            }
        }

        /// <summary>
        /// Enregistrer les données au niveau de MySql (EF) 
        /// </summary>
        /// <param name="list"></param>
        private void EnregistrerDonnees(IEnumerable<IReferenceTable> list)
        {
            DetachAllEntities(_context);
            _context.UpdateRange(list);
            _context.SaveChanges();
        }

        /// <summary>
        /// comparer les données et renvoyer True s'il y a des changements 
        /// </summary>
        /// <param name="typeTable"> type de la table</param>
        /// <param name="elementListData"> l'enregistrement à tester s'il a changé ou pas</param>
        /// <param name="originaleData"> la liste des données avant modification</param>
        /// <returns></returns>
        private static bool CompareListe(Type typeTable, IReferenceTable elementListData, List<IReferenceTable> originaleData, string userName)
        {
            bool elementChanged = false;
            string propertieLiee = string.Empty;
            //récupérer les attributs personnalisés de type Fonction
            var customattr = typeTable.GetCustomAttribute<FonctionAttribute>();
            if (customattr.TableLiee != null)
            {
                //récupérer le nom de la table liée
                propertieLiee = typeTable.GetProperties()
               .Where(p => p.PropertyType.IsClass == true)
               .Where(p => p.PropertyType.Assembly.FullName == typeTable.Assembly.FullName)
               .FirstOrDefault(p => p.Name.ToLower() == customattr.TableLiee.ToLower())?.Name;
            }
            var itemOriginal = originaleData.Where(e => e.ID == elementListData.ID).FirstOrDefault();
            if (itemOriginal != null)
            {
                foreach (var prp in typeTable.GetProperties())
                {
                    if (propertieLiee == prp.Name)
                        continue;
                    //récupérer la valeur de chaque propriété de l'enregistrement modifier
                    var value1 = elementListData.GetType().GetProperty(prp.Name).GetValue(elementListData);
                    //récupérer la valeur de chaque propriété de l'enregistrement avant modification
                    var value2 = itemOriginal.GetType().GetProperty(prp.Name).GetValue(itemOriginal);
                    if (value1 != null && !value1.Equals(value2))
                    {
                        //modifier directement la valeur de"UTILISATEUR_MODIFICATION" avec les informations de l'utilisateur connecté
                        elementListData.GetType().GetProperty("UTILISATEUR_MODIFICATION").SetValue(elementListData, userName);
                        elementChanged = true;
                        break;
                    }
                    if (value1 == null && value2 != null)
                    {
                        elementChanged = true;
                        //modifier directement la valeur de"UTILISATEUR_MODIFICATION" avec les informations de l'utilisateur connecté
                        elementListData.GetType().GetProperty("UTILISATEUR_MODIFICATION").SetValue(elementListData, userName);
                        break;
                    }
                }
            }
            else
            {
                var typesprp = typeTable.GetProperties();
                var userCreation = typesprp.Where(e => e.Name == "UTILISATEUR_CREATION").FirstOrDefault();
                if (userCreation == null)
                    elementListData.GetType().GetProperty("UTILISATEUR_MODIFICATION").SetValue(elementListData, userName);
                else
                    //modifier directement la valeur de"UTILISATEUR_CREATION" avec les informations de l'utilisateur connecté
                    elementListData.GetType().GetProperty("UTILISATEUR_CREATION").SetValue(elementListData, userName);

                elementChanged = true;
            }

            return elementChanged;
        }

        /// <summary>
        /// détacher les entités du DbContext pour pouvoir les modifier sans erreurs
        /// </summary>
        /// <param name="context"></param>
        private void DetachAllEntities(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity != null)
                {
                    entry.State = EntityState.Detached;
                }
            }
        }
        /// <summary>
        /// créer une liste générique 
        /// </summary>
        /// <param name="typeInList"></param>
        /// <returns></returns>
        private IList CreateGenericList(Type typeInList)
        {
            var genericListType = typeof(List<>).MakeGenericType(new[] { typeInList });
            return (IList)Activator.CreateInstance(genericListType);
        }
        #endregion

        #region Supprimer
        /// <summary>
        /// supprimer un enregistrement de la table "tableName"
        /// </summary>
        /// <param name="id">id de l'enregistrement à supprimer</param>
        /// <param name="tableName"> le nom de la table</param>
        /// <returns></returns>
        [HttpPost]
        public object Supprimer(int id, string tableName)
        {
            //récupérer les données de la table "tableName"
            List<IReferenceTable> originaleData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();

            var idsDB = originaleData.Select(e => e.ID);
            //vérifier si l'id à supprimer existe dans la liste des données
            var idToDelete = idsDB.Contains(id);

            if (idToDelete)
            {
                //récupérer l'él2ment à supprimer
                var elementToDelete = originaleData.Where(e => e.ID == id).FirstOrDefault();
                DetachAllEntities(_context);
                //supprimer l'élément sélectionné
                try
                {
                    _context.Remove(elementToDelete);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    //envoyer un message d'erreur 
                    return Json(new
                    {
                        success = false,
                        titre = tableName.ToUpper(),
                        responseText = "Vous ne pouvez pas supprimer cette ligne!",
                        redirect = ""//nameof(DetailsReferenceTable)
                    });
                }
            }
            return Json(new
            {
                success = true,
                titre = "",
                responseText = "Elément supprimer avec succès",
                redirect = ""//nameof(DetailsReferenceTable)
            });
        }
        #endregion
    }
}

