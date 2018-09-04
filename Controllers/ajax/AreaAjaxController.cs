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
    public class AreaAjaxController : WebAjaxController
    {
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            string search = req.search == null ? "" : req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            int pages = 0;//总页数
            int size = 12;//一页size条数据

            var condition = " isdeleted=0 and (areaName like '%" + search + "%' or areaNo like '%" + search + "%' ) ";
            var listAll = CacheHelper.GetDistinctCount<AreaInfo>("areainfo", condition);

            if (listAll.Count < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }

            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();

            var viewList = CacheHelper.GetByIds<AreaInfo>("AreaInfo", ids);

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;

            r.data = JsonHelperHere.JsonResult(pages, viewList, index);
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
            AreaInfo area = new AreaInfo();
            area.lat1 = Convert.ToDecimal(req.lat1);
            area.lng1 = Convert.ToDecimal(req.lng1);
            area.lat2 = Convert.ToDecimal(req.lat2);
            area.lng2 = Convert.ToDecimal(req.lng2);
            area.areaNo = req.areaNo;
            area.areaName = req.areaName;
            AreaInfoOper.Instance.Add(area);
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
            AreaInfo area = new AreaInfo();
            area.lat1 = Convert.ToDecimal(req.lat1);
            area.lng1 = Convert.ToDecimal(req.lng1);
            area.lat2 = Convert.ToDecimal(req.lat2);
            area.lng2 = Convert.ToDecimal(req.lng2);
            area.areaNo = req.areaNo;
            area.areaName = req.areaName;
            area.id = Convert.ToInt32(req.areaId);
            AreaInfoOper.Instance.Update(area);
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
            DoDelete(Convert.ToInt32(req.areaId));
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
            return JsonConvert.SerializeObject(r);
        }

        public void DoDelete(int areaId)
        {
            AreaInfo area = new AreaInfo();
            area.id = areaId;
            area.isDeleted = true;
            AreaInfoOper.Instance.Update(area);
        }

        //——————这两个不改了——————//

        /// <summary>
        /// 检查重复
        /// </summary>
        /// <returns></returns>
        public string IsNoExists(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            string areaNo = req.areaNo;
            var areaId = req.areaId;
            if (ControllerHelper.Instance.IsExists("areainfo", "areaNo", areaNo, areaId.ToString()))
                r.data = "1";
            else
                r.data = "0";
            return JsonConvert.SerializeObject(r) ;
        }

        public string IsNameExists(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            string areaName = req.areaName;
            string areaId = req.areaId.ToString() ;
            if (ControllerHelper.Instance.IsExists("areainfo", "areaName", areaName, areaId))
                r.data = "1";
            else
                r.data = "0";
            return JsonConvert.SerializeObject(r);
        }
    }
}
