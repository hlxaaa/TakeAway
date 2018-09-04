using System.Text;
using System.Linq;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Response;
using takeAwayWebApi.Models;
using System;
using takeAwayWebApi.Models.Request;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class RiderStockOper : SingleTon<RiderStockOper>
    {
        public List<RiderStock> GetByRiderId(int riderId)
        {
            return CacheHelper.GetByCondition<RiderStock>("RiderStock", " isdeleted=0 and riderId=" + riderId);
        }

        //RiderInfoOper riderOper = new RiderInfoOper();
        public List<int> GetFoodId(int riderId)
        {
            List<RiderStock> list = GetByRiderId(riderId);
            return list.Select(p => (int)p.foodId).ToList();
            //List<int> listR = new List<int>();
            //foreach (var item in list)
            //{
            //    listR.Add((int)item.foodId);
            //}
            //return listR;
        }

        /// 获取参数
        /// </summary>
        /// <param name="riderstock"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(RiderStock riderstock)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (riderstock.id != null)
                dict.Add("@id", riderstock.id.ToString());
            if (riderstock.riderId != null)
                dict.Add("@riderId", riderstock.riderId.ToString());
            if (riderstock.foodId != null)
                dict.Add("@foodId", riderstock.foodId.ToString());
            if (riderstock.amount != null)
                dict.Add("@amount", riderstock.amount.ToString());
            if (riderstock.isDeleted != null)
                dict.Add("@isDeleted", riderstock.isDeleted.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="riderstock"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(RiderStock riderstock)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (riderstock.riderId != null)
            {
                part1.Append("riderId,");
                part2.Append("@riderId,");
            }
            if (riderstock.foodId != null)
            {
                part1.Append("foodId,");
                part2.Append("@foodId,");
            }
            if (riderstock.amount != null)
            {
                part1.Append("amount,");
                part2.Append("@amount,");
            }
            if (riderstock.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into riderstock(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="riderstock"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(RiderStock riderstock)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update riderstock set ");
            if (riderstock.riderId != null)
                part1.Append("riderId = @riderId,");
            if (riderstock.foodId != null)
                part1.Append("foodId = @foodId,");
            if (riderstock.amount != null)
                part1.Append("amount = @amount,");
            if (riderstock.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }
        /// <summary>
        /// add
        /// </summary>
        /// <param name="RiderStock"></param>
        /// <returns></returns>
        public int Add(RiderStock model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="RiderStock"></param>
        /// <returns></returns>
        public void Update(RiderStock model)
        {
            CacheHelper.LockCache("RiderStock");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            CacheHelper.ReleaseLock("RiderStock");
        }

        /// <summary>
        /// 根据区域id获取rs的join视图
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<RiderStockJoin> GetRSJoinByAreaId(int areaId)
        {
            return CacheHelper.GetByCondition<RiderStockJoin>("riderstockjoin", " riderAreaId = " + areaId);
        }

        /// <summary>
        /// 获取区域内的主菜
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<RiderStockJoin> GetMainRSByAreaId(int areaId)
        {
            return CacheHelper.GetByCondition<RiderStockJoin>("riderstockjoin", " riderstatus=1 and  ismain=1 and riderAreaId = " + areaId);
        }

        /// <summary>
        /// 根据骑手id获取rs的视图
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public List<RiderStockJoin> GetRSViewByRiderId(int riderId)
        {
            return CacheHelper.GetByCondition<RiderStockJoin>("riderstockjoin", " riderid=" + riderId);
        }

        /// <summary>
        /// 获取区域内附加品
        /// </summary>
        /// <returns></returns>
        public List<RiderStockJoin> GetStockNotMain(int areaId)
        {
            return CacheHelper.GetByCondition<RiderStockJoin>("RiderStockJoin", " ismain=0 and riderAreaId=" + areaId);
        }

        /// <summary>
        /// 获取区域中的菜品标签
        /// </summary>
        /// <returns></returns>
        public List<TagRes> GetTagInArea(int areaId)
        {
            var list = CacheHelper.GetByCondition<RiderStockJoin>("RiderStockJoin", " riderStatus=1 and  riderAreaId = " + areaId);

            var dict = list.Select(p => new { p.tagId, p.tagName }).Distinct().ToDictionary(p => (int)p.tagId, p => p.tagName);
            var listTemp = new List<TagRes>();
            foreach (var item in dict)
            {
                TagRes tr = new TagRes(item.Key, item.Value);
                listTemp.Add(tr);
            }

            return listTemp;
        }

        /// <summary>
        /// 根据用户位置，区域，菜品，获取周围5千米以内的骑手位置信息
        /// 
        /// </summary>
        /// <param name="foodId"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public int[] GetRiderIdsByFoodId_Position(int foodId, decimal lat, decimal lng, int areaId)
        {
            var list = CacheHelper.GetByCondition<RiderStockJoin>("RiderStockJoin", " riderstatus=1 and  riderAreaId=" + areaId + " and foodId=" + foodId);
            int[] riderIds = list.Select(p => (int)p.riderId).Distinct().ToArray();

            List<int> r = new List<int>();
            foreach (var item in riderIds)
            {
                RiderPosition rp = CacheHelper.GetRiderPosition(item);
                var lat1 = Convert.ToDouble(lat);
                var lng1 = Convert.ToDouble(lng);
                var lat2 = Convert.ToDouble(rp.lat);
                var lng2 = Convert.ToDouble(rp.lng);
                if (DistanceHelperHere.Distance(lat1, lng1, lat2, lng2) < 5)
                {
                    r.Add(item);
                }
            }
            return r.ToArray();
        }

        /// <summary>
        /// 判断骑手能不能接这单
        /// </summary>
        /// <returns></returns>
        public bool IsEnough(int riderId, int orderId)
        {
            var orderFoods = OrderFoodsOper.Instance.GetByOrderId(orderId);
            var r = true;
            var ListRs = GetRSViewByRiderId(riderId);
            foreach (var item in orderFoods)
            {
                var temp = ListRs.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count < 1)
                {
                    r = false;
                    break;
                }
                else if (temp.Count > 0 && temp.First().amount < item.amount)
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// 判断骑手能不能这些订单
        /// </summary>
        /// <returns></returns>
        public List<riderEnough> IsEnough(int riderId, int[] orderIds)
        {
            var listR = new List<riderEnough>();
            var ListRs = GetRSViewByRiderId(riderId);
            if (ListRs.Count < 1)
                return listR;
            var str = string.Join(",", orderIds.Distinct());
            var ofList = CacheHelper.GetByCondition<OrderFoods>("orderfoods", " orderId in(" + str + ") ");
            foreach (var item in orderIds)
            {
                var listTemp = ofList.Where(p => p.orderId == item).ToList();
                foreach (var item2 in listTemp)
                {
                    var temp = ListRs.Where(p => p.foodId == item2.foodId).ToList();
                    riderEnough r = new riderEnough();
                    if (temp.Count > 0 && temp.First().amount >= item2.amount)
                    {
                        r = new riderEnough(item, true);
                        listR.Add(r);
                    }
                    else
                    {
                        r = new riderEnough(item, false);
                        listR.Add(r);
                        break;
                    }

                }
            }
            return listR;
        }

        /// <summary>
        /// 判断区域库存是否支持订单的需求
        /// </summary>
        /// <returns></returns>
        public bool IsStockEnough(int areaId, int orderId)
        {
            //获取订单中的菜品
            var orderFoods = OrderFoodsOper.Instance.GetByOrderId(orderId);
            //区域中有的菜品
            var list = GetRSJoinByAreaId(areaId);
            var r = true;
            foreach (var item in orderFoods)
            {
                var temp = list.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count < 1)
                {
                    r = false;
                    break;
                }
                else if (temp.Sum(p => p.amount) < item.amount)
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// 移除骑手身上的部分库存
        /// </summary>
        public bool RemoveRs(int riderId, List<foodId_Amount> listFoods)
        {
            var list = CacheHelper.GetByCondition<RiderStock>("riderstock", " isdeleted=0 and  riderid=" + riderId);

            var listRs = new List<RiderStock>();
            foreach (var item in listFoods)
            {
                var temp = list.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count < 1)
                    return false;
                var rs = temp.First();
                rs.amount -= item.amount;
                if (rs.amount < 0)
                    return false;
                //如果所有的都调配给别人了。自己这边的就删掉
                if (rs.amount == 0)
                    rs.isDeleted = true;
                listRs.Add(rs);

            }
            foreach (var item in listRs)
            {
                Update(item);
            }
            return true;
        }

        /// <summary>
        /// 增加库存
        /// </summary>
        /// <param name="riderId"></param>
        /// <param name="listFoods"></param>
        /// <returns></returns>
        public bool AddRs(int riderId, List<foodId_Amount> listFoods)
        {
            var list = CacheHelper.GetByCondition<RiderStock>("riderstock", " isdeleted=0 and  riderid=" + riderId);
            foreach (var item in listFoods)
            {
                var temp = list.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count < 1)
                {
                    RiderStock r = new RiderStock
                    {
                        riderId = riderId,
                        foodId = item.foodId,
                        amount = item.amount
                    };
                    Add(r);
                }
                else
                {
                    var rs = temp.First();
                    rs.amount += item.amount;
                    if (rs.amount < 0)
                        return false;
                    Update(rs);
                }
            }
            return true;
        }

        /// <summary>
        /// 恢复部分库存
        /// </summary>
        /// <param name="riderId"></param>
        /// <param name="listFoods"></param>
        /// <returns></returns>
        public bool RecoverRs(int riderId, List<foodId_Amount> listFoods)
        {
            var list = CacheHelper.GetByCondition<RiderStock>("riderstock", " isdeleted=0 and riderid=" + riderId);
            foreach (var item in listFoods)
            {
                var temp = list.Where(p => p.foodId == item.foodId).ToList();
                if (temp.Count < 1)
                    continue;
                var rs = temp.First();
                rs.amount += item.amount;
                Update(rs);
            }
            return true;
        }

        public void Delete(int rsId)
        {
            RiderStock rs = new RiderStock();
            rs.id = rsId;
            rs.isDeleted = true;
            Update(rs);
        }

        /// <summary>
        /// 后台删除某些菜品时，所有骑手库存都要删掉
        /// </summary>
        /// <param name="foodId"></param>
        public void DeleteRsByFoodId(int[] foodIds)
        {
            var idsStr = string.Join(",", foodIds);
            string str = " update riderstock set isdeleted=1 where foodId in (" + idsStr + ")";
            SqlHelperHere.ExcuteNon(str);
        }

        /// <summary>
        /// 后台删除某种菜品时，所有骑手库存都要删掉
        /// </summary>
        /// <param name="foodId"></param>
        public void DeleteRsByFoodId(int foodId)
        {
            string str = " update riderstock set isdeleted=1 where foodId = " + foodId;
            SqlHelperHere.ExcuteNon(str);
        }

        //—————————————不用了—————————————————————————————

        //public List<RiderStockView> GetRSView() {
        //    return (List<RiderStockView>)CacheHelper.Get<RiderStockView>("RiderStockView", "Ta_RiderStockView"); 
        //}

        ///// <summary>
        ///// 获取库存中的主菜信息
        ///// </summary>
        ///// <returns></returns>
        //public List<RiderStockView> GetStockMain()
        //{
        //    List<RiderStockView> list = (List<RiderStockView>)CacheHelper.Get<RiderStockView>("RiderStockView", "Ta_RiderStockView");
        //    return list.Where(p => p.isMain == true).ToList();
        //}



    }
}
