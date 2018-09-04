using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models
{
    [Serializable]
    public class CancelCache
    {
        public int userId { get; set; }

        /// <summary>
        /// 未接单取消次数
        /// </summary>
        public int time1 { get; set; }

        /// <summary>
        /// 已接单取消次数
        /// </summary>
        public int time2 { get; set; }

        public DateTime CreateTime { get; set; }
    }
}