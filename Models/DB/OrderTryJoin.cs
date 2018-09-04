using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class OrderTryJoin
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 orderId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? userId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? createTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isTake { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderPhone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? avgStars { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? sendCount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderDToken { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderMapAddress { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? areaId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String areaName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userDToken { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userDType { get; set; }
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
        public Decimal? foodPrice { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodImg { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? foodId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String lastFoodName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? lastFoodPrice { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String contactName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String payType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String timeArea { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String orderNo { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? useBalance { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? useCoupon { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String remarks { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? payMoney { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? sumMoney { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? payTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? arrivalTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isMain { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String mainType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? foodStars { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodComment { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderStars { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderComment { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? status { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String statuType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? userOrderStr { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderOrderStr { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isCancelling { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isActual { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? deposit { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String actualType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String contactPhone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String mapAddress { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String addrDetail { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? userAddressId { get; set; }

}
}
