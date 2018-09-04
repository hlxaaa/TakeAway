using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class RiderStockJoin
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 rsId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? foodId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? foodPrice { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? tagId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String tagName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? ismain { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodImg { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String mainType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isThisWeek { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? deposit { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? amount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? riderStatus { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String areaName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderAreaId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String lat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String lng { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String mapAddress { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? sexType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String deviceToken { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String name { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderPhone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String sex { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderStatusName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderTypeName { get; set; }

}
}
