using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System.Linq;
using System;
using takeAwayWebApi.Models.Response;
using takeAwayWebApi.Models;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class RiderInfoOper : SingleTon<RiderInfoOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="riderinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(RiderInfo riderinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (riderinfo.id != null)
                dict.Add("@id", riderinfo.id.ToString());
            if (riderinfo.riderAccount != null)
                dict.Add("@riderAccount", riderinfo.riderAccount.ToString());
            if (riderinfo.riderPwd != null)
                dict.Add("@riderPwd", riderinfo.riderPwd.ToString());
            if (riderinfo.riderType != null)
                dict.Add("@riderType", riderinfo.riderType.ToString());
            if (riderinfo.riderStatus != null)
                dict.Add("@riderStatus", riderinfo.riderStatus.ToString());
            if (riderinfo.riderAreaId != null)
                dict.Add("@riderAreaId", riderinfo.riderAreaId.ToString());
            if (riderinfo.isDeleted != null)
                dict.Add("@isDeleted", riderinfo.isDeleted.ToString());
            if (riderinfo.riderNo != null)
                dict.Add("@riderNo", riderinfo.riderNo.ToString());
            if (riderinfo.sexType != null)
                dict.Add("@sexType", riderinfo.sexType.ToString());
            if (riderinfo.name != null)
                dict.Add("@name", riderinfo.name.ToString());
            if (riderinfo.phone != null)
                dict.Add("@phone", riderinfo.phone.ToString());
            if (riderinfo.deviceToken != null)
                dict.Add("@deviceToken", riderinfo.deviceToken.ToString());
            if (riderinfo.avgStars != null)
                dict.Add("@avgStars", riderinfo.avgStars.ToString());
            if (riderinfo.starCount != null)
                dict.Add("@starCount", riderinfo.starCount.ToString());
            if (riderinfo.sendCount != null)
                dict.Add("@sendCount", riderinfo.sendCount.ToString());
            if (riderinfo.lat != null)
                dict.Add("@lat", riderinfo.lat.ToString());
            if (riderinfo.lng != null)
                dict.Add("@lng", riderinfo.lng.ToString());
            if (riderinfo.mapAddress != null)
                dict.Add("@mapAddress", riderinfo.mapAddress.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="riderinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(RiderInfo riderinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (riderinfo.riderAccount != null)
            {
                part1.Append("riderAccount,");
                part2.Append("@riderAccount,");
            }
            if (riderinfo.riderPwd != null)
            {
                part1.Append("riderPwd,");
                part2.Append("@riderPwd,");
            }
            if (riderinfo.riderType != null)
            {
                part1.Append("riderType,");
                part2.Append("@riderType,");
            }
            if (riderinfo.riderStatus != null)
            {
                part1.Append("riderStatus,");
                part2.Append("@riderStatus,");
            }
            if (riderinfo.riderAreaId != null)
            {
                part1.Append("riderAreaId,");
                part2.Append("@riderAreaId,");
            }
            if (riderinfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (riderinfo.riderNo != null)
            {
                part1.Append("riderNo,");
                part2.Append("@riderNo,");
            }
            if (riderinfo.sexType != null)
            {
                part1.Append("sexType,");
                part2.Append("@sexType,");
            }
            if (riderinfo.name != null)
            {
                part1.Append("name,");
                part2.Append("@name,");
            }
            if (riderinfo.phone != null)
            {
                part1.Append("phone,");
                part2.Append("@phone,");
            }
            if (riderinfo.deviceToken != null)
            {
                part1.Append("deviceToken,");
                part2.Append("@deviceToken,");
            }
            if (riderinfo.avgStars != null)
            {
                part1.Append("avgStars,");
                part2.Append("@avgStars,");
            }
            if (riderinfo.starCount != null)
            {
                part1.Append("starCount,");
                part2.Append("@starCount,");
            }
            if (riderinfo.sendCount != null)
            {
                part1.Append("sendCount,");
                part2.Append("@sendCount,");
            }
            if (riderinfo.lat != null)
            {
                part1.Append("lat,");
                part2.Append("@lat,");
            }
            if (riderinfo.lng != null)
            {
                part1.Append("lng,");
                part2.Append("@lng,");
            }
            if (riderinfo.mapAddress != null)
            {
                part1.Append("mapAddress,");
                part2.Append("@mapAddress,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into riderinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
                 /// 更新
                 /// </summary>
                 /// <param name="riderinfo"></param>
                 /// <returns>是否成功</returns>
        public string GetUpdateStr(RiderInfo riderinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update riderinfo set ");
            if (riderinfo.riderAccount != null)
                part1.Append("riderAccount = @riderAccount,");
            if (riderinfo.riderPwd != null)
                part1.Append("riderPwd = @riderPwd,");
            if (riderinfo.riderType != null)
                part1.Append("riderType = @riderType,");
            if (riderinfo.riderStatus != null)
                part1.Append("riderStatus = @riderStatus,");
            if (riderinfo.riderAreaId != null)
                part1.Append("riderAreaId = @riderAreaId,");
            if (riderinfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (riderinfo.riderNo != null)
                part1.Append("riderNo = @riderNo,");
            if (riderinfo.sexType != null)
                part1.Append("sexType = @sexType,");
            if (riderinfo.name != null)
                part1.Append("name = @name,");
            if (riderinfo.phone != null)
                part1.Append("phone = @phone,");
            if (riderinfo.deviceToken != null)
                part1.Append("deviceToken = @deviceToken,");
            if (riderinfo.avgStars != null)
                part1.Append("avgStars = @avgStars,");
            if (riderinfo.starCount != null)
                part1.Append("starCount = @starCount,");
            if (riderinfo.sendCount != null)
                part1.Append("sendCount = @sendCount,");
            if (riderinfo.lat != null)
                part1.Append("lat = @lat,");
            if (riderinfo.lng != null)
                part1.Append("lng = @lng,");
            if (riderinfo.mapAddress != null)
                part1.Append("mapAddress = @mapAddress,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="RiderInfo"></param>
        /// <returns></returns>
        public int Add(RiderInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="RiderInfo"></param>
        /// <returns></returns>
        public void Update(RiderInfo model)
        {
            //CacheHelper.LockCache("RiderInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("RiderInfo");
        }
        /// <summary>
        /// del
        /// </summary>
        /// <param name="RiderInfo"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            RiderInfo model = new RiderInfo();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }

        public RiderInfo GetById(int riderId)
        {
            return CacheHelper.GetById<RiderInfo>("RiderInfo", riderId);
        }

        /// <summary>
        /// 根据骑手id获取骑手视图
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public RiderView GetViewById(int riderId)
        {
            return CacheHelper.GetById<RiderView>("RiderView", riderId);
            //var list = GetView();
            //return list.Where(p => p.id == riderId).First();
        }

        /// <summary>
        /// 根据骑手账号获取骑手信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<RiderInfo> GetByAccount(string account)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("@account", account);
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", " riderAccount=@account", dict);
            return list;
        }

        /// <summary>
        /// 根据骑手账号获取骑手视图
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public RiderView GetViewByAccount(string account)
        {
            var list = CacheHelper.GetByCondition<RiderView>("RiderView", " riderAccount = '" + account + "'");
            if (list.Count == 0)
                return null;
            return list.First();
        }

        /// <summary>
        /// 骑手评价累计
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public bool AddCommentRecord(int riderId, int stars)
        {
            var rider = CacheHelper.GetById<RiderInfo>("RiderInfo", riderId);

            var avgStars = rider.avgStars ?? 5;
            var starCount = rider.starCount ?? 1;
            avgStars = (avgStars * starCount + stars) / (starCount + 1);
            starCount += 1;
            rider.avgStars = avgStars;
            rider.starCount = starCount;
            try
            {
                Update(rider);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 增加骑手配送次数
        /// </summary>
        /// <param name="riderId"></param>
        public void AddRiderSendCount(int riderId)
        {
            string str = "update RiderInfo set sendCount=sendCount+1 where id=" + riderId;
            SqlHelperHere.ExcuteNon(str);

            //var rider = GetById(riderId);
            ////rider.sendCount += 1;
            //var riderHere = new RiderInfo();
            //riderHere.id = riderId;
            //riderHere.sendCount = rider.sendCount + 1;
            //Update(riderHere);
        }

        /// <summary>
        /// 获取区域内的骑手的设备号,非自提点
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<string> GetRiderDeviceTokenInArea(int areaId)
        {
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", " riderareaId = " + areaId + " and isdeleted=0 and riderStatus=1 and riderType!=2 ");
            return list.Select(p => p.deviceToken).ToList();
        }

        /// <summary>
        /// 计算骑手和目的地的距离
        /// </summary>
        /// <param name="riderId"></param>
        /// <param name="userAddressId"></param>
        /// <returns></returns>
        public decimal GetDistance(int riderId, int userAddressId,string lat,string lng)
        {

            var list = CacheHelper.GetByCondition<UserAddressView>("UserAddressView", " id=" + userAddressId);
            var ua = list.First();
            var lat1 = 0d;
            var lng1 = 0d;
            var lat2 = 0d;
            var lng2 = 0d;

            if (ua.riderType != 2)
            {
                RiderPosition rp = CacheHelper.GetRiderPosition(riderId);
                if (rp == null)
                    return 0;
                lat1 = Convert.ToDouble(rp.lat);
                lng1 = Convert.ToDouble(rp.lng);
                lat2 = Convert.ToDouble(ua.lat);
                lng2 = Convert.ToDouble(ua.lng);
            }
            else
            {
                lat1 = Convert.ToDouble(ua.lat);
                lng1 = Convert.ToDouble(ua.lng);
                lat2 = Convert.ToDouble(lat);
                lng2 = Convert.ToDouble(lng);
            }


        
            var r = DistanceHelperHere.Distance(lat1, lng1, lat2, lng2);
            var r2 = r.ToString("#0.0");
            return Convert.ToDecimal(r2);
        }

        /// <summary>
        /// 只要骑手
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="thisRiderId"></param>
        /// <returns></returns>
        public List<riderId_name2> GetRidersInArea(int areaId, int thisRiderId)
        {
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", "  isdeleted=0 and  riderAreaId = " + areaId + " and id!=" + thisRiderId);
            List<riderId_name2> r = new List<riderId_name2>();
            foreach (var item in list)
            {
                riderId_name2 temp = new riderId_name2(item);
                r.Add(temp);
            }
            return r;
        }

        public List<RiderPosition> GetRiderPositionInArea(int areaId, int thisRiderId)
        {
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", " isdeleted=0 and  riderAreaId = " + areaId + " and id!=" + thisRiderId);
            List<RiderPosition> r = new List<RiderPosition>();
            foreach (var item in list)
            {
                var rp = CacheHelper.GetRiderPosition(item.id);
                rp.riderType = (int)item.riderType;
                if (rp != null)
                    r.Add(rp);
            }
            return r;
        }

        /// <summary>
        /// 骑手的所属区域设为0
        /// </summary>
        /// <param name="riderId"></param>
        public void RemoveArea(int riderId)
        {
            RiderInfo rider = new RiderInfo();
            rider.id = riderId;
            rider.riderAreaId = 0;
            Update(rider);
        }

        /// <summary>
        /// 新增或更新操作时，判断骑手账号是否存在，id传0表示新账号
        /// </summary>
        /// <param name="account"></param>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public bool IsHaveAccount(string account, int riderId)
        {
            var list = CacheHelper.GetDistinctCount<RiderInfo>("riderinfo", " riderAccount='" + account + "' and isdeleted=0 and id!=" + riderId);

            if (list.Count < 1)
                return false;
            return true;
        }

        /// <summary>
        /// 根据区域id来获取区域中的骑手们
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<RiderInfo> GetByAreaId(int areaId)
        {
            return CacheHelper.GetByCondition<RiderInfo>("riderinfo", " isdeleted=0 and riderAreaId=" + areaId);
        }

        /// <summary>
        /// 根据区域id来获取区域中在线的骑手们
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<RiderInfo> GetOnlineByAreaId(int areaId)
        {
            return CacheHelper.GetByCondition<RiderInfo>("riderinfo", " riderType!=2 and riderStatus=1 and isdeleted=0 and riderAreaId=" + areaId);
        }

        public Dictionary<int, string> GetRiderDict()
        {
            var list = CacheHelper.GetByCondition<RiderInfo>("riderinfo", " isdeleted=0 ");
            Dictionary<int, string> dict = new Dictionary<int, string>();
            foreach (var item in list)
            {
                dict.Add(item.id, item.name);
            }
            return dict;
        }

    }
}
