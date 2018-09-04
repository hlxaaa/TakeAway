using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    public class ResultResHere
    {
        public int httpcode { get; set; }

        public string message { get; set; }
    }

    public class Result
    {
        public Result(int code, object d, string msg)
        {
            httpcode = code;
            data = d;
            message = msg;
        }

        public Result() { }

        public int httpcode { get; set; }

        public object data { get; set; }

        public string message { get; set; }
    }

    public class ResultForWebRes
    {
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class ResultDataForWeb {
        public ResultDataForWeb() { }

        public ResultDataForWeb(int p, object o, int i) {
            pages = p;
            data = o;
            index = i;
        }

        public int pages { get; set; }
        public int index { get; set; }
        public object data { get; set; }
    }


    [Serializable]
    public class SettingRes
    {
        public SettingRes()
        {
            couponSaveDays = ConfigurationManager.AppSettings["couponSaveDays"];
            riderCancelTips = ConfigurationManager.AppSettings["riderCancelTips"];
            recoveryBoxTips = ConfigurationManager.AppSettings["recoveryBoxTips"];
            orderSendingTips = ConfigurationManager.AppSettings["orderSendingTips"];
            orderArrivelTips = ConfigurationManager.AppSettings["orderArrivelTips"];
            boxGetTips = ConfigurationManager.AppSettings["boxGetTips"];
            serverAssignRiderTips = ConfigurationManager.AppSettings["serverAssignRiderTips"];
            discount = ConfigurationManager.AppSettings["discount"];
            shopNewOrderTips = ConfigurationManager.AppSettings["shopNewOrderTips"];
            shopNewReserveOrderTips = ConfigurationManager.AppSettings["shopNewReserveOrderTips"];
        }
        /// <summary>
        ///
        /// </summary>
        public String couponSaveDays { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderCancelTips { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String recoveryBoxTips { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String orderSendingTips { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String orderArrivelTips { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String boxGetTips { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String serverAssignRiderTips { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String discount { get; set; }

        public String shopNewOrderTips { get; set; }
        public String shopNewReserveOrderTips { get; set; }

    }

    public class ResultForWeb{
        public int HttpCode { get; set; }
        public string data { get; set; }
        public string Message { get; set; }
    }
}