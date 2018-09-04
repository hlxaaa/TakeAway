
using System.Web.Mvc;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Attribute
{
    public class AjaxLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session["pfId"] == null)// 判断是否登录
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
                //filterContext.Result = View("_Login");
            }
            //base.OnActionExecuting(filterContext);
        }


        /// <summary>
        /// 判断是否登录
        /// </summary>
        protected bool checkLogin()
        {
            //var a = Session["pfId"];
            //if (a == null)
            //{
            //    return false;
            //}
            return true;
        }
    }
}