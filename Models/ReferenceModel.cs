using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class ReferenceModel
    {
        public List<ProprieteInfos> TypeClass { get; set; }
        public List<object> listeValeur { get; set; }
        public Dictionary<string, List<IReferenceTable>> ListeTablesLiees { get; set; }
    }
}
