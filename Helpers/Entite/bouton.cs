using System;

namespace RamMyERP3.Helpers.Entite
{
    public class bouton

    {

        public string Titre { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public string Id { get; set; }

        public string Style { get; set; }

        public string Data { get; set; }

        public Func<object, bool> ConditionAffichage { get; set; }

    }
}
