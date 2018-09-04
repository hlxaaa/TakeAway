using Common.Filter.Mvc;
using Newtonsoft.Json;
using System.Web.Mvc;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.webs
{
    //[MvcExceptionAttribute]
    public class WebBaseController : Controller
    {
        protected string hostUrl = "";
        /// <summary>
        /// Action执行前判断
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // url
            this.hostUrl = "http://" + this.Request.Url.Host;
            this.hostUrl += this.Request.Url.Port.ToString() == "80" ? "" : ":" + this.Request.Url.Port;
            this.hostUrl += this.Request.ApplicationPath;
            if (!this.checkLogin())// 判断是否登录
            {
                filterContext.Result = View("_Login");
            }
            base.OnActionExecuting(filterContext);
        }


        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // url
        //    this.hostUrl = "http://" + this.Request.Url.Host;
        //    this.hostUrl += this.Request.Url.Port.ToString() == "80" ? "" : ":" + this.Request.Url.Port;
        //    this.hostUrl += this.Request.ApplicationPath;
        //    if (!this.checkLogin())// 判断是否登录
        //    {
        //        ResultForWeb r = new ResultForWeb();
        //        r.HttpCode = 300;
        //        r.Message = "";
        //        r.data = "{}";
        //        JsonResult jr = new JsonResult();
        //        jr.ContentType = "application/json";
        //        jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //        jr.Data = r;
        //        filterContext.Result = jr;
        //        //filterContext.Result = View("_Login");
        //    }
        //    base.OnActionExecuting(filterContext);
        //}


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
