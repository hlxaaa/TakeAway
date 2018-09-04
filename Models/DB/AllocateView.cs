using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class AllocateView
    {
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
        public String riderName { get; set; }
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
        public Int32? amount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? targetRiderId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String targetName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? status { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String timestamp { get; set; }
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
