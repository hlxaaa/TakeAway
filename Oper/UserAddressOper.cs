using Common;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using takeAwayWebApi.Helper;

namespace DbOpertion.DBoperation
{
    public partial class UserAddressOper : SingleTon<UserAddressOper>
    {
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(UserAddress useraddress)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (useraddress.id != null)
                dict.Add("@id", useraddress.id.ToString());
            if (useraddress.userId != null)
                dict.Add("@userId", useraddress.userId.ToString());
            if (useraddress.name != null)
                dict.Add("@name", useraddress.name.ToString());
            if (useraddress.phone != null)
                dict.Add("@phone", useraddress.phone.ToString());
            if (useraddress.mapAddress != null)
                dict.Add("@mapAddress", useraddress.mapAddress.ToString());
            if (useraddress.lat != null)
                dict.Add("@lat", useraddress.lat.ToString());
            if (useraddress.lng != null)
                dict.Add("@lng", useraddress.lng.ToString());
            if (useraddress.detail != null)
                dict.Add("@detail", useraddress.detail.ToString());
            if (useraddress.isDeleted != null)
                dict.Add("@isDeleted", useraddress.isDeleted.ToString());
            if (useraddress.isRecently != null)
                dict.Add("@isRecently", useraddress.isRecently.ToString());
            if (useraddress.riderId != null)
                dict.Add("@riderId", useraddress.riderId.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(UserAddress useraddress)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (useraddress.userId != null)
            {
                part1.Append("userId,");
                part2.Append("@userId,");
            }
            if (useraddress.name != null)
            {
                part1.Append("name,");
                part2.Append("@name,");
            }
            if (useraddress.phone != null)
            {
                part1.Append("phone,");
                part2.Append("@phone,");
            }
            if (useraddress.mapAddress != null)
            {
                part1.Append("mapAddress,");
                part2.Append("@mapAddress,");
            }
            if (useraddress.lat != null)
            {
                part1.Append("lat,");
                part2.Append("@lat,");
            }
            if (useraddress.lng != null)
            {
                part1.Append("lng,");
                part2.Append("@lng,");
            }
            if (useraddress.detail != null)
            {
                part1.Append("detail,");
                part2.Append("@detail,");
            }
            if (useraddress.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (useraddress.isRecently != null)
            {
                part1.Append("isRecently,");
                part2.Append("@isRecently,");
            }
            if (useraddress.riderId != null)
            {
                part1.Append("riderId,");
                part2.Append("@riderId,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into useraddress(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(UserAddress useraddress)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update useraddress set ");
            if (useraddress.userId != null)
                part1.Append("userId = @userId,");
            if (useraddress.name != null)
                part1.Append("name = @name,");
            if (useraddress.phone != null)
                part1.Append("phone = @phone,");
            if (useraddress.mapAddress != null)
                part1.Append("mapAddress = @mapAddress,");
            if (useraddress.lat != null)
                part1.Append("lat = @lat,");
            if (useraddress.lng != null)
                part1.Append("lng = @lng,");
            if (useraddress.detail != null)
                part1.Append("detail = @detail,");
            if (useraddress.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (useraddress.isRecently != null)
                part1.Append("isRecently = @isRecently,");
            if (useraddress.riderId != null)
                part1.Append("riderId = @riderId,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="UserAddress"></param>
        /// <returns></returns>
        public int Add(UserAddress model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="UserAddress"></param>
        /// <returns></returns>
        public void Update(UserAddress model)
        {
            //CacheHelper.LockCache("UserAddress");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("UserAddress");
        }

        public void Delete(int id)
        {
            UserAddress userAddr = new UserAddress();
            userAddr.id = id;
            userAddr.isDeleted = true;
            Update(userAddr);
        }

        /// <summary>
        /// 获取用户地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserAddress> GetAddrByUserId(int userId)
        {
            return CacheHelper.GetByCondition<UserAddress>("UserAddress", " isdeleted=0 and userid=" + userId);
            //return GetExist().Where(p=>p.userId == userId).ToList();
        }

        /// <summary>
        /// 获取用户最近使用的地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserAddress> GetRecentlyAddr(int userId)
        {
            return CacheHelper.GetByCondition<UserAddress>("useraddress", " isDeleted=0 and isRecently=1 and userid=" + userId);
        }

        /// <summary>
        /// 将某个地址设为常用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addressId"></param>
        public void ChangeRecentlyAddr(int userId, int addressId)
        {
            var list = CacheHelper.GetByCondition<UserAddress>("UserAddress", " userId=" + userId);
            var list2 = list.Where(p => p.id != addressId).ToList();

            foreach (var item in list2)
            {
                item.isRecently = false;
                Update(item);
            }
            var addr = list.Where(p => p.id == addressId).First();
            addr.isRecently = true;
            Update(addr);
        }

        public UserAddress GetById(int uaId)
        {
            return CacheHelper.GetById<UserAddress>("useraddress", uaId);
        }

        /// <summary>
        /// 根据自提点id获取自提点的地址信息
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public UserAddress GetByRiderId(int riderId)
        {
            var list = CacheHelper.GetByCondition<UserAddress>("UserAddress", " riderid=" + riderId);
            return list.FirstOrDefault();
        }

        public UserAddressForOrderRes GetUaResById(int addressId)
        {
            var view = CacheHelper.GetById<UserAddressView>("UserAddressView", addressId);
            return new UserAddressForOrderRes(view);
        }

        public UserAddressForOrderRes GetUaResByUAView(UserAddressView ua)
        {
            //var view = CacheHelper.GetById<UserAddressView>("UserAddressView", addressId);
            return new UserAddressForOrderRes(ua);
        }

        public UserAddressView GetUAViewById(int addressId)
        {
            return CacheHelper.GetById<UserAddressView>("UserAddressView", addressId);
        }

    }
}
