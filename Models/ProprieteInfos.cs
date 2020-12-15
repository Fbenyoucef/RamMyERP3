/*
 * Type de fichier  : model
 * Nom du fichier   : ProprieteInfos.cs
 * Création         : 10/11/2020 - Youcef Chabane 
 */

using System;

namespace RamMyERP3.Models
{
    /// <summary>
    /// les informations des propriétés des tables de références
    /// </summary>
    public class ProprieteInfos
    {
        /// <summary>
        /// le nom de la propriété
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        /// le nom à afficher
        /// </summary>
        public string NomAfficher { get; set; }
        /// <summary>
        /// le Type de la propriété
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// la visibilité
        /// </summary>
        public bool Visibilite { get; set; }
        /// <summary>
        /// définir si le champ est editable ou non 
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// définir si la donnée est numeric ou bien string 
        /// </summary>
        public string NumericOrString { get; set; }
    }
}
