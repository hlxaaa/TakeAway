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
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class RiderAjaxController : WebAjaxController
    {
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var riderType = req.riderType;
            var areaId = req.areaId;

            Dictionary<int, string> dict = AreaInfoOper.Instance.GetAreaDict();
            string search = req.search ?? "";//搜索内容
            int index = Convert.ToInt32(req.index);//页码
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
            {
                r.data = JsonHelperHere.EmptyJson();
                return JsonConvert.SerializeObject(r);
            }
            var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
            var idsStr = string.Join(",", ids);

            var viewList = CacheHelper.GetByCondition<RiderView>("RiderView", " id in (" + idsStr + ")");

            var count = listAll.Count;
            pages = count / size;
            pages = pages * size == count ? pages : pages + 1;//总页数

            r.data= JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(viewList.OrderByDescending(p => p.id)), index);
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
            var pwd = req.pwd;
            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
            {
                r.HttpCode = 500;
                r.Message = "密码应为6~16位英文字母、数字";
                //r.data = JsonHelperHere.JsonMsg(false, "密码应为6~16位英文字母、数字");
                return JsonConvert.SerializeObject(r);
            }

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
                r.Message= "已存在这个账号";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
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
            //JsonHelperHere.JsonMsg(true, "");
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
            var pwd = req.pwd;

            if (!StringHelperHere.Instance.IsPwdValidate(pwd))
            {
                r.Message="密码应为6~16位英文字母、数字";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);

            }
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
                r.Message= "已存在这个账号";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
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
            //return JsonHelperHere.JsonMsg(true, "");
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
            var id = req.riderId;
            RiderInfoOper.Instance.Delete(id);
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
                RiderInfoOper.Instance.Delete(Convert.ToInt32(ids[i]));
            }
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 获取可指派的骑手
        /// </summary>
        /// <returns></returns>
        public string GetAssignRider(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
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

            r.data= JsonConvert.SerializeObject(listR);
            return JsonConvert.SerializeObject(r);
        }

        public string GetRiderPhone(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var riderId = req.riderId;
            var rider = RiderInfoOper.Instance.GetById(riderId);
            r.data= rider.phone;//-txy
            return JsonConvert.SerializeObject(r);
        }






        /// <summary>
        /// 批量骑手移除区域
        /// </summary>
        public string BatchRemoveArea(webReq req)
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
            r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(list), index);
            return JsonConvert.SerializeObject(r);
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
            var id = req.riderId;
            RiderInfoOper.Instance.RemoveArea(id);
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

    }
}
