using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    /// <summary>
    /// ReferenceController
    /// </summary>
    public class ReferenceController : Controller
    {
        ///
        private readonly MyContext _context;
        private readonly IHttpContextAccessor _userContext;

        public IActionResult Index()
        {
            return View(ListTable());
        }
        /// <summary>
        /// le constructeur du ReferenceController  
        /// </summary>
        /// <param name="context">DbContext</param>
        /// <param name="userContext"></param>
        public ReferenceController(MyContext context, IHttpContextAccessor userContext)
        {
            _context = context;
            _userContext = userContext;
        }
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
            //Récupérer la liste des données pour cette table de référence
            var ListeData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList().OrderBy(e => e.POSITION);

            var ListeDataLiee = new Dictionary<string, List<IReferenceTable>>();

            var customattr = typeTable.GetCustomAttribute<FonctionAttribute>();
            if (customattr.TableLiee != null)
            {
                var propetieLiee = typeTable.GetProperties()
                .Where(p => p.PropertyType.IsClass == true)
                .Where(p => p.PropertyType.Assembly.FullName == typeTable.Assembly.FullName)
                .FirstOrDefault(p => p.Name.ToLower() == customattr.TableLiee.ToLower())?.Name;

                var idPropetieLiee = typeTable.GetProperties()
                        .FirstOrDefault(p => p.Name.ToLower() == string.Format("{0}id", customattr.TableLiee).ToLower())?.Name;

                if (!string.IsNullOrEmpty(idPropetieLiee))
                {
                    ListeDataLiee.Add(idPropetieLiee, ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(customattr.TableLiee).GetValue(_context)).ToList());
                }
            }


            foreach (var item in ListeData)
            {
                foreach (var item1 in item.GetType().GetProperties())
                {
                    if (item1.PropertyType == typeof(DateTime?))
                    {
                        var attribute = item1.GetCustomAttribute<DisplayFormatAttribute>();
                        if (attribute != null)
                        {
                            var value = item1.GetValue(item, null);
                            var currentType = item1.PropertyType;
                            var tt = string.Format(attribute.DataFormatString, value);
                        }
                    }
                    //ProprieteInfos prpInfos = new ProprieteInfos();
                    //prpInfos.Nom = item1.Name;
                    //prpInfos.Type = item1.PropertyType;
                    //prpInfos.NomAfficher = GetPropertyName(item1);
                    //var testt = item1.GetCustomAttributes<ListerAttribute>().FirstOrDefault();
                    //if (testt != null)
                    //{
                    //    prpInfos.Visibilite = testt.Cacher;
                    //    prpInfos.IsReadOnly = testt.IsReadOnly;
                    //}
                    ////prpInfos.Originale = item;
                    //listPrpInfos.Add(prpInfos);
                }
            }

            foreach (var item in typeTable.GetProperties())
            {
                ProprieteInfos prpInfos = new ProprieteInfos();
                prpInfos.Nom = item.Name;
                prpInfos.Type = item.PropertyType;
                prpInfos.NomAfficher = GetPropertyName(item);
                prpInfos.NumericOrString = IsNumericOrStringType(item.PropertyType);
                var testt = item.GetCustomAttributes<ListerAttribute>().FirstOrDefault();
                if (testt != null)
                {
                    prpInfos.Visibilite = testt.Cacher;
                    prpInfos.IsReadOnly = testt.IsReadOnly;
                }
                //prpInfos.Originale = item;
                listPrpInfos.Add(prpInfos);
            }
            ReferenceModel referenceModel = new ReferenceModel();
            referenceModel.TypeClass = listPrpInfos;
            referenceModel.listeValeur = new List<object>(ListeData);
            referenceModel.ListeTablesLiees = ListeDataLiee;
            // Affecter le titre de la vue
            ViewData["title"] = "Home Page";
            ViewData["tableName"] = tableName;
            ViewData["displayTableName"] = displayTableName;
            // Afficher la vue
            return View(referenceModel);
        }
        public static string IsNumericOrStringType(Type o)
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
        private string GetPropertyName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                return attribute.Name;
            }

            return property.Name;
        }
        private object GetPropertyDisplayFormat(PropertyInfo property, object item)
        {
            var attribute = property.GetCustomAttribute<DisplayFormatAttribute>();

            if (attribute != null)
            {
                return attribute.DataFormatString;
            }

            return property.GetValue(item);
        }

        public ReferenceDomaine ListTable()
        {
            ReferenceDomaine reference = new ReferenceDomaine();
            //_referenceDomaine = new Dictionary<string, List<Type>>();
            string nomTable = string.Empty;
            var results = from type in AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(assembly => assembly.GetTypes())
                          where typeof(IReferenceTable).IsAssignableFrom(type)
                          select type;
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

        private void UpdateListe(IEnumerable<IReferenceTable> list)
        {
            DetachAllEntities(_context);
            _context.UpdateRange(list);
            _context.SaveChanges();
        }
        private void AddListe(IEnumerable<IReferenceTable> list)
        {
            //DetachAllEntities(_context);
            _context.AddRange(list);
            _context.SaveChanges();
        }

        /// <summary>
        /// Ajouter un nouveau enregistrement
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
                var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                                 where typeof(IReferenceTable).IsAssignableFrom(type)
                                 select type).FirstOrDefault();
                var listGeneric = CreateGenericList(typeTable);
                Type protocolType = (listGeneric.GetType());

                IEnumerable<IReferenceTable> data = (IEnumerable<IReferenceTable>)JsonConvert.DeserializeObject(listeData, protocolType,
                    new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy HH:mm:ss" });
                List<IReferenceTable> originaleData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();

                //TODO :: Youcef intégrer identity
                // userName = _userContext.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                userName = "y.chabane@my-kiwi.fr";
                //var listModifier = data.Where(d => originaleData.Any(i => i.ID == d.ID)).ToList();
                var listModifier = data.Where(d => CompareListe(typeTable, d, originaleData)).ToList();
                var listAjouter = data.Where(d => d.ID == 0).ToList();

                if (listModifier != null && listModifier.Any())
                    UpdateListe(listModifier);

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
                return Json(new
                {
                    success = false,
                    titre = tableName.ToUpper(),
                    responseText = ex.Message,
                    redirect = nameof(DetailsReferenceTable)
                });
            }
        }

        [HttpPost]
        public object Supprimer(int id, string tableName)
        {
            var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                             where typeof(IReferenceTable).IsAssignableFrom(type)
                             select type).FirstOrDefault();
            var listGeneric = CreateGenericList(typeTable);
            Type protocolType = (listGeneric.GetType());

            //IEnumerable<IReferenceTable> data = (IEnumerable<IReferenceTable>)JsonConvert.DeserializeObject(listeData, protocolType,
            //    new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy HH:mm:ss" });
            List<IReferenceTable> originaleData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();

            //var x2 = (IEnumerable<IReferenceTable>)xx;

            //var ids = data.Select(e => e.ID);
            var idsDB = originaleData.Select(e => e.ID);
            var idToDelete = idsDB.Contains(id);
            //foreach (var item in data)
            //{
            //    if (CompareListe(typeTable, item, originaleData))
            //    {
            //        DetachAllEntities(_context);
            //        _context.Update(item);
            //        _context.SaveChanges();
            //    }
            //}
            if (idToDelete)
            {
                var elementToDelete = originaleData.Where(e => e.ID == id).FirstOrDefault();
                DetachAllEntities(_context);
                try
                {
                    _context.Remove(elementToDelete);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
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

        private static bool CompareListe(Type typeTable, IReferenceTable elementListData, List<IReferenceTable> originaleData)
        {
            bool elementChanged = false;
            string propetieLiee = string.Empty;
            var customattr = typeTable.GetCustomAttribute<FonctionAttribute>();
            if (customattr.TableLiee != null)
            {
                propetieLiee = typeTable.GetProperties()
               .Where(p => p.PropertyType.IsClass == true)
               .Where(p => p.PropertyType.Assembly.FullName == typeTable.Assembly.FullName)
               .FirstOrDefault(p => p.Name.ToLower() == customattr.TableLiee.ToLower())?.Name;
            }
            var itemOriginal = originaleData.Where(e => e.ID == elementListData.ID).FirstOrDefault();
            if (itemOriginal != null)
            {
                foreach (var prp in typeTable.GetProperties())
                {
                    if (propetieLiee == prp.Name)
                        continue;
                    var value1 = elementListData.GetType().GetProperty(prp.Name).GetValue(elementListData);
                    var value2 = itemOriginal.GetType().GetProperty(prp.Name).GetValue(itemOriginal);
                    if (value1 != null && !value1.Equals(value2))
                    {
                        elementListData.GetType().GetProperty("UTILISATEUR_MODIFICATION").SetValue(elementListData, "y.chabane@my-kiwi.fr");
                        elementChanged = true;
                        break;
                    }
                    if (value1 == null && value2 != null)
                    {
                        elementChanged = true;
                        elementListData.GetType().GetProperty("UTILISATEUR_MODIFICATION").SetValue(elementListData, "y.chabane@my-kiwi.fr");
                        break;
                    }
                }
            }
            else
            {
                var typesprp = typeTable.GetProperties();
                var userCreation = typesprp.Where(e => e.Name == "UTILISATEUR_CREATION").FirstOrDefault();
                if (userCreation == null)
                    elementListData.GetType().GetProperty("UTILISATEUR_MODIFICATION").SetValue(elementListData, "y.chabane@my-kiwi.fr");
                else
                    elementListData.GetType().GetProperty("UTILISATEUR_CREATION").SetValue(elementListData, "y.chabane@my-kiwi.fr");

                elementChanged = true;
            }

            return elementChanged;
        }

        public void DetachAllEntities(DbContext context)
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

        private IList CreateGenericList(Type typeInList)
        {
            var genericListType = typeof(List<>).MakeGenericType(new[] { typeInList });
            return (IList)Activator.CreateInstance(genericListType);
        }
    }
}

