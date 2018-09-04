using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using takeAwayWebApi.Attribute;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers
{
    public class HomePageController : BaseController
    {
        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();
        int size = 10;

        /// <summary>
        /// 获取用户所属区域
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetArea(GetAreaReq req)
        {
            var lat = Convert.ToDecimal(req.lat);
            var lng = Convert.ToDecimal(req.lng);

            var listR = AreaInfoOper.Instance.GetAreaByLocation(lat, lng);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 获取区域内菜品种类，暂时不考虑区域-txy完成（2018年1月4日 17:30:07已经考虑了）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetTags(GetTagsReq req)
        {
            var areaId = Convert.ToInt32(req.areaId);
            var list = RiderStockOper.Instance.GetTagInArea(areaId);

            if (list.Count > 0)
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(list), "");
            else
                return ControllerHelper.Instance.JsonEmptyArr(200, "该区域没有菜品");
        }

        /// <summary>
        /// 获得区域内的库存。在区域中的骑手身上的某标签的菜品总和。其中用户点过的菜数量要附上去
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetFoods(GetFoodsReq req)
        {
            var tagId = Convert.ToInt32(req.tagId);
            var userId = Convert.ToInt32(req.userId);
            var areaId = Convert.ToInt32(req.areaId);

            List<RiderStockJoin> listRs = RiderStockOper.Instance.GetMainRSByAreaId(areaId);
            if (listRs.Count == 0)
            {
                return ControllerHelper.Instance.JsonEmptyArr(200, "该区域无菜品");
            }
            var foodIdsInArea = listRs.Select(p => (int)p.foodId).Distinct().ToArray();

            var listTempFoods = new List<foodId_Amount>();//用户点过的菜
            if (userId != 0)
            {
                var listafa = CacheHelper.GetTempFoods2(userId);
                listafa = listafa.Where(p => p.areaId == areaId).ToList();
                if (listafa.Count > 0)
                {
                    foreach (var item in listafa)
                    {
                        foodId_Amount fa = new foodId_Amount();
                        fa.amount = item.amount;
                        fa.foodId = item.foodId;
                        listTempFoods.Add(fa);
                    }
                }
            }
            //如果点过的菜不在区域中，删掉。数量为0的也删掉
            listTempFoods = listTempFoods.Where(p => foodIdsInArea.Contains(p.foodId) && p.amount > 0).ToList();

            //点过的菜的总价
            var money = new Decimal();
            if (listTempFoods.Count > 0)
            {
                money = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listTempFoods).price;
            }

            var foodIdsStr = string.Join(",", foodIdsInArea);

            var condition = " id in (" + foodIdsStr + ")";
            if (tagId != 0)
                condition += " and foodTagId=@tagId";
            var dict = new Dictionary<string, string>();
            dict.Add("@tagId", tagId.ToString());
            var listFoodView = CacheHelper.GetByCondition<FoodView>("foodview", condition, dict);

            if (listFoodView.Count > 0)
            {
                List<FoodsOnMapRes> listR = new List<FoodsOnMapRes>();

                if (listTempFoods.Count > 0)//如果缓存里有点过的菜 去库存里找
                {
                    foreach (var item in listFoodView)
                    {
                        var listHere = listTempFoods.Where(p => p.foodId == item.id).FirstOrDefault();//如果库存里有 缓存中的菜 就改上去
                        if (listHere == null)
                            listR.Add(new FoodsOnMapRes(item, apiHost, 0));
                        else
                            listR.Add(new FoodsOnMapRes(item, apiHost, listHere.amount));
                    }
                }
                else
                {
                    foreach (var item in listFoodView)
                    {
                        listR.Add(new FoodsOnMapRes(item, apiHost));
                    }
                }

                List_FoodsOnMapRes res = new List_FoodsOnMapRes();
                res.foods = listR;
                res.money = money.ToString();

                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
            }
            else
            {
                return ControllerHelper.Instance.JsonEmptyArr(200, "该区域无菜品");
            }
        }

        /// <summary>
        /// 获取用户正在配送中的最新的一个订单 实时的
        /// </summary>
        /// status 2派送中  1待抢单
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateModel]-txy
        public HttpResponseMessage GetSendingOrder(GetSendingOrderReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);
            var lat = req.lat;
            var lng = req.lng;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = OrderInfoOper.Instance.GetSendingOrderIdsByUserId(userId);
            if (orderId == 0)
                return ControllerHelper.Instance.JsonEmptyStr(200, "无配送中订单");
            var list2 = OrderInfoOper.Instance.GetViewByOrderId(orderId);

            OrderSendingRes res = new OrderSendingRes(list2, lat, lng);
            var riderId = list2.First().riderId;
            if (riderId != null)
            {
                var rider = RiderInfoOper.Instance.GetById((int)riderId);
                if (rider.riderType == 2)//如果是自提点，经纬度从数据库里拿
                {
                    var rp = new RiderPosition(rider);
                    res.riderInfo = rp;
                }
                else
                    res.riderInfo = CacheHelper.GetRiderPosition((int)riderId, (int)rider.riderType);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 获取周围的骑手,还要以用户为原点，半径5千米以内的骑手（暂定5千米）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetRiders(GetRidersReq req)
        {
            var lat = Convert.ToDecimal(req.lat);
            var lng = Convert.ToDecimal(req.lng);
            var areaId = Convert.ToInt32(req.areaId);

            var foodId = Convert.ToInt32(req.foodId);
            var riderIds = RiderStockOper.Instance.GetRiderIdsByFoodId_Position(foodId, lat, lng, areaId);
            if (riderIds.Count() < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "附近无骑手");

            List<RiderPosition> list = new List<RiderPosition>();
            foreach (var item in riderIds)
            {
                RiderPosition rp = CacheHelper.GetRiderPosition(item);
                list.Add(rp);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(list), "");
        }

        /// <summary>
        /// 首页点过的菜，未下单前保存
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage SetTempFoods(SetTempFoodsReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var areaId = Convert.ToInt32(req.areaId);

            var foodIds = new List<int>();
            if (areaId != 0)
            {
                var listRs = RiderStockOper.Instance.GetMainRSByAreaId(areaId);

                foodIds = listRs.Select(p => (int)p.foodId).Distinct().ToList();
            }

            var foodId = Convert.ToInt32(req.foodId);
            var math = Convert.ToInt32(req.math);

            var listafa = new List<AreaId_foodId_amount>();
            listafa = CacheHelper.GetTempFoods2(userId);

            var list = listafa.Where(p => p.areaId == areaId && p.foodId == foodId).ToList();

            //var list = listFoods.Where(p => p.foodId == foodId).ToList();
            //新点的没有 add
            if (list.Count < 1)
            {
                if (math > 0)
                {
                    var afa = new AreaId_foodId_amount();
                    afa.areaId = areaId;
                    afa.foodId = foodId;
                    afa.amount = 1;
                    listafa.Add(afa);
                }
                CacheHelper.SetTempFoods2(userId, listafa);
                var money = FoodInfoOper.Instance.GetFoodPriceAndDepositByAFA(listafa, areaId).price;

                return ControllerHelper.Instance.JsonResult(200, "money", money.ToString(), "更新成功");
            }
            //缓存里有菜品 点的也有 update
            else
            {
                var fa = list.First();
                fa.amount += math;
                if (fa.amount < 1)
                {
                    var i = listafa.IndexOf(listafa.Where(p => p.areaId == areaId && p.foodId == foodId).First());
                    listafa.RemoveAt(i);
                }
                CacheHelper.SetTempFoods2(userId, listafa);
                var money = FoodInfoOper.Instance.GetFoodPriceAndDepositByAFA(listafa, areaId).price;

                return ControllerHelper.Instance.JsonResult(200, "money", money.ToString(), "更新成功");
            }
        }

        /// <summary>
        /// 首页到支付页，获取用户在本区域内点过的菜，返回相应的信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage ToPayPage(ToPayPageReq req)
        {
            OrderRes res = new OrderRes();
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var areaId = Convert.ToInt32(req.areaId);
            List<RiderStockJoin> listRs = RiderStockOper.Instance.GetRSJoinByAreaId(areaId);

            var foodIdsInArea = listRs.Select(p => p.foodId).Distinct().ToArray();//区域内存在的菜品的id
            var list_foods = new List<foodId_Amount>();
            var listafa = CacheHelper.GetTempFoods2(userId);
            listafa = listafa.Where(p => p.areaId == areaId).ToList();//-txy
            foreach (var item in listafa)
            {
                foodId_Amount fa = new foodId_Amount();
                fa.amount = item.amount;
                fa.foodId = item.foodId;
                list_foods.Add(fa);
            }

            #region 根据库存调整菜品数量
            //点过的菜，不属于区域内的去掉
            list_foods = list_foods.Where(p => foodIdsInArea.Contains(p.foodId)).ToList();

            var listTempFoods = new List<foodId_Amount>();
            foreach (var item in list_foods)
            {
                var rsAmount = listRs.Where(p => p.foodId == item.foodId).ToList().Sum(p => p.amount);
                item.amount = item.amount > rsAmount ? (int)rsAmount : item.amount;
                listTempFoods.Add(item);
            }
            #endregion

            var list_addr = UserAddressOper.Instance.GetRecentlyAddr(userId);
            if (list_addr.Count < 1)
                res.userAddress = null;
            else
                res.userAddress = new UserAddressForOrderRes(list_addr.First());

            var user = UserInfoOper.Instance.GetById(userId);//获取用户信息
            var balance = (decimal)user.userBalance;
            var coupon = (decimal)user.coupon;

            var ids = listTempFoods.Select(p => p.foodId).ToArray();

            var tempList = CacheHelper.GetByIds<FoodView>("foodView", ids);

            var f_d = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listTempFoods);
            var deposit = f_d.deposit;
            var foodsPrice = f_d.price;
            var priceRes = ControllerHelper.Instance.GetThreePriceWithDeposit(balance, coupon, foodsPrice, deposit);

            res.balance = balance.ToString();
            res.coupon = coupon.ToString();
            PriceAll pa = new PriceAll();
            pa.payMoney = priceRes.payMoney.ToString();
            pa.price = (priceRes.payMoney + priceRes.useBalance).ToString();
            res.priceAll = pa;

            List<foodId_name_amount_price_isMain> foodListRes = new List<foodId_name_amount_price_isMain>();
            foreach (var item in tempList)
            {
                foodId_name_amount_price_isMain a = new foodId_name_amount_price_isMain(item);
                foodListRes.Add(a);
            }

            foreach (var item in listTempFoods)
            {
                foodListRes.Where(p => p.foodId == item.foodId).FirstOrDefault().amount = item.amount;
            }

            var list = RiderStockOper.Instance.GetStockNotMain(areaId);

            var extraFoodIdList = list.OrderByDescending(p => p.id).GroupBy(p => new { p.foodId, p.foodName, p.foodPrice }).Select(g => new foodId_name_amount_price_isMain
            {
                foodId = (int)g.Key.foodId,
                foodName = g.Key.foodName,
                foodPrice = g.Key.foodPrice.ToString(),
                amount = 0,
                isMain = false
            }).ToList();
            foreach (var item in extraFoodIdList)
            {
                foodListRes.Add(item);
            }

            res.listFoods = foodListRes.OrderByDescending(p => p.foodId).ToList();

            res.isUseBalance = balance <= 0 ? false : true;
            res.isUseCoupon = coupon <= 0 ? false : true;
            res.deposit = deposit.ToString();
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 跳转到评价页面
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage ToCommentPage(UserByOrderIdReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var orderId = Convert.ToInt32(req.orderId);

            var list = OrderInfoOper.Instance.GetViewByOrderId(orderId);
            list = list.Where(p => p.isMain == true).ToList();
            CommentRes res = new CommentRes(list, apiHost);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 获取预定菜品的标签
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetReserveTags(HomePageReq req)
        {
            var now = DateTime.Now;
            //if (now.Hour >= 17)
            //    return ControllerHelper.Instance.JsonEmptyArr(500, "17点之后无法预订");
            var weekday = (int)now.DayOfWeek;
            var day = weekday + 1;
            if (weekday == 6)
                day = 0;

            FoodInfoOper.Instance.ClearWeekFood();
            var list = new List<FoodView>();

            var nowInt = Convert.ToInt32(now.DayOfWeek);
            //本周
            if (nowInt > 0)
            {
                var dt = now.AddDays(8 - nowInt).Date;
                //var dt2 = dt.AddDays(-7).Date;
                list = CacheHelper.GetByCondition<FoodView>("foodview", " isdeleted=0 and ison=1 and  isweek=1  and isMain=1 and foodTime<'" + dt + "' and foodTime>'" + now.Date + "'");
            }
            //下一周
            else
            {
                var dt = now.AddDays(1).Date;
                list = CacheHelper.GetByCondition<FoodView>("foodview", " isdeleted=0 and ison=1 and  isweek=1  and isMain=1 and foodTime>'" + dt + "'");
            }


            //list = CacheHelper.GetByCondition<FoodView>("foodview", " isdeleted=0 and ison=1 and  isweek=1  and isMain=1 and secondTag=" + day);
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "无预定菜品");

            var dict = list.Select(p => new { p.foodTagId, p.foodTagName }).Distinct().ToDictionary(p => (int)p.foodTagId, p => (string)p.foodTagName);
            List<TagRes> listR = new List<TagRes>();

            TagRes tr2 = new TagRes(0, "普通商品");
            listR.Add(tr2);

            foreach (var item in dict)
            {
                TagRes tr = new TagRes(item.Key, item.Value);
                listR.Add(tr);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 根据标签 获取预定菜品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetReserveFoods(GetReserveFoodsReq req)
        {
            var tagId = req.tagId;
            var pageIndex = Convert.ToInt32(req.pageIndex);

            var weekday = (int)DateTime.Now.DayOfWeek;
            var day = weekday + 1;
            if (weekday == 6)
                day = 0;

            var condition = "";

            var now = DateTime.Now;
            //now = Convert.ToDateTime("2018-03-18");
            var nowInt = Convert.ToInt32(now.DayOfWeek);
            //本周
            if (nowInt > 0)
            {
                var dt = now.AddDays(8 - nowInt).Date;
                //var dt2 = dt.AddDays(-7).Date;
                condition = $"  isdeleted=0 and ison=1 and isweek=1 and isMain = 1 and foodtime<'{dt}' and foodtime>'{now.Date}'";
            }
            //下一周
            else
            {
                var dt = now.AddDays(1).Date;
                condition = "  isdeleted=0 and ison=1 and isweek=1 and isMain = 1 and foodtime>'" + dt + "'";
            }


            //condition = "  isdeleted=0 and ison=1 and isweek=1 and isMain = 1 and secondTag=" + day;
            if (!string.IsNullOrEmpty(tagId) && tagId != "0")
                condition += " and foodtagid =" + tagId;

            var list = CacheHelper.GetByConditionPaging<FoodView>("FoodView", condition, pageIndex, size);

            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "无预定菜品");

            var listR = new List<ReserveFoodRes>();
            foreach (var item in list)
            {
                ReserveFoodRes rf = new ReserveFoodRes(item, apiHost);
                listR.Add(rf);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 进入预订单的支付页面
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage ToReservePayPage(ToReservePayPage req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            ReserveOrderRes res = new ReserveOrderRes();

            var foodId = Convert.ToInt32(req.foodId);
            var amount = Convert.ToInt32(req.amount);

            //获取最近使用过的地址
            var list_addr = UserAddressOper.Instance.GetRecentlyAddr(userId);
            if (list_addr.Count < 1)
                res.userAddress = null;
            else
                res.userAddress = new UserAddressForOrderRes(list_addr.First());

            var user = UserInfoOper.Instance.GetById(userId);//获取用户信息
            var balance = (decimal)user.userBalance;
            var coupon = (decimal)user.coupon;

            var listFoods = new List<foodId_Amount>();
            foodId_Amount fa1 = new foodId_Amount(foodId, amount);
            listFoods.Add(fa1);

            var listWeekNotMainFoods = FoodInfoOper.Instance.GetReserveNotMainFood();
            if (listWeekNotMainFoods.Count > 0)
            {
                foreach (var item in listWeekNotMainFoods)
                {
                    foodId_Amount fa = new foodId_Amount(item.id, 0);
                    listFoods.Add(fa);
                }
            }
            var fd = FoodInfoOper.Instance.GetFoodPriceAndDeposit(listFoods);
            var deposit = fd.deposit;
            var foodsPrice = fd.price;

            //var sumMoney = foodsPrice + deposit;
            var priceRes = ControllerHelper.Instance.GetThreePriceWithDeposit(balance, coupon, foodsPrice, deposit);

            res.balance = balance.ToString();
            res.coupon = coupon.ToString();
            PriceAll pa = new PriceAll();
            pa.payMoney = priceRes.payMoney.ToString();
            pa.price = (priceRes.payMoney + priceRes.useBalance).ToString();
            res.priceAll = pa;

            res.listFoods = FoodInfoOper.Instance.GetFoodINAPI(listFoods);

            res.deposit = deposit.ToString();

            res.isUseBalance = balance <= 0 ? false : true;
            res.isUseCoupon = coupon <= 0 ? false : true;

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 获取首页的公告
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetNotice(OrderReq req)
        {
            var list = CacheHelper.GetByCondition<ArticleInfo>("ArticleInfo", " isdeleted = 0 and isarticle=0 and status=1 ");
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无公告");
            //return ControllerHelper.Instance.JsonResult(500, "暂无公告");

            var listR = new List<ArticleName_url>();
            foreach (var item in list.OrderByDescending(p => p.id))
            {
                ArticleName_url temp = new ArticleName_url(item, apiHost);
                listR.Add(temp);
            }

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetAricleList(GetArticleListReq req)
        {
            var pageIndex = Convert.ToInt32(req.pageIndex);
            //var list = (List<ArticleInfo>)CacheHelper.GetByCondition<ArticleInfo>("articleInfo", " isdeleted = 0 and isarticle=1 ");

            var list = CacheHelper.GetByConditionPaging<ArticleInfo>("ArticleInfo", " isdeleted = 0 and isarticle=1 and status=1 ", pageIndex, size);
            if (list.Count < 0)
                return ControllerHelper.Instance.JsonEmptyArr(200, "没文章了");

            var listR = new List<AricleName_Pic_content_url>();
            foreach (var item in list.OrderByDescending(p => p.id).ToList())
            {
                AricleName_Pic_content_url a = new AricleName_Pic_content_url(item, apiHost);
                listR.Add(a);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 获取区域内的自提点列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetShopList(GetShopList req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");
            var areaId = Convert.ToInt32(req.areaId);

            var list = CacheHelper.GetByCondition<UserAddressView>("UserAddressView", " riderStatus=1 and riderType=2 and  isdeleted=0 and riderAreaId=" + areaId);
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "暂无自提点");

            var listR = new List<riderType2>();
            foreach (var item in list)
            {
                riderType2 r = new riderType2(item);
                listR.Add(r);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 获取区域内的自提点列表,预定订单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage GetShopListForReserve(GetShopListReq2 req)
        {
            var userId = Convert.ToInt32(req.userId);
            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");
            //var areaId = Convert.ToInt32(req.areaId);
            var lat = Convert.ToDecimal(req.lat);
            var lng = Convert.ToDecimal(req.lng);
            var listArea = AreaInfoOper.Instance.GetAreaByLocation(lat, lng);
            if (listArea.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "暂无自提点");
            var areaIds = StringHelperHere.Instance.ArrJoin(listArea.Select(p => p.areaId).ToArray());

            var list = CacheHelper.GetByCondition<UserAddressView>("UserAddressView", " riderStatus=1 and riderType=2 and  isdeleted=0 and riderAreaId in(" + areaIds + ")");
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "暂无自提点");

            var listR = new List<riderType2>();
            foreach (var item in list)
            {
                riderType2 r = new riderType2(item);
                listR.Add(r);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        //分割线，以上已写接口文档
        /*————————————————————————————————————————————————*/

        /// <summary>
        /// （好像不用了）获取正在配送订单的骑手信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetSendingRider(HomeOrderReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var riderId = Convert.ToInt32(req.riderId);

            var r = CacheHelper.GetRiderPosition(riderId);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(r), "");
        }

        //[HttpPost]
        public void test()
        {
            //string str = "eyJ0eXAiOiJKV1QiLCJhbGciOiJNRDUifQ==.eyJleHAiOiIyMDE3LzEyLzI1IDk6NDI6MzciLCJVc2VySUQiOjM1LCJVc2VyTmFtZSI6ImZld2d0ZXczMTIzMSJ9.E1137F71E87D34DAF944BF430EEF8981";
            //string str2 = "eyJ0eXAiOiJKV1QiLCJhbGciOiJNRDUifQ==.eyJleHAiOiIyMDE3LzEyLzI2IDEwOjE3OjEwIiwiVXNlcklEIjozNSwiVXNlck5hbWUiOiJmZXdndGV3MzEyMzEifQ==.0C1E1820399EF862454310DE4EB5D1BF";
            //string str3 = "eyJ0eXAiOiJKV1QiLCJhbGciOiJNRDUifQ==.eyJleHAiOiIyMDE3LzEyLzI2IDEwOjE0OjM5IiwiVXNlcklEIjozNSwiVXNlck5hbWUiOiJmZXdndGV3MzEyMzEifQ==.FB0448176478D94B019AE1115C50FCDC";
            //Token token = new Token(str3);

        }
    }
}
