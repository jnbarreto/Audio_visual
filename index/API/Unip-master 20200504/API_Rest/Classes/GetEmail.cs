using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API_Rest.Classes;
using API_Rest.Models;
using System.Data;
using API_Rest.Controllers;

namespace API_Rest.Classes
{
    public class GetEmail
    {
        public List<Email> getEmailAdmin(List<Email> ListaEmail = null, DB Conn = null)
        {
            // SELECT DE VERIFICACAO SE PARAMETRO ESTA ATIVO
            //var query_parametro = "SELECT * FROM app_audiovisual_parametros WHERE parametro = 'AgendEnvioEmail' ";
            //Conn.Query(query_parametro);

            //var parametro_ativo = Conn.getValueByName("parametro_ativo");
            //Consulta dados parametros audio visual
            ParametrosController p = new ParametrosController();
            Parametro parametro = new Parametro();

            parametro = p.getParametro("AgendEnvioEmail", null, Conn);
            if (parametro.parametro_ativo == "S")
            {
                if (Conn.getRows() > 0)
                {
                    var query_envioemail = "SELECT c.cod_colaborador, c.nome_colaborador, c.email FROM app_colaboradores c, app_acessos a " +
                                           "WHERE 1 = 1 " +
                                           "AND a.cod_colaborador = c.cod_colaborador " +
                                           "AND a.status_ativo = 'S' ";
                    string para = Conn.getValueByName("personalizacao");
                    if (para == "Todos")
                    {
                        query_envioemail += "AND c.cod_permissao = '" + Conn.getValueByName("valor") + "'";
                    }
                    else
                    {
                        query_envioemail += "AND c.cod_colaborador in( ";

                        var valor = Conn.getValueByName("valor").Split('|');
                        string virgula = ",";
                        for (int i = 0; i < valor.Length; i++)
                        {
                            if (i == valor.Length - 1)
                            {
                                virgula = "";
                            }
                            query_envioemail += " '" + valor[i] + "'" + virgula;
                        }
                        query_envioemail += ")";
                    }
                    DataTable emails = Conn.getDataTable(query_envioemail);

                    // ADICIONA EMAIL EMAILS NA LISTA
                    foreach (DataRow row in emails.Rows)
                    {
                        Email contatoAdmin = new Email();
                        contatoAdmin.nome = row["nome_colaborador"].ToString();
                        contatoAdmin.email = row["email"].ToString();

                        ListaEmail.Add(contatoAdmin);
                    }
                }
            }

            return ListaEmail;
        }


        public List<Email> getEmailColaborador(List<Email> ListaEmail = null, DB Conn = null , int cod_colaborador = 0)
        {
            var query = "SELECT c.nome_colaborador, c.email FROM app_colaboradores c, app_acessos a " +
                        "WHERE a.cod_colaborador = c.cod_colaborador " +
                        "AND a.status_ativo = 'S' " +
                        "AND c.cod_colaborador = '" + cod_colaborador + "'";
            Conn.Query(query);

            //ADICIONA O NOME E EMAIL DO SOLICITANDO NO OBJETO PARA O ENVIO DE CONFIRMACAO DA SOLICITACAO
            Email contato = new Email();
            contato.nome = Conn.getValueByName("nome_colaborador");
            contato.email = Conn.getValueByName("email");

            //ADICIONA OBJETO A LISTA DE EMAIL
            ListaEmail.Add(contato);

            return ListaEmail;
        }
    }
}