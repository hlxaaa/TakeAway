using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Models.Request
{
    public class RiderReq:RiderToken
    {
        //public string Token { get; set; }

        //public string riderId { get; set; }

        public string deviceToken { get; set; }

        public string account { get; set; }

        public string password { get; set; }

        /// <summary>
        /// 登录方式（Token、Password）
        /// </summary>
        //public string loginMode { get; set; }

        /// <summary>
        /// 0电瓶车1汽车2自提点
        /// </summary>
        public string riderType { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        public string areaId { get; set; }

        public string orderId { get; set; }

        public string foodId { get; set; }

        public string amount { get; set; }

        public string targetRiderId { get; set; }

        public string allocateId { get; set; }

        public string listFoods { get; set; }

        public string pageIndex { get; set; }
    }

    public class RiderLoginReq {
        [PwdValidate]
        public string password { get; set; }
        public string account { get; set; }
        public string deviceToken { get; set; }
    }
}