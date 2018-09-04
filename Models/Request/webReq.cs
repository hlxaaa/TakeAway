using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Models.Request
{
    public class webReq
    {
        public int isThisWeek { get; set; }
        public string rsId { get; set; }
        public string batch { get; set; }
        public string foodStars { get; set; }
        public string areaName { get; set; }
        public string areaNo { get; set; }
        public string lat1 { get; set; }
        public string lat2 { get; set; }
        public string lng1 { get; set; }
        public string lng2 { get; set; }
        public string[] imgNames { get; set; }
        public string articleName { get; set; }
        public string isArticle { get; set; }
        public string content { get; set; }
        public string articleType { get; set; }
        public string secondTag { get; set; }
        public string foodText { get; set; }
        public string foodImg { get; set; }
        public string foodTypeId { get; set; }
        public string foodPrice { get; set; }
        public string foodName { get; set; }
        public string tempImg { get; set; }
        public string foodTagId { get; set; }
        public string isWeek { get; set; }
        public string foodTagName { get; set; }
        public string tagType { get; set; }
        public int sourceId { get; set; }
        public int stockId { get; set; }
        public int upId { get; set; }
        public int[] ids { get; set; }
        public string coupon { get; set; }
        public string userName { get; set; }
        public string userBalance { get; set; }
        public string userPwd { get; set; }
        public string phone { get; set; }
        public int isOn { get; set; }
        public string orderStr { get; set; }
        public int desc { get; set; }
        public int isDeposit { get; set; }
        public string discount { get; set; }
        public string time1 { get; set; }
        public string time2 { get; set; }
        public int weekTag { get; set; }
        public string isMain { get; set; }
        public int isDeleted { get; set; }
        public int lv { get; set; }
        public int tab { get; set; }
        public string pAccount { get; set; }
        public string pPwd { get; set; }

        public string shopNewOrderTips { get; set; }
        public string shopNewReserveOrderTips { get; set; }
        public string serverAssignRiderTips { get; set; }
        public string recoveryBoxTips { get; set; }
        public string orderSendingTips { get; set; }
        public string orderArrivelTips { get; set; }
        public string boxGetTips { get; set; }
        public string riderCancelTips { get; set; }
        public int isUse { get; set; }
        public string couponSaveDays { get; set; }
        public string birthday { get; set; }
        public string userPhone { get; set; }
        public int areaId { get; set; }
        public string mapAddress { get; set; }
        public int riderType { get; set; }
        public string account { get; set; }
        public string pwd { get; set; }
        public string riderName { get; set; }
        public int riderAreaId { get; set; }
        public decimal stars { get; set; }
        public int starCount { get; set; }
        public int sendCount { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int articleId { get; set; }

        public int userId { get; set; }

        public DateTime createTime { get; set; }

        public int status { get; set; }

        public string isActual { get; set; }

        public string remarks { get; set; }

        public int riderId { get; set; }

        public int orderAreaId { get; set; }

        public DateTime endTime { get; set; }
        public string riderComment { get; set; }
        public decimal useBalance { get; set; }
        public decimal useCoupon { get; set; }
        public decimal deposit { get; set; }
        public DateTime payTime { get; set; }
        public string payType { get; set; }
        public decimal payMoney { get; set; }
        public int[] foodIds { get; set; }
        public int[] amounts { get; set; }

        //public int riderId { get; set; }
        public int orderId { get; set; }
        public int id { get; set; }
        public string amount { get; set; }

        public string str { get; set; }

        public string couponMoney { get; set; }

        public string isRepeat { get; set; }

        public string name { get; set; }

        public string qcodeSaveFold { get; set; }

        public string couponId { get; set; }

        public string timestamp { get; set; }

        public string search { get; set; }

        public int index { get; set; }

        public int pageType { get; set; }

        public int foodId { get; set; }
    }

}