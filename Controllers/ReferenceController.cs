using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyErp.MyTagHelpers;
using MySql.Data.MySqlClient;
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
        private readonly MyContext _context;

        public IActionResult Index()
        {
            return View(ListTable());
        }

        public ReferenceController(MyContext context)
        {
            this._context = context;
        }
        public ViewResult DetailsReferenceTable(string tableName)
        {
            List<ProprieteInfos> listPrpInfos = new List<ProprieteInfos>();
            var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                             where typeof(IReferenceTable).IsAssignableFrom(type)
                             select type).FirstOrDefault();
            string displayTableName = string.Empty;
            var fonction = typeTable.GetCustomAttribute(typeof(FonctionAttribute));
            if (fonction.GetType().GetProperties().Count() > 1)
                displayTableName = fonction.GetType().GetProperties()[1].GetValue(fonction).ToString();
            else
                displayTableName = tableName;

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
        [HttpPost]
        public object Ajouter(string listeData, string tableName)
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

            var ids = data.Select(e => e.ID);
            var idsDB = originaleData.Select(e => e.ID);
            var idToDelete = idsDB.Except(ids);

            var positionMax = data.Select(e => e.POSITION).Max();

            foreach (var item in data)
            {
                if (item.POSITION == 0)
                {
                    positionMax++;
                    item.POSITION = positionMax;
                }
                if (CompareListe(typeTable, item, originaleData))
                {
                    DetachAllEntities(_context);
                    _context.Update(item);
                    _context.SaveChanges();
                }
            }
            foreach (var item in idToDelete)
            {
                var elementToDelete = originaleData.Where(e => e.ID == item).FirstOrDefault();
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
                        redirect = nameof(DetailsReferenceTable)
                    });
                }
            }
            return Json(new
            {
                success = true,
                titre = "",
                responseText = "Table MAJ avec succès",
                redirect = nameof(DetailsReferenceTable)
            });
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
                        redirect = nameof(DetailsReferenceTable)
                    });
                }
            }
            return Json(new
            {
                success = true,
                titre = "",
                responseText = "Table MAJ avec succès",
                redirect = nameof(DetailsReferenceTable)
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
                        elementChanged = true;
                        break;
                    }
                    if (value1 == null && value2 != null)
                    {
                        elementChanged = true;
                        break;
                    }
                }
            }
            else
            {
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

