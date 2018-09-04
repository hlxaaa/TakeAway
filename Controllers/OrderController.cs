using Common.Attribute;
using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers
{
    public class OrderController : BaseController
    {
        int size = 10;
        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();

        #region 客户端订单接口
        /// <summary>
        /// 首页下单或者调整饮料数量计算金额接口，余额是总余额，优惠券是总优惠券(选择自提点后，要掉这个接口，去掉押金的部分)
        /// </summary>
        ///
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage PlaceOrder(PlaceOrderReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            OrderRes res = new OrderRes();
            var listFoods = req.listFoods;
            var list_foods = JsonConvert.DeserializeObject<List<foodId_Amount>>(listFoods);
            var isUseB = Convert.ToInt32(req.isUseBalance) == 1 ? true : false;
            var isUseC = Convert.ToInt32(req.isUseCoupon) == 1 ? true : false;
            var areaId = Convert.ToInt32(req.areaId);
            var ua = new UserAddressView();
            if (req.addressId != null)
            {
                var addressId = Convert.ToInt32(req.addressId);
                ua = UserAddressOper.Instance.GetUAViewById(addressId);
                res.userAddress = UserAddressOper.Instance.GetUaResByUAView(ua);
            }
            else
            {
                var list_addr = UserAddressOper.Instance.GetRecentlyAddr(userId);
                if (list_addr.Count < 1)
                    res.userAddress = null;
                else
                    res.userAddress = new UserAddressForOrderRes(list_addr.First());
            }


            #region 根据库存调整菜品数量
            var listRs = RiderStockOper.Instance.GetRSJoinByAreaId(areaId);
            var listTemp = new List<foodId_Amount>();
            foreach (var item in list_foods)
            {
                var rsAmount = listRs.Where(p => p.foodId == item.foodId).ToList().Sum(p => p.amount);
                item.amount = item.amount > rsAmount ? (int)rsAmount : item.amount;
                listTemp.Add(item);
            }
            #endregion

            list_foods = listTemp;

            var user = UserInfoOper.Instance.GetById(userId);//获取用户信息

            var balance = (decimal)user.userBalance;
            var coupon = (decimal)user.coupon;

            var useBalance = isUseB ? balance : 0;
            var useCoupon = isUseC ? coupon : 0;

            var ids = list_foods.Select(p => p.foodId).ToArray();
            var idsStr = string.Join(",", ids);
            var tempList = CacheHelper.GetByCondition<FoodView>("FoodView", " id in (" + idsStr + ")");

            List<foodId_Amount> listfa = new List<foodId_Amount>();
            foreach (var item in list_foods)
            {
                foodId_Amount fa = new foodId_Amount(item.foodId, item.amount);
                listfa.Add(fa);
            }
            var fd = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listfa);
            var deposit = fd.deposit;
            deposit = ua.riderType == 2 ? 0 : deposit;
            var foodsPrice = fd.price;
            var priceRes = ControllerHelper.Instance.GetThreePriceWithDeposit(useBalance, useCoupon, foodsPrice, deposit);

            res.balance = balance.ToString();
            res.coupon = coupon.ToString();
            PriceAll pa = new PriceAll();
            pa.payMoney = priceRes.payMoney.ToString();
            pa.price = (priceRes.payMoney + priceRes.useBalance).ToString();
            res.priceAll = pa;

            List<foodId_name_amount_price_isMain> foodListRes = new List<foodId_name_amount_price_isMain>();
            foreach (var item in tempList)
            {
                foodId_name_amount_price_isMain a = new foodId_name_amount_price_isMain();
                a.foodId = item.id;
                a.foodName = item.foodName;
                a.foodPrice = item.foodPrice.ToString();
                a.isMain = (bool)item.isMain;
                foodListRes.Add(a);
            }

            for (int i = 0; i < foodListRes.Count; i++)
            {
                foreach (var item in list_foods)
                {
                    if (foodListRes[i].foodId == item.foodId)
                    {
                        foodListRes[i].amount = item.amount;
                        break;
                    }
                }
            }

            res.listFoods = foodListRes.OrderByDescending(p => p.foodId).ToList();
            res.isUseBalance = isUseB;
            res.isUseCoupon = isUseC;
            res.remarks = req.remarks ?? "";
            res.deposit = deposit.ToString();
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 创建未支付订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage CreateOrder(CreateOrderReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var addressId = Convert.ToInt32(req.addressId);

            var ua = CacheHelper.GetById<UserAddressView>("UserAddressView", addressId);
            var areaId = Convert.ToInt32(req.areaId);
            if (ua.riderType != 2)
            {
                var areas = AreaInfoOper.Instance.GetAreaByLocation(Convert.ToDecimal(ua.lat), Convert.ToDecimal(ua.lng));
                if (areas.Count < 1)
                    return ControllerHelper.Instance.JsonResult(500, "配送地址超出当前区域");

                var area = areas.Where(p => p.areaId == areaId).ToList();
                if (area.Count < 1)
                    return ControllerHelper.Instance.JsonResult(500, "配送地址超出当前区域");
            }

            var isUseB = Convert.ToInt32(req.isUseBalance) == 1 ? true : false;
            var isUseC = Convert.ToInt32(req.isUseCoupon) == 1 ? true : false;

            List<foodId_Amount> listFoods = JsonConvert.DeserializeObject<List<foodId_Amount>>(req.listFoods);
            if (listFoods.Count < 1)
            {
                CacheHelper.DelTempFoods2(userId);
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请重新下单");
            }
            var mainFood = FoodInfoOper.Instance.GetMainFood(listFoods);
            if (mainFood == null)
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请重新下单");


            listFoods = listFoods.Where(p => p.amount != 0).ToList();

            var user = UserInfoOper.Instance.GetById(userId);//获取用户信息
            var balance = isUseB ? user.userBalance : 0;
            var coupon = isUseC ? user.coupon : 0;

            var fd = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listFoods);
            var deposit = fd.deposit;
            deposit = ua.riderType == 2 ? 0 : deposit;
            var foodsPrice = fd.price;
            var priceRes = ControllerHelper.Instance.GetThreePriceWithDeposit((decimal)balance, (decimal)coupon, foodsPrice, deposit);

            if (priceRes.payMoney != Convert.ToDecimal(req.payMoney))
                return ControllerHelper.Instance.JsonResult(500, "金额有误1");

            var useBalance = priceRes.useBalance;
            var useCoupon = priceRes.useCoupon;
            var remarks = req.remarks == null ? "" : req.remarks;
            var payType = req.payType;
            var payMoney = Convert.ToDecimal(req.payMoney);
            var sumMoney = useBalance + payMoney;//总金额要不要，包括哪些金额

            OrderInfo order = new OrderInfo();
            order.userId = userId;
            order.addressId = addressId;
            order.useBalance = useBalance;
            order.useCoupon = useCoupon;
            order.remarks = remarks;
            order.payType = payType;
            order.payMoney = payMoney;
            order.sumMoney = sumMoney;
            order.orderAreaId = areaId;
            order.deposit = deposit;

            if (ua.riderType == 2)
            {
                order.riderId = ua.riderId;
                order.isTake = true;
            }


            var orderId = OrderInfoOper.Instance.Add(order);

            //生成订单号
            order.id = orderId;
            var orderNo = StringHelperHere.Instance.CreateOrderNo(order.userId.ToString());
            order.orderNo = orderNo;
            order.status = 0;
            OrderInfoOper.Instance.Update(order);

            foreach (var item in listFoods)
            {
                OrderFoods of = new OrderFoods();
                of.orderId = orderId;
                of.foodId = item.foodId;
                of.amount = item.amount;
                OrderFoodsOper.Instance.Add(of);
            }
            CacheHelper.DelTempFoods2(userId);
            OrderFoodsOper.Instance.SetSnapShot(orderId);
            return ControllerHelper.Instance.JsonResult(200, "orderId", orderId.ToString(), "创建订单成功");
        }

        /// <summary>
        /// 支付订单status（0→1）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PayOrder(PayOrderReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            OrderInfo order = OrderInfoOper.Instance.GetById(orderId);

            if (!RiderStockOper.Instance.IsStockEnough((int)order.orderAreaId, orderId))
            {
                return ControllerHelper.Instance.JsonResult(500, "库存不够");
            }

            var payType = req.payType;
            var payMoney = Convert.ToDecimal(req.payMoney);
            var sumMoney = order.useBalance + payMoney;

            if (order.payMoney != payMoney)
                return ControllerHelper.Instance.JsonResult(500, "金额有误2");

            order.payType = payType;
            order.sumMoney = sumMoney;
            order.status = 1;
            order.payTime = DateTime.Now;

            UserInfo user = UserInfoOper.Instance.GetById(userId);
            user.userBalance -= order.useBalance;
            user.coupon -= order.useCoupon;
            if (user.userBalance < 0 || user.coupon < 0)
                return ControllerHelper.Instance.JsonResult(500, "余额或优惠券金额不足");

            try
            {
                var sql1 = UserInfoOper.Instance.GetUpdateStr(user);
                var sql2 = OrderInfoOper.Instance.GetUpdateStr(order);
                var dict1 = UserInfoOper.Instance.GetParameters(user);
                var dict2 = OrderInfoOper.Instance.GetParameters(order);
                var dicts = new List<Dictionary<string, string>>();
                dicts.Add(dict1);
                dicts.Add(dict2);
                List<string> sqls = new List<string>();
                sqls.Add(sql1);
                sqls.Add(sql2);
                //push
                UPushHelper.Instance.PushRiderNewOrder((int)order.orderAreaId);

                var r = SqlHelperHere.ExecuteInTransaction(sqls, "pay-userId=" + userId + "-orderId=" + orderId, dicts);

                CacheHelper.DelTempFoods2(userId);

                if (r)
                {
                    return ControllerHelper.Instance.JsonResult(200, "支付成功");
                }
                else
                    return ControllerHelper.Instance.JsonResult(500, "支付失败");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "支付失败");
            }

        }

        /// <summary>
        /// 有自提点的支付
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage PayOrder2(PayOrder2Req req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var order = OrderInfoOper.Instance.GetById(orderId);

            if (order.status != 0)
                return ControllerHelper.Instance.JsonResult(500, "订单状态已改变，不能支付");

            if (!RiderStockOper.Instance.IsStockEnough((int)order.orderAreaId, orderId))
            {
                return ControllerHelper.Instance.JsonResult(500, "库存不够");
            }

            var Spbill_create_ip = req.spbill_create_ip;
            var payType = req.payType;
            var payMoney = Convert.ToDecimal(req.payMoney);
            var sumMoney = order.useBalance + payMoney;

            if (order.payMoney != payMoney)
                return ControllerHelper.Instance.JsonResult(500, "金额有误3");

            order.payType = payType;

            UserInfo user = UserInfoOper.Instance.GetById(userId);
            user.userBalance -= order.useBalance;
            user.coupon -= order.useCoupon;
            if (user.userBalance < 0 || user.coupon < 0)
                return ControllerHelper.Instance.JsonResult(500, "余额或优惠券金额不足");

            #region 第三方支付
            if (payType == "alipay" || payType == "wxpay")
            { //第三方支付
                order.payMoney = payMoney;
                OrderInfoOper.Instance.Update(order);

                if (payType == "alipay")
                {
                    AlipayHelperHere a = new AlipayHelperHere();
                    var b = a.CreateAlipayOrder(req.payMoney, order.orderNo, "https://fan-di.com/notify/alipayNotify");

                    JObject jo = new JObject();
                    jo.Add("payType", "alipay");
                    jo.Add("payStr", b);

                    return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
                }
                else
                {//微信支付
                    WXPayHelperHere w = new WXPayHelperHere();
                    var b = w.CreateWXOrder(payMoney, order.orderNo, "https://fan-di.com/notify/wxpayNotify", Spbill_create_ip);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(b);
                    if (doc.DocumentElement["return_code"].InnerText != "SUCCESS")
                    {
                        return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请稍后");
                    }

                    JObject jo = new JObject();
                    jo.Add("payType", "wxpay");
                    foreach (XmlNode item in doc.DocumentElement.ChildNodes)
                    {
                        if (item.Name != "sign")
                            jo.Add(item.Name, item.InnerText);
                    }

                    var timestamp = StringHelperHere.Instance.GetTimeStamp();
                    jo.Add("timestamp", timestamp);
                    var dict = new Dictionary<string, string>();

                    foreach (var item in jo)
                    {
                        if (item.Key == "prepay_id")
                            dict.Add("prepayid", item.Value.ToString());
                        if (item.Key == "nonce_str")
                            dict.Add("noncestr", item.Value.ToString());
                    }

                    dict.Add("appid", "wxd8482615c33b8859");
                    dict.Add("partnerid", "1494504712");

                    dict.Add("package", "Sign=WXPay");
                    dict.Add("timestamp", timestamp);

                    string sign = StringHelperHere.Instance.GetWXSign(dict, "2BCF8DD9490E328D2FCEDE7B26643231");
                    jo.Add("sign", sign);
                    return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
                }
            }
            #endregion

            order.sumMoney = sumMoney;
            if ((bool)order.isTake)//如果是自提订单，直接变为派送中订单
                order.status = 2;
            else
                order.status = 1;
            order.payTime = DateTime.Now;
            SqlCommand cmd = new SqlCommand();

            try
            {
                var sql1 = UserInfoOper.Instance.GetUpdateStr(user);
                var sql2 = OrderInfoOper.Instance.GetUpdateStr(order);
                var dict1 = UserInfoOper.Instance.GetParameters(user);
                var dict2 = OrderInfoOper.Instance.GetParameters(order);
                var dicts = new List<Dictionary<string, string>>();
                dicts.Add(dict1);
                dicts.Add(dict2);
                List<string> sqls = new List<string>();
                sqls.Add(sql1);
                sqls.Add(sql2);

                UPushHelper.Instance.PushAfterPayOrder(order);

                var r = SqlHelperHere.ExecuteInTransaction(sqls, "pay-uid=" + userId + "-oid=" + orderId, dicts);

                CacheHelper.DelTempFoods2(userId);

                if (r)
                {
                    OrderInfoOper.Instance.PayOrderRecord(order);
                    return ControllerHelper.Instance.JsonResult(200, "payType", "", "支付成功");
                }
                else
                    return ControllerHelper.Instance.JsonResult(500, "支付失败");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "支付失败");
            }

        }


        /// <summary>
        /// 获取进行中的订单，目前status：0未支付1待抢单2派送中
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetActiveOrder(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            OrderInfoOper.Instance.RemoveOverTime(userId);//把过时订单去除 -txy完成

            var list = CacheHelper.GetByCondition<OrderInfo>("orderinfo", " isactual=1 and  isdeleted = 0 and  status in (0,1,2) and  userId=" + userId + " order by id desc");

            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无进行中的订单");
            List<ActiveOrderRes> listR = new List<ActiveOrderRes>();
            var list_ov = OrderInfoOper.Instance.GetViewByUserId(userId).ToList();
            foreach (var item in list)
            {
                ActiveOrderRes res;
                if (item.status != 2)
                {
                    var listTemp = list_ov.Where(p => p.orderId == item.id).ToList();
                    res = new ActiveOrderRes(listTemp);
                }
                else
                {
                    //var listTemp = list_osv.Where(p => p.orderId == item.id).ToList();
                    var temp = list_ov.Where(p => p.status == 2 && p.orderId == item.id).ToList();
                    res = new ActiveOrderRes(temp, "");
                }
                listR.Add(res);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");

        }

        /// <summary>
        /// 获取历史订单列表status（3,5,7）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetHistoryOrder(GetHistoryOrderReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);
            var pageIndex = Convert.ToInt32(req.pageIndex);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var condition = "  status in (3,5,7,8,9) and  userId=" + userId;

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", "select DISTINCT id,userOrderStr from OrderTryJoin", condition, pageIndex, size, " order by userorderstr,id desc ", new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无历史订单");
            var ids = list.Select(p => p.id).Distinct().ToArray();
            List<ActiveOrderRes> listR = new List<ActiveOrderRes>();

            foreach (var item in ids)//列表的顺序是根据ids来的，所以不会头变尾
            {
                var listTemp = list.Where(p => p.orderId == item).ToList();
                ActiveOrderRes res = new ActiveOrderRes(listTemp, 1);
                listR.Add(res);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 订单列表里点击订单，返回订单详情页信息，之后要加押金deposit-txy完成
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetOrderByOrderId(UserByOrderIdReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var ovList = OrderInfoOper.Instance.GetViewByOrderId(orderId);
            OrderDetailRes res = new OrderDetailRes(ovList);

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 用户取消订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage UserCancelOrder(UserByOrderIdReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var status = 5;

            var order = OrderInfoOper.Instance.GetById(orderId);
            if (order.status != 1 && order.status != 2)
                return ControllerHelper.Instance.JsonResult(500, "不能取消");

            if (order.riderTakeTime != null)
            {
                if ((DateTime.Now - (DateTime)order.riderTakeTime).Ticks / 10000000 > 60)
                {
                    return ControllerHelper.Instance.JsonResult(500, "骑手已出发1分钟，不可取消");
                }
            }

            var statusNow = order.status;

            var cc = new CancelCache();
            var temp = CacheHelper.GetCancelCache(userId);
            cc = temp == null ? cc : temp;
            if (statusNow == 1)
            { //未被接单
                if (temp == null)
                {
                    cc.userId = userId;
                    cc.time1 = 1;
                    cc.time2 = 0;
                    cc.CreateTime = DateTime.Now;
                    CacheHelper.SetCancelCache(cc);
                }
                else if (cc.CreateTime.Date != DateTime.Now.Date)
                {
                    cc.userId = userId;
                    cc.time1 = 1;
                    cc.time2 = 0;
                    cc.CreateTime = DateTime.Now;
                    CacheHelper.SetCancelCache(cc);
                }
                else if (cc.time1 <= 2)
                {
                    cc.time1 += 1;
                    CacheHelper.SetCancelCache(cc);
                }
                else
                {
                    return ControllerHelper.Instance.JsonResult(500, "取消未派送的订单，一天不能超过3次");
                }
            }
            else if (statusNow == 2)
            { //已接单
                if (temp == null)
                {
                    cc.userId = userId;
                    cc.time1 = 0;
                    cc.time2 = 1;
                    cc.CreateTime = DateTime.Now;
                    CacheHelper.SetCancelCache(cc);
                }
                else if (cc.CreateTime.Date != DateTime.Now.Date)
                {
                    cc.userId = userId;
                    cc.time1 = 0;
                    cc.time2 = 1;
                    cc.CreateTime = DateTime.Now;
                    CacheHelper.SetCancelCache(cc);
                }
                else if (cc.time2 <= 1)
                {
                    cc.time2 += 1;
                    CacheHelper.SetCancelCache(cc);
                }
                else
                {
                    return ControllerHelper.Instance.JsonResult(500, "取消正在派送中的订单，一天不能超过2次");
                }
            }

            order.status = status;
            order.endTime = DateTime.Now;
            var str1 = OrderInfoOper.Instance.GetUpdateStr(order);
            var user = UserInfoOper.Instance.GetById(userId);
            var userHere = new UserInfo();
            userHere.id = user.id;
            userHere.coupon = user.coupon + order.useCoupon;
            userHere.userBalance = user.userBalance + order.useBalance + order.payMoney;

            var str2 = UserInfoOper.Instance.GetUpdateStr(userHere);

            var dict2 = UserInfoOper.Instance.GetParameters(userHere);
            var dict1 = OrderInfoOper.Instance.GetParameters(order);
            var dicts = new List<Dictionary<string, string>>();
            dicts.Add(dict1);
            dicts.Add(dict2);

            List<string> sqls = new List<string>();
            sqls.Add(str1);
            sqls.Add(str2);
            bool result = SqlHelperHere.ExecuteInTransaction(sqls, "ta_Cancel_oid=" + orderId + "_uid=" + userId, dicts);

            if (result)
            {
                UserPay up = new UserPay();
                up.type = 6;
                up.money = order.useBalance + order.payMoney;
                up.userId = order.userId;
                up.createTime = DateTime.Now;
                UserPayOper.Instance.Add(up);
                if (order.useCoupon > 0)
                {
                    UserPay up2 = new UserPay();
                    up2.type = 7;
                    up2.money = order.useCoupon;
                    up2.userId = order.userId;
                    up2.createTime = DateTime.Now;
                    UserPayOper.Instance.Add(up2);
                }

                if (statusNow == 2)
                {
                    var list = OrderFoodsOper.Instance.GetByOrderId(order.id);
                    var listFoods = new List<foodId_Amount>();
                    foreach (var item in list)
                    {
                        foodId_Amount fa = new foodId_Amount(item);
                        listFoods.Add(fa);
                    }
                    RiderStockOper.Instance.AddRs((int)order.riderId, listFoods);//用户取消订单，菜品返还给骑手
                }

                return ControllerHelper.Instance.JsonResult(200, "已取消订单");
            }
            else
            {
                return ControllerHelper.Instance.JsonResult(500, "服务器错误");
            }
        }

        /// <summary>
        /// 评论菜品和骑手
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage AddComment(AddCommentReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var listFoodComments = req.listFoodComments;
            var list_foodComments = JsonConvert.DeserializeObject<List<FoodComment>>(listFoodComments);

            var orderId = Convert.ToInt32(req.orderId);

            var riderStars = Convert.ToInt32(req.riderStars);

            OrderInfo order = OrderInfoOper.Instance.GetById(orderId);
            if (!string.IsNullOrEmpty(order.riderComment))
                return ControllerHelper.Instance.JsonResult(500, "已评价过");

            order.riderStars = riderStars;
            order.riderComment = req.riderComment;
            order.status = 7;

            FoodInfoOper.Instance.UpdateFoodStars(list_foodComments);

            try
            {
                OrderInfoOper.Instance.Update(order);
                RiderInfoOper.Instance.AddCommentRecord((int)order.riderId, riderStars);
                return ControllerHelper.Instance.JsonResult(200, "评论成功");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "评论失败");
            }
        }

        /// <summary>
        /// 用户请求回收 可回收订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage RequestRecovery(UserByOrderIdReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var order = OrderInfoOper.Instance.GetById(orderId);
            if (order.status != 8)
                return ControllerHelper.Instance.JsonResult(500, "订单状态已改变,无需通知");
            var r = OrderInfoOper.Instance.UpdateOrderStatus(orderId, 9);

            if (r)
            {
                UPushHelper.Instance.PushRiderToRecovery(order);
                return ControllerHelper.Instance.JsonResult(200, "已发送回收通知");
            }
            else
            {
                return ControllerHelper.Instance.JsonResult(500, "服务器错误");
            }
        }

        #endregion

        #region 骑手端订单接口

        /// <summary>
        /// 骑手获取某个订单的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage RiderGetOrderByOrderId(RiderByOrderIdReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var ovList = OrderInfoOper.Instance.GetViewByOrderId(orderId);
            OrderDetailForRiderRes res = new OrderDetailForRiderRes(ovList);
            if (ovList.First().isActual == true)
            {
                var r = RiderStockOper.Instance.IsEnough(riderId, orderId);
                res.isEnough = r;
            }
            else
                res.isEnough = true;
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 骑手获取实时的待接单的订单,非回收
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetActualWaitOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " isactual=1 and status = 1 and deposit=0 ";

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无可接订单");

            var orderIds = list.Select(p => p.orderId).Distinct().ToArray();
            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var listRE = RiderStockOper.Instance.IsEnough(riderId, orderIds);
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                if (temp.Count < 1)
                    continue;
                RiderOrderRes res = new RiderOrderRes(temp, 1);
                var temp2 = listRE.Where(p => p.orderId == item && !p.isEnough);
                if (temp2.Count() > 0)
                    res.isEnough = false;
                else
                    res.isEnough = true;
                listR.Add(res);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 骑手获取可接的可回收订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetCycleWaitOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " isactual=1 and status = 1 and deposit>0 ";

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无可回收订单");

            //var idsStr = string.Join(",", ids);
            ////查具体信息


            var orderIds = list.Select(p => p.orderId).Distinct().ToArray();
            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var listRE = RiderStockOper.Instance.IsEnough(riderId, orderIds);
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                if (temp.Count < 1)
                    continue;
                RiderOrderRes res = new RiderOrderRes(temp, 1);
                var temp2 = listRE.Where(p => p.orderId == item && !p.isEnough);
                if (temp2.Count() > 0)
                    res.isEnough = false;
                else
                    res.isEnough = true;
                listR.Add(res);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 骑手接单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage TakeOrder(RiderByOrderIdReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            //CacheHelper.LockCache("OrderInfo_" + orderId);//缓存锁后期再说吧-txy其他的还没弄

            var order = OrderInfoOper.Instance.GetById(orderId);
            order.riderId = riderId;
            if (order.status != 1)
            {
                //CacheHelper.ReleaseLock("OrderInfo_" + orderId);//我现在感觉是要的，但是开始写的时候，觉得不需要这行
                return ControllerHelper.Instance.JsonResult(500, "订单状态已改变，不可接单");
            }

            var ofs = OrderFoodsOper.Instance.GetByOrderId(order.id);
            var listFoods = new List<foodId_Amount>();
            foreach (var item in ofs)
            {
                foodId_Amount fa = new foodId_Amount(item);
                listFoods.Add(fa);
            }
            if (!RiderStockOper.Instance.RemoveRs(riderId, listFoods))
            {
                //CacheHelper.ReleaseLock("OrderInfo_" + orderId);
                return ControllerHelper.Instance.JsonResult(500, "库存不够，不可接单");
            }

            UPushHelper.Instance.PushUserSendingOrder(order);

            order.riderTakeTime = DateTime.Now;
            order.status = 2;
            try
            {
                OrderInfoOper.Instance.Update(order);
                //CacheHelper.ReleaseLock("OrderInfo_" + orderId);
                return ControllerHelper.Instance.JsonResult(200, "接单成功");
            }
            catch (Exception e)
            {
                //CacheHelper.ReleaseLock("OrderInfo_" + orderId);
                return ControllerHelper.Instance.JsonResult(500, "接单失败");
            }
        }

        /// <summary>
        /// 获取骑手正在配送中的订单 实时的
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetRiderSendingOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " isactual=1 and  riderId=" + riderId + " and status in (2,8,9) ";

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无进行中订单");

            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var orderIds = list.Select(p => p.orderId).Distinct();
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                RiderOrderRes res = new RiderOrderRes(temp);
                listR.Add(res);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 骑手获取已完成的订单status=3,7,非回收订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetCompletedOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");


            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " riderId=" + riderId + " and status in (3,7) and deposit=0";

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无已完成订单");

            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var orderIds = list.Select(p => p.orderId).Distinct();
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                RiderOrderRes res = new RiderOrderRes(temp, 1);
                listR.Add(res);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 骑手获取已完成的订单status=3,7,回收订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetCompletedCycleOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " riderId=" + riderId + " and status in (3,7) and deposit>0 ";

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无已完成订单");


            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var orderIds = list.Select(p => p.orderId).Distinct();
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                RiderOrderRes res = new RiderOrderRes(temp, 1);
                listR.Add(res);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 骑手确认送达订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage ArriveOrder(RiderByOrderIdReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var order = OrderInfoOper.Instance.GetById(orderId);
            if (order.status != 2)
                return ControllerHelper.Instance.JsonResult(500, "订单状态已改变，无法送达");
            var r = true;
            //如果是自提订单，无回收流程
            if ((bool)order.isTake)
            {
                r = OrderInfoOper.Instance.UpdateOrderStatus(orderId, 3);
            }
            else
            {
                if (order.deposit > 0)//如果有押金就是可回收单
                    r = OrderInfoOper.Instance.UpdateOrderStatus(orderId, 8);
                else
                    r = OrderInfoOper.Instance.UpdateOrderStatus(orderId, 3);
            }
            if (r)
            {
                if (!(bool)order.isTake)
                {
                    UPushHelper.Instance.PushUserOrderArrive(order);
                    RiderInfoOper.Instance.AddRiderSendCount(riderId);
                }
                return ControllerHelper.Instance.JsonResult(200, "确认成功");
            }
            else
            {
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定");
            }
        }

        /// <summary>
        /// 骑手申请取消
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage RiderCancelOrder(RiderByOrderIdReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            OrderInfoOper.Instance.UpdateOrderStatus(orderId, 99);
            return ControllerHelper.Instance.JsonResult(200, "后台正在审核");
        }

        /// <summary>
        /// 骑手  我的订单-预定订单 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetRiderReserveOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " isactual=0 and  riderId=" + riderId + " and status in (2,8,9) ";
            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无预定订单");

            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var orderIds = list.Select(p => p.orderId).Distinct();
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                RiderOrderRes res = new RiderOrderRes(temp);
                res.isEnough = true;//-txy 
                listR.Add(res);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 餐盒确认
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage Recovery(RiderByOrderIdReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            var order = OrderInfoOper.Instance.GetById(orderId);

            if (order.status != 9)
                return ControllerHelper.Instance.JsonResult(500, "不能重复确认");

            var r = OrderInfoOper.Instance.UpdateOrderStatus(orderId, 3);

            if (r)
            {
                if (order.deposit > 0)
                {
                    UserPayOper.Instance.DepositBackRecord(order);//押金退回用户余额，并添加记录
                }
                UPushHelper.Instance.PushUserRecovery(order);//告诉用户已经收到餐盒
                RiderInfoOper.Instance.AddRiderSendCount(riderId);//增加骑手配送次数
                return ControllerHelper.Instance.JsonResult(200, "确认成功");
            }
            else
            {
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定");
            }
        }

        #endregion

        #region 预定订单系列接口

        /// <summary>
        /// 预订单计算金额
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage PlaceReserveOrder(PlaceReserveOrderReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var isUseB = Convert.ToInt32(req.isUseBalance) == 1 ? true : false;
            var isUseC = Convert.ToInt32(req.isUseCoupon) == 1 ? true : false;
            ReserveOrderRes res = new ReserveOrderRes();
            //var addressId = 0;
            var ua = new UserAddressView();
            if (req.addressId != null)
            {
                var addressId = Convert.ToInt32(req.addressId);
                ua = UserAddressOper.Instance.GetUAViewById(addressId);
                res.userAddress = UserAddressOper.Instance.GetUaResByUAView(ua);

                //res.userAddress = UserAddressOper.Instance.GetUaResById(addressId);
            }
            else
            {
                var list_addr = UserAddressOper.Instance.GetRecentlyAddr(userId);
                if (list_addr.Count < 1)
                    res.userAddress = null;
                else
                    res.userAddress = new UserAddressForOrderRes(list_addr.First());
            }

            var listFoods = JsonConvert.DeserializeObject<List<foodId_Amount>>(req.listFoods);

            var user = UserInfoOper.Instance.GetById(userId);//获取用户信息
            var balance = (decimal)user.userBalance;
            var coupon = (decimal)user.coupon;

            var useBalance = isUseB ? balance : 0;
            var useCoupon = isUseC ? coupon : 0;

            var fd = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listFoods);
            var deposit = fd.deposit;
            deposit = ua.riderType == 2 ? 0 : deposit;
            var foodsPrice = fd.price;
            var priceRes = ControllerHelper.Instance.GetThreePriceWithDeposit(useBalance, useCoupon, foodsPrice, deposit);

            res.balance = balance.ToString();
            res.coupon = coupon.ToString();
            PriceAll pa = new PriceAll();
            pa.payMoney = priceRes.payMoney.ToString();
            pa.price = (priceRes.payMoney + priceRes.useBalance).ToString();
            res.priceAll = pa;

            res.listFoods = FoodInfoOper.Instance.GetFoodINAPI(listFoods);

            res.deposit = deposit.ToString();
            res.isUseBalance = isUseB;
            res.isUseCoupon = isUseC;
            res.remarks = req.remarks == null ? "" : req.remarks;
            res.timeArea = req.timeArea == null ? "" : req.timeArea;
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage CreateReserveOrder(CreateReserveOrderReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");
            var addressId = Convert.ToInt32(req.addressId);

            var ua = CacheHelper.GetById<UserAddressView>("UserAddressView", addressId);
            if (ua.riderType != 2)
            {
                var areas = AreaInfoOper.Instance.GetAreaByLocation(Convert.ToDecimal(ua.lat), Convert.ToDecimal(ua.lng));
                if (areas.Count < 1)
                    return ControllerHelper.Instance.JsonResult(500, "配送地址超出当前区域");
            }


            var isUseB = Convert.ToInt32(req.isUseBalance) == 1 ? true : false;
            var isUseC = Convert.ToInt32(req.isUseCoupon) == 1 ? true : false;

            List<foodId_Amount> listFoods = JsonConvert.DeserializeObject<List<foodId_Amount>>(req.listFoods);

            if (listFoods.Count < 1)
            {
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请重新下单");
            }

            var mainFood = FoodInfoOper.Instance.GetMainFood(listFoods);
            if (mainFood == null)
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请重新下单");

            //找这个菜是星期几的，算出那一天的日期
            var sTag = mainFood.secondTag == 0 ? 7 : (int)mainFood.secondTag;
            var now = Convert.ToInt32(DateTime.Now.DayOfWeek);
            var ts = sTag - now;
            ts = ts <= 0 ? ts + 7 : ts;
            var date = DateTime.Now.AddDays(ts);
            date = (DateTime)mainFood.FoodTime;

            listFoods = listFoods.Where(p => p.amount != 0).ToList();

            var fd = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listFoods);
            var deposit = fd.deposit;
            deposit = ua.riderType == 2 ? 0 : deposit;
            var foodsPrice = fd.price;

            var user = UserInfoOper.Instance.GetById(userId);//获取用户信息
            var balance = (decimal)user.userBalance;
            var coupon = (decimal)user.coupon;

            ///可用的余额
            var canUseBalance = isUseB ? balance : 0;
            var canUseCoupon = isUseC ? coupon : 0;

            var priceRes = ControllerHelper.Instance.GetThreePriceWithDeposit(canUseBalance, canUseCoupon, foodsPrice, deposit);
            if (priceRes.payMoney != Convert.ToDecimal(req.payMoney))
                return ControllerHelper.Instance.JsonResult(500, "金额有误4");

            //var addressId = req.addressId;
            //这单使用的金额和余额
            var useBalance = priceRes.useBalance;
            var useCoupon = priceRes.useCoupon;
            var remarks = req.remarks == null ? "" : req.remarks;
            var payType = req.payType;
            //第三方支付的金额
            var payMoney = Convert.ToDecimal(req.payMoney);
            var sumMoney = useBalance + payMoney;//总金额要不要，包括哪些金额

            OrderInfo order = new OrderInfo();
            order.userId = userId;
            order.addressId = addressId;
            order.useBalance = useBalance;
            order.useCoupon = useCoupon;
            order.remarks = remarks;
            order.payType = payType;
            order.payMoney = payMoney;
            order.sumMoney = sumMoney;
            order.timeArea = req.timeArea;
            order.deposit = deposit;
            order.isActual = false;

            var timeArea = req.timeArea;
            var index = timeArea.IndexOf(':');
            var hour = "";
            if (index == 1)
                hour = timeArea.Substring(0, 1);
            else
                hour = timeArea.Substring(0, 2);

            var minute = Convert.ToInt32(timeArea.Substring(index + 1, 1));
            var dateTemp = "";
            if (minute > 0)
            {
                dateTemp = $"{ date.Year}-{ date.Month}-{ date.Day}  {hour}:45";
            }
            else
            {
                dateTemp = $"{ date.Year}-{ date.Month}-{ date.Day}  {hour}:15";
            }
            order.arrivalTime = Convert.ToDateTime(dateTemp);
            //order.orderAreaId = areaId;

            if (ua.riderType == 2)
            {
                order.riderId = ua.riderId;
                order.isTake = true;
            }

            var orderId = OrderInfoOper.Instance.Add(order);

            //生成订单号
            order.id = orderId;
            var orderNo = StringHelperHere.Instance.CreateOrderNo(order.userId.ToString());
            order.orderNo = orderNo;
            order.status = 0;
            OrderInfoOper.Instance.Update(order);

            foreach (var item in listFoods)
            {
                OrderFoods of = new OrderFoods();
                of.orderId = orderId;
                of.foodId = item.foodId;
                of.amount = item.amount;
                OrderFoodsOper.Instance.Add(of);
            }

            return ControllerHelper.Instance.JsonResult(200, "orderId", orderId.ToString(), "创建预定订单成功");
        }

        /// <summary>
        /// 支付预定订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage PayReserveOrder(PayReserveOrderReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);
            OrderInfo order = OrderInfoOper.Instance.GetById(orderId);
            if (order.status != 0)
                return ControllerHelper.Instance.JsonResult(500, "状态已改变，不需要再次支付");

            var Spbill_create_ip = req.spbill_create_ip;
            var payType = req.payType;
            var payMoney = Convert.ToDecimal(req.payMoney);

            if (order.payMoney != payMoney)
                return ControllerHelper.Instance.JsonResult(500, "金额有误5");

            //order.payType = payType;

            #region 第三方支付

            if (payType == "alipay" || payType == "wxpay")
            { //第三方支付
                order.payMoney = payMoney;
                OrderInfoOper.Instance.Update(order);

                if (payType == "alipay")
                {
                    AlipayHelperHere a = new AlipayHelperHere();
                    var b = a.CreateAlipayOrder(req.payMoney, order.orderNo, "https://fan-di.com/notify/alipayNotify");

                    JObject jo = new JObject();
                    jo.Add("payType", "alipay");
                    jo.Add("payStr", b);

                    return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
                }
                else
                {//微信支付
                    WXPayHelperHere w = new WXPayHelperHere();
                    var b = w.CreateWXOrder(payMoney, order.orderNo, "https://fan-di.com/notify/wxpayNotify", Spbill_create_ip);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(b);
                    if (doc.DocumentElement["return_code"].InnerText != "SUCCESS")
                        return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请稍后");

                    JObject jo = new JObject();
                    jo.Add("payType", "wxpay");
                    foreach (XmlNode item in doc.DocumentElement.ChildNodes)
                    {
                        if (item.Name != "sign")
                            jo.Add(item.Name, item.InnerText);
                    }

                    var timestamp = StringHelperHere.Instance.GetTimeStamp();
                    jo.Add("timestamp", timestamp);
                    var dict = new Dictionary<string, string>();

                    foreach (var item in jo)
                    {
                        if (item.Key == "prepay_id")
                            dict.Add("prepayid", item.Value.ToString());
                        if (item.Key == "nonce_str")
                            dict.Add("noncestr", item.Value.ToString());
                    }

                    dict.Add("appid", "wxd8482615c33b8859");
                    dict.Add("partnerid", "1494504712");

                    dict.Add("package", "Sign=WXPay");
                    dict.Add("timestamp", timestamp);

                    string sign = StringHelperHere.Instance.GetWXSign(dict, "2BCF8DD9490E328D2FCEDE7B26643231");
                    jo.Add("sign", sign);
                    return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
                }
            }
            #endregion

            order.payType = payType;

            UserInfo user = UserInfoOper.Instance.GetById(userId);
            user.userBalance -= order.useBalance;
            user.coupon -= order.useCoupon;
            if (user.userBalance < 0 || user.coupon < 0)
                return ControllerHelper.Instance.JsonResult(500, "余额或优惠券金额不足");

            //order.sumMoney = sumMoney;
            if ((bool)order.isTake)
                order.status = 2;
            else
                order.status = 1;
            order.payTime = DateTime.Now;

            try
            {
                var sql1 = UserInfoOper.Instance.GetUpdateStr(user);
                var sql2 = OrderInfoOper.Instance.GetUpdateStr(order);
                var dict1 = UserInfoOper.Instance.GetParameters(user);
                var dict2 = OrderInfoOper.Instance.GetParameters(order);
                var dicts = new List<Dictionary<string, string>>();
                dicts.Add(dict1);
                dicts.Add(dict2);
                List<string> sqls = new List<string>();
                sqls.Add(sql1);
                sqls.Add(sql2);

                var r = SqlHelperHere.ExecuteInTransaction(sqls, "pay-userId=" + userId + "-orderId=" + orderId, dicts);

                if (r)
                {
                    OrderInfoOper.Instance.PayOrderRecord(order);
                    UPushHelper.Instance.PushAfterPayOrder(order);
                    return ControllerHelper.Instance.JsonResult(200, "payType", "", "支付成功");
                }
                else
                    return ControllerHelper.Instance.JsonResult(500, "支付失败");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "支付失败");
            }
        }

        /// <summary>
        /// 获取预定订单，只获取未支付和等待中的 isActual=0   status in（0，1,2）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetReserveOrder(UserToken req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            OrderInfoOper.Instance.RemoveOverTime(userId);
            var condition = " isactual=0 and status in (0,1,2) and  userId=" + userId;

            var list = CacheHelper.GetByCondition<OrderTryJoin>("ordertryjoin", condition);
            var ids = list.Select(p => p.orderId).Distinct().ToArray();

            List<ActiveOrderRes> listR = new List<ActiveOrderRes>();
            foreach (var item in ids)
            {
                var listTemp = list.Where(p => p.orderId == item).ToList();
                ActiveOrderRes res = new ActiveOrderRes(listTemp);
                listR.Add(res);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.orderId)), "");
        }

        /// <summary>
        /// 获取自提点进行中订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetShopSendingOrder(RiderByPageReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pageIndex = Convert.ToInt32(req.pageIndex);

            var condition = " riderId=" + riderId + " and status=2 ";

            //var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", condition, pageIndex, size, new Dictionary<string, string>());

            var list = SqlHelperHere.GetViewPaging<OrderTryJoin>("ordertryjoin", "select * from ordertryjoin ", condition, pageIndex, size, "order by  arrivaltime", new Dictionary<string, string>());

            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无进行中订单");

            List<RiderOrderRes> listR = new List<RiderOrderRes>();
            var orderIds = list.Select(p => p.orderId).Distinct();
            foreach (var item in orderIds)
            {
                var temp = list.Where(p => p.orderId == item).ToList();
                RiderOrderRes res = new RiderOrderRes(temp);
                listR.Add(res);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }


        #endregion

        //分割线，以上已写接口文档
        /*———————————————————————————————————————————*/

            [HttpPost]
        public ResultJson<payRes> PayOrder3()
        {
            var notifyUrl = "http://101.132.66.37:8039/notify/alipayNotify";
            var  payId = DateTime.Now.ToString("yyyyMMddHHmmss");
            AlipayHelperHere a = new AlipayHelperHere();
                    var b = a.CreateAlipayOrder("111", payId, "http://101.132.66.37:8039/notify/alipayNotify");
            var r = new ResultJson<payRes>();
            r.HttpCode = 200;
            r.ListData.Add(new payRes { payStr = b, payType = "alipay" });
            return r;
            //return JsonConvert.SerializeObject(r);
        }

        public string test() {
            return "111";
        }

    }
}
