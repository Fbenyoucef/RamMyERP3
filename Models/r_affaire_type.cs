﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class r_affaire_type
    {

        public int ID { get; set; }
        public string NOM { get; set; }
        public string CODE { get; set; }
        public DateTime DATE_CREATION { get; set; }
        public DateTime DATE_MODIFICATION { get; set; }
        public string USER_MODIFICATION { get; set; }
        public string USER_CREATION { get; set; }
    }
}
