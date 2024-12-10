
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Shared.Models
{
    public class Coactivos
    {
        [Required]
        [Key]
        public int id { get; set; }
        public string? comparendo { get; set; }
        public string? cedula { get; set; }

        public string? nombres { get; set; }
        public string? infraccion { get; set; }
        public DateTime fecha_comp { get; set; }
        public DateTime fecha_sancion { get; set; }
        public DateTime fecha_mdto { get; set; }
        public DateTime fecha_proceso { get; set; }
        public double valor { get; set; }

    }
}

