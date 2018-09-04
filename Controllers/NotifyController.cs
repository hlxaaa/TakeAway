using Aop.Api.Util;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using takeAwayWebApi.Helper;

namespace takeAwayWebApi.Controllers
{
    public class NotifyController : Controller
    {
        //
        // GET: /Notify/
        public ActionResult Index()
        {
            return View();
        }

        #region 支付宝支付异步通知
        string sellerId = "2088821922720702";
        string appId = "2017120100290167";
        string alipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArUAknZtt2dDC6PdS03VMVG2j9bkagJMbel5pJS7lKWXOjfbsM1Donk5Oj7pkORxuF0sXjRaKouCqNEikbPLreMeASaisOvVK+iaU4yaCtGQw8poceOYO+PzC43O1HryOf4MkckyfbM4QJ1QsNkLh3Wc3BJF3dZDE1sPWrOEzbNWgLdU14BiBZVbmwulmU+EGvjBTh4ST+bVnnrayrNyAUqEEgtVvcSOwurmpCf2DXnqMaA/tasFCUwqEEfCuyXzk+zGuoAPj6gLfKs783Jy1slmOiFJhUWza8ejdtsaWeWy32N9MF6XkvbZmrFSZVT7gOrTZ1W1BNbz3Thw+R0043QIDAQAB";

        /// <summary>
        /// 支付宝支付订单异步通知
        /// </summary>
        /// <returns></returns>
        public string alipayNotify()
        {
            string success = "success";
            string failure = "failure";
            try
            {
                var dict2 = GetRequestPost();
                var out_trade_no = dict2["out_trade_no"];
                var orderList = CacheHelper.GetByCondition<OrderInfo>("orderinfo", " orderNo= " + out_trade_no);
                if (orderList.Count < 0)
                    return failure;
                var order = orderList.First();
                if (order.status == 1)
                    return success;
                var outTradeNo = CheckAliPay(dict2, order.payMoney.ToString());

                txyInfo info = new txyInfo();
                info.content = "outTradeNo=====" + outTradeNo;
                txyInfoOper.Instance.Add(info);

                if (outTradeNo == null)
                    return failure;


                var areaId = order.orderAreaId;
                order.payType = "alipay";
                if ((bool)order.isTake)
                    order.status = 2;
                else
                    order.status = 1;
                order.payTime = DateTime.Now;
                OrderInfoOper.Instance.Update(order);
                var user = UserInfoOper.Instance.GetById((int)order.userId);
                user.userBalance -= order.useBalance;
                user.coupon -= order.useCoupon;
                UserInfoOper.Instance.Update(user);
                UPushHelper.Instance.PushAfterPayOrder(order);


                UserPayOper.Instance.payOrderAddRecord(order);

            }
            catch (Exception e)
            {
                txyInfo info = new txyInfo();
                info.content = e.Message;
                txyInfoOper.Instance.Add(info);
                return failure;
            }
            return success;

        }

        /// <summary>
        /// 检查阿里支付。正确返回订单号，错误返回null
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="Total_amount"></param>
        /// <returns></returns>
        public string CheckAliPay(IDictionary<string, string> dict, string Total_amount)
        {
            bool flag = AlipaySignature.RSACheckV1(dict, alipayPublicKey, "utf-8", "RSA2", false);

            txyInfo info = new txyInfo();
            info.content = JsonConvert.SerializeObject(dict + "==flag===" + flag);
            txyInfoOper.Instance.Add(info);

            if (flag)
            {
                var total_amount = dict["total_amount"];
                var out_trade_no = dict["out_trade_no"];
                string status = dict["trade_status"];
                string seller_id = dict["seller_id"];
                string app_id = dict["app_id"];

                if (total_amount == Total_amount && seller_id == sellerId && app_id == appId)
                {
                    switch (status)
                    {
                        case "TRADE_SUCCESS":
                            return out_trade_no;
                    }
                }
            }
            return null;
        }

        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组 
        /// request回来的信息组成的数组
        public IDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 支付宝充值余额异步通知
        /// </summary>
        /// <returns></returns>
        public string chargeByAlipayNotify()
        {
            string success = "success";
            string failure = "failure";
            var dict2 = GetRequestPost();
            var out_trade_no = dict2["out_trade_no"];

            var list = CacheHelper.GetByCondition<UserPay>("UserPay", " outTradeNo= \'" + out_trade_no + "\'");
            if (list.Count < 0)
                return failure;
            var up = list.First();

            var outTradeNo = CheckAliPay(dict2, up.money.ToString());
            txyInfo info = new txyInfo();
            info.content = "alipay--outTradeNo=====" + outTradeNo;
            txyInfoOper.Instance.Add(info);

            if (outTradeNo == null)
                return failure;

            if ((bool)up.status)
                return success;
            up.status = true;

            UserPayOper.Instance.Update(up);
            var user = UserInfoOper.Instance.GetById((int)up.userId);
            //user.userBalance += up.money;
            var userHere = new UserInfo();
            userHere.id = user.id;
            userHere.userBalance = user.userBalance + up.money;
            UserInfoOper.Instance.Update(userHere);
            return success;
        }

