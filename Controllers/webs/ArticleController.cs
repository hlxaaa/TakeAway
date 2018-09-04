using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAway.Controllers
{
    public class ArticleController : Controller
    {
        //public string content = "";

        public ActionResult Index()
        {
            if (Session["pfId"] != null)
                return View();
            else
                return View("_Login");
        }

        /// <summary>
        /// 文章url
        /// </summary>
        /// <returns></returns>
        public ActionResult ArticleUrl()
        {
            string content = "";
            var id = Request["articleId"];
            ArticleInfo article = ArticleInfoOper.Instance.GetById(Convert.ToInt32(id));

            if (article == null || (bool)article.isDeleted)
                return View();
            string content2 = article.content;
            content2 = content2.Replace("*gt;", ">");
            content2 = content2.Replace("*lt;", "<");
            content = content2.Replace("*amp", "&");
            ViewBag.content = content;
            ViewBag.title = article.articleName;
            return View();
        }

        public string Get(webReq req)
        {
            var status = req.status;
            string search = req.search == null ? "" : req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            int articleType = Convert.ToInt32(req.articleType);
            int pages = 0;//总页数
            int size = 12;//一页size条数据

            var condition = " isdeleted=0 ";
            if (articleType != 2)
                condition += " and isarticle = " + articleType;

            if (status != 2)
                condition += " and status = " + status;

            condition += " and ( articleName like '%" + search + "%' or content like '%" + search + "%' )";

            string str = "select id,lasttime,status from articleInfo where " + condition + " order by status desc,lastTime desc";
            var listAll = SqlHelperHere.ExecuteGetList<ArticleInfo>(str);

            if (listAll.Count < 1)
                return JsonHelperHere.EmptyJson();
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            if (ids.Count() < 1)
                return JsonHelperHere.EmptyJson();
            var idsStr = string.Join(",", ids);

            var list = CacheHelper.GetByCondition<ArticleView>("ArticleView", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;
            var listR = new List<ArticleViewForWeb>();
            foreach (var item in list)
            {
                ArticleViewForWeb temp = new ArticleViewForWeb(item);
                listR.Add(temp);
            }

            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.status).ThenBy(p => p.lastTime)), index);
        }

        /// <summary>
        /// 点击编辑之后
        /// </summary>
        /// <returns></returns>
        public string GetContentById(webReq req)
        {
            var id = Convert.ToInt32(req.articleId);
            ArticleInfo article = ArticleInfoOper.Instance.GetById(id);
            return ArticleInfoOper.Instance.GetContentHtml(article.content);
        }

        public void Add(webReq req)
        {
            var status = Convert.ToInt32(req.status);
            ArticleInfo article = new ArticleInfo();
            article.content = req.content;
            article.isArticle = Convert.ToBoolean(req.isArticle);
            article.articleName = req.articleName;
            article.status = Convert.ToBoolean(status);
            article.userIds = "0,";
            var id = ArticleInfoOper.Instance.Add(article);
            article = new ArticleInfo();
            var url = "/article/articleUrl?articleId=" + id;
            article.url = url;
            article.id = id;

            ArticleInfoOper.Instance.Update(article);

            var imgNames = Request.Form.GetValues("imgNames[]");
            //var imgNames = req.imgNames;
            foreach (var item in imgNames)
            {
                ControllerHelper.Instance.CopyFile(this.Server.MapPath("/img/temp/" + item), this.Server.MapPath("/img/article/" + item));
            }
            if (status == 1)
                CacheHelper.SetAllUserArticleNotClick();
        }

        public void Update(webReq req)
        {
            var imgNames = Request.Form.GetValues("imgNames[]");
            //var imgNames = req.imgNames;
            foreach (var item in imgNames)
            {
                ControllerHelper.Instance.CopyFile(this.Server.MapPath("/img/temp/" + item), this.Server.MapPath("/img/article/" + item));
            }
            var articleId = req.articleId;
            var oldImgNames = ArticleInfoOper.Instance.GetOldImgNames(articleId);

            List<string> oldImgUrls = new List<string>();
            List<string> newImgUrls = new List<string>();
            foreach (var item in oldImgNames)
            {
                oldImgUrls.Add(this.Server.MapPath("/img/article/" + item));
            }
            foreach (var item in imgNames)
            {
                newImgUrls.Add(this.Server.MapPath("/img/article/" + item));
            }
            ControllerHelper.Instance.UpdateImg(oldImgUrls.ToArray(), newImgUrls.ToArray());
            var status = req.status;

            ArticleInfo article = new ArticleInfo();
            article.url = @"/article/articleUrl?articleId=" + articleId;
            article.content = req.content;
            article.isArticle = Convert.ToBoolean(req.isArticle);
            article.articleName = req.articleName;
            article.status = Convert.ToBoolean(status);
            article.id = articleId;
            article.lastTime = DateTime.Now;
            ArticleInfoOper.Instance.Update(article);
            if (status == 1)
                CacheHelper.SetAllUserArticleNotClick();
        }

        public void Delete(webReq req)
        {
            var serverPath = this.Server.MapPath("/");
            ArticleInfoOper.Instance.Delete(Convert.ToInt32(req.articleId), serverPath);
        }

        public void BatchDel(webReq req)
        {
            var serverPath = this.Server.MapPath("");
            string[] ids = Request.Form.GetValues("ids[]");
            //var b = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                ArticleInfoOper.Instance.Delete(Convert.ToInt32(ids[i]), this.Server.MapPath(""));
            }
        }

        public string UpImg()
        {
            var files = Request.Files;
            if (files.Count <= 0)
            {
                return "";
            }
            HttpPostedFileBase file = files[0];
            if (file == null)
            {
                return "error|file is null";
            }
            else
            {
                Random r = new Random();
                string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                string path = this.Server.MapPath("/img/temp/");  //存储图片的文件夹
                string fname = file.FileName;
                string fileExtension = fname.Substring(fname.LastIndexOf('.'), fname.Length - fname.LastIndexOf('.'));
                string currentFileName = "article" + time + file.ContentLength + r.Next(100, 999) + fileExtension;  //文件名中不要带中文，否则会出错
                //生成文件路径
                string imagePath = path + currentFileName;
                //保存文件
                file.SaveAs(imagePath);
                return "/img/temp/" + currentFileName;
            }
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string UpArticle(webReq req)
        {
            var id = req.articleId;
            var articleTemp = ArticleInfoOper.Instance.GetById(id);
            if ((bool)articleTemp.status)
            {
                return JsonHelperHere.JsonMsg(false, "不能重复发布");
            }
            var article = new ArticleInfo();
            article.id = id;
            article.status = true;
            article.lastTime = DateTime.Now;
            //article.status = true;
            //article.lastTime = DateTime.Now;
            ArticleInfoOper.Instance.Update(article);
            CacheHelper.SetAllUserArticleNotClick();
            return JsonHelperHere.JsonMsg(true, "发布成功");
        }

        public string DownArticle(webReq req)
        {
            ArticleInfo article = new ArticleInfo();
            article.id = req.articleId;
            article.status = false;
            article.lastTime = DateTime.Now;
            ArticleInfoOper.Instance.Update(article);
            return JsonHelperHere.JsonMsg(true, "已撤下");
        }
    }
}
