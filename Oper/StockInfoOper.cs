using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class StockInfoOper : SingleTon<StockInfoOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="stockinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(StockInfo stockinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (stockinfo.id != null)
                dict.Add("@id", stockinfo.id.ToString());
            if (stockinfo.areaId != null)
                dict.Add("@areaId", stockinfo.areaId.ToString());
            if (stockinfo.foodId != null)
                dict.Add("@foodId", stockinfo.foodId.ToString());
            if (stockinfo.amount != null)
                dict.Add("@amount", stockinfo.amount.ToString());
            if (stockinfo.isDeleted != null)
                dict.Add("@isDeleted", stockinfo.isDeleted.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="stockinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(StockInfo stockinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (stockinfo.areaId != null)
            {
                part1.Append("areaId,");
                part2.Append("@areaId,");
            }
            if (stockinfo.foodId != null)
            {
                part1.Append("foodId,");
                part2.Append("@foodId,");
            }
            if (stockinfo.amount != null)
            {
                part1.Append("amount,");
                part2.Append("@amount,");
            }
            if (stockinfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into stockinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
                 /// 更新
                 /// </summary>
                 /// <param name="stockinfo"></param>
                 /// <returns>是否成功</returns>
        public string GetUpdateStr(StockInfo stockinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update stockinfo set ");
            if (stockinfo.areaId != null)
                part1.Append("areaId = @areaId,");
            if (stockinfo.foodId != null)
                part1.Append("foodId = @foodId,");
            if (stockinfo.amount != null)
                part1.Append("amount = @amount,");
            if (stockinfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="StockInfo"></param>
        /// <returns></returns>
        public int Add(StockInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="StockInfo"></param>
        /// <returns></returns>
        public void Update(StockInfo model)
        {
            //CacheHelper.LockCache("StockInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("StockInfo");
        }

        public void Delete(int stockId)
        {
            StockInfo stock = new StockInfo();
            stock.id = stockId;
            stock.isDeleted = true;
            Update(stock);
        }

        //public List<int> GetFoodIdsInArea(int areaId) {
        //    List<StockInfo> list = (List<StockInfo>)CacheHelper.Get<StockInfo>("StockInfo","Ta_StockInfo");
        //    list = list.Where(p => p.areaId == areaId && p.isDeleted == false).ToList();
        //    List<int> listR = new List<int>();
        //    foreach (var item in list)
        //    {
        //        listR.Add((int)item.foodId);
        //    }
        //    return listR;
        //}
    }
}
