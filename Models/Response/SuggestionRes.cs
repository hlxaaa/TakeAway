using DbOpertion.Models;
using System;

namespace takeAwayWebApi.Models.Response
{
    public class SuggestionRes
    {
        public SuggestionRes(SuggestionView view) {
            id = view.id;
            userId = view.userId;
            content = view.content;
            sTime = Convert.ToDateTime(view.sTime).ToString("yyyy-MM-dd HH:mm:ss");
            userName = view.userName;
            userName = string.IsNullOrEmpty(userName) ? "默认用户" : userName;
            phone = view.userPhone;
        }

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
        public string sTime { get; set; }
        /// <summary>
        ///
        /// </summary>

        /// <summary>
        ///
        /// </summary>
        public String userName { get; set; }

        public string phone { get; set; }
    }
}