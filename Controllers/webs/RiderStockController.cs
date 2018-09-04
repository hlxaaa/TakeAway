using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
//using takeAway.Helper;

namespace takeAway.Controllers
{
    public class RiderStockController : WebBaseController
    {
        //
        // GET: /RiderStock/
        //RiderStockOper oper = new RiderStockOper();
        //RiderInfoOper riderOper = new RiderInfoOper();
        //FoodTagInfoOper ftOper = new FoodTagInfoOper();

        public ActionResult Index()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            var riderId = Convert.ToInt32(Request["riderId"]);
            ViewBag.riderId = riderId;
            var riderNo = Request["riderNo"];
            RiderInfo rider = RiderInfoOper.Instance.GetById(riderId);
            ViewBag.areaId = rider.riderAreaId;
            ViewBag.riderNo = rider.riderNo;
            ViewBag.riderName = rider.name;
            return View();
        }

        public string Get(webReq req)
        {
            var riderId = Convert.ToInt32(Request["riderId"]);
            Dictionary<int, string> dict = FoodTagInfoOper.Instance.GetFoodTypeDict();
            string search = Request["search"] == null ? "" : Request["search"];//搜索内容

            int index = Convert.ToInt32(Request["index"]);//页码
            int pages = 0;//总页数
            int size = 14;//一页size条数据


            //查询条件
            var condition = " riderId = " + riderId + " and  foodName like '%" + search + "%' ";
            //所有符合条件的记录的id
            var listAll = CacheHelper.GetDistinctCount<RiderStockView>("RiderStockView", condition);

            if (listAll.Count < 1)
                return JsonHelperHere.EmptyJson();
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

            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(list), index);
        }

        public void Update(webReq req)
        {
            var id = Convert.ToInt32(Request["id"]);
            var foodId = Convert.ToInt32(Request["foodId"]);
            var amount = Convert.ToInt32(Request["amount"]);
            RiderStock rs = new RiderStock();
            rs.id = id;
            rs.foodId = foodId;
            rs.amount = amount;
            RiderStockOper.Instance.Update(rs);
        }

        public void Add(webReq req)
        {
            var riderId = Convert.ToInt32(Request["riderId"]);
            var foodId = Convert.ToInt32(Request["foodId"]);
            var amount = Convert.ToInt32(Request["amount"]);
            RiderStock rs = new RiderStock();
            rs.foodId = foodId;
            rs.riderId = riderId;
            rs.amount = amount;
            RiderStockOper.Instance.Add(rs);
        }

        public void Delete(webReq req)
        {
            var rsId = Convert.ToInt32(Request["rsId"]);
            RiderStockOper.Instance.Delete(rsId);
        }

        public void BatchDel(webReq req)
        {
            string[] ids = Request.Form.GetValues("ids[]");
            for (int i = 0; i < ids.Length; i++)
            {
                RiderStockOper.Instance.Delete(Convert.ToInt32(ids[i]));
            }
        }
    }
}

