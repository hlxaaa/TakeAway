using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using takeAwayWebApi.Models.Response;
using System.Net.Http;

namespace takeAwayWebApi.Attribute
{
    /// <summary>
    /// Api模型验证过滤器
    /// </summary>
    public class ValidateModelAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid) {
                var r = new ResultResHere() { httpcode = 500, message = "错了" };
                //自定义错误信息
                var item = actionContext.ModelState.Values.Take(1).SingleOrDefault();
                r.message = item.Errors.Where(b => !string.IsNullOrWhiteSpace(b.ErrorMessage)).Take(1).SingleOrDefault().ErrorMessage;

                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.OK, r);
            }
        }
    }
}