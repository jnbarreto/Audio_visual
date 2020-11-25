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
    public class LoginController : Controller
    {

        public JsonResult login(Login acesso)
        {
            Retorno Retorno = ProcessaLogin(acesso);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaLogin(Login acesso)
        {   
            Retorno Retorno = new Retorno();

            Retorno.ListaCookies = new List<Cookies>();
            Retorno.ListaAcessos = new List<Acessos>();
            DB Conn = new DB();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_acessos WHERE login = '" + acesso.login + "' AND senha = '" + acesso.senha + "' AND status_ativo = 'S'";
                    Conn.Query(query);

                    if (Conn.getRows() == 0)
                    {
                        query = "SELECT * FROM app_acessos WHERE login = '" + acesso.login + "' AND senha = '" + Sistema.EncodeBase64(acesso.senha) + "' AND status_ativo = 'S'";
                        Conn.Query(query);
                    }

                    if (Conn.getRows() > 0)
                    {
                        var auto_login = Conn.getValueByName("auto_login");
                        if (bool.Parse(auto_login) != acesso.auto_login)
                        {
                            if (acesso.auto_login)
                            {
                                var status = ativaAutoLogin(acesso, Conn);
                                if (!status)
                                {
                                    Conn.debug("Erro ao ativar Auto login.", "", true);
                                }
                            }
                        }

                        var cod_colaborador = Conn.getValueByName("cod_colaborador");
                        query = "select colaborador.*, cargo.tipo_cargo, permissao.tipo_permissao  " +
                                "from app_colaboradores colaborador, app_cargos cargo, app_permissoes permissao " +
                                "where colaborador.cod_cargo = cargo.cod_cargo and colaborador.cod_permissao = permissao.cod_permissao " +
                                "and colaborador.cod_colaborador = '" + cod_colaborador + "'";
                        Conn.Query(query);

                        if (acesso.auto_login)
                        {
                            Retorno.ListaAcessos.Add(getAutoLoginAtivo(Conn.getValueByName("cod_colaborador")));
                        }

                        
                        Cookies cookie = new Cookies();

                        cookie.cod_cargo = int.Parse(Conn.getValueByName("cod_cargo"));
                        cookie.cod_colaborador = int.Parse(Conn.getValueByName("cod_colaborador"));
                        cookie.cod_permissao = int.Parse(Conn.getValueByName("cod_permissao"));
                        cookie.email = Conn.getValueByName("email");
                        cookie.contato = Conn.getValueByName("contato");
                        cookie.nome_colaborador = Conn.getValueByName("nome_colaborador");
                        cookie.funcional = Conn.getValueByName("funcional");
                        cookie.tipo_cargo = Conn.getValueByName("tipo_cargo");
                        cookie.tipo_permissao = Conn.getValueByName("tipo_permissao");

                        Retorno.ListaCookies.Add(cookie);
                    }

                    if (Retorno.ListaCookies.Count == 0)
                    {
                        throw new Exception("Login ou senha inválidos");
                    }


                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                AgendamentoController agendamento = new AgendamentoController();
                bool status_agendamento = agendamento.verifica_atraso();

                if (status_agendamento) 
                {
                    Retorno.status = true;
                    Retorno.msg = "Processamento concluido com sucesso!";
                }
                else
                {
                    Retorno.erro = "Falha ao verificar agendamentos em atraso.";
                }
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();

            return Retorno;
        }


        public JsonResult loginAutomatico(string cod_auto_login)
        {
            Retorno Retorno = ProcessaLoginAutomatico(cod_auto_login);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaLoginAutomatico(string cod_auto_login)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_acessos WHERE cod_auto_login = '" + cod_auto_login + "' AND status_ativo = 'S'";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        var cod_colaborador = Conn.getValueByName("cod_colaborador");
                        query = "select colaborador.*, cargo.tipo_cargo, permissao.tipo_permissao  " +
                                "from app_colaboradores colaborador, app_cargos cargo, app_permissoes permissao " +
                                "where colaborador.cod_cargo = cargo.cod_cargo and colaborador.cod_permissao = permissao.cod_permissao " +
                                "and colaborador.cod_colaborador = '" + cod_colaborador + "'";
                        Conn.Query(query);

                        Retorno.ListaCookies = new List<Cookies>();
                        Cookies cookie = new Cookies();

                        cookie.cod_cargo = int.Parse(Conn.getValueByName("cod_cargo"));
                        cookie.cod_colaborador = int.Parse(Conn.getValueByName("cod_colaborador"));
                        cookie.cod_permissao = int.Parse(Conn.getValueByName("cod_permissao"));
                        cookie.email = Conn.getValueByName("email");
                        cookie.contato = Conn.getValueByName("contato");
                        cookie.nome_colaborador = Conn.getValueByName("nome_colaborador");
                        cookie.funcional = Conn.getValueByName("funcional");
                        cookie.tipo_cargo = Conn.getValueByName("tipo_cargo");
                        cookie.tipo_permissao = Conn.getValueByName("tipo_permissao");

                        Retorno.ListaCookies.Add(cookie);
                    }

                    if (Retorno.ListaCookies.Count == 0)
                    {
                        throw new Exception("Login ou senha inválidos");
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


        public bool ativaAutoLogin(Login acesso, DB Conn = null) 
        {
            bool status = true;

            try
            {
                Random random = new Random();
                int num = random.Next(1, 9999999);

                DateTime dateTime = DateTime.UtcNow.Date;
                string stringdate = dateTime.ToString("ddMMyyyy");
                stringdate += num;

                var query = "UPDATE app_acessos SET auto_login = '" + acesso.auto_login + "', cod_auto_login = '" + Sistema.EncodeBase64( stringdate ) + "' WHERE login = '" + acesso.login + "'";
                if (!Conn.Execute(query))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

        
        public JsonResult desativaAutoLogin(string cod_auto_login)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            try
            {
                if (Conn.Open()) 
                {
                    var query = "UPDATE app_acessos SET auto_login = 'false', cod_auto_login = null WHERE cod_auto_login = '" + cod_auto_login + "'";
                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao desativar auto login.");
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
                Retorno.erro = e.Message;
                Retorno.status = false;
                Conn.debug(e.Message, "", true);
            }
            Conn.Close();
            return API_Rest.Json.Serialize(Retorno);
        }

        public Acessos getAutoLoginAtivo(string cod_colaborador)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            Acessos objAcesso = new Acessos();
            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_acessos WHERE cod_colaborador = '" + cod_colaborador + "'";
                    Conn.Query(query);

                    Acessos auto_login = new Acessos();
                    auto_login.auto_login = bool.Parse(Conn.getValueByName("auto_login"));
                    auto_login.cod_auto_login = Conn.getValueByName("cod_auto_login");

                    objAcesso = auto_login;
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
    }
}