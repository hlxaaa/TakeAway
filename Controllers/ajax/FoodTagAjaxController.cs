using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public partial class FoodTagAjaxController : WebAjaxController
    {
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            int size = 12;
            int pages = 0;//总页数
            string search = req.search == null ? "" : req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            int tagType = Convert.ToInt32(req.tagType);
            var condition = " IsDeleted=0 and   foodTagName like '%" + search + "%' ";
            if (tagType != 2)
            {
                condition += " and isweek=" + tagType;
            }
            var listAll = CacheHelper.GetDistinctCount<FoodTagInfo>("FoodTagInfo", condition);

            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            if (ids.Count() < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
            var idsStr = string.Join(",", ids);
            //查具体信息
            var viewList = CacheHelper.GetByCondition<FoodTagInfo>("FoodTagInfo", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;
            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(viewList.OrderByDescending(p => p.id)), index);
            return JsonConvert.SerializeObject(r);
        }

        public string Add(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            FoodTagInfo tag = new FoodTagInfo();
            tag.foodTagName = req.foodTagName;
            string name = req.foodTagName;
            if (ControllerHelper.Instance.IsExists("foodtaginfo", "foodTagName", name, "0"))
            {
                r.HttpCode = 500;
                r.data = JsonHelperHere.JsonMsg(false, "已存在该标签");
                return JsonConvert.SerializeObject(r);
            }

            bool isWeek = true;
            if (req.isWeek == "0")
                isWeek = false;
            tag.isWeek = isWeek;

            FoodTagInfoOper.Instance.Add(tag);
            r.data = JsonHelperHere.JsonMsg(true, "");
            return JsonConvert.SerializeObject(r);
        }

        public string Update(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            FoodTagInfo tag = new FoodTagInfo();
            var tagId = Convert.ToInt32(req.foodTagId);
            tag.foodTagName = req.foodTagName;
            var name = req.foodTagName;
            if (ControllerHelper.Instance.IsExists("foodtaginfo", "foodTagName", name, tagId.ToString()))
            {
                r.HttpCode = 500;
                r.data = JsonHelperHere.JsonMsg(false, "已存在该标签");
                return JsonConvert.SerializeObject(r);
            }

            bool isWeek = true;
            if (req.isWeek == "0")
                isWeek = false;
            tag.isWeek = isWeek;
            tag.id = tagId;
            FoodTagInfoOper.Instance.Update(tag);
            r.data = JsonHelperHere.JsonMsg(true, "");
            return JsonConvert.SerializeObject(r);
        }

        public string Delete(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            DoDelete(Convert.ToInt32(req.foodTagId));
            return JsonConvert.SerializeObject(r);
        }

        public string BatchDel(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
         
            var ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                DoDelete(Convert.ToInt32(ids[i]));
            }
            return JsonConvert.SerializeObject(r);
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
