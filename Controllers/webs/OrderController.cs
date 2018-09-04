using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAway.Controllers
{
    public class OrderController : WebBaseController
    {
        //
        // GET: /Order/

        //OrderInfoOper orderOper = new OrderInfoOper();
        //UserInfoOper userOper = new UserInfoOper();
        //AreaInfoOper areaOper = new AreaInfoOper();
        //RiderInfoOper riderOper = new RiderInfoOper();
        //FoodInfoOper foodOper = new FoodInfoOper();
        //OrderFoodsOper ofOper = new OrderFoodsOper();
        //UserAddressOper uaOper = new UserAddressOper();
        //UserPayOper upOper = new UserPayOper();

        public ActionResult Index()
        {
            if (Session["pfId"] == null)
                return View("_Login");
            return View();
        }

        public ActionResult Detail(webReq req)
        {
            if (Session["pfId"] == null)
                return View("_Login");
            var id = req.id;
            ViewBag.isAdd = false;

            ViewBag.statusDict = OrderInfoOper.Instance.GetStatusDict();
            ViewBag.areaDict = AreaInfoOper.Instance.GetAreaDict();
            ViewBag.riderDict = RiderInfoOper.Instance.GetRiderDict();
            ViewBag.foodDict = FoodInfoOper.Instance.GetFoodDict();
            ViewBag.userDict = UserInfoOper.Instance.GetUserDict();

            if (id != 0)
            {
                var list = CacheHelper.GetByCondition<OrderTryJoin>("ordertryjoin", " id=" + id);
                if (list.Count < 0)
                    ViewBag.isRight = false;
                else
                {
                    List<foodId_name_amount_price_isMain> listFoods = new List<foodId_name_amount_price_isMain>();
                    foreach (var item in list)
                    {
                        foodId_name_amount_price_isMain temp = new foodId_name_amount_price_isMain(item);
                        listFoods.Add(temp);
                    }
                    ViewBag.listFoods = listFoods;

                    ViewBag.isRight = true;
                    var order = list.First();
                    ViewBag.order = order;
                    ViewBag.orderId = order.id;

                    ViewBag.userName = order.userName;
                    ViewBag.userId = order.userId;
                    ViewBag.createTime = order.createTime;
                    ViewBag.status = order.status;
                    ViewBag.isActual = order.isActual;
                    ViewBag.remarks = order.remarks;

                    ViewBag.riderName = order.riderName;
                    ViewBag.riderPhone = order.riderPhone;
                    ViewBag.riderId = order.riderId;
                    ViewBag.areaId = order.areaId;
                    ViewBag.areaName = order.areaName;
                    ViewBag.userAddress = order.mapAddress + order.addrDetail;
                    ViewBag.contactName = order.contactName;
                    ViewBag.contactPhone = order.contactPhone;

                    ViewBag.arrivalTime = Convert.ToDateTime(order.arrivalTime).ToString("yyyy-MM-dd") + order.timeArea;
                    ViewBag.endTime = order.endTime;
                    ViewBag.comment = order.riderComment;

                    ViewBag.payType = order.payType;
                    ViewBag.payMoney = order.payMoney;
                    ViewBag.payTime = order.payTime;
                    ViewBag.useBalance = order.useBalance;
                    ViewBag.useCoupon = order.useCoupon;
                    ViewBag.deposit = order.deposit;
                }
            }
            else
            {
                ViewBag.isAdd = true;
                OrderInfo order = new OrderInfo();
                ViewBag.isRight = true;
            }
            return View();

        }

        /// <summary>
        /// 根据条件在OrderTryJoin里找到所有的，并distinct出当前页的那些orderid，再去ordertryjoin里找
        /// </summary>
        /// <returns></returns>
        public string Get(webReq req)
        {
            string search = req.search == null ? "" : req.search;//搜索内容
            int index = Convert.ToInt32(req.index);//页码
            var status = Convert.ToInt32(req.status);
            //var condition = "";
            int pages = 0;//总页数
            int size = 12;//一页size条数据
            var isActual = Convert.ToInt32(req.isActual);
            var condition = " 1=1 and ";
            if (isActual == 1)
                condition += "  isactual=1 and";
            else if (isActual == 0)
                condition += "  isactual=0 and";

            #region 全部或者未支付
            if (status == 100 || status == 0)//未支付和全部
            {

                OrderInfoOper.Instance.RemoveOverTime();

                var listR = new List<OrderNotPayRes>();


                if (status == 0)
                    condition += "  status = 0 and ";

                condition += "  (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' )";

                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);

                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");

                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderNotPayRes res = new OrderNotPayRes(listTemp);
                    listR.Add(res);
                }


                var count = listAll.Count;
                pages = count / size;
                pages = pages * size == count ? pages : pages + 1;//总页数
                //listR = listR.Skip((index - 1) * size).Take(size).ToList();

                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }
            #endregion
            #region 待接单
            else if (status == 1)
            {//待接单
                var listR = new List<OrderWaitRes>();
                //查询条件
                condition += "  status = 1 and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' )";


                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderWaitRes res = new OrderWaitRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }
            #endregion
            #region 配送中
            else if (status == 2)
            {//配送中
                var listR = new List<OrderSendingWebRes>();
                //查询条件
                condition += " status = 2 and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' or riderName like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderSendingWebRes res = new OrderSendingWebRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                
            }
            #endregion
            #region 已送达
            else if (status == 3)
            {
                var listR = new List<OrderArrivalRes>();
                //查询条件
                condition += "  status in (3) and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' or riderName like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderArrivalRes res = new OrderArrivalRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }
            #endregion
            #region 已评论
            else if (status == 7)
            {//

                var listR = new List<OrderArrivalRes>();
                //查询条件
                condition += "  status in (7) and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' or riderName like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderArrivalRes res = new OrderArrivalRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }

            #endregion
            #region 取消订单
            else if (status == 5)
            {//用户取消

                var listR = new List<OrderCancelledRes>();
                //查询条件
                condition += "  status =5 and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%'  or contactPhone like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderCancelledRes res = new OrderCancelledRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }
            #endregion
            #region 骑手申请取消
            else if (status == 99)
            {
                var listR = new List<OrderSendingWebRes>();
                //查询条件
                condition += "  status =99 and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%'  or contactPhone like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderSendingWebRes res = new OrderSendingWebRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }
            #endregion
            #region 待回收
            else if (status == 8)
            {//已送达和已评论 3和7都要

                var listR = new List<OrderArrivalRes>();
                //查询条件
                condition += "  status in (8) and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' or riderName like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderArrivalRes res = new OrderArrivalRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);


            }

            #endregion
            #region 回收中
            else if (status == 9)
            {//已送达和已评论 3和7都要

                var listR = new List<OrderArrivalRes>();
                //查询条件
                condition += "  status in (9) and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' or riderName like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                    return JsonHelperHere.EmptyJson();
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);
                //查具体信息
                var viewList = CacheHelper.GetByCondition<OrderTryJoin>("OrderTryJoin", " id in (" + idsStr + ")");
                foreach (var item in ids)
                {
                    var listTemp = viewList.Where(p => p.orderId == item).ToList();
                    OrderArrivalRes res = new OrderArrivalRes(listTemp);
                    listR.Add(res);
                }
                //总共多少条记录
                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
            }

            #endregion
            return null;
        }

        public void DeleteOrder(webReq req)
        {
            var orderId = req.orderId;
            OrderInfoOper.Instance.Delete(orderId);
        }

        public void Update(webReq req)
        {
            var createTime = Request["createTime"];
            var status = req.status;
            var isActual = Convert.ToBoolean(Request["isActual"]);
            var remarks = Request["remarks"];
            var riderId = req.riderId;
            var orderAreaId = req.orderAreaId;
            var endTime = Request["endTime"];
            var riderComment = Request["riderComment"];
            var useBalance = req.useBalance;
            var useCoupon = req.useCoupon;
            var deposit = req.deposit;
            var payTime = Request["payTime"];
            var payType = req.payType;
            var payMoney = req.payMoney;
            var foodIds = Request.Form.GetValues("foodIds[]");
            var amounts = Request.Form.GetValues("amounts[]");
            var orderId = req.orderId;
            OrderInfo order = new OrderInfo();
            order.createTime = Convert.ToDateTime(createTime);
            order.status = Convert.ToInt32(status);
            order.isActual = isActual;
            order.remarks = remarks;
            order.riderId = riderId;
            order.orderAreaId = orderAreaId;
            if (endTime != "")
                order.endTime = Convert.ToDateTime(endTime);
            order.riderComment = riderComment;
            order.useBalance = useBalance;
            order.useCoupon = useCoupon;
            order.deposit = deposit;
            order.payTime = Convert.ToDateTime(payTime);
            order.payType = payType;
            order.payMoney = payMoney;
            order.id = orderId;
            OrderInfoOper.Instance.Update(order);
            var listFoods = new List<foodId_Amount>();
            for (int i = 0; i < foodIds.Length; i++)
            {
                foodId_Amount fa = new foodId_Amount(Convert.ToInt32(foodIds[i]), Convert.ToInt32(amounts[i]));
                listFoods.Add(fa);
            }
            OrderFoodsOper.Instance.UpdateOrderFoodsForWeb(orderId, listFoods);
            //OrderInfo type = new OrderInfo();
            ////type.comment = Request["comment"];

            //type.id = Convert.ToInt32(Request["foodTypeId"]);
            //DoUpdate(type);
        }

        /// <summary>
        /// 获取最近使用的地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetRecentlyAddress(webReq req)
        {
            var userId = req.userId;
            var ua = UserAddressOper.Instance.GetRecentlyAddr(userId).FirstOrDefault();
            return JsonConvert.SerializeObject(ua);
        }

        public void Add(webReq req)
        {
            var userId = req.userId;
            var createTime = Request["createTime"];
            var status = req.status;
            var isActual = Convert.ToBoolean(Request["isActual"]);
            var remarks = Request["remarks"];
            var riderId = req.riderId;
            var orderAreaId = req.orderAreaId;
            var endTime = Request["endTime"];
            var riderComment = Request["riderComment"];
            var useBalance = req.useBalance;
            var useCoupon = req.useCoupon;
            var deposit = req.deposit;
            var payTime = Request["payTime"];
            var payType = req.payType;
            var payMoney = req.payMoney;
            var foodIds = Request.Form.GetValues("foodIds[]");
            var amounts = Request.Form.GetValues("amounts[]");

            OrderInfo order = new OrderInfo();
            order.userId = userId;
            order.createTime = Convert.ToDateTime(createTime);
            order.status = Convert.ToInt32(status);
            order.isActual = isActual;
            order.remarks = remarks;
            order.riderId = riderId;
            order.orderAreaId = orderAreaId;
            if (endTime != "")
                order.endTime = Convert.ToDateTime(endTime);
            order.riderComment = riderComment;
            order.useBalance = useBalance;
            order.useCoupon = useCoupon;
            order.deposit = deposit;
            order.payTime = Convert.ToDateTime(payTime);
            order.payType = payType;
            order.payMoney = payMoney;

            var orderId = OrderInfoOper.Instance.Add(order);
            var listFoods = new List<foodId_Amount>();
            for (int i = 0; i < foodIds.Length; i++)
            {
                foodId_Amount fa = new foodId_Amount(Convert.ToInt32(foodIds[i]), Convert.ToInt32(amounts[i]));
                listFoods.Add(fa);
            }
            OrderFoodsOper.Instance.UpdateOrderFoodsForWeb(orderId, listFoods);
        }

        public void BatchDel(webReq req)
        {
            //var ids = req.ids;
            var ids = Request.Form.GetValues("ids[]");
            var temps= Array.ConvertAll<string, int>(ids, s => int.Parse(s)); 
            SqlHelperHere.DeleteByIds("orderinfo",temps);
            //string str = "update set ";
            ////string[] ids = Request.Form.GetValues("ids[]");
            //for (int i = 0; i < ids.Length; i++)
            //{
            //    DoDelete(ids[i]);
            //}
        }

        public void DoUpdate(OrderInfo order)
        {
            OrderInfoOper.Instance.Update(order);
        }

        public void DoDelete(int foodTypeId)
        {
            OrderInfo order = new OrderInfo();
            order.id = foodTypeId;
            order.isDeleted = true;
            DoUpdate(order);
        }

        /// <summary>
        /// 后台指派订单给骑手
        /// </summary>
        public string AssignRider(webReq req)
        {
            var orderId = req.orderId;
            var riderId = req.riderId;
            //var orderId = Convert.ToInt32(Request["orderId"]);
            //var riderId = Convert.ToInt32(Request["riderId"]);
            var msg = OrderInfoOper.Instance.AssignRider(orderId, riderId);
            if (msg != "")
                return JsonHelperHere.JsonMsg(false, msg);

            return JsonHelperHere.JsonMsg(true, msg);
        }

        /// <summary>
        /// 后台同意骑手取消
        /// </summary>
        /// <returns></returns>
        public string AgreeRiderCancel(webReq req)
        {
            var orderId = req.orderId;
            //var orderId = Convert.ToInt32(Request["orderId"]);
            var order = OrderInfoOper.Instance.GetById(orderId);
            if (order.status != 99)
                return JsonHelperHere.JsonMsg(false, "订单状态已改变");
            order.status = 5;

            OrderInfoOper.Instance.Update(order);

            OrderInfoOper.Instance.OrderCancelDoMoney(order);

            UPushHelper.Instance.PushUserOrderCancel(order);

            //用户取消订单，菜品返还给骑手
            var list = OrderFoodsOper.Instance.GetByOrderId(order.id);
            var listFoods = new List<foodId_Amount>();
            foreach (var item in list)
            {
                foodId_Amount fa = new foodId_Amount(item);
                listFoods.Add(fa);
            }
            RiderStockOper.Instance.AddRs((int)order.riderId, listFoods);


            return JsonHelperHere.JsonMsg(true, "");
        }
    }
}
