using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class ArticleAjaxController : WebAjaxController
    {
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
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
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            if (ids.Count() < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
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

            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.status).ThenBy(p => p.lastTime)), index);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 点击编辑之后
        /// </summary>
        /// <returns></returns>
        public string GetContentById(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var id = Convert.ToInt32(req.articleId);
            ArticleInfo article = ArticleInfoOper.Instance.GetById(id);
            r.data = ArticleInfoOper.Instance.GetContentHtml(article.content);
            return JsonConvert.SerializeObject(r);
        }

        public string Add(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };

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
            var imgNames = req.imgNames;
            foreach (var item in imgNames)
            {
                ControllerHelper.Instance.CopyFile(this.Server.MapPath("/img/temp/" + item), this.Server.MapPath("/img/article/" + item));
            }
            if (status == 1)
                CacheHelper.SetAllUserArticleNotClick();

            return JsonConvert.SerializeObject(r);
        }

        public string Update(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var imgNames = req.imgNames;
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

            return JsonConvert.SerializeObject(r);
        }

        public string Delete(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var serverPath = this.Server.MapPath("/");
            ArticleInfoOper.Instance.Delete(Convert.ToInt32(req.articleId), serverPath);
            return JsonConvert.SerializeObject(r);
        }

        public string BatchDel(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var serverPath = this.Server.MapPath("");
   
            var ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                ArticleInfoOper.Instance.Delete(Convert.ToInt32(ids[i]), this.Server.MapPath(""));
            }
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string UpArticle(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var id = req.articleId;
            var articleTemp = ArticleInfoOper.Instance.GetById(id);
            if ((bool)articleTemp.status)
            {
                r.HttpCode = 500;
                r.Message = "不能重复发布";
                return JsonConvert.SerializeObject(r);
                //return JsonHelperHere.JsonMsg(false, "不能重复发布");
            }
            var article = new ArticleInfo();
            article.id = id;
            article.status = true;
            article.lastTime = DateTime.Now;
            //article.status = true;
            //article.lastTime = DateTime.Now;
            ArticleInfoOper.Instance.Update(article);
            CacheHelper.SetAllUserArticleNotClick();
            return JsonConvert.SerializeObject(r);
            //return JsonHelperHere.JsonMsg(true, "发布成功");
        }

        public string DownArticle(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            ArticleInfo article = new ArticleInfo();
            article.id = req.articleId;
            article.status = false;
            article.lastTime = DateTime.Now;
            ArticleInfoOper.Instance.Update(article);
            return JsonConvert.SerializeObject(r);
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

    }
}