        #endregion

        #region 微信支付异步通知

        /// <summary>
        /// 微信商户密钥
        /// </summary>
        string mchKey = ConfigurationManager.AppSettings["wxMchKey"].ToString();

        /// <summary>
        /// 微信支付订单异步通知
        /// </summary>
        /// <returns></returns>
        public string wxpayNotify()
        {
            var success = "<xml><return_code><![CDATA[SUCCESS]]></return_code>\n<return_msg><![CDATA[OK]]></return_msg></xml>";

            var error = "<xml><return_code><![CDATA[FAIL]]></return_code>\n<return_msg><![CDATA[error]]></return_msg></xml>";
            string xmlStr = getPostStr();

            var outTradeNo = WXCheck(xmlStr);
            txyInfo info = new txyInfo();
            info.content = "outTradeNo ====" + outTradeNo;
            txyInfoOper.Instance.Add(info);

            if (outTradeNo == null)
                return error;
            var orderList2 = CacheHelper.GetByCondition<OrderInfo>("orderinfo", " orderNo= " + outTradeNo);
            if (orderList2.Count < 1)
                return error;
            var order = orderList2.First();
            if (order.status == 1)
                return success;

            order.payType = "wxpay";
            if ((bool)order.isTake)
                order.status = 2;
            else
                order.status = 1;
            order.payTime = DateTime.Now;
            OrderInfoOper.Instance.Update(order);
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            user.userBalance -= order.useBalance;
            user.coupon -= order.useCoupon;
            UserInfoOper.Instance.Update(user);
            UPushHelper.Instance.PushAfterPayOrder(order);

            UserPayOper.Instance.payOrderAddRecord(order);
            return success;
        }

        /// <summary>
        /// 微信充值余额异步通知
        /// </summary>
        /// <returns></returns>
        public string ChargeByWxNotify()
        {
            var success = "<xml><return_code><![CDATA[SUCCESS]]></return_code>\n<return_msg><![CDATA[OK]]></return_msg></xml>";

            var error = "<xml><return_code><![CDATA[FAIL]]></return_code>\n<return_msg><![CDATA[error]]></return_msg></xml>";
            string xmlStr = getPostStr();

            var outTradeNo = WXCheck(xmlStr);
            txyInfo info = new txyInfo();
            info.content = "wx--outTradeNo ====" + outTradeNo;
            txyInfoOper.Instance.Add(info);

            if (outTradeNo == null)
                return error;
            var list = CacheHelper.GetByCondition<UserPay>("UserPay", " outTradeNo= \'" + outTradeNo + "\'");
            if (list.Count < 1)
                return error;
            var up = list.First();
            if ((bool)up.status)
                return success;

            up.status = true;

            UserPayOper.Instance.Update(up);
            //orderOper.Update(up);
            var user2 = UserInfoOper.Instance.GetById((int)up.userId);
            //user2.userBalance += up.money;
            var userHere = new UserInfo();
            userHere.id = user2.id;
            userHere.userBalance = user2.userBalance + up.money;
            //user2.coupon -= up.useCoupon;
            UserInfoOper.Instance.Update(userHere);
            return success;
        }

        /// <summary>
        /// 获取 Post 提交的参数
        /// </summary>
        /// <returns></returns>
        public string getPostStr()
        {
            Int32 intLen = Convert.ToInt32(Request.InputStream.Length);
            byte[] b = new byte[intLen];
            Request.InputStream.Read(b, 0, intLen);
            return System.Text.Encoding.UTF8.GetString(b);
        }

        /// <summary>
        /// 成功返回订单号out_trade_no 错误返回 null
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public string WXCheck(string xmlStr)
        {
            try
            {
                var signFromWX = "";
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlStr);
                var dict = new Dictionary<string, string>();
                foreach (XmlNode item in xml.FirstChild)
                {
                    if (item.Name != "sign")
                        dict.Add(item.Name, item.InnerText);
                    else
                        signFromWX = item.InnerText;
                }

                string sign = StringHelperHere.Instance.GetWXSign(dict, mchKey);
                if (dict["result_code"] == "SUCCESS")
                {
                    //交易成功
                    string out_trade_no = dict["out_trade_no"];//商户订单号
                    if (sign != signFromWX)
                        return null;
                    return out_trade_no;
                }
            }
            catch (Exception e)
            {
                txyInfo info = new txyInfo();
                info.content = e.Message;
                txyInfoOper.Instance.Add(info);
            }
            return null;
        }

        #endregion
    }
}
