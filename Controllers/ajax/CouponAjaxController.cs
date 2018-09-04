using Common.Helper;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web.Mvc;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class CouponAjaxController : Controller
    {
        string key = "fdCkey";

        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();

        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var set = new SettingRes();
            var days = Convert.ToInt32(set.couponSaveDays);
            int isUse = req.isUse;
            string search = req.search == null ? "" : req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            var batch = req.batch;
            var isRepeat = Convert.ToInt32(req.isRepeat);
            int pages = 0;//总页数
            int size = 12;//一页size条数据
            var condition = " isdeleted=0 and ";
            if (isUse != 2)
                condition += " isuse=" + isUse + " and ";
            if (batch != "0")
                condition += " batch=" + batch + " and ";
            if (isRepeat != 2)
                condition += " isRepeat=" + isRepeat + " and ";
            condition += "   (batch like '%" + search + "%' or couponNo like '%" + search + "%' )";
            var listAll = CacheHelper.GetDistinctCount<Coupon>("Coupon", condition);

            if (listAll.Count < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);

            }
            //分页
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            if (ids.Length < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);

            }
            var idsStr = string.Join(",", ids);
            //查具体信息
            var viewList = CacheHelper.GetByCondition<Coupon>("Coupon", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;
            var listR = new List<CouponRes>();
            foreach (var item in viewList)
            {
                CouponRes t = new CouponRes(item, days);
                listR.Add(t);
            }

            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.id)), index);
            return JsonConvert.SerializeObject(r);
        }

        public string GetDownloadList(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            string search = req.search == null ? "" : req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            int pages = 0;//总页数
            int size = 12;//一页size条数据
            var isRepeat = Convert.ToInt32(req.isRepeat);
            var condition = " ";
            if (isRepeat != 2)
                condition += " isRepeat=" + isRepeat + " and ";
            condition += " 1=1 and  name like '%" + search + "%' ";

            string str = "select * from couponView where " + condition + " order by createTime desc";
            var listAll = SqlHelperHere.ExecuteGetList<CouponView>(str);
            if (listAll.Count < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
            //分页
            var list2 = listAll.Skip((index - 1) * size).Take(size).ToArray();
            if (list2.Length < 1)
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }

            var count = listAll.Count;
            pages = count / size;
            //总页数
            pages = pages * size == count ? pages : pages + 1;
            var listR = new List<DownList>();
            foreach (var item in list2)
            {
                DownList d = new DownList(item);
                listR.Add(d);
            }

            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
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
            CouponOper.Instance.Delete(Convert.ToInt32(req.couponId));
            return JsonConvert.SerializeObject(r);
        }

        public string BatchDel(webReq reqs)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
         
            var ids = reqs.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                CouponOper.Instance.Delete(Convert.ToInt32(ids[i]));
         
            }
            return JsonConvert.SerializeObject(r);
        }

        [HttpPost]
        public string CreateCode(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var money = Convert.ToDecimal(req.couponMoney);
            var cname = req.name;
            var isRepeat = req.isRepeat == "1" ? true : false;
            var amount = Convert.ToInt32(req.amount);

            if (ControllerHelper.Instance.IsExists("coupon", "name", cname, "0"))
            {
                r.Message = "已存在该名称";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
            }

            var url =  apiHost + "/coupon/download?#";
            var timeStamp = StringHelperHere.Instance.GetTimeStamp();

            var lastId = CouponOper.Instance.GetLastCoupon();
            string key = "fdCkey";
            var now = DateTime.Now;
            var path = Server.MapPath("~/") + "/img/coupon/" + cname;
            path = path.Replace('/', '\\');
            for (int i = 0; i < amount; i++)
            {
                Coupon coupon = new Coupon();
                lastId++;
                var no = StringHelperHere.Instance.GetLastFiveStr(lastId.ToString());
                var ran = RandHelper.Instance.Str_char(1, true).ToLower();
                var word = StringHelperHere.Instance.IntToKey(no);
                var couponNo = StringHelperHere.Instance.Encrypt(word, ran + key) + ran;
                coupon.batch = timeStamp;
                coupon.couponNo = couponNo;
                coupon.createTime = now;
                coupon.money = money;
                coupon.isRepeat = isRepeat;
                coupon.name = cname;
                CouponOper.Instance.Add(coupon);

                var bitmap = QCodeHelper.Instance.Create_ImgCode(url + couponNo, 32);
                DirectorySecurity ds = new DirectorySecurity();
                ds.SetAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                Directory.CreateDirectory(path, ds);
                var name = couponNo;
                var pathStr = path + "\\" + name + ".jpg";
                bitmap.Save(pathStr, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            QCodeHelper.Instance.PackageFolder(path, path + ".zip", true);
            return JsonConvert.SerializeObject(r);
            //return JsonHelper.Instance.JsonMsg(true, "");
        }

        public string DeleteByTs(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var timestamp = req.timestamp;
            CouponOper.Instance.DeleteByTs(timestamp);
            return JsonConvert.SerializeObject(r);
        }

    }
}
