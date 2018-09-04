using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class OrderInfo
    {
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
        public Int32? riderId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? foodStars { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderStars { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodComment { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userPhone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? orderAreaId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isActual { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? status { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isCancelling { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? addressId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? arrivalTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String orderNo { get; set; }
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
        public String riderComment { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? createTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? riderTakeTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? deposit { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isTake { get; set; }

}
}
