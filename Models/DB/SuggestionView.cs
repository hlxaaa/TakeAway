using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class SuggestionView
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
        public String content { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? sTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userPhone { get; set; }

}
}
