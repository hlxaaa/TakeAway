using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class AreaInfo
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? lat1 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? lng1 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? lat2 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? lng2 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? x3 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? y3 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? x4 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? y4 { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String areaName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String areaNo { get; set; }
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
