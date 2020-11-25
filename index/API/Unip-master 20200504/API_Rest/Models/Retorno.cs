using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace API_Rest.Models
{
    public class Retorno
    {

        public bool status { get; set; }

        public string msg { get; set; }

        public string erro { get; set; }

        public List<Cookies> ListaCookies { get; set; }

        public List<Cargos> ListaCargos { get; set; }

        public List<Permissoes> ListaPermissoes { get; set; }

        public List<Colaboradores> ListaColaboradores { get; set; }

        public List<Acessos> ListaAcessos { get; set; }

        public List<StatusAgendamento> ListaStatusAgendamento { get; set; }

        public List<Equipamentos> ListaEquipamentos { get; set; }

        public List<Agendamento> ListaAgendamentos { get; set; }

        public List<Email> ListaEmail { get; set; }

        public List<solicitacaoCadastro> ListaSolicCadastro { get; set; }
        public List<Parametro> ListaParametros { get; set; }
        public List<HoraAgendamento> ListaHorarios { get; set; }
        public List<Relatorio> ListaRelatorio { get; set; }
        public List<Link> donwload { get; set; }
    }
}