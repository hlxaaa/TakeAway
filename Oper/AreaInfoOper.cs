
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Response;
using System.Linq;
using takeAwayWebApi.Models;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class AreaInfoOper : SingleTon<AreaInfoOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="areainfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(AreaInfo areainfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (areainfo.id != null)
                dict.Add("@id", areainfo.id.ToString());
            if (areainfo.lat1 != null)
                dict.Add("@lat1", areainfo.lat1.ToString());
            if (areainfo.lng1 != null)
                dict.Add("@lng1", areainfo.lng1.ToString());
            if (areainfo.lat2 != null)
                dict.Add("@lat2", areainfo.lat2.ToString());
            if (areainfo.lng2 != null)
                dict.Add("@lng2", areainfo.lng2.ToString());
            if (areainfo.x3 != null)
                dict.Add("@x3", areainfo.x3.ToString());
            if (areainfo.y3 != null)
                dict.Add("@y3", areainfo.y3.ToString());
            if (areainfo.x4 != null)
                dict.Add("@x4", areainfo.x4.ToString());
            if (areainfo.y4 != null)
                dict.Add("@y4", areainfo.y4.ToString());
            if (areainfo.areaName != null)
                dict.Add("@areaName", areainfo.areaName.ToString());
            if (areainfo.isDeleted != null)
                dict.Add("@isDeleted", areainfo.isDeleted.ToString());
            if (areainfo.areaNo != null)
                dict.Add("@areaNo", areainfo.areaNo.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="areainfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(AreaInfo areainfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (areainfo.lat1 != null)
            {
                part1.Append("lat1,");
                part2.Append("@lat1,");
            }
            if (areainfo.lng1 != null)
            {
                part1.Append("lng1,");
                part2.Append("@lng1,");
            }
            if (areainfo.lat2 != null)
            {
                part1.Append("lat2,");
                part2.Append("@lat2,");
            }
            if (areainfo.lng2 != null)
            {
                part1.Append("lng2,");
                part2.Append("@lng2,");
            }
            if (areainfo.x3 != null)
            {
                part1.Append("x3,");
                part2.Append("@x3,");
            }
            if (areainfo.y3 != null)
            {
                part1.Append("y3,");
                part2.Append("@y3,");
            }
            if (areainfo.x4 != null)
            {
                part1.Append("x4,");
                part2.Append("@x4,");
            }
            if (areainfo.y4 != null)
            {
                part1.Append("y4,");
                part2.Append("@y4,");
            }
            if (areainfo.areaName != null)
            {
                part1.Append("areaName,");
                part2.Append("@areaName,");
            }
            if (areainfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (areainfo.areaNo != null)
            {
                part1.Append("areaNo,");
                part2.Append("@areaNo,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into areainfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="areainfo"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(AreaInfo areainfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update areainfo set ");
            if (areainfo.lat1 != null)
                part1.Append("lat1 = @lat1,");
            if (areainfo.lng1 != null)
                part1.Append("lng1 = @lng1,");
            if (areainfo.lat2 != null)
                part1.Append("lat2 = @lat2,");
            if (areainfo.lng2 != null)
                part1.Append("lng2 = @lng2,");
            if (areainfo.x3 != null)
                part1.Append("x3 = @x3,");
            if (areainfo.y3 != null)
                part1.Append("y3 = @y3,");
            if (areainfo.x4 != null)
                part1.Append("x4 = @x4,");
            if (areainfo.y4 != null)
                part1.Append("y4 = @y4,");
            if (areainfo.areaName != null)
                part1.Append("areaName = @areaName,");
            if (areainfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (areainfo.areaNo != null)
                part1.Append("areaNo = @areaNo,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="AreaInfo"></param>
        /// <returns></returns>
        public int Add(AreaInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="AreaInfo"></param>
        /// <returns></returns>
        public void Update(AreaInfo model)
        {
            //CacheHelper.LockCache("AreaInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("AreaInfo");
        }

        /// <summary>
        /// 获取没删除的
        /// </summary>
        /// <returns></returns>
        public List<AreaInfo> GetExist() {
            return CacheHelper.GetByCondition<AreaInfo>("areainfo"," isdeleted=0");
        }

        /// <summary>
        /// 获取区域id和名称的集合
        /// </summary>
        /// <returns></returns>
        public List<AreaId_Name> GetAreaList() {
            List<AreaId_Name> r = new List<AreaId_Name>();
            var list = GetExist();
            foreach (var item in list)
            {
                AreaId_Name area = new AreaId_Name(item);
                r.Add(area);
            }
            return r;
        }

        /// <summary>
        /// 根据经纬度，获得目前所在位置(用Location)
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public List<AreaId_Name> GetAreaByLocation(decimal lat, decimal lng)
        {
            List<AreaId_Name> r = new List<AreaId_Name>();
            //var list = Get().Where(p => p.lat1 > lat && p.lat2 < lat && p.lng1 < lng && p.lng2 > lng).ToList();
            var list = CacheHelper.GetByCondition<AreaInfo>("areainfo", " lat1>" + lat + " and lat2<" + lat + " and lng1<" + lng + " and lng2>" + lng + " and isDeleted=0");
            foreach (var item in list)
            {
                AreaId_Name area = new AreaId_Name(item);
                r.Add(area);
            }
            return r;
        }

        /// <summary>
        /// 获取区域中的所有骑手
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<RiderInfo> GetRiderByAreaId(int areaId)
        {
            return CacheHelper.GetByCondition<RiderInfo>("RiderInfo", " isdeleted=0 and riderAreaId=" + areaId);

            //List<RiderInfo> list = (List<RiderInfo>)CacheHelper.Get<RiderInfo>("RiderInfo", "Ta_RiderInfo");
            //return list.Where(p => p.riderAreaId == areaId).ToList();
        }

        public Dictionary<int, string> GetAreaDict()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var list = CacheHelper.GetByCondition<AreaInfo>("areainfo"," isdeleted=0");

            foreach (var item in list)
            {
                dict.Add(item.id, item.areaName);
            }
            //List<AreaInfo> list = (List<AreaInfo>)CacheHelper.Get<AreaInfo>("AreaInfo", "Ta_AreaInfo");
            //list = list.Where(p => p.isDeleted == false).ToList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    dict.Add(list[i].id, list[i].areaName);
            //}
            return dict;
        }

        public AreaInfo GetById(int areaId) {
            return CacheHelper.GetById<AreaInfo>("areaInfo", areaId);
        }

        //_______________________________________________

        ///// <summary>
        ///// 根据经纬度，获得目前所在位置(用Location)
        ///// </summary>
        ///// <param name="lat"></param>
        ///// <param name="lng"></param>
        //public AreaInfo GetAreaByPosition(decimal lat,decimal lng) {
        //   return GetExist().Where(p => p.lng1 < lng && p.lng2 > lng && p.lat1 > lat && p.lat2 < lat).FirstOrDefault();
        //}


    }
}
