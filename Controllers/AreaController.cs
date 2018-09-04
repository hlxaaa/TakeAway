using BeIT.MemCached;
using Common.Helper;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using takeAwayWebApi.Helper;

namespace takeAwayWebApi.Controllers
{
    public class AreaController : BaseController
    {
        //MemcachedClient cache = MemCacheHelper.GetMyConfigInstance();
        ////AreaInfoOper areaOper = new AreaInfoOper();


        //[HttpPost]
        //public HttpResponseMessage GetAreaName(decimal x,decimal y) {
        //    //cache.Delete("List_AreaInfo");
        //    List<AreaInfo> a = (List<AreaInfo>)CacheHelper.Get<AreaInfo>("AreaInfo", "List_AreaInfo");
        //    List<AreaInfo> listResult = a.Where(p => p.lng1 < x && p.lng2 > x && p.lat1 > y && p.lat2 < y).ToList();
        //    string b="";
        //    if(listResult.Count>0)
        //        b = listResult.First().areaName.ToString();
        //    string str = JsonHelperHere.OneToJson(true, "区域名", b, "");
        //    string str2 = JsonHelperHere.JsonPlus(true, JsonConvert.SerializeObject(b), "");
        //    HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        //    return result;
        //}

        //[HttpPost]
        //public HttpResponseMessage test()
        //{
        //    return null;
        //}
    }
}
