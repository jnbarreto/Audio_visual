using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Cookies
    {
        public int cod_colaborador { get; set; }
        public int cod_cargo { get; set; }
        public int cod_permissao { get; set; }
        public string nome_colaborador { get; set; }
        public string email { get; set; }
        public string contato { get; set; }
        public string tipo_cargo { get; set; }
        public string tipo_permissao {get; set;}
        public string funcional { get; set; }
        public bool auto_login { get; set; }
        public int cod_auto_login { get; set; }
    }
}