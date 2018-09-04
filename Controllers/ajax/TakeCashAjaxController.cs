using Common.Helper;
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
    public class TakeCashAjaxController : WebAjaxController
    {
        public string Get(webReq req)
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
            string search = req.search ?? "";//搜索内容
            int index = Convert.ToInt32(req.index);//页码

            int pages = 0;//总页数
            int size = 12;//一页size条数据

            var condition = " type = 99 and status = 0 ";

            condition += " and (takeTypeName like '%" + search + "%' or username like '%" + search + "%' or userphone like '%" + search + "%' ) order by createTime desc ";

            string str = "select distinct id,createTime from userpayview where 1=1 and " + condition;
            var dt = SqlHelperHere.ExecuteGetDt(str);
            var listAll = dt.ConvertToList<UserPayView>();

            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            var idsStr = string.Join(",", ids);
            var viewList = CacheHelper.GetByCondition<UserPayView>("UserPayView", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            pages = pages * size == count ? pages : pages + 1;//总页数
            var listR = new List<UserPayViewRes>();
            foreach (var item in viewList)
            {
                UserPayViewRes temp = new UserPayViewRes(item);
                listR.Add(temp);
            }

            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.id)), index);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 确认提现
        /// </summary>
        public string DoTake(webReq req)
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
            int upId = Convert.ToInt32(req.upId);//页码
            var up = UserPayOper.Instance.GetById(upId);
            up.status = true;
            up.createTime = DateTime.Now;
            UserPayOper.Instance.Update(up);
            return JsonConvert.SerializeObject(r);
        }

    }
}
