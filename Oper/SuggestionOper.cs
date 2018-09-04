using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class SuggestionOper : SingleTon<SuggestionOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="suggestion"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(Suggestion suggestion)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (suggestion.id != null)
                dict.Add("@id", suggestion.id.ToString());
            if (suggestion.userId != null)
                dict.Add("@userId", suggestion.userId.ToString());
            if (suggestion.content != null)
                dict.Add("@content", suggestion.content.ToString());
            if (suggestion.sTime != null)
                dict.Add("@sTime", suggestion.sTime.ToString());
            if (suggestion.isDeleted != null)
                dict.Add("@isDeleted", suggestion.isDeleted.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="suggestion"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(Suggestion suggestion)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (suggestion.userId != null)
            {
                part1.Append("userId,");
                part2.Append("@userId,");
            }
            if (suggestion.content != null)
            {
                part1.Append("content,");
                part2.Append("@content,");
            }
            if (suggestion.sTime != null)
            {
                part1.Append("sTime,");
                part2.Append("@sTime,");
            }
            if (suggestion.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into suggestion(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
                 /// 更新
                 /// </summary>
                 /// <param name="suggestion"></param>
                 /// <returns>是否成功</returns>
        public string GetUpdateStr(Suggestion suggestion)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update suggestion set ");
            if (suggestion.userId != null)
                part1.Append("userId = @userId,");
            if (suggestion.content != null)
                part1.Append("content = @content,");
            if (suggestion.sTime != null)
                part1.Append("sTime = @sTime,");
            if (suggestion.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="Suggestion"></param>
        /// <returns></returns>
        public int Add(Suggestion model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="Suggestion"></param>
        /// <returns></returns>
        public void Update(Suggestion model)
        {
            //CacheHelper.LockCache("Suggestion");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("Suggestion");
        }
        /// <summary>
        /// del
        /// </summary>
        /// <param name="Suggestion"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            Suggestion model = new Suggestion();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }
    }
}
