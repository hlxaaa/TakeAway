using System.Text;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using System;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class UserPayOper : SingleTon<UserPayOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="userpay"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(UserPay userpay)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (userpay.id != null)
                dict.Add("@id", userpay.id.ToString());
            if (userpay.type != null)
                dict.Add("@type", userpay.type.ToString());
            if (userpay.money != null)
                dict.Add("@money", userpay.money.ToString());
            if (userpay.status != null)
                dict.Add("@status", userpay.status.ToString());
            if (userpay.userId != null)
                dict.Add("@userId", userpay.userId.ToString());
            if (userpay.createTime != null)
                dict.Add("@createTime", userpay.createTime.ToString());
            if (userpay.takeType != null)
                dict.Add("@takeType", userpay.takeType.ToString());
            if (userpay.takeAccount != null)
                dict.Add("@takeAccount", userpay.takeAccount.ToString());
            if (userpay.outTradeNo != null)
                dict.Add("@outTradeNo", userpay.outTradeNo.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="userpay"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(UserPay userpay)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (userpay.type != null)
            {
                part1.Append("type,");
                part2.Append("@type,");
            }
            if (userpay.money != null)
            {
                part1.Append("money,");
                part2.Append("@money,");
            }
            if (userpay.status != null)
            {
                part1.Append("status,");
                part2.Append("@status,");
            }
            if (userpay.userId != null)
            {
                part1.Append("userId,");
                part2.Append("@userId,");
            }
            if (userpay.createTime != null)
            {
                part1.Append("createTime,");
                part2.Append("@createTime,");
            }
            if (userpay.takeType != null)
            {
                part1.Append("takeType,");
                part2.Append("@takeType,");
            }
            if (userpay.takeAccount != null)
            {
                part1.Append("takeAccount,");
                part2.Append("@takeAccount,");
            }
            if (userpay.outTradeNo != null)
            {
                part1.Append("outTradeNo,");
                part2.Append("@outTradeNo,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into userpay(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="userpay"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(UserPay userpay)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update userpay set ");
            if (userpay.type != null)
                part1.Append("type = @type,");
            if (userpay.money != null)
                part1.Append("money = @money,");
            if (userpay.status != null)
                part1.Append("status = @status,");
            if (userpay.userId != null)
                part1.Append("userId = @userId,");
            if (userpay.createTime != null)
                part1.Append("createTime = @createTime,");
            if (userpay.takeType != null)
                part1.Append("takeType = @takeType,");
            if (userpay.takeAccount != null)
                part1.Append("takeAccount = @takeAccount,");
            if (userpay.outTradeNo != null)
                part1.Append("outTradeNo = @outTradeNo,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }
        /// <summary>
        /// add
        /// </summary>
        /// <param name="UserPay"></param>
        /// <returns></returns>
        public int Add(UserPay model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="UserPay"></param>
        /// <returns></returns>
        public void Update(UserPay model)
        {
            //CacheHelper.LockCache("UserPay");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            //CacheHelper.ReleaseLock("UserPay");
        }

        /// <summary>
        /// getById
        /// </summary>
        /// <param name="UserPay"></param>
        /// <returns></returns>
        public UserPay GetById(int id)
        {
            return CacheHelper.GetById<UserPay>("UserPay", id);
        }

        /// <summary>
        /// 支付成功的时候。添加收支明细  up.type 2余额支付  3礼金券支付
        /// </summary>
        /// <param name="order"></param>
        public void payOrderAddRecord(OrderInfo order)
        {
            if (order.useBalance > 0)
            {
                UserPay up = new UserPay
                {
                    type = 2,
                    money = order.useBalance,
                    userId = order.userId,
                    createTime = DateTime.Now
                };
                Add(up);
            }
            if (order.useCoupon > 0)
            {
                UserPay up = new UserPay();
                up.type = 3;
                up.money = order.useCoupon;
                up.userId = order.userId;
                up.createTime = DateTime.Now;
                Add(up);
            }
        }

        /// <summary>
        /// 押金退回用户余额，并添加记录
        /// </summary>
        public void DepositBackRecord(OrderInfo order)
        {
            var user = UserInfoOper.Instance.GetById((int)order.userId);
            user.userBalance += order.deposit;
            var userHere = new UserInfo
            {
                id = user.id,
                userBalance = user.userBalance
            };
            UserInfoOper.Instance.Update(userHere);
            UserPay up = new UserPay
            {
                userId = order.userId,
                type = 5,
                money = order.deposit,
                createTime = DateTime.Now
            };
            Add(up);//收支明细-押金退回
        }
    }
}
