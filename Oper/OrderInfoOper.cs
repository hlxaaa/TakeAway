
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using System.Linq;
using takeAwayWebApi.Models.Request;
using UPush;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class OrderInfoOper : SingleTon<OrderInfoOper>
    {
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="orderinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(OrderInfo orderinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (orderinfo.id != null)
                dict.Add("@id", orderinfo.id.ToString());
            if (orderinfo.userId != null)
                dict.Add("@userId", orderinfo.userId.ToString());
            if (orderinfo.riderId != null)
                dict.Add("@riderId", orderinfo.riderId.ToString());
            if (orderinfo.riderName != null)
                dict.Add("@riderName", orderinfo.riderName.ToString());
            if (orderinfo.foodStars != null)
                dict.Add("@foodStars", orderinfo.foodStars.ToString());
            if (orderinfo.riderStars != null)
                dict.Add("@riderStars", orderinfo.riderStars.ToString());
            if (orderinfo.foodComment != null)
                dict.Add("@foodComment", orderinfo.foodComment.ToString());
            if (orderinfo.userPhone != null)
                dict.Add("@userPhone", orderinfo.userPhone.ToString());
            if (orderinfo.orderAreaId != null)
                dict.Add("@orderAreaId", orderinfo.orderAreaId.ToString());
            if (orderinfo.isActual != null)
                dict.Add("@isActual", orderinfo.isActual.ToString());
            if (orderinfo.status != null)
                dict.Add("@status", orderinfo.status.ToString());
            if (orderinfo.isCancelling != null)
                dict.Add("@isCancelling", orderinfo.isCancelling.ToString());
            if (orderinfo.isDeleted != null)
                dict.Add("@isDeleted", orderinfo.isDeleted.ToString());
            if (orderinfo.addressId != null)
                dict.Add("@addressId", orderinfo.addressId.ToString());
            if (orderinfo.arrivalTime != null)
                dict.Add("@arrivalTime", orderinfo.arrivalTime.ToString());
            if (orderinfo.orderNo != null)
                dict.Add("@orderNo", orderinfo.orderNo.ToString());
            if (orderinfo.payType != null)
                dict.Add("@payType", orderinfo.payType.ToString());
            if (orderinfo.timeArea != null)
                dict.Add("@timeArea", orderinfo.timeArea.ToString());
            if (orderinfo.useBalance != null)
                dict.Add("@useBalance", orderinfo.useBalance.ToString());
            if (orderinfo.useCoupon != null)
                dict.Add("@useCoupon", orderinfo.useCoupon.ToString());
            if (orderinfo.remarks != null)
                dict.Add("@remarks", orderinfo.remarks.ToString());
            if (orderinfo.payMoney != null)
                dict.Add("@payMoney", orderinfo.payMoney.ToString());
            if (orderinfo.sumMoney != null)
                dict.Add("@sumMoney", orderinfo.sumMoney.ToString());
            if (orderinfo.payTime != null)
                dict.Add("@payTime", orderinfo.payTime.ToString());
            if (orderinfo.endTime != null)
                dict.Add("@endTime", orderinfo.endTime.ToString());
            if (orderinfo.riderComment != null)
                dict.Add("@riderComment", orderinfo.riderComment.ToString());
            if (orderinfo.createTime != null)
                dict.Add("@createTime", orderinfo.createTime.ToString());
            if (orderinfo.riderTakeTime != null)
                dict.Add("@riderTakeTime", orderinfo.riderTakeTime.ToString());
            if (orderinfo.deposit != null)
                dict.Add("@deposit", orderinfo.deposit.ToString());
            if (orderinfo.isTake != null)
                dict.Add("@isTake", orderinfo.isTake.ToString());

            return dict;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="orderinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(OrderInfo orderinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (orderinfo.userId != null)
            {
                part1.Append("userId,");
                part2.Append("@userId,");
            }
            if (orderinfo.riderId != null)
            {
                part1.Append("riderId,");
                part2.Append("@riderId,");
            }
            if (orderinfo.riderName != null)
            {
                part1.Append("riderName,");
                part2.Append("@riderName,");
            }
            if (orderinfo.foodStars != null)
            {
                part1.Append("foodStars,");
                part2.Append("@foodStars,");
            }
            if (orderinfo.riderStars != null)
            {
                part1.Append("riderStars,");
                part2.Append("@riderStars,");
            }
            if (orderinfo.foodComment != null)
            {
                part1.Append("foodComment,");
                part2.Append("@foodComment,");
            }
            if (orderinfo.userPhone != null)
            {
                part1.Append("userPhone,");
                part2.Append("@userPhone,");
            }
            if (orderinfo.orderAreaId != null)
            {
                part1.Append("orderAreaId,");
                part2.Append("@orderAreaId,");
            }
            if (orderinfo.isActual != null)
            {
                part1.Append("isActual,");
                part2.Append("@isActual,");
            }
            if (orderinfo.status != null)
            {
                part1.Append("status,");
                part2.Append("@status,");
            }
            if (orderinfo.isCancelling != null)
            {
                part1.Append("isCancelling,");
                part2.Append("@isCancelling,");
            }
            if (orderinfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (orderinfo.addressId != null)
            {
                part1.Append("addressId,");
                part2.Append("@addressId,");
            }
            if (orderinfo.arrivalTime != null)
            {
                part1.Append("arrivalTime,");
                part2.Append("@arrivalTime,");
            }
            if (orderinfo.orderNo != null)
            {
                part1.Append("orderNo,");
                part2.Append("@orderNo,");
            }
            if (orderinfo.payType != null)
            {
                part1.Append("payType,");
                part2.Append("@payType,");
            }
            if (orderinfo.timeArea != null)
            {
                part1.Append("timeArea,");
                part2.Append("@timeArea,");
            }
            if (orderinfo.useBalance != null)
            {
                part1.Append("useBalance,");
                part2.Append("@useBalance,");
            }
            if (orderinfo.useCoupon != null)
            {
                part1.Append("useCoupon,");
                part2.Append("@useCoupon,");
            }
            if (orderinfo.remarks != null)
            {
                part1.Append("remarks,");
                part2.Append("@remarks,");
            }
            if (orderinfo.payMoney != null)
            {
                part1.Append("payMoney,");
                part2.Append("@payMoney,");
            }
            if (orderinfo.sumMoney != null)
            {
                part1.Append("sumMoney,");
                part2.Append("@sumMoney,");
            }
            if (orderinfo.payTime != null)
            {
                part1.Append("payTime,");
                part2.Append("@payTime,");
            }
            if (orderinfo.endTime != null)
            {
                part1.Append("endTime,");
                part2.Append("@endTime,");
            }
            if (orderinfo.riderComment != null)
            {
                part1.Append("riderComment,");
                part2.Append("@riderComment,");
            }
            if (orderinfo.createTime != null)
            {
                part1.Append("createTime,");
                part2.Append("@createTime,");
            }
            if (orderinfo.riderTakeTime != null)
            {
                part1.Append("riderTakeTime,");
                part2.Append("@riderTakeTime,");
            }
            if (orderinfo.deposit != null)
            {
                part1.Append("deposit,");
                part2.Append("@deposit,");
            }
            if (orderinfo.isTake != null)
            {
                part1.Append("isTake,");
                part2.Append("@isTake,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into orderinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="orderinfo"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(OrderInfo orderinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update orderinfo set ");
            if (orderinfo.userId != null)
                part1.Append("userId = @userId,");
            if (orderinfo.riderId != null)
                part1.Append("riderId = @riderId,");
            if (orderinfo.riderName != null)
                part1.Append("riderName = @riderName,");
            if (orderinfo.foodStars != null)
                part1.Append("foodStars = @foodStars,");
            if (orderinfo.riderStars != null)
                part1.Append("riderStars = @riderStars,");
            if (orderinfo.foodComment != null)
                part1.Append("foodComment = @foodComment,");
            if (orderinfo.userPhone != null)
                part1.Append("userPhone = @userPhone,");
            if (orderinfo.orderAreaId != null)
                part1.Append("orderAreaId = @orderAreaId,");
            if (orderinfo.isActual != null)
                part1.Append("isActual = @isActual,");
            if (orderinfo.status != null)
                part1.Append("status = @status,");
            if (orderinfo.isCancelling != null)
                part1.Append("isCancelling = @isCancelling,");
            if (orderinfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (orderinfo.addressId != null)
                part1.Append("addressId = @addressId,");
            if (orderinfo.arrivalTime != null)
                part1.Append("arrivalTime = @arrivalTime,");
            if (orderinfo.orderNo != null)
                part1.Append("orderNo = @orderNo,");
            if (orderinfo.payType != null)
                part1.Append("payType = @payType,");
            if (orderinfo.timeArea != null)
                part1.Append("timeArea = @timeArea,");
            if (orderinfo.useBalance != null)
                part1.Append("useBalance = @useBalance,");
            if (orderinfo.useCoupon != null)
                part1.Append("useCoupon = @useCoupon,");
            if (orderinfo.remarks != null)
                part1.Append("remarks = @remarks,");
            if (orderinfo.payMoney != null)
                part1.Append("payMoney = @payMoney,");
            if (orderinfo.sumMoney != null)
                part1.Append("sumMoney = @sumMoney,");
            if (orderinfo.payTime != null)
                part1.Append("payTime = @payTime,");
            if (orderinfo.endTime != null)
                part1.Append("endTime = @endTime,");
            if (orderinfo.riderComment != null)
                part1.Append("riderComment = @riderComment,");
            if (orderinfo.createTime != null)
                part1.Append("createTime = @createTime,");
            if (orderinfo.riderTakeTime != null)
                part1.Append("riderTakeTime = @riderTakeTime,");
            if (orderinfo.deposit != null)
                part1.Append("deposit = @deposit,");
            if (orderinfo.isTake != null)
                part1.Append("isTake = @isTake,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="OrderInfo"></param>
        /// <returns></returns>
        public int Add(OrderInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="OrderInfo"></param>
        /// <returns></returns>
        public void Update(OrderInfo model)
        {
            //CacheHelper.LockCache("OrderInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("OrderInfo");
        }
        /// <summary>
        /// del
        /// </summary>
        /// <param name="OrderInfo"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            OrderInfo model = new OrderInfo();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }

        /// <summary>
        /// 根据订单id获取订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OrderInfo GetById(int orderId)
        {
            return CacheHelper.GetById<OrderInfo>("OrderInfo", orderId);
            //return Get().Where(p => p.id == orderId).First();
        }

        /// <summary>
        /// 根据orderId获取OrderJoin
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<OrderTryJoin> GetViewByOrderId(int orderId)
        {
            return CacheHelper.GetByCondition<OrderTryJoin>("ordertryjoin", " orderid = " + orderId);
        }

        public List<OrderTryJoin> GetViewByUserId(int userId)
        {
            return CacheHelper.GetByCondition<OrderTryJoin>("ordertryjoin", " userid = " + userId);
        }

        /// <summary>
        /// 获取正在配送和待接单的订单视图 实时的
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<OrderTryJoin> GetWaitAndSendOrder(int userId, int orderId)
        {
            return CacheHelper.GetByCondition<OrderTryJoin>("ordertryjoin", " status in (1,2) and   userid=" + userId + " and orderid=" + orderId + " order by id desc");
        }

        /// <summary>
        /// 根据用户id获取的最新的一个订单 status 1或2
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetSendingOrderIdsByUserId(int userId)
        {
            string str = "select top 1 id from orderinfo where   isdeleted = 0 and status in (1,2) and userid=@userId order by id desc";
            var dict = new Dictionary<string, string>();
            dict.Add("@userId", userId.ToString());
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateOrderStatus(int orderId, int status)
        {
            var order = new OrderInfo();
            order.id = orderId;
            order.status = status;
            if (status == 5 || status == 3 || status == 8)
            {
                order.endTime = DateTime.Now;
            }
            try
            {
                Update(order);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 把超时的未支付订单取消掉（status0→5）
        /// </summary>
        public void RemoveOverTime(int userId)
        {
            var list = CacheHelper.GetByCondition<OrderInfo>("orderinfo", " userId=" + userId + " and status=0 and datediff(SECOND,createTime,GETDATE())>900");//-txy 120秒。

            if (list.Count > 0)
            {//后期更新或者不更新缓存 什么意思啊 应该是（后期更新缓存或者不使用缓存）
                foreach (var item in list)
                {
                    var order = item;
                    order.status = 5;
                    order.endTime = order.createTime.Value.AddMinutes(16);
                    Update(order);
                }
            }
        }

        /// <summary>
        /// 把所有超时的未支付订单去掉
        /// </summary>
        public void RemoveOverTime()
        {
            var list = CacheHelper.GetByCondition<OrderInfo>("orderinfo", " status=0 and datediff(SECOND,createTime,GETDATE())>900");//-txy 120秒。

            if (list.Count > 0)
            {//后期更新或者不更新缓存 什么意思啊 应该是（后期更新缓存或者不使用缓存）
                foreach (var item in list)
                {
                    var order = item;
                    order.status = 5;
                    order.endTime = order.createTime.Value.AddMinutes(16);
                    Update(order);
                }
            }
        }

        /// <summary>
        /// 没人用
        /// </summary>
        /// <returns></returns>
        public List<OrderTryJoin> GetActualWaitOrder()
        {
            //return null;
            return CacheHelper.GetByCondition<OrderTryJoin>("ordertryjoin", " isactual=1 and status = 1 order by orderId desc");
        }

        /// <summary>
        /// 指派某个订单给骑手
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public string AssignRider(int orderId, int riderId)
        {
            try
            {
                var order = GetById(orderId);
                if (order.status != 1)
                    return "订单状态已改变，无法分配";
                order.riderId = riderId;
                order.status = 2;
                Update(order);
                //给骑手客户端的消息
                UPushHelper.Instance.PushRiderServerAssign(order);
                //告诉用户，正在派送
                UPushHelper.Instance.PushUserSendingOrder(order);
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }



        }

        public Dictionary<int, string> GetStatusDict()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "未支付");
            dict.Add(1, "待抢单");
            dict.Add(2, "派送中");
            dict.Add(3, "已送达");
            dict.Add(5, "订单取消");
            dict.Add(7, "已评价");
            dict.Add(8, "待回收");
            dict.Add(9, "申请回收中");
            dict.Add(99, "骑手申请取消中");
            return dict;
        }

        /// <summary>
        /// 取消订单后，金额处理
        /// </summary>
        /// <param name="order"></param>
        public bool OrderCancelDoMoney(OrderInfo order)
        {
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            user.userBalance += order.useBalance + order.payMoney;
            user.coupon += order.useCoupon;
            UserInfoOper.Instance.Update(user);
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
            return true;
        }

        /// <summary>
        /// 支付订单，添加收支明细
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool PayOrderRecord(OrderInfo order)
        {
            if (order.useBalance > 0)
            {
                UserPay up = new UserPay();
                up.type = 2;
                up.money = order.useBalance;
                up.createTime = DateTime.Now;
                up.userId = order.userId;
                UserPayOper.Instance.Add(up);
            }
            if (order.useCoupon > 0)
            {
                UserPay up = new UserPay();
                up.type = 3;
                up.money = order.useCoupon;
                up.createTime = DateTime.Now;
                up.userId = order.userId;
                UserPayOper.Instance.Add(up);
            }
            return true;

        }

        //—————————————————不用了—————————————————————————————————
        ///// <summary>
        ///// 获取OrderView的列表
        ///// </summary>
        ///// <returns></returns>
        //public List<OrderView> GetView()
        //{
        //    return (List<OrderView>)CacheHelper.Get<OrderView>("OrderView", "Ta_OrderView");
        //}

        ///// <summary>
        ///// 获取正在配送中的订单
        ///// </summary>
        ///// <returns></returns>
        //public List<OrderSendingView> GetSendingView()
        //{
        //    return (List<OrderSendingView>)CacheHelper.Get<OrderSendingView>("OrderSendingView", "Ta_OrderSendingView");
        //}

        ///// <summary>
        ///// 获取正在配送订单的视图
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public List<OrderSendingView> GetSendingViewByUserId(int userId) {
        //    return  GetSendingView().Where(p=>p.userId==userId).ToList();
        //}


        ///// <summary>
        ///// 根据用户id，订单id获取sendingOrderView
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="orderId"></param>
        ///// <returns></returns>
        //public List<OrderSendingView> GetSVByUserOrderId(int userId, int orderId)
        //{
        //    return GetSendingView().Where(p => p.userId == userId && p.orderId == orderId).ToList();
        //}
    }
}
