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
    public class StatusagendamentoController : Controller
    {

        [HttpPost]
        public JsonResult insert(StatusAgendamento status)
        {
            Retorno Retorno = processaInsert(status);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInsert(StatusAgendamento status)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();
                    var query = "SELECT * FROM app_status_agendamento WHERE tipo_status_agendamento = '" + status.tipo_status_agendamento + "'";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Tipo de status já cadastrado.");
                    }

                    query = "INSERT INTO app_status_agendamento(tipo_status_agendamento, descricao_status_agendamento) VALUES ('" + status.tipo_status_agendamento + "', '" + status.descricao_status_agendamento + "')";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao inserir registro na tabela app_status_agendamento. " + Conn.ErrorMsg);
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso";
            }
            catch (Exception e)
            {
                Conn.Rollback();
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult update(StatusAgendamento status)
        {
            Retorno Retorno = processaUpdate(status);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaUpdate(StatusAgendamento status)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    var query = "SELECT * FROM app_status_agendamento WHERE tipo_status_agendamento = '" + status.tipo_status_agendamento + "'";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Tipo de status já cadastrado.");
                    }

                    query = "UPDATE app_status_agendamento SET tipo_status_agendamento = '" + status.tipo_status_agendamento + "', descricao_status_agendamento = '" + status.descricao_status_agendamento + "' WHERE cod_status_agendamento = '" + status.cod_status_agendamento + "'";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao atualizar registro na tabela app_status_agendamento. " + Conn.ErrorMsg);
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso";
            }
            catch (Exception e)
            {
                Conn.Rollback();
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult select()
        {
            Retorno Retorno = processaSelect();
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaSelect()
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaStatusAgendamento = new List<StatusAgendamento>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_status_agendamento";
                    DataTable tableCargos = Conn.getDataTable(query);

                    foreach (DataRow dtCargos in tableCargos.Rows)
                    {
                        StatusAgendamento statusExistente = new StatusAgendamento();

                        statusExistente.cod_status_agendamento = int.Parse(dtCargos["cod_status_agendamento"].ToString());
                        statusExistente.tipo_status_agendamento = dtCargos["tipo_status_agendamento"].ToString();
                        statusExistente.descricao_status_agendamento = dtCargos["descricao_status_agendamento"].ToString();

                        Retorno.ListaStatusAgendamento.Add(statusExistente);
                    }

                    if (Retorno.ListaStatusAgendamento.Count == 0)
                    {
                        Retorno.status = false;
                        Retorno.erro = "Não a registros a ser exibidos.";
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult get(StatusAgendamento status)
        {
            Retorno Retorno = processaGet(status);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaGet(StatusAgendamento status)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaStatusAgendamento = new List<StatusAgendamento>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_status_agendamento WHERE cod_status_agendamento = '" + status.cod_status_agendamento + "'";
                    DataTable tableCargos = Conn.getDataTable(query);

                    foreach (DataRow dtCargos in tableCargos.Rows)
                    {
                        StatusAgendamento statusExistente = new StatusAgendamento();

                        statusExistente.cod_status_agendamento = int.Parse(dtCargos["cod_status_agendamento"].ToString());
                        statusExistente.tipo_status_agendamento = dtCargos["tipo_status_agendamento"].ToString();
                        statusExistente.descricao_status_agendamento = dtCargos["descricao_status_agendamento"].ToString();

                        Retorno.ListaStatusAgendamento.Add(statusExistente);
                    }

                    if (Retorno.ListaStatusAgendamento.Count == 0)
                    {
                        Retorno.status = false;
                        Retorno.erro = "Não a registros a ser exibidos.";
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }
    }
}