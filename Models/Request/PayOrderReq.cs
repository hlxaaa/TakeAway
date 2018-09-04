using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Request
{
    public class PayOrderReq:UserToken
    {

        public string orderId { set; get; }

        public string addressId { get; set; }

        public string timeArea { get; set; }

        /// <summary>
        /// 押金decimal
        /// </summary>
        public string deposit { get; set; }

        public string listFoods { get; set; }

        /// <summary>
        /// 余额使用金额
        /// </summary>
        public string useBalance { get; set; }

        /// <summary>
        /// 优惠券使用金额
        /// </summary>
        public string useCoupon { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 支付方式wxpay，alipay
        /// </summary>
        public string payType { get; set; }

        /// <summary>
        /// 支付宝或微信支付的金额
        /// </summary>
        public string payMoney { get; set; }

        /// <summary>
        /// 支付的总金额
        /// </summary>
        //public decimal sumMoney { get; set; }

        /// <summary>
        /// 是否使用余额
        /// </summary>
        public string isUseBalance { get; set; }

        /// <summary>
        /// 是否使用礼金券
        /// </summary>
        public string isUseCoupon { get; set; }

        public string areaId { get; set; }

        public string spbill_create_ip { get; set; }
    }


}