
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using takeAwayWebApi.Models.Request;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class OrderFoodsOper : SingleTon<OrderFoodsOper>
    {
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="orderfoods"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(OrderFoods orderfoods)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (orderfoods.id != null)
                dict.Add("@id", orderfoods.id.ToString());
            if (orderfoods.orderId != null)
                dict.Add("@orderId", orderfoods.orderId.ToString());
            if (orderfoods.foodId != null)
                dict.Add("@foodId", orderfoods.foodId.ToString());
            if (orderfoods.amount != null)
                dict.Add("@amount", orderfoods.amount.ToString());
            if (orderfoods.isDeleted != null)
                dict.Add("@isDeleted", orderfoods.isDeleted.ToString());
            if (orderfoods.LastName != null)
                dict.Add("@LastName", orderfoods.LastName.ToString());
            if (orderfoods.LastPrice != null)
                dict.Add("@LastPrice", orderfoods.LastPrice.ToString());

            return dict;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="orderfoods"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(OrderFoods orderfoods)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (orderfoods.orderId != null)
            {
                part1.Append("orderId,");
                part2.Append("@orderId,");
            }
            if (orderfoods.foodId != null)
            {
                part1.Append("foodId,");
                part2.Append("@foodId,");
            }
            if (orderfoods.amount != null)
            {
                part1.Append("amount,");
                part2.Append("@amount,");
            }
            if (orderfoods.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (orderfoods.LastName != null)
            {
                part1.Append("LastName,");
                part2.Append("@LastName,");
            }
            if (orderfoods.LastPrice != null)
            {
                part1.Append("LastPrice,");
                part2.Append("@LastPrice,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into orderfoods(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="orderfoods"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(OrderFoods orderfoods)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update orderfoods set ");
            if (orderfoods.orderId != null)
                part1.Append("orderId = @orderId,");
            if (orderfoods.foodId != null)
                part1.Append("foodId = @foodId,");
            if (orderfoods.amount != null)
                part1.Append("amount = @amount,");
            if (orderfoods.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (orderfoods.LastName != null)
                part1.Append("LastName = @LastName,");
            if (orderfoods.LastPrice != null)
                part1.Append("LastPrice = @LastPrice,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="OrderFoods"></param>
        /// <returns></returns>
        public int Add(OrderFoods model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="OrderFoods"></param>
        /// <returns></returns>
        public void Update(OrderFoods model)
        {
            //CacheHelper.LockCache("OrderFoods");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("OrderFoods");
        }

        /// <summary>
        /// del
        /// </summary>
        /// <param name="OrderFoods"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            OrderFoods model = new OrderFoods();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }

        public List<OrderFoods> GetByOrderId(int orderId) {
            return CacheHelper.GetByCondition<OrderFoods>("orderfoods"," orderid = "+orderId);
        }

        /// <summary>
        /// 后台更新订单菜品
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="listFoods"></param>
        public void UpdateOrderFoodsForWeb(int orderId,List<foodId_Amount> listFoods) {
            var list = CacheHelper.GetByCondition<OrderFoods>("orderFoods"," isdeleted=0 and orderid="+orderId);
            foreach (var item in list)
            {
                item.isDeleted = true;
                Update(item);
            }
            foreach (var item in listFoods)
            {
                OrderFoods of = new OrderFoods();
                of.orderId = orderId;
                of.foodId = item.foodId;
                of.amount = item.amount;
                Add(of);
            }
        }

        /// <summary>
        /// 保存订单菜品最终的内容
        /// </summary>
        /// <param name="order"></param>
        public void SetSnapShot(int orderId) {
            var ofList = GetByOrderId(orderId);
            var ids = ofList.Select(p => (int)p.foodId).ToArray();
            var foodList = CacheHelper.GetByIds<FoodInfo>("foodinfo",ids);
            foreach (var item in ofList)
            {
                var lastName = foodList.Where(p => p.id == item.foodId).FirstOrDefault().foodName;
                var lastPrice = foodList.Where(p => p.id == item.foodId).FirstOrDefault().foodPrice;
                lastName = lastName ?? "";
                lastPrice = lastPrice ?? 0m;
                item.LastName = lastName;
                item.LastPrice = lastPrice;
                Update(item);
            }
        }
    }
}
