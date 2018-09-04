using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class txyInfoOper : SingleTon<txyInfoOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="txyinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(txyInfo txyinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (txyinfo.id != null)
                dict.Add("@id", txyinfo.id.ToString());
            if (txyinfo.content != null)
                dict.Add("@content", txyinfo.content.ToString());
            if (txyinfo.ttime != null)
                dict.Add("@ttime", txyinfo.ttime.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="txyinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(txyInfo txyinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (txyinfo.content != null)
            {
                part1.Append("content,");
                part2.Append("@content,");
            }
            if (txyinfo.ttime != null)
            {
                part1.Append("ttime,");
                part2.Append("@ttime,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into txyinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        
        /// <summary>
                 /// 更新
                 /// </summary>
                 /// <param name="txyinfo"></param>
                 /// <returns>是否成功</returns>
        public string GetUpdateStr(txyInfo txyinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update txyinfo set ");
            if (txyinfo.content != null)
                part1.Append("content = @content,");
            if (txyinfo.ttime != null)
                part1.Append("ttime = @ttime,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }
        /// <summary>
        /// add
        /// </summary>
        /// <param name="txyInfo"></param>
        /// <returns></returns>
        public int Add(txyInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="txyInfo"></param>
        /// <returns></returns>
        public void Update(txyInfo model)
        {
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
        }
    }
}
