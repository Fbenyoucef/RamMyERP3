/*
 * Type de fichier  : interface
 * Nom du fichier   : IReferenceTable.cs
 * Création         : 08/11/2020 - Youcef Chabane 
 * Modification     : 07/12/2020 - Youcef Chabane rajouter la position 
 */

using System;

namespace RamMyERP3.Models
{
    /// <summary>
    /// l'interface IReferenceTable contient les propriétés en commun entre les tables de références 
    /// </summary>
    public interface IReferenceTable
    {
        /// <summary>
        /// Id de la table
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Nom
        /// </summary>
        public string NOM { get; set; }
        /// <summary>
        /// Code ou la valeur de l'enregistrement
        /// </summary>
        public string CODE { get; set; }
        /// <summary>
        /// la date de création
        /// </summary>
        public DateTime? DATE_CREATION { get; set; }
        /// <summary>
        /// la date de la modification
        /// </summary>
        public DateTime? DATE_MODIFICATION { get; set; }
        /// <summary>
        /// l'utilisateur qui a modifier l'enregistrement
        /// </summary>
        public string UTILISATEUR_MODIFICATION { get; set; }
        /// <summary>
        /// la position de la ligne
        /// </summary>
        public int POSITION { get; set; }
    }
}
