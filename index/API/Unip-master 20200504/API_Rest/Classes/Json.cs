using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using API_Rest.Models;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace API_Rest
{
    public class Json : Controller
    {
        public static string GetJsonGigante(Controller c)
        {
            Stream req = c.Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();
            return json;
        }

        public static JsonResult Serialize(Retorno ret)
        {
            var JsonInstance = new API_Rest.Json();
            var JsonResult = JsonInstance.getJsonResult(ret);
            return JsonResult;
        }

        private JsonResult getJsonResult(Retorno ret)
        {
            var JsonRet = Json(ret, JsonRequestBehavior.AllowGet);
            JsonRet.MaxJsonLength = 2147483647;
            return JsonRet;
        }
    }
}