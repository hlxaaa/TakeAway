using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class UserAddressView
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
        public String name { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String phone { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String mapAddress { get; set; }
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
        public String detail { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isRecently { get; set; }
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
        public Boolean? riderStatus { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderAreaId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String deviceToken { get; set; }

}
}
