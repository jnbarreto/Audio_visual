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
    public class ParametrosController : Controller
    {
        public Parametro getParametro(string parametro = "", Retorno Retorno = null, DB Conn = null)
        {
            Parametro objParametro = new Parametro();
			try
			{
                if (Conn.Open())
                {
                    var query = "SELECT * FROM app_audiovisual_parametros WHERE parametro = '" + parametro + "'";
                    DataTable dtparametro = Conn.getDataTable(query);

                    foreach (DataRow row in dtparametro.Rows)
                    {
                        Parametro p = new Parametro();

                        p.cod_parametro = int.Parse(row["cod_parametro"].ToString());
                        p.parametro = row["parametro"].ToString();
                        p.valor = row["valor"].ToString();
                        p.parametro_ativo = row["parametro_ativo"].ToString();
                        p.personalizacao = row["personalizacao"].ToString();

                        objParametro = p;
                    }

                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

            }
            catch (Exception e)
			{
                Retorno.status = false;
                Retorno.erro = e.Message;
			}

            return objParametro;
        }
    }
}