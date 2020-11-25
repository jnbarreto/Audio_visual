using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Equipamentos
    {
        public int cod_equipamento { get; set; }
        public int cod_patrimonio { get; set; }
        public int num_controle { get; set; }
        public string marca { get; set; }
        public string tipo { get; set; }
        public string modelo { get; set; }
        public string status_ativo { get; set; }
    }
}