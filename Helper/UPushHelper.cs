using Common;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;
using UPush;

namespace takeAwayWebApi.Helper
{
    public class UPushHelper : SingleTon<UPushHelper>
    {
        SettingRes set = new SettingRes();

        /// <summary>
        /// 告诉某个区域内的骑手，有新的订单可以接
        /// </summary>
        /// <param name="areaId"></param>
        public void PushRiderNewOrder(int areaId)
        {
            var listRiderDeviceToken = RiderInfoOper.Instance.GetRiderDeviceTokenInArea(areaId);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            var r = RiderPush.PostMany(listRiderDeviceToken, "有新的订单可以接取", "newOrder", dict);
        }

        /// <summary>
        /// 骑手向别人请求菜品，发送到别人那的消息
        /// </summary>
        /// <param name="targetRiderId"></param>
        /// <param name="foodId"></param>
        /// <param name="amount"></param>
        /// <param name="riderId"></param>
        public void PushRiderApply(int targetRiderId, List<foodId_Amount> listFoods, int riderId, string timestamp)
        {
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", " id in (" + riderId + "," + targetRiderId + ")");
            var name = list.Where(p => p.id == riderId).First().name;
            var deviceToken = list.Where(p => p.id == targetRiderId).First().deviceToken;
            var ids = listFoods.Select(p => p.foodId).Distinct().ToArray();
            var idsStr = string.Join(",", ids);
            var foodList = CacheHelper.GetByCondition<FoodInfo>("foodinfo", " id in (" + idsStr + ")");
            var str = "骑手" + name + "向您请求调配";
            foreach (var item in foodList)
            {
                var listTemp = listFoods.Where(p => p.foodId == item.id).ToList();
                if (listTemp.Count > 0)
                {
                    var amount = listTemp.First().amount;
                    str += amount + "份" + item.foodName + ",";
                }
            }
            str = StringHelperHere.Instance.RemoveLastOne(str);

            var dict = new Dictionary<string, string>();
            dict.Add("timestamp", timestamp);
            //RiderPush.PostOne(deviceToken, "有新的调配请求", "allocate", dict);
            RiderPush.PostOne(deviceToken, str, "allocate", dict);
        }

        /// <summary>
        /// 骑手同意别人的请求
        /// </summary>
        /// <param name="targetRiderId"></param>
        /// <param name="riderId"></param>
        public void PushRiderAgree(int targetRiderId, int riderId)
        {
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", " id in (" + riderId + "," + targetRiderId + ")");
            var name = list.Where(p => p.id == targetRiderId).First().name;
            var deviceToken = list.Where(p => p.id == riderId).First().deviceToken;
            var str = "骑手" + name + "已同意您的调配请求";
            RiderPush.PostOne(deviceToken, str, "allocate", new Dictionary<string, string>());
        }

        /// <summary>
        /// 后台指派订单给骑手，通知骑手
        /// </summary>
        public void PushRiderServerAssign(OrderInfo order)
        {
            var rider = RiderInfoOper.Instance.GetById((int)order.riderId);
            var dict = new Dictionary<string, string>();
            RiderPush.PostOne(rider.deviceToken, set.serverAssignRiderTips, "test", dict);
        }

        /// <summary>
        /// 告诉用户，订单配送中
        /// </summary>
        /// <param name="order"></param>
        public void PushUserSendingOrder(OrderInfo order)
        {
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            var deviceToken = user.deviceToken;
            if (user.deviceType == "ios")
            {
                IOSPush.PostOne(deviceToken, set.orderSendingTips, "TakeOrder", new Dictionary<string, string>());
            }
            else
            {
                AndroidPush.PostOne(deviceToken, set.orderSendingTips, "TakeOrder", new Dictionary<string, string>());
            }
        }

