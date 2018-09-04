using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace takeAwayWebApi.Controllers
{
    public class SettingsController : BaseController
    {
        string host = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();

        public string GetOpenImg() {
            return host + "/img/userHead/head.jpg";
        }
    }
}
