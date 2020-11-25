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
    public class SolicitacaoCadastroController : Controller
    {
        [HttpPost]
        public JsonResult insert(solicitacaoCadastro colaborador)
        {
            Retorno Retorno = processaInsert(colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInsert(solicitacaoCadastro colaborador)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            List<Email> ListaEmail = new List<Email>();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_solic_cadastros WHERE funcional = '" + colaborador.funcional + "'";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Solicitação já efetuada. Por favor aguarde o retorno de nossa equipe.");
                    }

                    Conn.Begin();

                    query = "INSERT INTO app_solic_cadastros(nome, email, contato, funcional, status_cadastro) VALUES('" + colaborador.nome + "', '" + colaborador.email + "', '" + colaborador.contato + "', '" + colaborador.funcional + "', 'N')";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao efetuar solicitação de cadastro." + Conn.ErrorMsg);
                    }

                    var body = "Nome Coloborador : " + colaborador.nome + "</br>" +
                               "Email : " + colaborador.email + "</br>" +
                               "Contato : " + colaborador.contato + "</br>" +
                               "Funcional : " + colaborador.funcional;

                    // GET EMAIL ADMINISTRADORES
                    GetEmail getEmail = new GetEmail();
                    getEmail.getEmailAdmin(ListaEmail , Conn);

                    //EFETUA O ENVIO DE EMAIL
                    EnviaEmail enviar = new EnviaEmail();
                    enviar.sendEmail(body, ListaEmail, "Nova Solicitação de Cadastro de Acesso.");

                    Conn.Commit();
                    Retorno.status = true;
                    Retorno.msg = "Solicitação de Cadastro efetuado com sucesso! ";
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                Conn.Rollback();
                Retorno.erro = e.Message;
                Retorno.status = false;
            }

            Conn.Close();
            return Retorno;
        }

        public Retorno update(Colaboradores colaborador, DB Conn = null, Retorno Retorno = null)
        {
            try
            {
                var query = "UPDATE app_solic_cadastros SET status_cadastro = 'C' WHERE funcional = '" + colaborador.funcional + "' ";
                if (!(Conn.Execute(query)))
                {
                    throw new Exception("Falha ao Atualizar registros na tabela app_solic_cadastros. " + Conn.ErrorMsg);
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

        public JsonResult select()
        {
            Retorno Retorno = processaSelect();
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaSelect()
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            Retorno.ListaSolicCadastro = new List<solicitacaoCadastro>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_solic_cadastros WHERE status_cadastro = 'N' ";
                    DataTable solicCadastro = Conn.getDataTable(query);

                    foreach (DataRow row in solicCadastro.Rows)
                    {
                        solicitacaoCadastro solicitacaoExistente = new solicitacaoCadastro();

                        solicitacaoExistente.cod_solic = int.Parse( row["cod_solic"].ToString() );
                        solicitacaoExistente.nome = row["nome"].ToString();
                        solicitacaoExistente.email = row["email"].ToString();
                        solicitacaoExistente.contato = row["contato"].ToString();
                        solicitacaoExistente.funcional = row["funcional"].ToString();
                        solicitacaoExistente.status_cadastros = row["status_cadastro"].ToString();

                        Retorno.ListaSolicCadastro.Add(solicitacaoExistente);
                    }

                    if (Retorno.ListaSolicCadastro.Count == 0)
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


        public JsonResult get(solicitacaoCadastro colaborador)
        {
            Retorno Retorno = processaGet(colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaGet(solicitacaoCadastro colaborador)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            Retorno.ListaSolicCadastro = new List<solicitacaoCadastro>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_solic_cadastros WHERE status_cadastro = 'N' AND cod_solic = '" + colaborador.cod_solic + "'";
                    DataTable solicCadastro = Conn.getDataTable(query);

                    foreach (DataRow row in solicCadastro.Rows)
                    {
                        solicitacaoCadastro solicitacaoExistente = new solicitacaoCadastro();

                        solicitacaoExistente.cod_solic = int.Parse(row["cod_solic"].ToString());
                        solicitacaoExistente.nome = row["nome"].ToString();
                        solicitacaoExistente.email = row["email"].ToString();
                        solicitacaoExistente.contato = row["contato"].ToString();
                        solicitacaoExistente.funcional = row["funcional"].ToString();
                        solicitacaoExistente.status_cadastros = row["status_cadastro"].ToString();

                        Retorno.ListaSolicCadastro.Add(solicitacaoExistente);
                    }

                    if (Retorno.ListaSolicCadastro.Count == 0)
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

        public JsonResult getRF(string funcional)
        {
            Retorno Retorno = processaGetRF(funcional);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaGetRF(string funcional)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            Retorno.ListaSolicCadastro = new List<solicitacaoCadastro>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_solic_cadastros WHERE funcional = '" + funcional + "'";
                    DataTable solicCadastro = Conn.getDataTable(query);

                    foreach (DataRow row in solicCadastro.Rows)
                    {
                        solicitacaoCadastro solicitacaoExistente = new solicitacaoCadastro();

                        solicitacaoExistente.status_cadastros = row["status_cadastro"].ToString();

                        Retorno.ListaSolicCadastro.Add(solicitacaoExistente);
                    }

                    if (Retorno.ListaSolicCadastro.Count == 0)
                    {
                        Retorno.status = true;
                        Retorno.msg = "Não a registros a ser exibidos.";
                    }
                    else 
                    {
                        if (Retorno.ListaSolicCadastro[0].status_cadastros == "N")
                        {
                            throw new Exception("Sua solicitação de cadastro se encontra pendente desculpe o atraso. <br> Entraremos em contato assim que analisarmos sua solicitação");
                        }

                        if (Retorno.ListaSolicCadastro[0].status_cadastros == "C")
                        {
                            throw new Exception("Olá, tudo bem ? <br> Verificamos que você já possui cadastro. Gostaria de...");
                        }
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                Retorno.erro = e.Message;
                Retorno.status = false;
            }

            Conn.Close();
            return Retorno;
        }


        [HttpPost]
        public JsonResult delete(solicitacaoCadastro colaborador)
        {
            Retorno Retorno = processaDelete(colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaDelete(solicitacaoCadastro colaborador)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            try
            {
                if (Conn.Open())
                {
                    var query = "DELETE from app_solic_cadastros WHERE status_cadastro = 'N' and funcional = '" + colaborador.funcional + "'";
                    Conn.Query(query);

                    Retorno.status = true;
                    Retorno.msg = "Solicitação deletada com sucesso!";
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
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