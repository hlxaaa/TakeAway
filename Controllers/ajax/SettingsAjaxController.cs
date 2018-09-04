using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class SettingsAjaxController : WebAjaxController
    {
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var tab = req.tab;

            if (tab == 0)
            {
                var settings = new SettingRes();
                //TipsSettingsRes res = new TipsSettingsRes(settings);
                r.data = JsonConvert.SerializeObject(settings);
                return JsonConvert.SerializeObject(r);
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
                r.data = JsonConvert.SerializeObject(listR);
                return JsonConvert.SerializeObject(r);
            }
            return JsonConvert.SerializeObject(r);
        }

        public string update(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var lv = req.lv;
            var id = req.id;
            var thisLv = Convert.ToInt32(Session["lv"]);
            if (thisLv < lv)
            {
                r.Message = "权限不够";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);

            }
            var account = req.account;
            if (!StringHelperHere.Instance.IsAccountValidate(account))
            {
                r.Message = "账号应为5~16位英文字母、数字";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
            }
            if (ControllerHelper.Instance.IsExists("platform", "account", account, id.ToString()))
            {
                r.Message = "已存在该账号";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
            }
            var pwd = req.pwd;
            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
            {
                r.Message = "密码应为6~16位英文字母、数字";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);

            }


            Platform pf = new Platform();
            pf.id = id;
            pf.level = lv;
            pf.account = account;
            pf.pwd = pwd;
            PlatformOper.Instance.Update(pf);
            //return JsonHelperHere.JsonMsg(true, "");
            return JsonConvert.SerializeObject(r);

        }

        /// <summary>
        /// 添加平台账号
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string add(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 500,
                Message = "",
                data = "{}"
            };
            var lv = req.lv;
            var thisLv = Convert.ToInt32(Session["lv"]);
            var account = req.account;
            var pwd = req.pwd;
            if (thisLv < lv)
                r.Message = "权限不够";
            else if (!StringHelperHere.Instance.IsAccountValidate(account))
                r.Message = "账号应为5~16位英文字母、数字";
            else if (ControllerHelper.Instance.IsExists("platform", "account", account, "0"))
                r.Message = "已存在该账号";
            else if (!StringHelperHere.Instance.IsPwdValidate(pwd))
                r.Message = "密码应为6~16位英文字母、数字";
            else
            {
                r.HttpCode = 200;
                Platform pf = new Platform();
                pf.level = lv;
                pf.account = account;
                pf.pwd = pwd;
                PlatformOper.Instance.Add(pf);
                //return JsonHelperHere.JsonMsg(true, "");
            }
            return JsonConvert.SerializeObject(r);
        }

        public string delete(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var id = req.id;
            Platform pf = new Platform();
            pf.id = id;
            pf.isDeleted = true;
            PlatformOper.Instance.Update(pf);
            //return JsonHelperHere.JsonMsg(true, "");
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 更新所有配置
        /// </summary>
        /// <param name="req"></param>
        public string UpdateAll(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
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
            return JsonConvert.SerializeObject(r);
        }

    }
}
