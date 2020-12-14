using System.Collections.Generic;

namespace RamMyERP3.Models
{
    public class ReferenceModel
    {
        public List<ProprieteInfos> TypeClass { get; set; }
        public List<object> listeValeur { get; set; }
        public Dictionary<string, List<IReferenceTable>> ListeTablesLiees { get; set; }
    }
}
