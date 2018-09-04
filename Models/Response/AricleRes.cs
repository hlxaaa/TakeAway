using DbOpertion.DBoperation;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{

    [Serializable]
    public class AricleName_Pic_content_url {
        public AricleName_Pic_content_url(ArticleInfo article,string apiHost) {
            articleName = article.articleName;
            articleUrl =   apiHost + article.url;
            content = ArticleInfoOper.Instance.GetSomeContent(article.content, 10);
            var imgPath = ArticleInfoOper.Instance.GetImgPath(article.content)[0];
            articleImg =  apiHost + imgPath;
        }


        public string articleName { get; set; }

        public string articleImg { get; set; }

        public string content { get; set; }

        public string articleUrl { get; set; }
    }

    [Serializable]
    public class ArticleName_url {
        public ArticleName_url(ArticleInfo article,string apiHost) {
            articleName = article.articleName;
            articleUrl = apiHost + article.url;
        }
        public string articleName { get; set; }
        public string articleUrl { get; set; }
    }

    public class ArticleViewForWeb {
        public ArticleViewForWeb(ArticleView view) {
            id = view.id;
            url = view.url;
            content = ArticleInfoOper.Instance.GetSomeContent(view.content, 20);
            isArticle = view.isArticle;
            articleName = view.articleName;
            status = view.status;
            lastTime = Convert.ToDateTime(view.lastTime).ToString("yyyy-MM-dd HH:mm:ss");
            articleType = view.articleType;
        }

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
        public string lastTime { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String articleType { get; set; }
    }
}