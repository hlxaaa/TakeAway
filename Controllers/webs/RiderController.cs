using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using takeAwayWebApi.Controllers.webs;
//using takeAway.Helper;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAway.Controllers
{
    public class RiderController : WebBaseController
    {
        //
        // GET: /Rider/
        //RiderInfoOper riderOper = new RiderInfoOper();
        //AreaInfoOper areaOper = new AreaInfoOper();
        //OrderInfoOper orderOper = new OrderInfoOper();
        //UserAddressOper uaOper = new UserAddressOper();

        public ActionResult Index()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            Dictionary<int, string> dict = AreaInfoOper.Instance.GetAreaDict();
            ViewBag.dictArea = dict;
            return View();
        }

        public ActionResult Detail(webReq req)
        {
            if (Session["pfId"] == null)
                return View("_Login");
            var riderId = req.riderId;
            ViewBag.riderId = riderId;
            ViewBag.areaDict = AreaInfoOper.Instance.GetAreaDict();
            ViewBag.rider = new RiderInfo();
            var isAdd = 0;
            if (riderId == 0)//新增
            {
                ViewBag.isRight = true;
                ViewBag.type = 2;
                isAdd = 1;
            }
            else
            {
                var rider = RiderInfoOper.Instance.GetById(riderId);
                if (rider == null)
                { //不存在的骑手id
                    ViewBag.isRight = false;
                }
                else
                { //正戏
                    ViewBag.isRight = true;
                    ViewBag.type = rider.riderType;
                    ViewBag.rider = rider;
                }
            }
            ViewBag.isAdd = isAdd;
            return View();
        }

        public string Get(webReq req)
        {
            var riderType = req.riderType;
            var areaId = req.areaId;

            Dictionary<int, string> dict = AreaInfoOper.Instance.GetAreaDict();
            string search = Request["search"] == null ? "" : Request["search"];//搜索内容
            int index = Convert.ToInt32(Request["index"]);//页码
            int pages = 0;//总页数
            int size = 12;//一页size条数据

            var condition = " 1=1 and ";
            if (riderType != 3)
                condition += "  ridertype=" + riderType + " and";
            if (areaId != 0)
                condition += "  riderareaid=" + areaId + " and";

            condition += "  ( rideraccount like '%" + search + "%' or name like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%'  )";

            var listAll = CacheHelper.GetDistinctCount<RiderView>("RiderView", condition);
            if (listAll.Count < 1)
                return JsonHelperHere.EmptyJson();
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            var idsStr = string.Join(",", ids);

            var viewList = CacheHelper.GetByCondition<RiderView>("RiderView", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            pages = pages * size == count ? pages : pages + 1;//总页数

            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(viewList.OrderByDescending(p => p.id)), index);
        }

        public string Add(webReq req)
        {
            var pwd = req.pwd;
            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
                return JsonHelperHere.JsonMsg(false, "密码应为6~16位英文字母、数字");

            var lat = req.lat;
            var lng = req.lng;
            var mapAddress = req.mapAddress;
            var riderType = req.riderType;
            var account = req.account;
            var phone = req.phone;

            var riderName = req.riderName;
            var status = req.status;
            var areaId = req.riderAreaId;
            var stars = req.stars;
            stars = stars < 0 ? 0 : stars;
            stars = stars > 5 ? 5 : stars;
            var starCount = req.starCount;
            var sendCount = req.sendCount;
            if (RiderInfoOper.Instance.IsHaveAccount(account, 0))
            {
                return JsonHelperHere.JsonMsg(false, "已存在这个账号");
            }

            RiderInfo rider = new RiderInfo();
            rider.riderAccount = account;
            rider.riderPwd = pwd;
            rider.riderType = riderType;
            rider.riderStatus = Convert.ToBoolean(status);
            rider.name = riderName;
            rider.riderAreaId = areaId;
            rider.phone = phone;
            rider.avgStars = stars;
            rider.starCount = starCount;
            rider.sendCount = sendCount;

            if (riderType == 2)
            {
                rider.lat = lat;
                rider.lng = lng;
                rider.mapAddress = mapAddress;
            }

            var riderId = RiderInfoOper.Instance.Add(rider);
            var temp = CacheHelper.GetRiderPosition(riderId);
            if (riderType != 2)
                CacheHelper.SetRiderPosition(riderId, temp.lat, temp.lng, riderType);
            else
            {
                CacheHelper.SetRiderPosition(riderId, lat, lng, riderType);
                UserAddress ua = new UserAddress();
                ua.riderId = riderId;
                ua.lat = lat;
                ua.lng = lng;
                ua.mapAddress = mapAddress;
                ua.phone = phone;
                UserAddressOper.Instance.Add(ua);
            }
            return JsonHelperHere.JsonMsg(true, "");
        }

        public string Update(webReq req)
        {
            var pwd = req.pwd;

            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
                return JsonHelperHere.JsonMsg(false, "密码应为6~16位英文字母、数字");
            var riderId = req.riderId;
            var lat = req.lat;
            var lng = req.lng;
            var mapAddress = req.mapAddress;
            var riderType = req.riderType;
            var account = req.account;
            var riderName = req.riderName;
            var status = req.status;
            var areaId = req.riderAreaId;
            var stars = req.stars;
            stars = stars < 0 ? 0 : stars;
            stars = stars > 5 ? 5 : stars;
            var phone = req.phone;
            var starCount = req.starCount;
            var sendCount = req.sendCount;
            if (RiderInfoOper.Instance.IsHaveAccount(account, riderId))
            {
                return JsonHelperHere.JsonMsg(false, "已存在这个账号");
            }

            RiderInfo rider = new RiderInfo();
            rider.riderAccount = account;
            rider.riderPwd = pwd;
            rider.riderType = riderType;
            rider.riderStatus = Convert.ToBoolean(status);
            rider.name = riderName;
            rider.riderAreaId = areaId;
            rider.id = riderId;
            rider.phone = phone;
            rider.avgStars = stars;
            rider.starCount = starCount;
            rider.sendCount = sendCount;

            if (riderType == 2)
            {
                rider.lat = lat;
                rider.lng = lng;
                rider.mapAddress = mapAddress;

                var ua = UserAddressOper.Instance.GetByRiderId(riderId);
                if (ua == null)
                {
                    ua = new UserAddress();
                    ua.riderId = riderId;
                    ua.lat = lat;
                    ua.lng = lng;
                    ua.mapAddress = mapAddress;
                    ua.phone = phone;
                    UserAddressOper.Instance.Add(ua);
                }
                else
                {
                    ua.lat = lat;
                    ua.lng = lng;
                    ua.mapAddress = mapAddress;
                    ua.phone = phone;
                    UserAddressOper.Instance.Update(ua);
                }
            }
            else
            {
                rider.lat = "";
                rider.lng = "";
                rider.mapAddress = "";
            }

            RiderInfoOper.Instance.Update(rider);
            var temp = CacheHelper.GetRiderPosition(riderId);
            if (riderType != 2)
                CacheHelper.SetRiderPosition(riderId, temp.lat, temp.lng, riderType);
            else
            {
                CacheHelper.SetRiderPosition(riderId, lat, lng, riderType);
            }
            return JsonHelperHere.JsonMsg(true, "");
        }

        public void Delete(webReq req)
        {
            var id = req.riderId;
            //int id = Convert.ToInt32(Request["riderId"]);
            RiderInfoOper.Instance.Delete(id);
        }

        public void BatchDel(webReq req)
        {
            //var ids = req.ids;
            string[] ids = Request.Form.GetValues("ids[]");
            for (int i = 0; i < ids.Length; i++)
            {
                RiderInfoOper.Instance.Delete(Convert.ToInt32(ids[i]));
            }
        }

        /// <summary>
        /// 骑手更换所属区域
        /// </summary>
        public string ChangeArea(webReq req)
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
            var areaId = req.areaId;
            var id = req.riderId;
            RiderInfo rider = new RiderInfo();
            rider.id = id;
            rider.riderAreaId = areaId;
            RiderInfoOper.Instance.Update(rider);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 将骑手从区域中移除
        /// </summary>
        public string RemoveArea(webReq req)
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
            var id =req.riderId;
            RiderInfoOper.Instance.RemoveArea(id);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 批量骑手移除区域
        /// </summary>
        public string BatchRemoveArea(webReq req)
        {
            //string[] ids = Request.Form.GetValues("ids[]");
            var ids = req.ids;
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            //var ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                RiderInfoOper.Instance.RemoveArea(Convert.ToInt32(ids[i]));
            }
            return JsonConvert.SerializeObject(r);
        }

        public string GetByAreaId(webReq req)
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
            int index = req.index;
            int pages = 0;//总页数
            int size = 12;//一页size条数据
            int areaId = req.areaId;
            List<RiderInfo> list = RiderInfoOper.Instance.GetByAreaId(areaId);
            list = list.OrderByDescending(p => p.id).Where(p => p.isDeleted == false).ToList();
            pages = list.Count / size;
            pages = pages * size == list.Count ? pages : pages + 1;//总页数
            list = list.Skip((index - 1) * size).Take(size).ToList();
            r.data= JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(list), index);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 获取不在区域内的骑手
        /// </summary>
        /// <returns></returns>
        public string GetOtherRider(webReq req)
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
            var areaId = req.areaId;
            List<RiderInfo> list = (List<RiderInfo>)CacheHelper.Get<RiderInfo>("RiderInfo", "Ta_RiderInfo");
            list = list.Where(p => p.riderAreaId != areaId).ToList();
            r.data = JsonConvert.SerializeObject(list);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 改变rider所在的区域
        /// </summary>
        public string Change(webReq req)
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
            var sourceId = req.sourceId;
            var riderId = req.riderId;
            var areaId = req.areaId;
            RiderInfo rider1 = new RiderInfo();
            rider1.riderAreaId = 0;
            rider1.id = sourceId;
            RiderInfoOper.Instance.Update(rider1);
            RiderInfo rider2 = new RiderInfo();
            rider2.id = riderId;
            rider2.riderAreaId = areaId;
            RiderInfoOper.Instance.Update(rider2);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 获取可指派的骑手
        /// </summary>
        /// <returns></returns>
        public string GetAssignRider(webReq req)
        {
            var orderId = req.orderId;
            var order = OrderInfoOper.Instance.GetById(orderId);
            var ua = UserAddressOper.Instance.GetById((int)order.addressId);
            var listArea = AreaInfoOper.Instance.GetAreaByLocation(Convert.ToDecimal(ua.lat), Convert.ToDecimal(ua.lng));

            List<AssignRiderRes> listR = new List<AssignRiderRes>();
            foreach (var item in listArea)
            {
                var list = RiderInfoOper.Instance.GetOnlineByAreaId(item.areaId);

                foreach (var item2 in list)
                {
                    AssignRiderRes res = new AssignRiderRes(item2);
                    listR.Add(res);
                }
            }

            return JsonConvert.SerializeObject(listR);
        }

        public string GetRiderPhone(webReq req)
        {
            var riderId = req.riderId;
            var rider = RiderInfoOper.Instance.GetById(riderId);
            return rider.phone;
        }
    }
}