        /// <summary>
        /// 告诉骑手，订单可以回收了
        /// </summary>
        public void PushRiderToRecovery(OrderInfo order) {
            var rider = RiderInfoOper.Instance.GetById((int)order.riderId);
            var deviceToken = rider.deviceToken;
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(deviceToken))
            {
                RiderPush.PostOne(deviceToken, set.recoveryBoxTips, "CycleOrder", dict);
            }
        }

        /// <summary>
        /// 告诉用户，订单送达了
        /// </summary>
        /// <param name="order"></param>
        public void PushUserOrderArrive(OrderInfo order) {
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            var deviceToken = user.deviceToken;
            var dict = new Dictionary<string, string>();
            var orderId = order.id.ToString();
            if (!string.IsNullOrEmpty(deviceToken))
            {
                if (user.deviceType == "ios")
                {
                    dict.Add("orderId", orderId.ToString());
                    IOSPush.PostOne(deviceToken, set.orderArrivelTips, "ArriveOrder", dict);
                }
                else
                {
                    dict.Add("arriveOrderId", orderId.ToString());
                    AndroidPush.PostOne(deviceToken, set.orderArrivelTips, "ArriveOrder", dict);
                }
            }
        }

        /// <summary>
        /// 告诉用户，餐盒已经收到
        /// </summary>
        /// <param name="order"></param>
        public void PushUserRecovery(OrderInfo order) {
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            user.userBalance += order.deposit;//押金退回余额
            UserInfoOper.Instance.Update(user);
            var deviceToken = user.deviceToken;
            var orderId = order.id;
            var dict = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(deviceToken))
            {
                if (user.deviceType == "ios")
                {
                    dict.Add("orderId", orderId.ToString());
                    IOSPush.PostOne(deviceToken, set.boxGetTips, "Recovery", dict);
                }
                else
                {
                    dict.Add("arriveOrderId", orderId.ToString());
                    AndroidPush.PostOne(deviceToken, set.boxGetTips, "Recovery", dict);
                }
            }
        }

        /// <summary>
        /// 告诉用户，订单被取消了
        /// </summary>
        /// <param name="order"></param>
        public void PushUserOrderCancel(OrderInfo order) {
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            var dict = new Dictionary<string, string>();
            if (user.deviceType == "ios")
            {
                UPush.IOSPush.PostOne(user.deviceToken, set.riderCancelTips, "RiderCancel", dict);
            }
            else
            {
                UPush.AndroidPush.PostOne(user.deviceToken, set.riderCancelTips, "RiderCancel", dict);
            }
        }

        /// <summary>
        /// 告诉自提点有新实时订单
        /// </summary>
        /// <param name="order"></param>
        public void PushShopNewOrder(OrderInfo order) {
            var rider = RiderInfoOper.Instance.GetById((int)order.riderId);
            var deviceToken = rider.deviceToken;
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(deviceToken))
            {
                RiderPush.PostOne(deviceToken, "您有新的实时订单", "shopNewOrder", dict);
            }
        }

        /// <summary>
        /// 告诉自提点有新预定订单
        /// </summary>
        /// <param name="order"></param>
        public void PushShopNewReserveOrder(OrderInfo order)
        {
            var rider = RiderInfoOper.Instance.GetById((int)order.riderId);
            var deviceToken = rider.deviceToken;
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(deviceToken))
            {
                RiderPush.PostOne(deviceToken, "您有新的预定订单", "shopNewOrder", dict);
            }
        }

        


        //————————组合拳——————————————

        /// <summary>
        /// 支付之后根据订单的类型推送消息（普通实时推区域，自提推给自提点）
        /// </summary>
        /// <param name="order"></param>
        public void PushAfterPayOrder(OrderInfo order) {
            var isTake = (bool)order.isTake;
            var isActual = (bool)order.isActual;
            if (!isTake && isActual)
                PushRiderNewOrder((int)order.orderAreaId);
            else if (isTake)
            {
                if (isActual)
                    PushShopNewOrder(order);
                else
                    PushShopNewReserveOrder(order);
            }
        }
    }
}