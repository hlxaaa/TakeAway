using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class UserView
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? userBalance { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String wechat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String qq { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userPhone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userHead { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? birthday { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? coupon { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String deviceToken { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String deviceType { get; set; }
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
