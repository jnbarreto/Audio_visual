using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace API_Rest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //protected void Application_Start()
        //{
        //    AreaRegistration.RegisterAllAreas();
        //    GlobalConfiguration.Configure(WebApiConfig.Register);
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BundleConfig.RegisterBundles(BundleTable.Bundles);
        //}

        public static Dictionary<string, string> Parametros = new Dictionary<string, string>();
        //public static Models.SMTP SMTPParams = new Models.SMTP();
        protected void Application_Start()
        {

            Parametros.Add("reenviar_notif_erro", "N");
            Parametros.Add("ativar_notificacoes", "N");
            Parametros.Add("ativar_log_query", "S");
            Parametros.Add("ativar_log", "S");

            //SMTPParams = Mail.SmtpParams();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        protected void ProcessosAutomaticos(object sender, EventArgs e)
        {
            try
            {
                //Mail.EnvioAutomatico();
            }
            catch (Exception a)
            {

            }
            finally
            {
                Thread.Sleep(30000);
                this.ProcessosAutomaticos(sender, e);
            }
        }
    }
}
