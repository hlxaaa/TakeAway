using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class CouponView
    {
        /// <summary>
        ///
        /// </summary>
        public String batch { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String name { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? createTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isRepeat { get; set; }

}
}
