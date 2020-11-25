using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using API_Rest.Models;
using API_Rest.Classes;

namespace API_Rest.Controllers
{
    public class CargosController : Controller
    {
        [HttpPost]
        public JsonResult insert(Cargos cargo)
        {
            Retorno Retorno = processaInsert(cargo);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInsert(Cargos cargo)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();
                    var query = "SELECT * FROM app_cargos WHERE tipo_cargo = '" + cargo.tipo_cargo + "'";
                    Conn.Query(query);

                    if(Conn.getRows() > 0)
                    {
                        throw new Exception("Cargo já cadastrado. " + Conn.ErrorMsg);
                    }

                    query = "INSERT INTO app_cargos (tipo_cargo, descricao_cargo) VALUES ('" + cargo.tipo_cargo + "', '" + cargo.descricao_cargo + "')";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao inserir registro na tabela app_cargos. " + Conn.ErrorMsg);
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada. " + Conn.ErrorMsg);
                }
                
                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
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
            Retorno.ListaCargos = new List<Cargos>();
            
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_cargos";
                    DataTable tableCargos = Conn.getDataTable(query);

                    foreach(DataRow dtCargos in tableCargos.Rows)
                    {
                        Cargos cargoExistente = new Cargos();

                        cargoExistente.cod_cargo        = int.Parse(dtCargos["cod_cargo"].ToString());
                        cargoExistente.tipo_cargo       = dtCargos["tipo_cargo"].ToString();
                        cargoExistente.descricao_cargo  = dtCargos["descricao_cargo"].ToString();

                        Retorno.ListaCargos.Add(cargoExistente);
                    }

                    if (Retorno.ListaCargos.Count == 0)
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
        public JsonResult get(Cargos cargo)
        {
            Retorno Retorno = processaGet(cargo);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaGet(Cargos cargo)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaCargos = new List<Cargos>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_cargos WHERE cod_cargo = '" + cargo.cod_cargo + "'";
                    DataTable tableCargos = Conn.getDataTable(query);

                    foreach (DataRow dtCargos in tableCargos.Rows)
                    {
                        Cargos cargoExistente = new Cargos();

                        cargoExistente.cod_cargo = int.Parse(dtCargos["cod_cargo"].ToString());
                        cargoExistente.tipo_cargo = dtCargos["tipo_cargo"].ToString();
                        cargoExistente.descricao_cargo = dtCargos["descricao_cargo"].ToString();

                        Retorno.ListaCargos.Add(cargoExistente);
                    }

                    if (Retorno.ListaCargos.Count == 0)
                    {
                        Retorno.status = false;
                        Retorno.erro = "Não a registros a ser exibidos.";
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada. " + Conn.ErrorMsg);
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
        public JsonResult update(Cargos cargo)
        {
            Retorno Retorno = processaUpdate(cargo);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaUpdate(Cargos cargo)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();
                    var query = "SELECT * FROM app_cargos WHERE tipo_cargo = '" + cargo.tipo_cargo + "'";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Cargo já cadastrado.");
                    }

                    query = "UPDATE app_cargos SET tipo_cargo = '" + cargo.tipo_cargo + "', descricao_cargo = '" + cargo.descricao_cargo + "' WHERE cod_cargo = '"+cargo.cod_cargo+"'";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao inserir registro na tabela app_cargos. " + Conn.ErrorMsg);
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
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
