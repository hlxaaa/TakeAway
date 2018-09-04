using BeIT.MemCached;
using Common.Helper;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Attribute;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
//using takeAway.Helper;

namespace takeAway.Controllers
{
    public partial class FoodTagController : WebBaseController
    {
        //
        // GET: /FoodType/
        MemcachedClient cache = MemCacheHelper.GetMyConfigInstance();
        //public static int size = 10;
        public ActionResult Index()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            return View();
        }
   
        public string Get(webReq req)
        {
            int size = 12;
            int pages = 0;//总页数
            string search = req.search== null ? "" : req.search;//搜索内容
            int index = req.index;//页码
            int tagType = Convert.ToInt32(req.tagType);
            var condition = " IsDeleted=0 and   foodTagName like '%" + search + "%' ";
            if (tagType != 2)
            {
                condition += " and isweek=" + tagType;
            }
            var listAll = CacheHelper.GetDistinctCount<FoodTagInfo>("FoodTagInfo", condition);

            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            if (ids.Count() < 1)
                return JsonHelperHere.EmptyJson();
            var idsStr = string.Join(",", ids);
            //查具体信息
            var viewList = CacheHelper.GetByCondition<FoodTagInfo>("FoodTagInfo", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;
            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(viewList.OrderByDescending(p => p.id)), index);
        }

        public string Add(webReq req)
        {
            FoodTagInfo tag = new FoodTagInfo();
            tag.foodTagName =req.foodTagName;
            string name = req.foodTagName;
            if (ControllerHelper.Instance.IsExists("foodtaginfo", "foodTagName", name, "0"))
                return JsonHelperHere.JsonMsg(false, "已存在该标签");

            bool isWeek = true;
            if (Request["isWeek"] == "0")
                isWeek = false;
            tag.isWeek = isWeek;

            FoodTagInfoOper.Instance.Add(tag);
            return JsonHelperHere.JsonMsg(true, "");
        }

        public string Update(webReq req)
        {
            FoodTagInfo tag = new FoodTagInfo();
            var tagId = Convert.ToInt32(req.foodTagId);
            tag.foodTagName = req.foodTagName;
            var name = req.foodTagName;
            if (ControllerHelper.Instance.IsExists("foodtaginfo", "foodTagName", name, tagId.ToString()))
                return JsonHelperHere.JsonMsg(false, "已存在该标签");
            bool isWeek = true;
            if (Request["isWeek"] == "0")
                isWeek = false;
            tag.isWeek = isWeek;
            tag.id = tagId;
            FoodTagInfoOper.Instance.Update(tag);
            return JsonHelperHere.JsonMsg(true, "");
        }

        public void Delete(webReq req)
        {
            DoDelete(Convert.ToInt32(req.foodTagId));
        }

        public void BatchDel(webReq req)
        {
            //var ids = req.ids;
            string[] ids = Request.Form.GetValues("ids[]");
            for (int i = 0; i < ids.Length; i++)
            {
                DoDelete(Convert.ToInt32(ids[i]));
            }
        }

        public void DoDelete(int foodTagId)
        {
            FoodTagInfo tag = new FoodTagInfo();
            tag.id = foodTagId;
            tag.isDeleted = true;
            FoodTagInfoOper.Instance.Update(tag);
        }

    }
}
