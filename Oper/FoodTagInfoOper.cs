
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class FoodTagInfoOper : SingleTon<FoodTagInfoOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="foodtaginfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(FoodTagInfo foodtaginfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (foodtaginfo.id != null)
                dict.Add("@id", foodtaginfo.id.ToString());
            if (foodtaginfo.foodTagName != null)
                dict.Add("@foodTagName", foodtaginfo.foodTagName.ToString());
            if (foodtaginfo.isDeleted != null)
                dict.Add("@isDeleted", foodtaginfo.isDeleted.ToString());
            if (foodtaginfo.isWeek != null)
                dict.Add("@isWeek", foodtaginfo.isWeek.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="foodtaginfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(FoodTagInfo foodtaginfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (foodtaginfo.foodTagName != null)
            {
                part1.Append("foodTagName,");
                part2.Append("@foodTagName,");
            }
            if (foodtaginfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (foodtaginfo.isWeek != null)
            {
                part1.Append("isWeek,");
                part2.Append("@isWeek,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into foodtaginfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="foodtaginfo"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(FoodTagInfo foodtaginfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update foodtaginfo set ");
            if (foodtaginfo.foodTagName != null)
                part1.Append("foodTagName = @foodTagName,");
            if (foodtaginfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (foodtaginfo.isWeek != null)
                part1.Append("isWeek = @isWeek,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="FoodTagInfo"></param>
        /// <returns></returns>
        public int Add(FoodTagInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="FoodTagInfo"></param>
        /// <returns></returns>
        public void Update(FoodTagInfo model)
        {
            //CacheHelper.LockCache("FoodTagInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("FoodTagInfo");
        }

        /// <summary>
        /// 获取菜品种类id，name字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetFoodTypeDict()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var list = CacheHelper.GetByCondition<FoodTagInfo>("foodTaginfo"," isdeleted=0");
            foreach (var item in list)
            {
                dict.Add(item.id, item.foodTagName);
            }
            //List<FoodTagInfo> list = (List<FoodTagInfo>)CacheHelper.Get<FoodTagInfo>("FoodTagInfo", "Ta_FoodTagInfo");
            //list = list.Where(p => p.isDeleted == false).ToList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    dict.Add(list[i].id, list[i].foodTagName);
            //}
            return dict;
        }

        /// <summary>
        /// 根据是不是周标签 获取foodTag的字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetFoodTypeDictIsWeek(bool isWeek)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var condition=0;
            if (isWeek)
                condition = 1;
            var list = CacheHelper.GetByCondition<FoodTagInfo>("foodTaginfo", " isdeleted=0  and isweek="+condition);
            foreach (var item in list)
            {
                dict.Add(item.id, item.foodTagName);
            }

            //List<FoodTagInfo> list = (List<FoodTagInfo>)CacheHelper.Get<FoodTagInfo>("FoodTagInfo", "Ta_FoodTagInfo");
            //list = list.Where(p => p.isDeleted == false && p.isWeek == isWeek).ToList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    dict.Add(list[i].id, list[i].foodTagName);
            //}
            return dict;
        }
    }
}
