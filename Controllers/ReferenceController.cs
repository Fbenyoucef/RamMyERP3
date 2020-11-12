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
            //( ListTable();
            return View(ListTable());
        }

        public ReferenceController(MyContext context)
        {
            this._context = context;
        }
        public ViewResult DetailsReferenceTable(string tableName)
        {
            List<ProprieteInfos> listPrpInfos = new List<ProprieteInfos>();
            // Type typeTable = Type.GetType("RamMyERP3.Models." + tableName);
            var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                             where typeof(IReferenceTable).IsAssignableFrom(type)
                             select type).FirstOrDefault();

            var ListeData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();

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

            foreach (var item in typeTable.GetProperties())
            {
                ProprieteInfos prpInfos = new ProprieteInfos();
                prpInfos.Nom = item.Name;
                prpInfos.Type = item.PropertyType;
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
            // Afficher la vue
            return View(referenceModel);
        }

        // private Dictionary<string, List<Type>> _referenceDomaine;
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
                if (item.CustomAttributes.Count() == 0)
                    continue;
                var fonction = item.GetCustomAttribute(typeof(FonctionAttribute));
                var nomFonction = fonction.GetType().GetProperties()[0].GetValue(fonction).ToString();
                if (item.CustomAttributes.Count() > 1)
                    nomTable = fonction.GetType().GetProperties()[1].GetValue(fonction).ToString();
                else
                    nomTable = item.Name;
                try
                {
                    reference.ReferenceTable[nomFonction].Add(nomTable);
                }
                catch
                {
                    reference.ReferenceTable.Add(nomFonction, new List<string> { nomTable });
                }
            }
            return reference;
        }
        [HttpPost]
        public void Ajouter(string listeData, string tableName)
        {
            //string tablename = "r_affaire_type";
            var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                             where typeof(IReferenceTable).IsAssignableFrom(type)
                             select type).FirstOrDefault();
            //JsonSerializerSettings jsttings = new JsonSerializerSettings();
            var listGeneric = createGenericList(typeTable);
            Type protocolType = (listGeneric.GetType());

            var xx = JsonConvert.DeserializeObject(listeData, protocolType);

            //var ListeData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();
            DetachAllEntities(_context);
            foreach (var item in (IEnumerable<IReferenceTable>)xx)
            {
                _context.Update(item);
                _context.SaveChanges();
                //_context.GetType().GetProperty(tableName).SetValue(_context, item);
            }

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

        private IList createGenericList(Type typeInList)
        {
            var genericListType = typeof(List<>).MakeGenericType(new[] { typeInList });
            return (IList)Activator.CreateInstance(genericListType);
        }




    }
    public class ReferenceModel
    {
        public List<ProprieteInfos> TypeClass { get; set; }
        public List<object> listeValeur { get; set; }
        public Dictionary<string, List<IReferenceTable>> ListeTablesLiees { get; set; }
    }


}

