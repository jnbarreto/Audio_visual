using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Acessos
    {
        public int cod_acesso { get; set; }
        public int cod_colaborador { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public string status_ativo { get; set; }
        public bool auto_login { get; set; }
        public string cod_auto_login { get; set; }
    }
}