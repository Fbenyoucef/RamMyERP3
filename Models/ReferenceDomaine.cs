using System.Collections.Generic;

namespace RamMyERP3.Models
{
    public class ReferenceDomaine
    {
        public ReferenceDomaine()
        {
            ReferenceTable = new Dictionary<string, List<string>>();
        }
        public Dictionary<string, List<string>> ReferenceTable { get; set; }
    }
}
