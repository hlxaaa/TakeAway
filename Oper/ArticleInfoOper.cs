
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using Common.Helper;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class ArticleInfoOper : SingleTon<ArticleInfoOper>
    {
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="articleinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(ArticleInfo articleinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (articleinfo.id != null)
                dict.Add("@id", articleinfo.id.ToString());
            if (articleinfo.url != null)
                dict.Add("@url", articleinfo.url.ToString());
            if (articleinfo.content != null)
                dict.Add("@content", articleinfo.content.ToString());
            if (articleinfo.isArticle != null)
                dict.Add("@isArticle", articleinfo.isArticle.ToString());
            if (articleinfo.isDeleted != null)
                dict.Add("@isDeleted", articleinfo.isDeleted.ToString());
            if (articleinfo.articleName != null)
                dict.Add("@articleName", articleinfo.articleName.ToString());
            if (articleinfo.status != null)
                dict.Add("@status", articleinfo.status.ToString());
            if (articleinfo.userIds != null)
                dict.Add("@userIds", articleinfo.userIds.ToString());
            if (articleinfo.lastTime != null)
                dict.Add("@lastTime", articleinfo.lastTime.ToString());

            return dict;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="articleinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(ArticleInfo articleinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (articleinfo.url != null)
            {
                part1.Append("url,");
                part2.Append("@url,");
            }
            if (articleinfo.content != null)
            {
                part1.Append("content,");
                part2.Append("@content,");
            }
            if (articleinfo.isArticle != null)
            {
                part1.Append("isArticle,");
                part2.Append("@isArticle,");
            }
            if (articleinfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (articleinfo.articleName != null)
            {
                part1.Append("articleName,");
                part2.Append("@articleName,");
            }
            if (articleinfo.status != null)
            {
                part1.Append("status,");
                part2.Append("@status,");
            }
            if (articleinfo.userIds != null)
            {
                part1.Append("userIds,");
                part2.Append("@userIds,");
            }
            if (articleinfo.lastTime != null)
            {
                part1.Append("lastTime,");
                part2.Append("@lastTime,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into articleinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="articleinfo"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(ArticleInfo articleinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update articleinfo set ");
            if (articleinfo.url != null)
                part1.Append("url = @url,");
            if (articleinfo.content != null)
                part1.Append("content = @content,");
            if (articleinfo.isArticle != null)
                part1.Append("isArticle = @isArticle,");
            if (articleinfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (articleinfo.articleName != null)
                part1.Append("articleName = @articleName,");
            if (articleinfo.status != null)
                part1.Append("status = @status,");
            if (articleinfo.userIds != null)
                part1.Append("userIds = @userIds,");
            if (articleinfo.lastTime != null)
                part1.Append("lastTime = @lastTime,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        public ArticleInfo GetById(int articleId)
        {
            //List<ArticleInfo> list = (List<ArticleInfo>)CacheHelper.Get<ArticleInfo>("ArticleInfo", "Ta_ArticleInfo");
            //List<ArticleInfo> list2 = list.Where(p => p.isDeleted == false && p.id == articleId).ToList();

            return CacheHelper.GetById<ArticleInfo>("ArticleInfo", articleId);

            //var list2 = (List<ArticleInfo>)CacheHelper.GetByCondition<ArticleInfo>("ArticleInfo", " id="+articleId);

            //if (list2.Count > 0)
            //    return list2.First();
            //return null;
        }

        /// <summary>
        /// 获取更新前文章中所有图片的名字
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public string[] GetOldImgNames(int articleId)
        {
            ArticleInfo article = GetById(articleId);
            string content = article.content.Replace("*gt;", ">").Replace("*lt;", "<").Replace("*amp", "&");
            string[] imgs = GetImgPath(content);
            for (int i = 0; i < imgs.Length; i++)
            {
                imgs[i] = imgs[i].Substring(imgs[i].LastIndexOf('/') + 1);
            }
            return imgs;
        }

        public string[] GetImgPath(string content)
        {
            List<string> list = new List<string>();
            string[] temp = content.Split('<');
            for (int i = 0; i < temp.Length; i++)
            {
                string result = Regex.Match(temp[i], "(?<=src=\").*?(?=\")").Value;
                if (result != "" && result != null)
                    list.Add(result);
            }
            return list.ToArray();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="ArticleInfo"></param>
        /// <returns></returns>
        public int Add(ArticleInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="ArticleInfo"></param>
        /// <returns></returns>
        public void Update(ArticleInfo model)
        {
            //CacheHelper.LockCache("ArticleInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("ArticleInfo");
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="serverPath"></param>
        public void Delete(int articleId, string serverPath)
        {
            ArticleInfo article = GetById(articleId);
            article.isDeleted = true;
            string content = article.content.Replace("*gt;", ">").Replace("*lt;", "<").Replace("*amp", "&");
            string[] imgs = GetImgPath(content);
            Update(article);
            DeleteImgFile(imgs, serverPath);
        }

        /// <summary>
        /// 删除文章中的图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <param name="serverPath"></param>
        public void DeleteImgFile(string[] imgs, string serverPath)
        {
            for (int i = 0; i < imgs.Length; i++)
            {
                if (imgs[i] != "")
                {
                    string path = serverPath + imgs[i].Replace("/", "\\").Substring(1);
                    //string path = Server.MapPath(imgs[i]);
                    ControllerHelper.Instance.DeleteFile(path);
                }
            }
        }

        /// <summary>
        /// 获取最后一个公告
        /// </summary>
        /// <returns></returns>
        public ArticleInfo GetLastNotice() {
            string str = "select top 1 * from articleInfo where isarticle=0 and status=1 and isdeleted=0 order by id desc";
            var dt  =SqlHelperHere.ExecuteGetDt(str);
            return dt.ConvertToList<ArticleInfo>().FirstOrDefault();
        }

        /// <summary>
        /// 获取已发布的文章数量
        /// </summary>
        /// <returns></returns>
        public int GetArticleCount() {
            string str = "select Count(*) from articleInfo where isarticle=1 and status=1 and isdeleted=0 ";
            var dt = SqlHelperHere.ExecuteScalar(str);
            return Convert.ToInt32(dt);
        }

        /// <summary>
        /// 获取文章内容的部分中文
        /// </summary>
        /// <param name="content"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetSomeContent(string content,int size) {
            string temp = content.Replace("*gt;", ">").Replace("*lt;", "<").Replace("*amp", "&");
            temp = Regex.Replace(temp, @"[^\u4e00-\u9fa5]+", "");
            int l = temp.Length > size ? size : temp.Length;
            return temp.Substring(0, l);
        }

        public string GetContentHtml(string content) { 
            return content.Replace("*gt;", ">").Replace("*lt;", "<").Replace("*amp", "&");
        }
    }
}
