using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class UserOpenView
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
        public String lat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String lng { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? openTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? closeTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String address { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? useTime { get; set; }

}
}
