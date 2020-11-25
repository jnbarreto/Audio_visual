using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Relatorio
    {
        public string processado_por { get; set; }
        public string cod_colaborador {get; set;}
        public string nome_colaborador { get; set; }
        public string funcional { get; set; }
        public string data_reserva { get; set; }
        public string hora_reserva { get; set; }
        public string agendado_para { get; set; }
        public string horario_de { get; set; }
        public string horario_ate { get; set; }
        public string data_entrega { get; set; }
        public string hora_entrega { get; set; }
        public string num_controle { get; set; }
        public string marca { get; set; }
        public string tipo { get; set; }
        public string modelo { get; set; }
        public string cod_equipamento { get; set; }
        public string mes_de { get; set; }
        public string mes_ate { get; set; }
    }

    public class Link
    {
        public string link { get; set; }
    }

}