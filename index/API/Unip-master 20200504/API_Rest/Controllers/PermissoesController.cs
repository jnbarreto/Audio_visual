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
    public class PermissoesController : Controller
    {

        [HttpPost]
        public JsonResult select()
        {
            Retorno Retorno = processaSelect();
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaSelect()
        {
            Retorno Retorno = new Retorno();
            Retorno.ListaPermissoes = new List<Permissoes>();
            DB Conn = new DB();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_permissoes";
                    DataTable permissoes = Conn.getDataTable(query);

                    foreach (DataRow dtPermissoes in permissoes.Rows)
                    {
                        Permissoes permissaoExistente = new Permissoes();

                        permissaoExistente.cod_permissao = int.Parse(dtPermissoes["cod_permissao"].ToString());
                        permissaoExistente.tipo_permissao = dtPermissoes["tipo_permissao"].ToString();
                        permissaoExistente.descricao_permissao = dtPermissoes["descricao_permissao"].ToString();

                        Retorno.ListaPermissoes.Add(permissaoExistente);
                    }

                    if (Retorno.ListaPermissoes.Count == 0)
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
        public JsonResult get(Permissoes permissoes)
        {
            Retorno Retorno = ProcessaGet(permissoes);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaGet(Permissoes permissoes)
        {
            Retorno Retorno = new Retorno();
            Retorno.ListaPermissoes = new List<Permissoes>();
            DB Conn = new DB();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_permissoes WHERE cod_permissao = '" + permissoes.cod_permissao + "'";
                    DataTable permissao = Conn.getDataTable(query);

                    foreach (DataRow dtPermissoes in permissao.Rows)
                    {
                        Permissoes permissaoExistente = new Permissoes();

                        permissaoExistente.cod_permissao = int.Parse(dtPermissoes["cod_permissao"].ToString());
                        permissaoExistente.tipo_permissao = dtPermissoes["tipo_permissao"].ToString();
                        permissaoExistente.descricao_permissao = dtPermissoes["descricao_permissao"].ToString();

                        Retorno.ListaPermissoes.Add(permissaoExistente);
                    }

                    if (Retorno.ListaPermissoes.Count == 0)
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

        public JsonResult insert(Permissoes permissoes)
        {
            Retorno Retorno = ProcessaInsert(permissoes);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaInsert(Permissoes permissoes)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    var query = "SELECT * FROM app_permissoes where tipo_permissao = '" + permissoes.tipo_permissao + "'";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Permissão já Cadastrada.");
                    }

                    //var cod = Conn.getNextID("app_permissao", "cod_permissao");
                    query = "INSERT INTO app_permissoes(tipo_permissao, descricao_permissao) VALUES ('" + permissoes.tipo_permissao + "','" + permissoes.descricao_permissao + "') ";

                    if (!(Conn.Execute(query)))
                    {
                        throw new Exception("Falha ao incluir registro na tabela app_permissoes. " + Conn.ErrorMsg);
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
            catch (Exception ex)
            {
                Conn.Rollback();
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }

        public JsonResult update(Permissoes permissoes)
        {
            Retorno Retorno = ProcessaUpdate(permissoes);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaUpdate(Permissoes permissoes)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    var query = "SELECT * FROM app_permissoes WHERE tipo_permissao = '" + permissoes.tipo_permissao + "' ";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Tipo de Permissão já existente");
                    }

                    query = "UPDATE app_permissoes SET tipo_permissao = '" + permissoes.tipo_permissao + "', descricao_permissao = '" + permissoes.descricao_permissao + "' WHERE cod_permissao = '" + permissoes.cod_permissao + "'";
                    if (!(Conn.Execute(query)))
                    {
                        throw new Exception("Falha ao atualizar registro na tabela app_permissoes." + Conn.ErrorMsg);
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
            catch (Exception ex)
            {
                Conn.Rollback();
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }

    }
}