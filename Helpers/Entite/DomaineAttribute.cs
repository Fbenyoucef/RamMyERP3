using System;

namespace RamMyERP3.Helpers.Entite
{
    public class FonctionAttribute : Attribute
    {
        public FonctionAttribute(string fonction, string nomTable, string tableLiee=null)
        {
            Fonction = fonction;
            NomTable = nomTable;
            TableLiee = tableLiee;
        }
        public string Fonction { get; set; }
        public string NomTable { get; set; }
        public string TableLiee { get; set; }


    }
}
