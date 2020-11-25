using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API_Rest.Classes;
using API_Rest.Models;
using System.Data;

namespace API_Rest.Controllers
{
    public class CarregaListasController : Controller
    {
        [HttpPost]
        public JsonResult montarLista() 
        {
            Retorno Retorno = processaMontaLista();
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaMontaLista()
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            try
            {
                if (Conn.Open())
                {
                    //Monta lista Cargos
                    Retorno.ListaCargos = new List<Cargos>();
                    CargosController cargos = new CargosController();
                    Retorno.ListaCargos = cargos.processaSelect().ListaCargos;

                    //Monta Lista Permissoes
                    Retorno.ListaPermissoes = new List<Permissoes>();
                    PermissoesController permissoes = new PermissoesController();
                    Retorno.ListaPermissoes = permissoes.processaSelect().ListaPermissoes;

                    //Monta Lista Equipamentos
                    Retorno.ListaEquipamentos = new List<Equipamentos>();
                    EquipamentosController equipamentos = new EquipamentosController();
                    Retorno.ListaEquipamentos = equipamentos.processaSelect().ListaEquipamentos;
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento Concluido com Sucesso.";
            }
            catch (Exception e)
            {
                Retorno.erro = e.Message;
                Retorno.status = false;
            }

            Conn.Close();
            return Retorno;
        }
    }
}