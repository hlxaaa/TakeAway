using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class BillRecord
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? billType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? balanceChange { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String payType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? payMoney { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? userId { get; set; }
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
