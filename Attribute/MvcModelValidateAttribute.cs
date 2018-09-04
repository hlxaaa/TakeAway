using Common.Extend;
using Common.Result;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using takeAwayWebApi.Models.Response;

namespace Common.Filter.Mvc
{
    /// <summary>
    /// Mvc模型验证过滤器
    /// </summary>
    public class MvcModelValidateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Model验证
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var Controller = actionContext.Controller;
            ModelStateDictionary ModelState = (ModelStateDictionary)GetPropertyValue(Controller, "ModelState");
            bool? IsValid = null;
            if (ModelState != null)
            {
                IsValid = ModelState.IsValid;
            }
            if (IsValid == false)
            {
                ResultForWebRes result = new ResultForWebRes();
                result.status = false;
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        if (!error.ErrorMessage.IsNullOrEmpty())
                        {
                            result.message += error.ErrorMessage + ",";
                        }
                    }
                }
                result.message = result.message.Remove(result.message.Count() - 1, 1);
                var JsonString = JsonConvert.SerializeObject(result);
                JsonResult jsonResult = new JsonResult();
                jsonResult.Data = result;
                jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                
                actionContext.Result = jsonResult;
            }
        }

        public object GetPropertyValue(object info, string field)
        {
            if (info == null) return null;
            Type t = info.GetType();
            IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
            return property.First().GetValue(info, null);
        }
    }
}
