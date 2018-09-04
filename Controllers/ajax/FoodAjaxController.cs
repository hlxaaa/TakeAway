using Common.Helper;
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
    public class FoodAjaxController : WebAjaxController
    {
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };

            FoodInfoOper.Instance.ClearWeekFood();




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
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
            //分页
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            var idsStr = string.Join(",", ids);

            var viewList = CacheHelper.GetByCondition<FoodView>("FoodView", " id in (" + idsStr + ") " + order);

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;

            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(viewList), index);
            return JsonConvert.SerializeObject(r);
        }

        //用了
        public string GetFoodInfo(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var foodId = req.foodId;
            var food = FoodInfoOper.Instance.GetById(foodId);
            r.data = JsonConvert.SerializeObject(food);
            return JsonConvert.SerializeObject(r);

        }
        //用
        public string GetFoodDictForWeb(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var dict = FoodInfoOper.Instance.GetFoods();
            List<foodId_name_amount_price_isMain> listFoods = new List<foodId_name_amount_price_isMain>();
            foreach (var item in dict)
            {
                foodId_name_amount_price_isMain fn = new foodId_name_amount_price_isMain(item, 1);
                listFoods.Add(fn);
            }
            r.data = JsonConvert.SerializeObject(listFoods);
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

            string tempImg = req.tempImg;
            FoodInfo food = new FoodInfo();
            var foodName = req.foodName;

            var isThisWeek = Convert.ToBoolean(req.isThisWeek);
            var secondTag = Convert.ToInt32(req.secondTag);
            food.secondTag = secondTag;
            var now = Convert.ToInt32(DateTime.Now.DayOfWeek);
            var foodTime = DateTime.Now.AddDays(secondTag - now);
            if (!isThisWeek)
                foodTime = foodTime.AddDays(7);
            food.FoodTime = foodTime;


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

            //food.secondTag = Convert.ToInt32(req.secondTag);

            FoodInfoOper.Instance.Add(food);
            ControllerHelper.Instance.CopyFile(this.Server.MapPath(tempImg), this.Server.MapPath(food.foodImg));
            r.data = JsonHelperHere.JsonMsg(true, "");
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
            string tempImg = req.tempImg;
            FoodInfo food = new FoodInfo();
            var foodName = req.foodName;
            var foodId = req.foodId;
            var isThisWeek = Convert.ToBoolean(req.isThisWeek);
            var secondTag = Convert.ToInt32(req.secondTag);
            food.secondTag = secondTag;
            var now = Convert.ToInt32(DateTime.Now.DayOfWeek);
            var foodTime = DateTime.Now.AddDays(secondTag - now);
            if (!isThisWeek)
                foodTime = foodTime.AddDays(7);
            food.FoodTime = foodTime;


            food.foodName = foodName;
            food.foodPrice = Convert.ToDecimal(req.foodPrice);
            food.foodTagId = Convert.ToInt32(req.foodTypeId);
            food.foodImg = req.foodImg;
            food.foodText = req.foodText;
            var stars = Convert.ToDecimal(req.foodStars);
            stars = stars < 0 ? 0 : stars;
            stars = stars > 5 ? 5 : stars;
            food.star = stars;
            food.id = Convert.ToInt32(foodId);

            //if (req.isCycle == "0")
            //    food.isCycle = false;
            //else
            //    food.isCycle = true;
            food.deposit = Convert.ToDecimal(req.deposit);

            if (req.isMain == "0")
                food.isMain = false;
            else
                food.isMain = true;



            FoodInfoOper.Instance.Update(food);
            ControllerHelper.Instance.CopyFile(this.Server.MapPath(tempImg), this.Server.MapPath(food.foodImg));
            r.data = JsonHelperHere.JsonMsg(true, "");
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 菜品上架
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string Up(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var foodId = req.foodId;
            FoodInfo food = new FoodInfo();
            food.id = foodId;
            food.isDeleted = false;
            FoodInfoOper.Instance.Update(food);
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
            var foodId = req.foodId;
            DoDelete(Convert.ToInt32(foodId));
            RiderStockOper.Instance.DeleteRsByFoodId(foodId);
            return JsonConvert.SerializeObject(r);
        }

        public string DoDelete(int foodId)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            FoodInfo food = new FoodInfo();
            food.id = foodId;
            food.isDeleted = true;
            food.isOn = false;
            string url = this.Server.MapPath(FoodInfoOper.Instance.GetById(foodId).foodImg);
            FoodInfoOper.Instance.Update(food);
            ControllerHelper.Instance.DeleteFile(url);
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
            var ids = req.ids;

            for (int i = 0; i < ids.Length; i++)
            {
                DoDelete(Convert.ToInt32(ids[i]));
            }

            RiderStockOper.Instance.DeleteRsByFoodId(ids);
            return JsonConvert.SerializeObject(r);
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
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var list = CacheHelper.GetByCondition<FoodTagInfo>("foodtaginfo", " isdeleted=0 and isWeek=1");
            r.data = JsonHelperHere.JsonPlus(true, JsonConvert.SerializeObject(list), "");
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// riderstock.js中,获取骑手身上没有的菜品的id和name
        /// </summary>
        /// <returns></returns>
        public string GetRiderStockOther(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
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
            {
                r.data = "{\"data\":\"\"}";
                return JsonConvert.SerializeObject(r);
            }
            Dictionary<int, string> dictFood = FoodInfoOper.Instance.GetFoodDict();
            List<string> list3 = new List<string>();
            string json = "[";
            foreach (var item in list)
            {
                list3.Add(dictFood[item]);
                json += "{\"id\":" + item + ",\"foodName\":\"" + dictFood[item] + "\"},";
            }

            r.data = json.Substring(0, json.Length - 1) + "]";
            return JsonConvert.SerializeObject(r);
        }

        //周菜品下架
        public string downFood(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var id = req.foodId;
            FoodInfo food = new FoodInfo();
            food.id = id;
            food.isOn = false;
            FoodInfoOper.Instance.Update(food);
            return JsonConvert.SerializeObject(r);
        }

        public string upFood(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var id = req.foodId;
            FoodInfo food = new FoodInfo();
            food.id = id;
            food.isOn = true;
            FoodInfoOper.Instance.Update(food);
            return JsonConvert.SerializeObject(r);
        }

    }
}
