using System.Collections.Generic;

namespace RamMyERP3.Models
{
    public class ReferenceDomaine
    {
        public ReferenceDomaine()
        {
            ReferenceTable = new Dictionary<string, List<TableReferenceDTO>>();
        }
        public Dictionary<string, List<TableReferenceDTO>> ReferenceTable { get; set; }
    }

    public class TableReferenceDTO
    {
        public string TableName { get; set; }
        public string DisplayTableName { get; set; }
    }
}
