using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class RiderView
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderAccount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderPwd { get; set; }
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
        public Int32? riderAreaId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String areaName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderNo { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? sexType { get; set; }
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
        public Decimal? avgStars { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? starCount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? sendCount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String deviceToken { get; set; }
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

}
}
