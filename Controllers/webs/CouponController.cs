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
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.webs
{
    public class CouponController : Controller
    {
        //batch就是ts

        string key = "fdCkey";

        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();

        public ActionResult Index()
        {
            if (Session["pfId"] != null)
            {
                var days = Convert.ToInt32(new SettingRes().couponSaveDays);
                //过时的二维码图片和zip删掉
                ControllerHelper.Instance.ClearTempQCode(this.Server.MapPath("/Img/coupon"), days);
                //过时的优惠券删掉
                string str = "update Coupon set isDeleted=1 where  DATEDIFF(SECOND, createTime, GETDATE())>" + days * 24 * 3600;
                SqlHelperHere.ExcuteNon(str);
                var tsDict = CouponOper.Instance.GetTs();
                ViewBag.tss = tsDict;

                return View();
            }
            else
                return View("_Login");
        }

        public ActionResult DownCoupon()
        {
            if (Session["pfId"] != null)
                return View();
            else
                return View("_Login");
        }

        public ActionResult DownLoad()
        {
            return View();
        }

        public string Get(webReq req)
        {
            var set = new SettingRes();
            var days = Convert.ToInt32(set.couponSaveDays);
            int isUse = req.isUse;
            string search = Request["search"] == null ? "" : Request["search"];//搜索内容
            int index = Convert.ToInt32(Request["index"]);//页码
            var batch = Request["batch"];
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
                return JsonHelperHere.EmptyJson();
            //分页
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            if (ids.Length < 1)
                return JsonHelperHere.EmptyJson();
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

            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.id)), index);
        }

        public string GetDownloadList(webReq req)
        {
            string search = Request["search"] == null ? "" : Request["search"];//搜索内容
            int index = Convert.ToInt32(Request["index"]);//页码
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
                return JsonHelperHere.EmptyJson();
            //分页
            var list2 = listAll.Skip((index - 1) * size).Take(size).ToArray();
            if (list2.Length < 1)
                return JsonHelperHere.EmptyJson();
            //var idsStr = string.Join(",", ids);
            //查具体信息
            //var viewList = CacheHelper.GetByCondition<CouponView>("CouponView", " name in (" + idsStr + ")");


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

            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
        }

        public void Delete(webReq req)
        {
            CouponOper.Instance.Delete(Convert.ToInt32(req.couponId));
        }

        public void BatchDel(webReq reqs)
        {
            string[] ids = Request.Form.GetValues("ids[]");
            for (int i = 0; i < ids.Length; i++)
            {
                CouponOper.Instance.Delete(Convert.ToInt32(ids[i]));
                //DoDelete(Convert.ToInt32(ids[i]));
            }
        }
        

        [HttpPost]
        public string CreateCode(webReq req)
        {
            var money = Convert.ToDecimal(req.couponMoney);
            var cname = req.name;
            var isRepeat = req.isRepeat == "1" ? true : false;
            var amount = Convert.ToInt32(req.amount);

            if(money<0)
                return JsonHelper.Instance.JsonMsg(false, "价格不能为负数");
            if (ControllerHelper.Instance.IsExists("coupon", "name", cname, "0"))
                return JsonHelper.Instance.JsonMsg(false, "已存在该名称");

            var url = apiHost + "/coupon/download?#";
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

            return JsonHelper.Instance.JsonMsg(true, "");
        }

        public void DeleteByTs(webReq req)
        {
            var timestamp = req.timestamp;
            CouponOper.Instance.DeleteByTs(timestamp);
        }

    }
}
