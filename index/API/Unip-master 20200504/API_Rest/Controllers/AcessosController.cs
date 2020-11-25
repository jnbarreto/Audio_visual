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
    public class AcessosController : Controller
    {

        [HttpPost]
        public Retorno insert(Acessos acessos, DB Conn = null, Retorno Retorno = null)
        {
            try
            {
                var query = "SELECT * FROM app_acessos WHERE login = '"+acessos.login+"'";
                Conn.Query(query);

                if (Conn.getRows() > 0)
                {
                    throw new Exception("Login de acesso já cadastrado.");
                }

                var cod_colaborador = Conn.getID("app_colaboradores", "cod_colaborador", "cod_colaborador");
                query = "INSERT INTO app_acessos(cod_colaborador, login, senha, status_ativo, auto_login) VALUES('" + cod_colaborador + "', '" + acessos.login + "', '" + Sistema.EncodeBase64(acessos.senha) + "', '"+acessos.status_ativo+"', false)";
                if (!(Conn.Execute(query)))
                {
                    throw new Exception("Falha ao inserir registros na tabela app_acessos");
                }

                Retorno.status = true;
            }
            catch (Exception e)
            {
                Retorno.erro = e.Message;
                Retorno.status = false;                
            }

            return Retorno;
        }

        [HttpPost]
        public JsonResult select()
        {
            Retorno Retorno = ProcessaSelect();
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaSelect()
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            Retorno.ListaAcessos = new List<Acessos>();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_acessos";
                    DataTable dtacesso = Conn.getDataTable(query);

                    foreach (DataRow row in dtacesso.Rows)
                    {
                        Acessos acesso = new Acessos();
                        acesso.cod_acesso = int.Parse(row["cod_acesso"].ToString());
                        acesso.cod_colaborador = int.Parse(row["cod_colaborador"].ToString());
                        acesso.login = row["login"].ToString();
                        acesso.senha = row["senha"].ToString();
                        acesso.status_ativo = row["status_ativo"].ToString();
                        acesso.status_ativo = row["auto_login"].ToString();
                        acesso.status_ativo = row["cod_auto_login"].ToString();

                        Retorno.ListaAcessos.Add(acesso);
                    }

                    if (Retorno.ListaAcessos.Count == 0)
                    {
                        Retorno.status = false;
                        Retorno.erro = "Não a registros a ser exibidos";
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso.";
            }
            catch (Exception e)
            {
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        //[HttpPost]
        //public JsonResult get(Acessos acesso)
        //{
        //    Retorno Retorno = ProcessaGet(acesso);
        //    return API_Rest.Json.Serialize(Retorno);
        //}

        public Acessos ProcessaGet(string cod_colaborador = "", DB Conn = null, Retorno Retorno = null)
        {
            Acessos objAcesso = new Acessos();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_acessos WHERE cod_colaborador = '"+cod_colaborador+"'";
                    DataTable dtacesso = Conn.getDataTable(query);

                    foreach (DataRow row in dtacesso.Rows)
                    {
                        Acessos acesso = new Acessos();
                        acesso.cod_acesso = int.Parse(row["cod_acesso"].ToString());
                        acesso.cod_colaborador = int.Parse(row["cod_colaborador"].ToString());
                        acesso.login = row["login"].ToString();
                        acesso.senha = row["senha"].ToString();
                        acesso.status_ativo = row["status_ativo"].ToString();

                        objAcesso = acesso;
                    }

                    if (Retorno.ListaAcessos.Count == 0)
                    {
                        throw new Exception("Não a registros a ser exibidos");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso.";
            }
            catch (Exception e)
            {
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return objAcesso;
        }

        public Retorno update(Acessos acesso, DB Conn, Retorno Retorno)
        {
            try
            {
                //var query = "SELECT * FROM app_acessos WHERE login = '"+acesso.login+"'";
                //Conn.Query(query);

                //if (Conn.getRows() > 0)
                //{
                //    throw new Exception("Acesso já cadastrado para outro colaborador.");
                //}

                var query = "UPDATE app_acessos SET login = '"+acesso.login+"', senha = '"+acesso.senha+"', status_ativo = '"+acesso.status_ativo+"' WHERE cod_colaborador = '"+acesso.cod_colaborador + "' ";
                if (!(Conn.Execute(query)))
                {
                    throw new Exception("Falha ao Atualizar registros na tabela app_acessos.");
                }
                
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso.";
            }
            catch (Exception e)
            {
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            return Retorno;
        }

        public JsonResult inativarReativar(Acessos acesso)
        {
            Retorno Retorno = processaInativarReativar(acesso);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInativarReativar(Acessos acesso)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();

                    var query = "UPDATE app_acessos SET status_ativo = '" + acesso.status_ativo + "' WHERE cod_colaborador = '" + acesso.cod_colaborador + "' ";
                    if (!(Conn.Execute(query)))
                    {
                        throw new Exception("Falha ao Atualizar registros na tabela app_acessos.");
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