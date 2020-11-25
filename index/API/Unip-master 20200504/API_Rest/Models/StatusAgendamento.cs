using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class StatusAgendamento
    {
        public int cod_status_agendamento { get; set; }
        public string tipo_status_agendamento { get; set; }
        public string descricao_status_agendamento { get; set; }
    }
}