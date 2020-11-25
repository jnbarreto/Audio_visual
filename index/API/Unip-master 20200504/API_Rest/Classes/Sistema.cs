using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Classes
{
    public class Sistema
    {
        public static string EncodeBase64(string Character)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Character);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception a)
            {
                // CeosMessage.Error(a.Message);
                return Character;
            }
        }

        public static string RootPath()
        {
            return System.Web.HttpRuntime.AppDomainAppPath.ToString().Replace("\\", "/");
        }

        public static string DecodeBase64(string CharEncoded)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(CharEncoded);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception a)
            {
                //    CeosMessage.Error(a.Message);
                return CharEncoded;
            }
        }
    }
}