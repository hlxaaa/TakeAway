using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Request
{
    public class OrderReq
    {
        public string pageIndex { get; set; }

        public string Token { get; set; }

        public string userId { get; set; }

        public string orderId { get; set; }

        /// <summary>
        /// 0未支付1待抢单2派送中3已送达4确认收到（完成）5用户取消订单6骑手取消订单99骑手申请取消中
        /// </summary>
        public string status { get; set; }

        public string foodId { get; set; }

        public string amount { get; set; }
    }
}