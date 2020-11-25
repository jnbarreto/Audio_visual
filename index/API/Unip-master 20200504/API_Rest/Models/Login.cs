using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Rest.Models
{
    public class Login
    {
        public string login { get; set; }
        public string senha { get; set; }
        public bool auto_login { get; set; }
    }
}