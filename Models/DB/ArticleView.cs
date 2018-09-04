using System;

namespace DbOpertion.Models
{
    [Serializable]
    public class ArticleView
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String url { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String content { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isArticle { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String articleName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? status { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userIds { get; set; }
        /// <summary>
        ///
        /// </summary>
        public DateTime? lastTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String articleType { get; set; }

}
}
