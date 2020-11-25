using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Cargos
    {
        public int cod_cargo { get; set; }
        public string tipo_cargo { get; set; }
        public string descricao_cargo { get; set; }
    }
}