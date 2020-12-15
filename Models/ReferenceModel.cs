/*
 * Type de fichier  : model
 * Nom du fichier   : ReferenceModel.cs
 * Création         : 10/11/2020 - Youcef Chabane 
 */

using System.Collections.Generic;

namespace RamMyERP3.Models
{
    /// <summary>
    /// DTO qui contient tous les données d'une table de référence avec le type du Model
    /// </summary>
    public class ReferenceModel
    {
        /// <summary>
        /// la liste des propriétés du model
        /// </summary>
        public List<ProprieteInfos> TypeClass { get; set; }
        /// <summary>
        /// la liste des données de la table
        /// </summary>
        public List<object> ListeValeur { get; set; }
        /// <summary>
        /// les tables liées à la table de référence 
        /// </summary>
        public Dictionary<string, List<IReferenceTable>> ListeTablesLiees { get; set; }
    }
}
