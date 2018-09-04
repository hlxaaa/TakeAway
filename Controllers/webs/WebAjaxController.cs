using Common.Filter.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.webs
{
    [MvcException]
    public class WebAjaxController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.checkLogin())// 判断是否登录
            {
                ResultForWeb r = new ResultForWeb();
                r.HttpCode = 300;
                r.Message = "";
                r.data = "{}";
                JsonResult jr = new JsonResult();
                jr.ContentType = "application/json";
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                jr.Data = r;
                filterContext.Result = jr;
            }
            //base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 判断是否登录
        /// </summary>
        public bool checkLogin()
        {
            var a = Session["pfId"];
            if (a == null)
            {
                return false;
            }
            return true;
        }

    }
}
