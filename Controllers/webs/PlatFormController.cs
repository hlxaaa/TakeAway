using DbOpertion.DBoperation;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;

namespace takeAwayWebApi.Controllers.webs
{
    public class PlatFormController : Controller
    {
        //
        // GET: /Login/
        //PlatformOper pOper = new PlatformOper();
        public ActionResult Index()
        {
           // 没用了
 
            return View();
        }

        //public ActionResult _Login() {
        //    LoginOut
        //    return View("_Login");
        //}

        public string Login(webReq req) {
            var account = req.pAccount;
            var pwd = req.pPwd;
            var pf = PlatformOper.Instance.GetByAccount(account);
            if (pf == null)
                return JsonHelperHere.JsonMsg(false, "没有这个账号");
            if(pf.pwd!=pwd)
                return JsonHelperHere.JsonMsg(false, "密码错误");
            Session.Timeout = 60;
            Session["pfId"] = pf.id;
            Session["lv"] = pf.level;

            return JsonHelperHere.JsonMsg(true, "登录成功");
        }

        public void LoginOut()
        {
            Session.Abandon();
        }

        public string ClearSession() {
            Session.RemoveAll();
            return ""; 
        }
    }
}
