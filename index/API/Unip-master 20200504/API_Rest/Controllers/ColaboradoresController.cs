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
    public class ColaboradoresController : Controller
    {
        [HttpPost]
        public JsonResult insert(Colaboradores colaborador)
        {
            Retorno Retorno = ProcessaInsert(colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaInsert(Colaboradores colaborador)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            List<Email> ListaEmail = new List<Email>();
            try
            {
                if (Conn.Open())
                {
                    if (
                        String.IsNullOrWhiteSpace(colaborador.nome_colaborador) || 
                        String.IsNullOrWhiteSpace(colaborador.email) || 
                        String.IsNullOrWhiteSpace(colaborador.funcional) || 
                        String.IsNullOrWhiteSpace(colaborador.Acesso.senha)
                       )
                    {
                        throw new Exception("Por favor Preencher todos os campo(s) obrigatório(s)");
                    }
                    Conn.Begin();
                    var query = "SELECT * FROM app_colaboradores WHERE funcional = '" + colaborador.funcional+"' ";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Colaborador já Cadastrado.");
                    }
                    else
                    {

                        query = "INSERT INTO app_colaboradores(cod_cargo, cod_permissao, nome_colaborador, email, contato, funcional) " +
                                "VALUES('"+colaborador.cod_cargo+"', '"+colaborador.cod_permissao+"', '"+colaborador.nome_colaborador+"', '"+colaborador.email+"', '"+colaborador.contato+"', '"+colaborador.funcional+"')";

                        if (!(Conn.Execute(query)))
                        {
                            throw new Exception("Falha ao incluir registro na tabela app_colaboradores. " + Conn.ErrorMsg);
                        }
                        colaborador.cod_colaborador = int.Parse( Conn.getID("app_colaboradores", "cod_colaborador", "cod_colaborador") );

                        AcessosController acesso = new AcessosController();
                        Retorno = acesso.insert(colaborador.Acesso, Conn, Retorno);
                        if (!Retorno.status) 
                        {
                            throw new Exception(Retorno.erro);
                        }

                        //VERIFICA SE COLABORADOR EFETUOU SOLICITAÇÃO DE CADASTRO ATRAVES DO SISTEMA
                        //SE SIM ATUALIZA STATUS DA SOLICITAÇÃO PARA 'C' DE CADASTRADO
                        query = "SELECT * FROM app_solic_cadastros WHERE funcional = '" + colaborador.funcional + "' AND status_cadastro = 'N' ";
                        Conn.Query(query);
                        if (Conn.getRows() > 0)
                        {
                            //EXECUTA UPDATE DE SOLICITAÇÃO DE CADASTRO
                            SolicitacaoCadastroController solicCadastro = new SolicitacaoCadastroController();
                            Retorno = solicCadastro.update(colaborador, Conn, Retorno);
                            if (!Retorno.status)
                            {
                                throw new Exception(Retorno.erro);
                            }
                        }

                        //GET EMAIL COLABORADOR
                        GetEmail getEmailColaborador = new GetEmail();
                        ListaEmail = getEmailColaborador.getEmailColaborador(ListaEmail, Conn, colaborador.cod_colaborador);

                        Random random = new Random();
                        int num = random.Next(1, 9999999);
                        
                        DateTime dateTime = DateTime.UtcNow.Date;
                        string stringdate = dateTime.ToString("ddMMyyyy");
                        stringdate += num;
                        //string link = "http://localhost:8075/frontend/pages/page-primeiro-acesso.html?session='" + stringdate + "'&id='" + colaborador.cod_colaborador + "'";

                        //ENVIA EMAIL PARA COLABORADOR INFORMANDO QUE SEU CADASTRO FOI EFETUADO
                        var body = colaborador.nome_colaborador + " seu cadastro foi efetuado segue seu acesso abaixo. " +
                                   "<hr>" +
                                   "Seu login de acesso será seu Registro Funcional </br>" +
                                   "Favor acessar o link abaixo e efetuar a alteração de sua Senha. </br>" +
                                   "<a href=\"http://localhost:8075/frontend/pages/page-primeiro-acesso.html?session=" + stringdate + "&cod=" + colaborador.cod_colaborador +"\" > Clique Aqui </a>";

                        EnviaEmail enviar = new EnviaEmail();
                        enviar.sendEmail(body, ListaEmail, "Colaborador Cadastrado.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento Concluido com Sucesso.";
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
            DB Conn = new DB();

            Retorno Retorno = new Retorno();

            AcessosController acesso = new AcessosController();
            Retorno.ListaColaboradores = new List<Colaboradores>();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT colaborador.*, cargo.tipo_cargo, permissao.tipo_permissao " +
                                "FROM app_colaboradores colaborador, app_cargos cargo, app_permissoes permissao " +
                                "WHERE colaborador.cod_cargo = cargo.cod_cargo AND colaborador.cod_permissao = permissao.cod_permissao ";
                    DataTable dtColaboradores = Conn.getDataTable(query);

                    foreach (DataRow row in dtColaboradores.Rows)
                    {
                        Colaboradores colaboradoresExistentes = new Colaboradores();

                        colaboradoresExistentes.cod_colaborador = int.Parse(row["cod_colaborador"].ToString());
                        colaboradoresExistentes.cod_cargo = int.Parse(row["cod_cargo"].ToString());
                        colaboradoresExistentes.cod_permissao = int.Parse(row["cod_permissao"].ToString());
                        colaboradoresExistentes.nome_colaborador = row["nome_colaborador"].ToString();
                        colaboradoresExistentes.email = row["email"].ToString();
                        colaboradoresExistentes.contato = row["contato"].ToString();
                        colaboradoresExistentes.funcional = row["funcional"].ToString();
                        colaboradoresExistentes.tipo_cargo = row["tipo_cargo"].ToString();
                        colaboradoresExistentes.tipo_permissao = row["tipo_permissao"].ToString();

                        colaboradoresExistentes.Acesso = new Acessos();
                        colaboradoresExistentes.Acesso = acesso.ProcessaGet(row["cod_colaborador"].ToString(), Conn, Retorno);

                        Retorno.ListaColaboradores.Add(colaboradoresExistentes);
                    }


                    Retorno.ListaAcessos = null;
                    if (Retorno.ListaColaboradores.Count == 0)
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
            catch (Exception  e)
            {
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult get(Colaboradores colaborador)
        {
            Retorno Retorno = ProcessaGet(colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaGet(Colaboradores colaborador) 
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            
            AcessosController acesso = new AcessosController();
            Retorno.ListaColaboradores = new List<Colaboradores>();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT colaborador.*, cargo.tipo_cargo, permissao.tipo_permissao " +
                                "FROM app_colaboradores colaborador, app_cargos cargo, app_permissoes permissao " +
                                "WHERE colaborador.cod_cargo = cargo.cod_cargo " +
                                "AND colaborador.cod_permissao = permissao.cod_permissao " +
                                "AND colaborador.cod_colaborador = '" + colaborador.cod_colaborador + "' ";

                    DataTable dtColaboradores = Conn.getDataTable(query);

                    foreach (DataRow row in dtColaboradores.Rows)
                    {
                        Colaboradores colaboradoresExistentes = new Colaboradores();

                        colaboradoresExistentes.cod_colaborador = int.Parse(row["cod_colaborador"].ToString());
                        colaboradoresExistentes.cod_cargo = int.Parse(row["cod_cargo"].ToString());
                        colaboradoresExistentes.cod_permissao = int.Parse(row["cod_permissao"].ToString());
                        colaboradoresExistentes.nome_colaborador = row["nome_colaborador"].ToString();
                        colaboradoresExistentes.email = row["email"].ToString();
                        colaboradoresExistentes.contato = row["contato"].ToString();
                        colaboradoresExistentes.funcional = row["funcional"].ToString();
                        colaboradoresExistentes.tipo_cargo = row["tipo_cargo"].ToString();
                        colaboradoresExistentes.tipo_permissao = row["tipo_permissao"].ToString();

                        colaboradoresExistentes.Acesso = new Acessos();
                        colaboradoresExistentes.Acesso = acesso.ProcessaGet(colaborador.cod_colaborador.ToString(), Conn, Retorno);

                        Retorno.ListaColaboradores.Add(colaboradoresExistentes);
                    }

                    //Consulta dados parametros audio visual
                    ParametrosController p = new ParametrosController();
                    Parametro parametro = new Parametro();

                    parametro = p.getParametro("carregarOutrasListas", Retorno, Conn);
                    if (Convert.ToBoolean(parametro.valor))
                    {
                        CargosController cargo = new CargosController();
                        Retorno.ListaCargos = new List<Cargos>();
                        Retorno.ListaCargos = cargo.processaSelect().ListaCargos;

                        PermissoesController permissoes = new PermissoesController();
                        Retorno.ListaPermissoes = new List<Permissoes>();
                        Retorno.ListaPermissoes = permissoes.processaSelect().ListaPermissoes;
                    }

                    Retorno.ListaAcessos = null;
                    if (Retorno.ListaColaboradores.Count == 0)
                    {
                        throw new Exception("Não a registros a ser exibidos." + Conn.ErrorMsg);
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
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult update(Colaboradores colaborador)
        {
            Retorno Retorno = ProcessaUpdate(colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaUpdate(Colaboradores colaborador)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();
                    var query = "SELECT * FROM app_colaboradores WHERE nome_colaborador = '"+colaborador.nome_colaborador+"'";
                    Conn.Query(query);

                    if (Conn.getRows() == 0)
                    {
                        throw new Exception("Colaborador já Cadastrado.");
                    }
                    else
                    {
                        query = "UPDATE app_colaboradores SET cod_cargo = '" + colaborador.cod_cargo + "' , cod_permissao = '" + colaborador.cod_permissao + "', nome_colaborador = '" + colaborador.nome_colaborador + "', email = '" + colaborador.email + "', contato = '" + colaborador.contato + "', funcional = '"+colaborador.funcional+"' WHERE cod_colaborador = '" + colaborador.cod_colaborador + "'";
                        if (!(Conn.Execute(query)))
                        {
                            throw new Exception("Falha ao atualizar registro na app_colaboradores. " + Conn.ErrorMsg);
                        }

                        AcessosController acesso = new AcessosController();
                        Retorno = acesso.update(colaborador.Acesso, Conn, Retorno);
                        if (!Retorno.status)
                        {
                            throw new Exception(Retorno.erro);
                        }
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento Concluido com Sucesso.";
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
        public JsonResult getListaSolicitante(string cod_colaborador)
        {
            Retorno Retorno = ProcessaGetListaSolicitante(cod_colaborador);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaGetListaSolicitante(string cod_colaborador)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            Retorno.ListaColaboradores = new List<Colaboradores>();
            try
            {
                if (Conn.Open())
                {

                    //Consulta dados parametros audio visual
                    ParametrosController p = new ParametrosController();
                    Parametro parametro = new Parametro();
                    parametro = p.getParametro("Administrador", Retorno, Conn);

                    var valor = parametro.valor;

                    var query = "SELECT * FROM app_colaboradores " +
                                "WHERE cod_colaborador = '"+cod_colaborador+"'";
                    Conn.Query(query);
                    var cod_permissao = Conn.getValueByName("cod_permissao");

                    query = "SELECT c.* FROM app_colaboradores c, app_acessos a WHERE 1 = 1 AND c.cod_colaborador = a.cod_colaborador AND a.status_ativo = 'S' ";
                    if (valor != cod_permissao)
                    {
                        query += "AND c.cod_colaborador = '" + cod_colaborador + "'";
                    }

                    DataTable dtColaboradores = Conn.getDataTable(query);

                    foreach (DataRow row in dtColaboradores.Rows)
                    {
                        Colaboradores colaboradoresExistentes = new Colaboradores();

                        colaboradoresExistentes.cod_colaborador = int.Parse(row["cod_colaborador"].ToString());
                        colaboradoresExistentes.nome_colaborador = row["nome_colaborador"].ToString();
                        colaboradoresExistentes.funcional = row["funcional"].ToString();

                        Retorno.ListaColaboradores.Add(colaboradoresExistentes);
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
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }
    }
}