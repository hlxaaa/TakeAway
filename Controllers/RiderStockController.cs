using Common.Attribute;
using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers
{
    public class RiderStockController : BaseController
    {
        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();

        /// <summary>
        /// 骑手获取自己的库存
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetStock(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var list = RiderStockOper.Instance.GetRSViewByRiderId(riderId);
            list = list.Where(p => p.amount > 0).ToList();
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无库存");
            else
            {
                List<RiderStockRes> listR = new List<RiderStockRes>();
                foreach (var item in list)
                {
                    RiderStockRes res = new RiderStockRes(item, apiHost);
                    listR.Add(res);
                }
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
            }
        }

        /// <summary>
        /// 获取同区域内的其他骑手列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetOtherRiders(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var rider = RiderInfoOper.Instance.GetById(riderId);
            var areaId = (int)rider.riderAreaId;
            var r = RiderInfoOper.Instance.GetRidersInArea(areaId, riderId);
            if (r.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "无其他骑手");

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(r), "");
        }

        /// <summary>
        /// 给别人。应该是直接给了，不存调配记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GiveToOtherRider(GiveToOtherRider req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var list_foods = JsonConvert.DeserializeObject<List<foodId_Amount>>(req.listFoods);
            var listRs = CacheHelper.GetByCondition<RiderStock>("RiderStock", " isdeleted=0 and riderid=" + riderId);
            var ids = list_foods.Select(p => p.foodId).Distinct().ToArray();
            var tempList = listRs.Where(p => ids.Contains((int)p.foodId)).ToList();
            foreach (var item in list_foods)
            {
                var amount = tempList.Where(p => p.foodId == item.foodId).First().amount;
                if (amount < item.amount)
                    return ControllerHelper.Instance.JsonResult(500, "库存不够");
            }
            var targetRiderId = Convert.ToInt32(req.targetRiderId);

            RiderStockOper.Instance.RemoveRs(riderId, list_foods);
            RiderStockOper.Instance.AddRs(targetRiderId, list_foods);
            return ControllerHelper.Instance.JsonResult(200, "调配成功");
        }

        /// <summary>
        /// 向别人要，存调配记录，还需要统一  
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage ApplyToOtherRider(GiveToOtherRider req)
        {
            //-txy 这里要不要判断请求的人够不够呢
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var list_foods = JsonConvert.DeserializeObject<List<foodId_Amount>>(req.listFoods);



            var targetRiderId = Convert.ToInt32(req.targetRiderId);
            var targetListFoods = RiderStockOper.Instance.GetByRiderId(targetRiderId);
            if (targetListFoods.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "对方库存不足");
            foreach (var item in list_foods)
            {
                var temp = targetListFoods.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count < 1)
                    return ControllerHelper.Instance.JsonResult(500, "对方库存不足");
                if (temp.First().amount < item.amount)
                    return ControllerHelper.Instance.JsonResult(500, "对方库存不足");
            }


            var timestamp = StringHelperHere.Instance.GetTimeStamp() + StringHelperHere.Instance.GetLastFiveStr(riderId.ToString());
            var allocateId = 0;
            foreach (var item in list_foods)
            {
                AllocateRecord ar = new AllocateRecord();
                ar.riderId = riderId;
                ar.foodId = item.foodId;
                ar.amount = item.amount;
                ar.targetRiderId = targetRiderId;
                ar.status = 0;
                ar.timestamp = timestamp;
                allocateId = AllocateRecordOper.Instance.Add(ar);
            }

            //推送消息
            UPushHelper.Instance.PushRiderApply(targetRiderId, list_foods, riderId, timestamp);

            return ControllerHelper.Instance.JsonResult(200, "已发送调配请求");
        }

        /// <summary>
        /// 获取发出的请求
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetApplyRecord(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var list = CacheHelper.GetByCondition<AllocateView>("allocateview", " status=0 and riderid=" + riderId);

            if (list.Count < 0)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无请求");

            var targetRiderIds = list.Select(p => p.targetRiderId).Distinct().ToArray();
            List<AllocateRes> listR = new List<AllocateRes>();
            foreach (var item in targetRiderIds)
            {
                var temp = list.Where(p => p.targetRiderId == item).ToList();
                AllocateRes res = new AllocateRes(temp);
                listR.Add(res);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 获取收到的请求（别人向我申请的）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetOtherApplyRecord(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var list = CacheHelper.GetByCondition<AllocateView>("allocateview", " status=0 and targetRiderid=" + riderId);

            if (list.Count < 0)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无请求");

            var riderIds = list.Select(p => p.riderId).Distinct().ToArray();
            List<AllocateMeRes> listR = new List<AllocateMeRes>();
            foreach (var item in riderIds)
            {
                var temp = list.Where(p => p.riderId == item).ToList();
                AllocateMeRes res = new AllocateMeRes(temp);
                listR.Add(res);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 同意申请
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage AgreeApply(AgreeApplyReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            //此处的两个骑手id要换一下
            var targetRiderId = Convert.ToInt32(req.targetRiderId);
            var listAr = AllocateRecordOper.Instance.GetByRiderId(targetRiderId);

            var listArTemp = new List<AllocateRecord>();


            foreach (var item in listAr)
            {
                if (item.status != 1)//防止连点
                {
                    item.status = 1;
                    listArTemp.Add(item);
                    foodId_Amount fa = new foodId_Amount((int)item.foodId, (int)item.amount);
                    var listFoods = new List<foodId_Amount>
                    {
                        fa
                    };
                    if (RiderStockOper.Instance.RemoveRs((int)item.targetRiderId, listFoods))
                        RiderStockOper.Instance.AddRs((int)item.riderId, listFoods);
                    else
                        return ControllerHelper.Instance.JsonResult(500, "您的库存不足");
                }
            }

            foreach (var item in listArTemp)
            {
                AllocateRecordOper.Instance.Update(item);
            }

            UPushHelper.Instance.PushRiderAgree(riderId, targetRiderId);
            return ControllerHelper.Instance.JsonResult(200, "调配成功");
        }

        //分割线，以上已写接口文档
        /*————————————————————————————————————————————————*/

    }
}
