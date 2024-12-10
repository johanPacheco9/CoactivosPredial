﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Models
{
    public class Usuarios
    {
        [Key]
        [Required]
        public int id { get; set; }
        public string login { get; set; }
        public string nombre { get; set; }
        public bool habilitado { get; set; }
        public string clave { get; set; }
        public int nivel { get; set; }
        public string dir { get; set; }
        public string tel { get; set; }
        public string? correo { get; set; }
        public int? usuario_creo { get; set; }
        public int municipio { get; set; }
        public string mun { get; set; }
    }
}
