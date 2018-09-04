using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class AllocateRecordOper : SingleTon<AllocateRecordOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="allocaterecord"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(AllocateRecord allocaterecord)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (allocaterecord.id != null)
                dict.Add("@id", allocaterecord.id.ToString());
            if (allocaterecord.riderId != null)
                dict.Add("@riderId", allocaterecord.riderId.ToString());
            if (allocaterecord.foodId != null)
                dict.Add("@foodId", allocaterecord.foodId.ToString());
            if (allocaterecord.amount != null)
                dict.Add("@amount", allocaterecord.amount.ToString());
            if (allocaterecord.targetRiderId != null)
                dict.Add("@targetRiderId", allocaterecord.targetRiderId.ToString());
            if (allocaterecord.status != null)
                dict.Add("@status", allocaterecord.status.ToString());
            if (allocaterecord.isDeleted != null)
                dict.Add("@isDeleted", allocaterecord.isDeleted.ToString());
            if (allocaterecord.timestamp != null)
                dict.Add("@timestamp", allocaterecord.timestamp.ToString());

            return dict;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="allocaterecord"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(AllocateRecord allocaterecord)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (allocaterecord.riderId != null)
            {
                part1.Append("riderId,");
                part2.Append("@riderId,");
            }
            if (allocaterecord.foodId != null)
            {
                part1.Append("foodId,");
                part2.Append("@foodId,");
            }
            if (allocaterecord.amount != null)
            {
                part1.Append("amount,");
                part2.Append("@amount,");
            }
            if (allocaterecord.targetRiderId != null)
            {
                part1.Append("targetRiderId,");
                part2.Append("@targetRiderId,");
            }
            if (allocaterecord.status != null)
            {
                part1.Append("status,");
                part2.Append("@status,");
            }
            if (allocaterecord.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (allocaterecord.timestamp != null)
            {
                part1.Append("timestamp,");
                part2.Append("@timestamp,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into allocaterecord(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="allocaterecord"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(AllocateRecord allocaterecord)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update allocaterecord set ");
            if (allocaterecord.riderId != null)
                part1.Append("riderId = @riderId,");
            if (allocaterecord.foodId != null)
                part1.Append("foodId = @foodId,");
            if (allocaterecord.amount != null)
                part1.Append("amount = @amount,");
            if (allocaterecord.targetRiderId != null)
                part1.Append("targetRiderId = @targetRiderId,");
            if (allocaterecord.status != null)
                part1.Append("status = @status,");
            if (allocaterecord.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (allocaterecord.timestamp != null)
                part1.Append("timestamp = @timestamp,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="AllocateRecord"></param>
        /// <returns></returns>
        public int Add(AllocateRecord model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="AllocateRecord"></param>
        /// <returns></returns>
        public void Update(AllocateRecord model)
        {
            CacheHelper.LockCache("AllocateRecord");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            CacheHelper.ReleaseLock("AllocateRecord");
        }
        /// <summary>
        /// del
        /// </summary>
        /// <param name="AllocateRecord"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            AllocateRecord model = new AllocateRecord();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }

        public List<AllocateRecord> GetByRiderId(int riderId) {
            return CacheHelper.GetByCondition<AllocateRecord>("allocateRecord"," status=0 and riderId = "+riderId);
        }
    }
}
