
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
    public partial class PlatformOper : SingleTon<PlatformOper>
    {        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(Platform platform)
        {
             Dictionary<string, string> dict = new Dictionary<string, string>();
            if (platform.id != null)
                dict.Add("@id",platform.id.ToString());
            if (platform.account != null)
                dict.Add("@account",platform.account.ToString());
            if (platform.pwd != null)
                dict.Add("@pwd",platform.pwd.ToString());
            if (platform.isDeleted != null)
                dict.Add("@isDeleted",platform.isDeleted.ToString());
            if (platform.level != null)
                dict.Add("@level",platform.level.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(Platform platform)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();
            
            if (platform.account != null)
            {
                    part1.Append("account,");
                    part2.Append("@account,");
            }
            if (platform.pwd != null)
            {
                    part1.Append("pwd,");
                    part2.Append("@pwd,");
            }
            if (platform.isDeleted != null)
            {
                    part1.Append("isDeleted,");
                    part2.Append("@isDeleted,");
            }
            if (platform.level != null)
            {
                    part1.Append("level,");
                    part2.Append("@level,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into platform(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length-1)).Append(")");
            return sql.ToString();
        }        
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(Platform platform)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update platform set ");
            if (platform.account != null)
                part1.Append("account = @account,");
            if (platform.pwd != null)
                part1.Append("pwd = @pwd,");
            if (platform.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (platform.level != null)
                part1.Append("level = @level,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        public List<Platform> GetByLv(string lv)
        {
            return CacheHelper.GetByCondition<Platform>("platform"," isdeleted=0 and  level<="+lv);
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public int Add(Platform model)
        {
            var str = GetInsertStr(model)+" select @@identity";
              var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str,dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public void Update(Platform model)
        {
            //CacheHelper.LockCache("Platform");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("Platform");
        }
        /// <summary>
        /// del
        /// </summary>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            Platform model = new Platform();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }
        /// <summary>
        /// getById
        /// </summary>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public Platform GetById(int id)
        {
            return CacheHelper.GetById<Platform>("platform", id);
            //return Get().Where(p => p.id == id).First();

        }

        public Platform GetByAccount(string account) {
            var dict =new Dictionary<string, string>();
            dict.Add("@account", account);
            var list = CacheHelper.GetByCondition<Platform>("Platform", " account=@account",dict);
            return list.FirstOrDefault();
        }
    }
}
