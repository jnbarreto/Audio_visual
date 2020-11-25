using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using API_Rest.Models;
using API_Rest.Classes;
using Microsoft.Ajax.Utilities;

namespace API_Rest.Controllers
{
    public class SenhaController : Controller
    {
        public JsonResult recuperaSenha(Colaboradores col)
        {
            Retorno Retorno = ProcessaSenha(col);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno ProcessaSenha(Colaboradores col)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            List<Email> ListaEmail = new List<Email>();
            try
            {

                if (Conn.Open())
                {
                    if (

                        String.IsNullOrWhiteSpace(col.email) ||
                        String.IsNullOrWhiteSpace(col.funcional)

                       )
                    {
                        throw new Exception("Por favor Preencher todos os campo(s) obrigatório(s)");
                    }

                    var query = "SELECT * from app_colaboradores as col WHERE col.funcional = '" + col.funcional + "' and col.email = '" + col.email + "'";

                    Conn.Query(query);


                    if (Conn.getRows() > 0)
                    {
                        col.cod_colaborador = int.Parse(Conn.getValueByName("cod_colaborador"));
                        col.nome_colaborador = Conn.getValueByName("nome_colaborador");

                        //GET EMAIL COLABORADOR
                        GetEmail getEmailColaborador = new GetEmail();
                        ListaEmail = getEmailColaborador.getEmailColaborador(ListaEmail, Conn, col.cod_colaborador);

                        Random random = new Random();
                        int num = random.Next(1, 9999999);

                        DateTime dateTime = DateTime.UtcNow.Date;
                        string stringdate = dateTime.ToString("ddMMyyyy");
                        stringdate += num;
                        //string link = "http://localhost:8075/frontend/pages/page-primeiro-acesso.html?session='" + stringdate + "'&id='" + colaborador.cod_colaborador + "'";

                        //ENVIA EMAIL PARA COLABORADOR INFORMANDO QUE SEU CADASTRO FOI EFETUADO
                        var body = col.nome_colaborador + " seu cadastro foi efetuado segue seu acesso abaixo. " +
                                   "<hr>" +
                                   "Seu login de acesso será seu Registro Funcional </br>" +
                                   "Favor acessar o link abaixo e efetuar a alteração de sua Senha. </br>" +
                                   "<a href=\"http://localhost:8075/frontend/pages/page-primeiro-acesso.html?session=" + stringdate + "&cod=" + col.cod_colaborador + "\" > Clique Aqui </a>";

                        EnviaEmail enviar = new EnviaEmail();
                        enviar.sendEmail(body, ListaEmail, "Recuperação de senha");
                    }
                    else
                    {
                        throw new Exception("Usuário não encontrado.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Solicitação de senha efetuado! você logo recebera um e-mail para recupera-lá";
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();

            return Retorno;
        }
        public JsonResult update(Acessos acesso)
        {
            Retorno Retorno = UpdateSenha(acesso);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno UpdateSenha(Acessos acesso)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();

            try
            {

                if (Conn.Open())
                {
                    if (

                        String.IsNullOrWhiteSpace(acesso.login) ||
                        String.IsNullOrWhiteSpace(acesso.senha)

                       )
                    {
                        throw new Exception("Por favor Preencher todos os campo(s) obrigatório(s)");
                    }

                    var query = "SELECT * from app_acessos as acesso WHERE login = '" + acesso.login + "'";

                    Conn.Query(query);


                    if (Conn.getRows() > 0)
                    {
                        Conn.Begin();
                        query = "UPDATE app_acessos SET senha = '" + Sistema.EncodeBase64(acesso.senha) + "' WHERE login = '" + acesso.login + "' AND '" + acesso.senha + "' = '" + acesso.senha + "'";
                        if (!(Conn.Execute(query)))
                        {
                            throw new Exception("Falha ao Atualizar registros na tabela app_acessos.");
                        }
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Senha alterada com sucesso!";
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