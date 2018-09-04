using DbOpertion.DBoperation;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;

namespace takeAway.Controllers
{
    public class AreaController : WebBaseController
    {
        public ActionResult Index()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            return View();
        }

        public ActionResult Detail()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            int id = Convert.ToInt32(Request["areaId"]);
            AreaInfo area = AreaInfoOper.Instance.GetById(id);
            if (area != null)
            {
                ViewBag.areaName = area.areaName;
                ViewBag.lat1 = area.lat1;
                ViewBag.lat2 = area.lat2;
                ViewBag.lng1 = area.lng1;
                ViewBag.lng2 = area.lng2;
                ViewBag.areaNo = area.areaNo;
                ViewBag.areaId = area.id;
            }
            return View();
        }

        public string Get(webReq req)
        {
            string search = req.search == null ? "" :req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            int pages = 0;//总页数
            int size = 12;//一页size条数据

            var condition = " isdeleted=0 and (areaName like '%" + search + "%' or areaNo like '%" + search + "%' ) ";
            var listAll = CacheHelper.GetDistinctCount<AreaInfo>("areainfo", condition);

            if (listAll.Count < 1)
                return JsonHelperHere.EmptyJson();

            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();

            var viewList = CacheHelper.GetByIds<AreaInfo>("AreaInfo", ids);

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;

            return JsonHelperHere.JsonResult(pages, viewList, index);
        }

        public void Add(webReq req)
        {
            AreaInfo area = new AreaInfo();
            area.lat1 = Convert.ToDecimal(req.lat1);
            area.lng1 = Convert.ToDecimal(req.lng1);
            area.lat2 = Convert.ToDecimal(req.lat2);
            area.lng2 = Convert.ToDecimal(req.lng2);
            area.areaNo = req.areaNo;
            area.areaName = req.areaName;
            AreaInfoOper.Instance.Add(area);
        }

        public void Update(webReq req)
        {
            AreaInfo area = new AreaInfo();
            area.lat1 = Convert.ToDecimal(req.lat1);
            area.lng1 = Convert.ToDecimal(req.lng1);
            area.lat2 = Convert.ToDecimal(req.lat2);
            area.lng2 = Convert.ToDecimal(req.lng2);
            area.areaNo = req.areaNo;
            area.areaName = req.areaName;
            area.id = Convert.ToInt32(req.areaId);
            AreaInfoOper.Instance.Update(area);
        }

        public void Delete(webReq req)
        {
            DoDelete(Convert.ToInt32(req.areaId));
        }

        public void BatchDel(webReq req)
        {
            string[] ids = Request.Form.GetValues("ids[]");
            //var ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                DoDelete(Convert.ToInt32(ids[i]));
            }
        }

        public void DoDelete(int areaId)
        {
            AreaInfo area = new AreaInfo();
            area.id = areaId;
            area.isDeleted = true;
            AreaInfoOper.Instance.Update(area);
        }

        /// <summary>
        /// 检查重复
        /// </summary>
        /// <returns></returns>
        public bool IsNoExists(webReq req)
        {
            string areaNo = req.areaNo;
            var areaId =req.areaId;
            return ControllerHelper.Instance.IsExists("areainfo", "areaNo", areaNo, areaId.ToString());
        }

        public bool IsNameExists(webReq req)
        {
            string areaName = req.areaName;
            var areaId = req.areaId ;
            return ControllerHelper.Instance.IsExists("areainfo", "areaName", areaName, areaId.ToString());
        }

        //——————————不用了——————————

        public ActionResult StockRider()
        {
            int id = Convert.ToInt32(Request["areaId"]);
            List<RiderInfo> list = AreaInfoOper.Instance.GetRiderByAreaId(id);
            if (list.Count > 0)
            {
                ViewBag.Ta_RiderInfo = list;
            }
            return View();

        }
    }
}
