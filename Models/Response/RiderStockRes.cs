using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    public class RiderStockRes
    {
        public RiderStockRes(RiderStockJoin view, string host)
        {
            riderStockId = view.rsId;
            foodId = view.foodId;
            foodName = view.foodName;
            amount = view.amount;
            isMain = view.ismain;
            mainType = view.mainType;
            headImg = host + view.foodImg;
        }

        /// <summary>
        ///
        /// </summary>
        public Int32 riderStockId { get; set; }

        public Int32? foodId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? amount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isMain { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String mainType { get; set; }

        public String headImg { get; set; }
    }
}