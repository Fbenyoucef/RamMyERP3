using Microsoft.AspNetCore.Mvc;
using MyErp.MyTagHelpers;
using RamMyERP3.DataContext;
using RamMyERP3.Helpers.Entite;
using RamMyERP3.Models;
using System;
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
            //var testville = _context.r_ville.Include(p => p.R_PAYS).ToList();
            var customattr = typeTable.GetCustomAttribute<FonctionAttribute>();
            if(customattr.TableLiee!=null)
            {
                //string tableLiee = customattr.TableLiee;
                var ListeDataLiee = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(customattr.TableLiee).GetValue(_context)).ToList();

            }

            foreach (var item in typeTable.GetProperties())
            {
                ProprieteInfos prpInfos = new ProprieteInfos();
                prpInfos.Nom = item.Name;
                prpInfos.Type = item.PropertyType;
                var testt = item.GetCustomAttributes<ListerAttribute>().FirstOrDefault();
                if(testt != null && testt.Cacher)
                {
                    prpInfos.Visibilite = true;
                }
                prpInfos.Originale = item;
                listPrpInfos.Add(prpInfos);
            }
            ReferenceModel referenceModel = new ReferenceModel();
            referenceModel.TypeClass = typeTable;
            referenceModel.listeValeur = new List<object>(ListeData);
            // Affecter le titre de la vue
            ViewData["title"] = "Home Page";
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
    }
    public class ReferenceModel
    {
        public Type TypeClass { get; set; }
        public List<object> listeValeur { get; set; }
    }
}

