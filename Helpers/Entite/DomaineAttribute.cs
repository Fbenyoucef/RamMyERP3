using System;

namespace RamMyERP3.Helpers.Entite
{
    public class FonctionAttribute : Attribute
    {
        public FonctionAttribute(string fonction, string nomTable)
        {
            Fonction = fonction;
            NomTable = nomTable;
        }
        public string Fonction { get; set; }
        public string NomTable { get; set; }

    }
}
