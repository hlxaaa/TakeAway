using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class Platform
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String account { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String pwd { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? level { get; set; }

}
}
