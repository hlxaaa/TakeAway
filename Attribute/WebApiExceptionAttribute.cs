using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using takeAwayWebApi.Models.Response;
using System.Net.Http;
using System.Configuration;

namespace takeAwayWebApi.Attribute
{
    public class WebApiExceptionAttribute : ExceptionFilterAttribute
    {
        string isServer = ConfigurationManager.AppSettings.Get("isServer");

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ResultResHere r = new ResultResHere();
            r.httpcode = 500;
            var test = actionExecutedContext.Exception.Message;
            if (isServer == "0")
                r.message = test;
            else
                r.message = "网络不稳定";
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(System.Net.HttpStatusCode.OK, r);
            base.OnException(actionExecutedContext);
        }
    }
}