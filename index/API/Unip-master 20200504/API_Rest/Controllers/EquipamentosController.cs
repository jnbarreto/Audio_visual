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
    public class EquipamentosController : Controller
    {
        [HttpPost]
        public JsonResult insert(List<Equipamentos> equipamento)
        {
            Retorno Retorno = processaInsert(equipamento);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInsert(List<Equipamentos> equipamento)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    if (equipamento.Count > 0)
                    {
                        for (int i = 0; i < equipamento.Count; i++)
                        {
                            var query = "SELECT * FROM app_equipamentos WHERE num_controle = '" + equipamento[i].num_controle + "'";
                            Conn.Query(query);

                            if (Conn.getRows() > 0)
                            {
                                throw new Exception("Equipamento já cadastrado.");
                            }

                            query = "INSERT INTO app_equipamentos(cod_patrimonio, num_controle, marca, tipo, modelo) VALUES ('" + equipamento[i].cod_patrimonio + "', '" + equipamento[i].num_controle + "', '" + equipamento[i].marca + "', '" + equipamento[i].tipo + "', '" + equipamento[i].modelo + "')";
                            if (!Conn.Execute(query))
                            {
                                throw new Exception("Falha ao inserir registro na tabela app_equipamentos. " + Conn.ErrorMsg);
                            }
                        }
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
        public JsonResult update(Equipamentos equipamento)
        {
            Retorno Retorno = processaUpdate(equipamento);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaUpdate(Equipamentos equipamento)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    //var query = "SELECT * FROM app_equipamentos WHERE num_controle != '" + equipamento.num_controle + "'";
                    //Conn.Query(query);

                    //if (Conn.getRows() > 0)
                    //{
                    //    throw new Exception("Tipo de status já cadastrado.");
                    //}

                    var query = "UPDATE app_equipamentos SET cod_patrimonio = '" + equipamento.cod_patrimonio + "', num_controle = '" + equipamento.num_controle + "', marca = '" + equipamento.marca + "', tipo = '" + equipamento.tipo + "', modelo = '" + equipamento.modelo + "', status_ativo = '" + equipamento.status_ativo + "' WHERE cod_equipamento = '" + equipamento.cod_equipamento + "' ";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao atualizar registro na tabela app_equipamentos. " + Conn.ErrorMsg);
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
        public JsonResult select(bool agendamento = false)
        {
            Retorno Retorno = processaSelect(agendamento);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaSelect(bool agendamento = false)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaEquipamentos = new List<Equipamentos>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_equipamentos WHERE 1 = 1";

                    if (agendamento)
                    {
                        query += " AND status_ativo = 'S'"; 
                    }

                    DataTable tableEquipamentos = Conn.getDataTable(query);

                    foreach (DataRow dtEquipamentos in tableEquipamentos.Rows)
                    {
                        Equipamentos equipamentoExistente = new Equipamentos();

                        equipamentoExistente.cod_equipamento = int.Parse(dtEquipamentos["cod_equipamento"].ToString());
                        equipamentoExistente.cod_patrimonio = int.Parse(dtEquipamentos["cod_patrimonio"].ToString());
                        equipamentoExistente.num_controle = int.Parse(dtEquipamentos["num_controle"].ToString());
                        equipamentoExistente.marca = dtEquipamentos["marca"].ToString();
                        equipamentoExistente.tipo = dtEquipamentos["tipo"].ToString();
                        equipamentoExistente.modelo = dtEquipamentos["modelo"].ToString();
                        equipamentoExistente.status_ativo = dtEquipamentos["status_ativo"].ToString();

                        Retorno.ListaEquipamentos.Add(equipamentoExistente);
                    }

                    if (Retorno.ListaEquipamentos.Count == 0)
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
        public JsonResult get(Equipamentos equipamento)
        {
            Retorno Retorno = processaGet(equipamento);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaGet(Equipamentos equipamento)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaEquipamentos = new List<Equipamentos>();

            try
            {
                if (Conn.Open())
                {

                    var query = "SELECT * FROM app_equipamentos WHERE cod_equipamento = '" + equipamento.cod_equipamento + "' ";
                    DataTable tableEquipamentos = Conn.getDataTable(query);

                    foreach (DataRow dtEquipamentos in tableEquipamentos.Rows)
                    {
                        Equipamentos equipamentoExistente = new Equipamentos();

                        equipamentoExistente.cod_equipamento = int.Parse(dtEquipamentos["cod_equipamento"].ToString());
                        equipamentoExistente.cod_patrimonio = int.Parse(dtEquipamentos["cod_patrimonio"].ToString());
                        equipamentoExistente.num_controle = int.Parse(dtEquipamentos["num_controle"].ToString());
                        equipamentoExistente.marca = dtEquipamentos["marca"].ToString();
                        equipamentoExistente.tipo = dtEquipamentos["tipo"].ToString();
                        equipamentoExistente.modelo = dtEquipamentos["modelo"].ToString();
                        equipamentoExistente.status_ativo = dtEquipamentos["status_ativo"].ToString();

                        Retorno.ListaEquipamentos.Add(equipamentoExistente);
                    }

                    if (Retorno.ListaEquipamentos.Count == 0)
                    {
                        throw new Exception("Não a registros a ser exibidos.");
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

        // monta lista de equipamentos disponiveis para agendamento em determinada data
        //[HttpPost]
        //public JsonResult getEquipamentosDisponiveis(HoraAgendamento datahora)
        //{
        //    Retorno Retorno = processaGetEquipamentosDisponiveis(datahora);
        //    return API_Rest.Json.Serialize(Retorno);
        //}
        //public Retorno processaGetEquipamentosDisponiveis(HoraAgendamento datahora)
        //{
        //    Retorno Retorno = new Retorno();
        //    DB Conn = new DB();
        //    Retorno.ListaEquipamentos = new List<Equipamentos>();

        //    try
        //    {
        //        if (Conn.Open())
        //        {
        //            var query = "select e.* from app_equipamentos e where e.cod_equipamento " +
        //                        "not in(select ea.cod_equipamento_agendado from app_agendamento a, app_equipamentos_agendados ea where a.agendado_para = '" + datahora.data + "' and ea.cod_agendamento = a.cod_agendamento) and e.status_ativo = 'S' ";
        //            DataTable tableEquipamentos = Conn.getDataTable(query);

        //            foreach (DataRow dtEquipamentos in tableEquipamentos.Rows)
        //            {
        //                Equipamentos equipamentoExistente = new Equipamentos();

        //                equipamentoExistente.cod_equipamento = int.Parse(dtEquipamentos["cod_equipamento"].ToString());
        //                equipamentoExistente.cod_patrimonio = int.Parse(dtEquipamentos["cod_patrimonio"].ToString());
        //                equipamentoExistente.num_controle = int.Parse(dtEquipamentos["num_controle"].ToString());
        //                equipamentoExistente.marca = dtEquipamentos["marca"].ToString();
        //                equipamentoExistente.tipo = dtEquipamentos["tipo"].ToString();
        //                equipamentoExistente.modelo = dtEquipamentos["modelo"].ToString();
        //                equipamentoExistente.status_ativo = dtEquipamentos["status_ativo"].ToString();

        //                Retorno.ListaEquipamentos.Add(equipamentoExistente);
        //            }

        //            if (Retorno.ListaEquipamentos.Count == 0)
        //            {
        //                throw new Exception("Não a registros a ser exibidos.");
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
        //        }

        //        Retorno.status = true;
        //        Retorno.msg = "Processamento concluido com sucesso!";
        //    }
        //    catch (Exception ex)
        //    {
        //        Retorno.status = false;
        //        Retorno.erro = ex.Message;
        //    }

        //    Conn.Close();
        //    return Retorno;
        //}

        public JsonResult inativarReativar(Equipamentos equipamento)
        {
            Retorno Retorno = processaInativarReativar(equipamento);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInativarReativar(Equipamentos equipamento)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    var query = "UPDATE app_equipamentos SET status_ativo = '" + equipamento.status_ativo + "' WHERE cod_equipamento = '" + equipamento.cod_equipamento + "' ";
                    if (!(Conn.Execute(query)))
                    {
                        throw new Exception("Falha ao Atualizar registros na tabela app_equipamentos.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso.";
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
    }
}