using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class StockRiderAjaxController : WebAjaxController
    {
        public string Add(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            int areaId = req.areaId;
            int foodId = req.foodId;
            int amount = Convert.ToInt32(req.amount);
            StockInfo stock = new StockInfo();
            stock.areaId = areaId;
            stock.foodId = foodId;
            stock.amount = amount;
            StockInfoOper.Instance.Add(stock);
            return JsonConvert.SerializeObject(r);
        }

        public string Update(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            int stockId = req.stockId;
            int amount = Convert.ToInt32(req.amount);
            StockInfo stock = new StockInfo();
            stock.id = stockId;
            stock.amount = amount;
            StockInfoOper.Instance.Update(stock);
            return JsonConvert.SerializeObject(r);
        }

        public string Delete(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            int stockId = req.stockId;
            StockInfoOper.Instance.Delete(stockId);
            return JsonConvert.SerializeObject(r);
        }

        public string BatchDel(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            var ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                StockInfoOper.Instance.Delete(Convert.ToInt32(ids[i]));
            }
            return JsonConvert.SerializeObject(r);
        }

    }
}
