using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Agendamento
    {
        public int cod_agendamento { get; set; }
        public int cod_colaborador { get; set; }
        public int cod_equipamento { get; set; }
        public string cod_resp_alteracao { get; set; }
        public string cod_resp_entrega { get; set; }
        public string cod_resp_recebimento { get; set; }
        public int cod_agendado_por { get; set; }
        public int cod_status_agendamento { get; set; }
        public string data_entrega { get; set; }
        public string hora_entrega { get; set; }
        public string data_reserva { get; set; }
        public string hora_reserva { get; set; }
        public string agendado_para { get; set; }
        public string horario_de { get; set; }
        public string horario_ate { get; set; }

        public string agendado_por { get; set; }
        public string resp_alteracao { get; set; }

        public Colaboradores Colaborador { get; set; }
        public List<Equipamentos> Equipamento { get; set; }
        public StatusAgendamento Status { get; set; }
    }

    public class HoraAgendamento
    {
        public int cod_horario { get; set; }
        public string data { get; set; }
        public string horario_de { get; set; }
        public string horario_ate { get; set; }
    }

}