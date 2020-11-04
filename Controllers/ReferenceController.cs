using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RamMyERP3.DataContext;
using RamMyERP3.Helpers.Entite;
using RamMyERP3.Models;
using RamMyERP3.Helpers.DAL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.ComponentModel;

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
            // Type typeTable = Type.GetType("RamMyERP3.Models." + tableName);
            var typeTable = (from type in AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(assembly => assembly.GetTypes().Where(e => e.Name == tableName))
                         where typeof(IReferenceTable).IsAssignableFrom(type)
                         select type).FirstOrDefault();
            var ListeData = ((IEnumerable<IReferenceTable>)_context.GetType().GetProperty(tableName).GetValue(_context)).ToList();

            ReferenceModel referenceModel = new ReferenceModel();
            referenceModel.TypeClass = typeTable;
            referenceModel.listeValeur = new List<object>(ListeData);
            // Affecter le titre de la vue
            ViewData["title"] = "Home Page";
            // Afficher la vue
            return View(referenceModel);
        }


        private static DataTable ConvertToDatatable<T>(List<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        // private Dictionary<string, List<Type>> _referenceDomaine;
        public ReferenceDomaine ListTable()
        {
            ReferenceDomaine reference = new ReferenceDomaine();
            //_referenceDomaine = new Dictionary<string, List<Type>>();

            var results = from type in AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(assembly => assembly.GetTypes())
                          where typeof(IReferenceTable).IsAssignableFrom(type)
                          select type;
            foreach (var item in results)
            {
                if (item.CustomAttributes.Count() == 0)
                    continue;
                var domaine = item.GetCustomAttribute(typeof(DomaineAttribute));
                var nom = domaine.GetType().GetProperties().FirstOrDefault().GetValue(domaine).ToString();
                try
                {
                    reference.ReferenceTable[nom].Add(item.Name);
                }
                catch
                {
                    reference.ReferenceTable.Add(nom, new List<string> { item.Name });
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

