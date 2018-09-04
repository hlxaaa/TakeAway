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
    public class OrderAjaxController : WebAjaxController
    {
        /// <summary>
        /// 根据条件在OrderTryJoin里找到所有的，并distinct出当前页的那些orderid，再去ordertryjoin里找
        /// </summary>
        /// <returns></returns>
        public string Get(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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

                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);

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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
            }
            #endregion
            #region 已评论
            else if (status == 7)
            {
                var listR = new List<OrderArrivalRes>();
                //查询条件
                condition += "  status in (7) and (orderno='" + search + "' or username like '%" + search + "%' or foodname like '%" + search + "%' or areaname like '%" + search + "%' or mapaddress like '%" + search + "%' or contactPhone like '%" + search + "%' or riderName like '%" + search + "%')";
                //所有符合条件的记录的id
                var listAll = CacheHelper.GetDistinctCount<OrderTryJoin>("OrderTryJoin", condition);
                if (listAll.Count < 1)
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);
                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);


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
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);

                }
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
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
            }

            #endregion
            return null;
        }

        public string DeleteOrder(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var orderId = req.orderId;
            OrderInfoOper.Instance.Delete(orderId);
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
            var createTime = req.createTime;
            var status = req.status;
            var isActual = Convert.ToBoolean(req.isActual);
            var remarks = req.remarks;
            var riderId = req.riderId;
            var orderAreaId = req.orderAreaId;
            var endTime = req.endTime;
            var riderComment = req.riderComment;
            var useBalance = req.useBalance;
            var useCoupon = req.useCoupon;
            var deposit = req.deposit;
            var payTime = req.payTime;
            var payType = req.payType;
            var payMoney = req.payMoney;

            var foodIds = req.foodIds;
            var amounts = req.amounts;

            var orderId = req.orderId;
            OrderInfo order = new OrderInfo();
            order.createTime = Convert.ToDateTime(createTime);
            order.status = Convert.ToInt32(status);
            order.isActual = isActual;
            order.remarks = remarks;
            order.riderId = riderId;
            order.orderAreaId = orderAreaId;
            if (endTime <new DateTime(2017,1,1))
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
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 获取最近使用的地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public string GetRecentlyAddress(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var userId = req.userId;
            var ua = UserAddressOper.Instance.GetRecentlyAddr(userId).FirstOrDefault();
            r.data = JsonConvert.SerializeObject(ua);
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
            var userId = req.userId;
            var createTime = req.createTime;
            var status = req.status;
            var isActual = Convert.ToBoolean(req.isActual);
            var remarks = req.remarks;
            var riderId = req.riderId;
            var orderAreaId = req.orderAreaId;
            var endTime = req.endTime;
            var riderComment = req.riderComment;
            var useBalance = req.useBalance;
            var useCoupon = req.useCoupon;
            var deposit = req.deposit;
            var payTime = req.payTime;
            var payType = req.payType;
            var payMoney = req.payMoney;
        

            var foodIds = req.foodIds;
            var amounts = req.amounts;

            OrderInfo order = new OrderInfo();
            order.userId = userId;
            order.createTime = Convert.ToDateTime(createTime);
            order.status = Convert.ToInt32(status);
            order.isActual = isActual;
            order.remarks = remarks;
            order.riderId = riderId;
            order.orderAreaId = orderAreaId;
            //if (endTime != "")
            if (endTime < new DateTime(2007, 1, 1))
                endTime = DateTime.Now;
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
            SqlHelperHere.DeleteByIds("orderinfo", ids);
            return JsonConvert.SerializeObject(r);
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
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var orderId = req.orderId;
            var riderId = req.riderId;
            var msg = OrderInfoOper.Instance.AssignRider(orderId, riderId);
            if (msg != "")
            {
                //return JsonHelperHere.JsonMsg(false, msg);
                r.Message = msg;
                r.HttpCode = 500;
            }
            else
            {
                r.Message = msg;
            }
            //return JsonHelperHere.JsonMsg(true, msg);
            return JsonConvert.SerializeObject(r);
        }

        /// <summary>
        /// 后台同意骑手取消
        /// </summary>
        /// <returns></returns>
        public string AgreeRiderCancel(webReq req)
        {
            ResultForWeb r = new ResultForWeb
            {
                HttpCode = 200,
                Message = "",
                data = "{}"
            };
            var orderId = req.orderId;
            var order = OrderInfoOper.Instance.GetById(orderId);
            if (order.status != 99)
            {
                r.Message = "订单状态已改变，不再需要取消";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);

            }
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


            r.data = JsonHelperHere.JsonMsg(true, "");
            return JsonConvert.SerializeObject(r);
        }

    }
}
