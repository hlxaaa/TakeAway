using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;
using Common.Helper;
using takeAwayWebApi.Controllers.webs;

namespace takeAway.Controllers
{
    public class FoodController : WebBaseController
    {
        public ActionResult Index()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            ControllerHelper.Instance.ClearTempPic(this.Server.MapPath("/Img/temp"));
            var dict = FoodTagInfoOper.Instance.GetFoodTypeDictIsWeek(true);
            ViewBag.dict = dict;
            return View();
        }

        public ActionResult Detail()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            string reqWeek = Request["isweek"];
            if (reqWeek == "1")//添加周菜品
            {
                ViewBag.isWeek = true;
                Dictionary<int, string> dict = FoodTagInfoOper.Instance.GetFoodTypeDictIsWeek(true);
                ViewBag.dict = dict;
                ViewBag.isAdd = true;
            }
            else
            {//food点编辑，根据isweek分别--添加普通菜品
                string foodId = Request["foodId"];
                //FoodInfo food = GetByFoodId(Convert.ToInt32(foodId));
                if (string.IsNullOrEmpty(foodId))
                {
                    ViewBag.isWeek = false;
                    Dictionary<int, string> dict = FoodTagInfoOper.Instance.GetFoodTypeDictIsWeek(false);
                    ViewBag.dict = dict;
                    ViewBag.isAdd = true;
                }
                else
                {
                    var list = CacheHelper.GetByCondition<FoodView>("FoodView", " id=" + foodId);
                    var food = list.First();
                    if (food != null)
                    {
                        var fTime = (DateTime)food.FoodTime;
                        //var fi = Convert.ToInt32(fTime.DayOfWeek);
                        var now = Convert.ToInt32(DateTime.Now.DayOfWeek);
                        now = now == 0 ? 7 : now;
                        var monday = DateTime.Now.AddDays(-now + 1).Date;
                        var isThisWeek = true;
                        if (fTime > monday.AddDays(7))
                            isThisWeek = false;
                        ViewBag.isThisWeek = isThisWeek;


                        ViewBag.foodName = food.foodName;
                        ViewBag.foodPrice = food.foodPrice;
                        ViewBag.foodTagId = food.foodTagId;
                        ViewBag.foodText = food.foodText;
                        ViewBag.foodImg = food.foodImg;
                        ViewBag.foodId = food.id;
                        ViewBag.isAdd = false;
                        ViewBag.isMain = (bool)food.isMain;
                        //ViewBag.isCycle = food.isCycle;
                        ViewBag.deposit = food.deposit;
                        ViewBag.isWeek = (bool)food.isWeek;
                        ViewBag.secondTag = (int)food.secondTag;
                        ViewBag.secondTagName = food.secondTagName;
                        ViewBag.stars = food.star;
                    }
                    else
                    {
                        ViewBag.isAdd = true;
                    }
                    var isweek = false;
                    if ((bool)food.isWeek)
                        isweek = true;
                    var dict = FoodTagInfoOper.Instance.GetFoodTypeDictIsWeek(isweek);
                    ViewBag.dict = dict;
                }
            }
            return View();

        }

        public string Get(webReq req)
        {
            var isOn = req.isOn;
            var orderStr = req.orderStr;
            var desc = req.desc;
            var isDeleted = req.isDeleted;
            Dictionary<int, string> dict = FoodTagInfoOper.Instance.GetFoodTypeDict();
            string search = req.search == null ? "" : req.search;//搜索内容
            string isWeekTemp = req.isWeek;
            var isMainTemp = req.isMain;
            var weekTag = req.weekTag;
            var isDeposit = req.isDeposit;

            int index = req.index;//页码
            int pages = 0;//总页数
            int size = 12;//一页size条数据

            //查询条件
            var condition = " 1=1 and  (foodname like '%" + search + "%' or foodText like '%" + search + "%' or foodTagName like '%" + search + "%') ";
            if (!string.IsNullOrEmpty(isMainTemp))
            {
                var isMain = 0;
                if (isMainTemp == "true")
                    isMain = 1;
                condition += " and ismain=" + isMain;
            }
            if (!string.IsNullOrEmpty(isWeekTemp) && isWeekTemp != "2")
            {
                condition += " and isweek=" + isWeekTemp;
                if (isWeekTemp == "1" && isOn != 2)
                    condition += " and isOn=" + isOn;
            }
            //else
            condition += " and isdeleted=0 ";

            if (!string.IsNullOrEmpty(isWeekTemp) && isWeekTemp != "0")
            {
                if (weekTag != 0)
                {
                    condition += " and foodTagId=" + weekTag;
                }
            }

            if (isDeposit == 1)
                condition += " and deposit>0";
            else if (isDeposit == 0)
                condition += " and deposit=0";

            string str = "select id,isdeleted from foodview where " + condition;
            string order = " order by isdeleted,id desc";
            if (string.IsNullOrEmpty(orderStr))
            {
                str += order;
            }
            else
            {
                order = " order by " + orderStr;
                if (desc == 1)
                    order += " desc";
                order += " ,isdeleted,id desc";
                str += order;
            }

            var dt = SqlHelperHere.ExecuteGetDt(str);
            var listAll = dt.ConvertToList<FoodView>();
            if (listAll.Count < 1)
                return JsonHelperHere.EmptyJson();
            //分页
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            var idsStr = string.Join(",", ids);

            var viewList = CacheHelper.GetByCondition<FoodView>("FoodView", " id in (" + idsStr + ") " + order);

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;

            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(viewList), index);
        }

        //用了
        public string GetFoodInfo(webReq req)
        {
            var foodId = req.foodId;
            var food = FoodInfoOper.Instance.GetById(foodId);
            return JsonConvert.SerializeObject(food);
        }
        //用
        public string GetFoodDictForWeb(webReq req)
        {
            var dict = FoodInfoOper.Instance.GetFoods();
            List<foodId_name_amount_price_isMain> listFoods = new List<foodId_name_amount_price_isMain>();
            foreach (var item in dict)
            {
                foodId_name_amount_price_isMain fn = new foodId_name_amount_price_isMain(item, 1);
                listFoods.Add(fn);
            }
            return JsonConvert.SerializeObject(listFoods);
        }

        public string Add(webReq req)
        {
            string tempImg = req.tempImg;
            FoodInfo food = new FoodInfo();
            var foodName = req.foodName;

            food.foodName = foodName;
            food.foodPrice = Convert.ToDecimal(req.foodPrice);
            food.foodTagId = Convert.ToInt32(req.foodTypeId);
            food.foodImg = req.foodImg;
            food.foodText = req.foodText;
            var stars = Convert.ToDecimal(req.foodStars);
            stars = stars < 0 ? 0 : stars;
            stars = stars > 5 ? 5 : stars;
            food.star = stars;
            //if (req.isCycle == "0")
            //    food.isCycle = false;
            //else
            //    food.isCycle = true;
            food.deposit = req.deposit;

            if (req.isMain == "0")
                food.isMain = false;
            else
                food.isMain = true;

            food.secondTag = Convert.ToInt32(req.secondTag);

            FoodInfoOper.Instance.Add(food);
            ControllerHelper.Instance.CopyFile(this.Server.MapPath(tempImg), this.Server.MapPath(food.foodImg));
            return JsonHelperHere.JsonMsg(true, "");
        }

        public string Update(webReq req)
        {
            string tempImg = req.tempImg;
            FoodInfo food = new FoodInfo();
            var foodName = req.foodName;
            var foodId = req.foodId;

            food.foodName = foodName;
            food.foodPrice = Convert.ToDecimal(req.foodPrice);
            food.foodTagId = Convert.ToInt32(req.foodTypeId);
            food.foodImg = req.foodImg;
            food.foodText = req.foodText;
            food.id = Convert.ToInt32(foodId);
            var stars = Convert.ToDecimal(req.foodStars);
            stars = stars < 0 ? 0 : stars;
            stars = stars > 5 ? 5 : stars;
            food.star = stars;

            //if (req.isCycle == "0")
            //    food.isCycle = false;
            //else
            //    food.isCycle = true;
            food.deposit = Convert.ToDecimal(req.deposit);

            if (req.isMain == "0")
                food.isMain = false;
            else
                food.isMain = true;

            food.secondTag = Convert.ToInt32(req.secondTag);

            FoodInfoOper.Instance.Update(food);
            ControllerHelper.Instance.CopyFile(this.Server.MapPath(tempImg), this.Server.MapPath(food.foodImg));
            return JsonHelperHere.JsonMsg(true, "");
        }

        /// <summary>
        /// 菜品上架
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public void Up(webReq req)
        {
            var foodId = req.foodId;
            FoodInfo food = new FoodInfo();
            food.id = foodId;
            food.isDeleted = false;
            FoodInfoOper.Instance.Update(food);
        }

        public void Delete(webReq req)
        {
            var foodId = req.foodId;
            DoDelete(Convert.ToInt32(foodId));
            RiderStockOper.Instance.DeleteRsByFoodId(foodId);
        }

        public void DoDelete(int foodId)
        {
            FoodInfo food = new FoodInfo();
            food.id = foodId;
            food.isDeleted = true;
            food.isOn = false;
            string url = this.Server.MapPath(FoodInfoOper.Instance.GetById(foodId).foodImg);
            FoodInfoOper.Instance.Update(food);
            ControllerHelper.Instance.DeleteFile(url);
        }

        public void BatchDel(webReq req)
        {
            //var ids = req.ids;
            string[] ids = Request.Form.GetValues("ids[]");
            for (int i = 0; i < ids.Length; i++)
            {
                DoDelete(Convert.ToInt32(ids[i]));
            }
            var temps = Array.ConvertAll<string, int>(ids, s => int.Parse(s));
            RiderStockOper.Instance.DeleteRsByFoodId(temps);
        }

        public string UpImg()
        {
            HttpFileCollectionBase f = Request.Files;
            string fname = f[0].FileName;
            int j = fname.LastIndexOf(".");
            string newext = fname.Substring(j);
            string url = "/Img//temp//food" + DateTime.Now.ToString("yyyyMMddHHmmss") + newext;
            f[0].SaveAs(this.Server.MapPath(url));
            return url;
        }

        /// <summary>
        /// 获取周标签
        /// </summary>
        /// <returns></returns>
        public string GetWeekTags(webReq req)
        {
            var list = CacheHelper.GetByCondition<FoodTagInfo>("foodtaginfo", " isdeleted=0 and isWeek=1");
            return JsonHelperHere.JsonPlus(true, JsonConvert.SerializeObject(list), "");
        }

        /// <summary>
        /// riderstock.js中,获取骑手身上没有的菜品的id和name
        /// </summary>
        /// <returns></returns>
        public string GetRiderStockOther(webReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var areaId = Convert.ToInt32(req.areaId);

            //所有 菜品id
            List<int> list = FoodInfoOper.Instance.GetFoodIds();

            //骑手身上有的菜品id
            List<int> list2 = RiderStockOper.Instance.GetFoodId(riderId);
            foreach (var item in list2)
            {
                list.Remove(item);
            }
            if (list.Count < 1)
                return "{\"data\":\"\"}";
            Dictionary<int, string> dictFood = FoodInfoOper.Instance.GetFoodDict();
            List<string> list3 = new List<string>();
            string json = "[";
            foreach (var item in list)
            {
                list3.Add(dictFood[item]);
                json += "{\"id\":" + item + ",\"foodName\":\"" + dictFood[item] + "\"},";
            }

            return json.Substring(0, json.Length - 1) + "]";
        }

        //周菜品下架
        public void downFood(webReq req)
        {
            var id = req.foodId;
            FoodInfo food = new FoodInfo();
            food.id = id;
            food.isOn = false;
            FoodInfoOper.Instance.Update(food);
        }

        public void upFood(webReq req)
        {
            var id = req.foodId;
            FoodInfo food = new FoodInfo();
            food.id = id;
            food.isOn = true;
            FoodInfoOper.Instance.Update(food);
        }

    }
}
