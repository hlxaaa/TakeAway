using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Mvc;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.webs
{
    public class SettingsController : WebBaseController
    {
        //
        // GET: /Settings/
        //PlatformOper pfOper = new PlatformOper();
        public ActionResult Index()
        {
            var settings = new SettingRes();
            ViewBag.settings = settings;

            var pf = PlatformOper.Instance.GetById(Convert.ToInt32(Session["pfId"]));
            ViewBag.pfLevel = pf.level;
            return View();

        }

        public string Get(webReq req)
        {
            var tab = req.tab;

            if (tab == 0)
            {
                var settings = new SettingRes();
                //TipsSettingsRes res = new TipsSettingsRes(settings);
                return JsonConvert.SerializeObject(settings);
            }
            else if (tab == 1)
            {
                var thisId = Convert.ToInt32(Session["pfId"]);
                var pf = PlatformOper.Instance.GetById(thisId);
                var list = PlatformOper.Instance.GetByLv(pf.level.ToString());
                var listR = new List<PlatformRes>();
                foreach (var item in list)
                {
                    PlatformRes res = new PlatformRes(item);
                    listR.Add(res);
                }
                return JsonConvert.SerializeObject(listR);
            }
            return "";
        }

        public string update(webReq req)
        {
            var lv = req.lv;
            var id = req.id;
            var thisLv = Convert.ToInt32(Session["lv"]);
            if (thisLv < lv)
                return JsonHelperHere.JsonMsg(false, "权限不够");
            var account = req.account;
            if (!StringHelperHere.Instance.IsAccountValidate(account))
                return JsonHelperHere.JsonMsg(false, "账号应为5~16位英文字母、数字");
            if (ControllerHelper.Instance.IsExists("platform", "account", account, id.ToString()))
                return JsonHelperHere.JsonMsg(false, "已存在该账号");
            var pwd = req.pwd;
            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
                return JsonHelperHere.JsonMsg(false, "密码应为6~16位英文字母、数字");


            Platform pf = new Platform();
            pf.id = id;
            pf.level = lv;
            pf.account = account;
            pf.pwd = pwd;
            PlatformOper.Instance.Update(pf);
            return JsonHelperHere.JsonMsg(true, "");
        }

        /// <summary>
        /// 添加平台账号
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string add(webReq req)
        {
            var lv = req.lv;
            var thisLv = Convert.ToInt32(Session["lv"]);
            if (thisLv < lv)
                return JsonHelperHere.JsonMsg(false, "权限不够");
            var account = req.account;
            if (!StringHelperHere.Instance.IsAccountValidate(account))
                return JsonHelperHere.JsonMsg(false, "账号应为5~16位英文字母、数字");
            if (ControllerHelper.Instance.IsExists("platform", "account", account, "0"))
                return JsonHelperHere.JsonMsg(false, "已存在该账号");
            var pwd = req.pwd;
            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
                return JsonHelperHere.JsonMsg(false, "密码应为6~16位英文字母、数字");
            Platform pf = new Platform();
            pf.level = lv;
            pf.account = account;
            pf.pwd = pwd;
            PlatformOper.Instance.Add(pf);
            return JsonHelperHere.JsonMsg(true, "");
        }

        public string delete(webReq req)
        {
            var id = req.id;
            Platform pf = new Platform();
            pf.id = id;
            pf.isDeleted = true;
            PlatformOper.Instance.Update(pf);
            return JsonHelperHere.JsonMsg(true, "");
        }

        /// <summary>
        /// 更新所有配置
        /// </summary>
        /// <param name="req"></param>
        public void UpdateAll(webReq req)
        {
            var couponSaveDays = req.couponSaveDays;
            var riderCancelTips = req.riderCancelTips;
            var recoveryBoxTips = req.recoveryBoxTips;
            var orderSendingTips = req.orderSendingTips;
            var orderArrivelTips = req.orderArrivelTips;
            var boxGetTips = req.boxGetTips;
            var serverAssignRiderTips = req.serverAssignRiderTips;
            var discount = req.discount;
            var shopNewReserveOrderTips = req.shopNewReserveOrderTips;
            var shopNewOrderTips = req.shopNewOrderTips;

            Configuration cfa = WebConfigurationManager.OpenWebConfiguration("~");
            cfa.AppSettings.Settings["couponSaveDays"].Value = couponSaveDays;
            cfa.AppSettings.Settings["riderCancelTips"].Value = riderCancelTips;
            cfa.AppSettings.Settings["recoveryBoxTips"].Value = recoveryBoxTips;
            cfa.AppSettings.Settings["orderSendingTips"].Value = orderSendingTips;
            cfa.AppSettings.Settings["orderArrivelTips"].Value = orderArrivelTips;
            cfa.AppSettings.Settings["boxGetTips"].Value = boxGetTips;
            cfa.AppSettings.Settings["serverAssignRiderTips"].Value = serverAssignRiderTips;
            cfa.AppSettings.Settings["shopNewReserveOrderTips"].Value = shopNewReserveOrderTips;
            cfa.AppSettings.Settings["shopNewOrderTips"].Value = shopNewOrderTips;
            cfa.AppSettings.Settings["discount"].Value = discount;
            cfa.Save();
        }

    }
}
