using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class UserInfo
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
        public String userPwd { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userPhone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
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
        public Boolean? sexType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String deviceType { get; set; }


}
}
