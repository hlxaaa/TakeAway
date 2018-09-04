using Newtonsoft.Json;
using System.Web.Mvc;
using takeAwayWebApi.Models.Response;

namespace Common.Filter.Mvc
{
    /// <summary>
    /// Mvc异常过滤器
    /// </summary>
    public class MvcExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// 发生错误之后
        /// </summary>
        /// <param name="filterContext">条件内容</param>
        //public  void OnException(ExceptionContext filterContext)
        //{
        //    JsonResult jsonResult = new JsonResult();
        //    ResultForWebRes result = new ResultForWebRes();
        //    result.status = false;
        //    result.message = filterContext.Exception.Message;
        //    jsonResult.Data = result;
        //    filterContext.Result = jsonResult;
        //    filterContext.ExceptionHandled = true;//这样表示异常已经处理，status code会是200，然后我的result也能返回给ajax的success
        //}

        /// <summary>
        /// 发生错误之后
        /// </summary>
        /// <param name="filterContext">条件内容</param>
        public void OnException(ExceptionContext filterContext)
        {
            JsonResult jsonResult = new JsonResult();
            ResultForWeb result = new ResultForWeb();
            result.HttpCode = 500;
            result.Message = filterContext.Exception.Message;
            jsonResult.Data = result;
            jsonResult.ContentType = "application/json";
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            filterContext.Result = jsonResult;
            filterContext.ExceptionHandled = true;
        }

    }
}
