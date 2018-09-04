using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class RiderStockAjaxController : WebAjaxController
    {

        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var riderId = Convert.ToInt32(req.riderId);
            Dictionary<int, string> dict = FoodTagInfoOper.Instance.GetFoodTypeDict();
            string search = req.search ?? "";//搜索内容

            int index = Convert.ToInt32(req.index);//页码
            int pages = 0;//总页数
            int size = 14;//一页size条数据

            //查询条件
            var condition = " riderId = " + riderId + " and  foodName like '%" + search + "%' ";
            //所有符合条件的记录的id
            var listAll = CacheHelper.GetDistinctCount<RiderStockView>("RiderStockView", condition);

            if (listAll.Count < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
            //分页
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            var idsStr = string.Join(",", ids);
            //查具体信息
            var viewList = CacheHelper.GetByCondition<RiderStockView>("RiderStockView", " id in (" + idsStr + ")");

            List<RiderStockView> list = (List<RiderStockView>)CacheHelper.Get<RiderStockView>("RiderStockView", "Ta_RiderStockView");

            list = list.OrderByDescending(p => p.id).Where(p => p.riderId == riderId && p.foodName.Contains(search)).ToList();

            pages = list.Count / size;
            pages = pages * size == list.Count ? pages : pages + 1;//总页数
            list = list.Skip((index - 1) * size).Take(size).ToList();

            r.data= JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(list), index);
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
            var id = Convert.ToInt32(req.id);
            var foodId = Convert.ToInt32(req.foodId);
            var amount = Convert.ToInt32(req.amount);
            RiderStock rs = new RiderStock
            {
                id = id,
                foodId = foodId,
                amount = amount
            };
            RiderStockOper.Instance.Update(rs);
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
            var riderId = Convert.ToInt32(req.riderId);
            var foodId = Convert.ToInt32(req.foodId);
            var amount = Convert.ToInt32(req.amount);
            RiderStock rs = new RiderStock();
            rs.foodId = foodId;
            rs.riderId = riderId;
            rs.amount = amount;
            RiderStockOper.Instance.Add(rs);
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
            var rsId = Convert.ToInt32(req.rsId);
            RiderStockOper.Instance.Delete(rsId);
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
                RiderStockOper.Instance.Delete(Convert.ToInt32(ids[i]));
            }
            return JsonConvert.SerializeObject(r);
        }

    }
}
