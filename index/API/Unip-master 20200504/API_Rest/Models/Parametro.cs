using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Parametro
    {
        public int cod_parametro { get; set; }
        public string parametro { get; set; }
        public string valor { get; set; }
        public string parametro_ativo { get; set; }
        public string personalizacao { get; set; }
    }
}