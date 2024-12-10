﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Models
{
    public class Procesos
    {
        [Key]
        public int id { get; set; }
        public string comparendo { get; set; }
        public DateTime fecha_mdto { get; set; }
        public int estado { get; set; }
        public string nestado { get; set; }

        public string resolucion { get; set; }

        public string expediente { get; set; }
        public DateTime fecha_proc { get; set; }

    }
}
