using System.Text;
using System.Linq;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common.Helper;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class UserOpenInfoOper : SingleTon<UserOpenInfoOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="useropeninfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(UserOpenInfo useropeninfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (useropeninfo.id != null)
                dict.Add("@id", useropeninfo.id.ToString());
            if (useropeninfo.userId != null)
                dict.Add("@userId", useropeninfo.userId.ToString());
            if (useropeninfo.lat != null)
                dict.Add("@lat", useropeninfo.lat.ToString());
            if (useropeninfo.lng != null)
                dict.Add("@lng", useropeninfo.lng.ToString());
            if (useropeninfo.openTime != null)
                dict.Add("@openTime", useropeninfo.openTime.ToString());
            if (useropeninfo.closeTime != null)
                dict.Add("@closeTime", useropeninfo.closeTime.ToString());
            if (useropeninfo.address != null)
                dict.Add("@address", useropeninfo.address.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="useropeninfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(UserOpenInfo useropeninfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (useropeninfo.userId != null)
            {
                part1.Append("userId,");
                part2.Append("@userId,");
            }
            if (useropeninfo.lat != null)
            {
                part1.Append("lat,");
                part2.Append("@lat,");
            }
            if (useropeninfo.lng != null)
            {
                part1.Append("lng,");
                part2.Append("@lng,");
            }
            if (useropeninfo.openTime != null)
            {
                part1.Append("openTime,");
                part2.Append("@openTime,");
            }
            if (useropeninfo.closeTime != null)
            {
                part1.Append("closeTime,");
                part2.Append("@closeTime,");
            }
            if (useropeninfo.address != null)
            {
                part1.Append("address,");
                part2.Append("@address,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into useropeninfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
                 /// 更新
                 /// </summary>
                 /// <param name="useropeninfo"></param>
                 /// <returns>是否成功</returns>
        public string GetUpdateStr(UserOpenInfo useropeninfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update useropeninfo set ");
            if (useropeninfo.userId != null)
                part1.Append("userId = @userId,");
            if (useropeninfo.lat != null)
                part1.Append("lat = @lat,");
            if (useropeninfo.lng != null)
                part1.Append("lng = @lng,");
            if (useropeninfo.openTime != null)
                part1.Append("openTime = @openTime,");
            if (useropeninfo.closeTime != null)
                part1.Append("closeTime = @closeTime,");
            if (useropeninfo.address != null)
                part1.Append("address = @address,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="UserOpenInfo"></param>
        /// <returns></returns>
        public int Add(UserOpenInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="UserOpenInfo"></param>
        /// <returns></returns>
        public void Update(UserOpenInfo model)
        {
            //CacheHelper.LockCache("UserOpenInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("UserOpenInfo");
        }

        /// <summary>
        /// 获取某用户使用app的最后一条记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserOpenInfo GetLastByUserId(int userId)
        {
            string str = "select top 1 * from UserOpenInfo where userid=@userId order by id desc";
            var dict = new Dictionary<string, string>();
            dict.Add("@userId", userId.ToString());
            var dt = SqlHelperHere.ExecuteGetDt(str, dict);
            return dt.ConvertToList<UserOpenInfo>().FirstOrDefault();
        }
    }
}
