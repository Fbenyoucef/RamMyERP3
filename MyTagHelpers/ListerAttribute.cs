﻿
/*
 * Type de fichier  : MyTagHelpers
 * Nom du fichier   : ListerAttribute.cs
 * Création         : 16/12/2019 - Mourad Yamani 
 * Modification     : 18/12/2019 - Mourad Yamani
 * Modification     : 23/08/2019 - Mourad Yamani
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyErp.MyTagHelpers
{
    public class ListerAttribute: Attribute
    {
        //Si le champs Primary Key
        public bool Cle { get; set; }
        //La position de colonne
        public int Position { get; set; }
        //Définir la classe CSS pour les champs de type bool True
        public string IconeOk { get; set; }
        //Définir la classe CSS pour les champs de type bool False
        public string IconeKo { get; set; }
        //Définir true ou false si le champ contient un lien
        public bool IsUrl { get; set; }
        //Définir le lien
        public string Url { get; set; }
        //Définir le Chemin de la photo
        public string CheminPhoto { get; set; }
        //Définir si le champte contient une collection true or false
        public bool IsList { get; set; }
        //Définir le nom de la pripertie pour l'affichage
        public string DisplayChamp { get; set; }
        //Définir le type d'objet dans la collection
        public Type TypeObjet { get; set; }
        public bool Cacher { get; set; }

    }
    
}