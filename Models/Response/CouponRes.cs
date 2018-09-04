using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    public class CouponRes
    {
        //int days = Convert.ToInt32(ConfigurationManager.AppSettings["couponSaveDays"]);
        public CouponRes(Coupon c,int days) {
            id = c.id;
            batch = c.batch;
            ids = c.ids;
            isUse = c.isUse;
            couponNo = c.couponNo;
            isDeleted = c.isDeleted;
            createTime = Convert.ToDateTime(c.createTime).ToString("yyyy-MM-dd HH:mm:ss");
            endTime = Convert.ToDateTime(c.createTime).AddDays(days).ToString("yyyy-MM-dd HH:mm:ss") ;
            money = c.money;
            isRepeat = c.isRepeat;
            name = c.name;
        }


        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String batch { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String ids { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isUse { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String couponNo { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string createTime { get; set; }

        public string endTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? money { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isRepeat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String name { get; set; }
    }

    public class DownList {
        public DownList(CouponView view) {
            name = view.name;
            url = "/img/coupon/"+view.name+".zip";
            batch = view.batch;
            createTime = Convert.ToDateTime(view.createTime).ToString("yyyy-MM-dd HH:mm:ss");
            isRepeat = (bool)view.isRepeat;
        }
        public string batch { get; set; }
        public string createTime { get; set; }
        public bool isRepeat { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
}