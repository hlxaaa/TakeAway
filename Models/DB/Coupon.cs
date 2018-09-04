using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class Coupon
    {
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
        public DateTime? createTime { get; set; }
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
        /// <summary>
        /// 获取对应主键
        /// </summary>
        public string GetBuilderPrimaryKey()
        {
            return "id";
        }
        /// <summary>
        /// 排序语句格式为 字段名,字段名,字段名...
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 排序语句 字段名,字段名,字段名...
        /// </summary>
        public string GroupBy { get; set; }
        /// <summary>
        /// 筛选字段
        /// </summary>
        public string Field { get; set; }

}
}
