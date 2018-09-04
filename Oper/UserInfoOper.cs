using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class UserInfoOper : SingleTon<UserInfoOper>
    {
        public List<UserInfo> GetByPhone(string phone)
        {
            //List<UserInfo> list = (List<UserInfo>)CacheHelper.Get<UserInfo>("UserInfo","Ta_UserInfo");
            var dict = new Dictionary<string, string>();
            dict.Add("@phone", phone);
            return CacheHelper.GetByCondition<UserInfo>("userinfo", " isdeleted=0 and userphone=@phone", dict);
            //return GetExist().Where(p => p.userPhone == phone).ToList();
        }

        public List<UserInfo> GetByWechat(string wechat)
        {
            //List<UserInfo> list = (List<UserInfo>)CacheHelper.Get<UserInfo>("UserInfo", "Ta_UserInfo");
            var dict = new Dictionary<string, string>();
            dict.Add("@wechat", wechat);
            return CacheHelper.GetByCondition<UserInfo>("userinfo", " isdeleted=0 and wechat=@wechat", dict);
            //return GetExist().Where(p => p.userPhone == phone).ToList();
            //return GetExist().Where(p => p.wechat == wechat ).ToList();
        }

        public List<UserInfo> GetByQQ(string qq)
        {
            //List<UserInfo> list = (List<UserInfo>)CacheHelper.Get<UserInfo>("UserInfo", "Ta_UserInfo");
            var dict = new Dictionary<string, string>();
            dict.Add("@qq", qq);
            return CacheHelper.GetByCondition<UserInfo>("userinfo", " isdeleted=0 and qq=@qq", dict);

            //return GetExist().Where(p => p.qq == qq).ToList();
        }

        public UserInfo GetById(int id)
        {
            return CacheHelper.GetById<UserInfo>("UserInfo", id);
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public int Add(UserInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public void Update(UserInfo model)
        {
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
        }

        public void Delete(int userId)
        {
            UserInfo user = new UserInfo();
            user.id = userId;
            user.isDeleted = true;
            Update(user);
        }

        /// 获取参数
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(UserInfo userinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (userinfo.id != null)
                dict.Add("@id", userinfo.id.ToString());
            if (userinfo.userName != null)
                dict.Add("@userName", userinfo.userName.ToString());
            if (userinfo.userBalance != null)
                dict.Add("@userBalance", userinfo.userBalance.ToString());
            if (userinfo.wechat != null)
                dict.Add("@wechat", userinfo.wechat.ToString());
            if (userinfo.qq != null)
                dict.Add("@qq", userinfo.qq.ToString());
            if (userinfo.userPwd != null)
                dict.Add("@userPwd", userinfo.userPwd.ToString());
            if (userinfo.userPhone != null)
                dict.Add("@userPhone", userinfo.userPhone.ToString());
            if (userinfo.isDeleted != null)
                dict.Add("@isDeleted", userinfo.isDeleted.ToString());
            if (userinfo.userHead != null)
                dict.Add("@userHead", userinfo.userHead.ToString());
            if (userinfo.birthday != null)
                dict.Add("@birthday", userinfo.birthday.ToString());
            if (userinfo.coupon != null)
                dict.Add("@coupon", userinfo.coupon.ToString());
            if (userinfo.deviceToken != null)
                dict.Add("@deviceToken", userinfo.deviceToken.ToString());
            if (userinfo.sexType != null)
                dict.Add("@sexType", userinfo.sexType.ToString());
            if (userinfo.deviceType != null)
                dict.Add("@deviceType", userinfo.deviceType.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(UserInfo userinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (userinfo.userName != null)
            {
                part1.Append("userName,");
                part2.Append("@userName,");
            }
            if (userinfo.userBalance != null)
            {
                part1.Append("userBalance,");
                part2.Append("@userBalance,");
            }
            if (userinfo.wechat != null)
            {
                part1.Append("wechat,");
                part2.Append("@wechat,");
            }
            if (userinfo.qq != null)
            {
                part1.Append("qq,");
                part2.Append("@qq,");
            }
            if (userinfo.userPwd != null)
            {
                part1.Append("userPwd,");
                part2.Append("@userPwd,");
            }
            if (userinfo.userPhone != null)
            {
                part1.Append("userPhone,");
                part2.Append("@userPhone,");
            }
            if (userinfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (userinfo.userHead != null)
            {
                part1.Append("userHead,");
                part2.Append("@userHead,");
            }
            if (userinfo.birthday != null)
            {
                part1.Append("birthday,");
                part2.Append("@birthday,");
            }
            if (userinfo.coupon != null)
            {
                part1.Append("coupon,");
                part2.Append("@coupon,");
            }
            if (userinfo.deviceToken != null)
            {
                part1.Append("deviceToken,");
                part2.Append("@deviceToken,");
            }
            if (userinfo.sexType != null)
            {
                part1.Append("sexType,");
                part2.Append("@sexType,");
            }
            if (userinfo.deviceType != null)
            {
                part1.Append("deviceType,");
                part2.Append("@deviceType,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into userinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
                 /// 更新
                 /// </summary>
                 /// <param name="userinfo"></param>
                 /// <returns>是否成功</returns>
        public string GetUpdateStr(UserInfo userinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update userinfo set ");
            if (userinfo.userName != null)
                part1.Append("userName = @userName,");
            if (userinfo.userBalance != null)
                part1.Append("userBalance = @userBalance,");
            if (userinfo.wechat != null)
                part1.Append("wechat = @wechat,");
            if (userinfo.qq != null)
                part1.Append("qq = @qq,");
            if (userinfo.userPwd != null)
                part1.Append("userPwd = @userPwd,");
            if (userinfo.userPhone != null)
                part1.Append("userPhone = @userPhone,");
            if (userinfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (userinfo.userHead != null)
                part1.Append("userHead = @userHead,");
            if (userinfo.birthday != null)
                part1.Append("birthday = @birthday,");
            if (userinfo.coupon != null)
                part1.Append("coupon = @coupon,");
            if (userinfo.deviceToken != null)
                part1.Append("deviceToken = @deviceToken,");
            if (userinfo.sexType != null)
                part1.Append("sexType = @sexType,");
            if (userinfo.deviceType != null)
                part1.Append("deviceType = @deviceType,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        public UserView GetViewByUserId(int userId)
        {
            return CacheHelper.GetById<UserView>("UserView", userId);
        }

        /// <summary>
        /// 支付后改变余额和优惠券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="useBalance"></param>
        /// <param name="useCoupon"></param>
        /// <returns></returns>
        public bool ChangeBalanceCoupon(int userId, decimal useBalance, decimal useCoupon)
        {
            var user = GetById(userId);
            var balance = user.userBalance - useBalance;
            var coupon = user.coupon - useCoupon;
            if (balance < 0 || coupon < 0)
                return false;
            var userHere = new UserInfo();
            userHere.id = user.id;
            userHere.userBalance = balance;
            userHere.coupon = coupon;
            Update(userHere);
            return true;
        }

        public Dictionary<int, string> GetUserDict()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var list = CacheHelper.GetByCondition<UserInfo>("UserInfo", " isdeleted=0");
            foreach (var item in list)
            {
                dict.Add(item.id, item.userName);
            }
            return dict;
        }


    }
}
