using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Permissoes
    {
        public int cod_permissao { get; set; }
        public string tipo_permissao { get; set; }
        public string descricao_permissao { get; set; }
    }
}