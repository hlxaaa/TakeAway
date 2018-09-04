
using System.Data.SqlClient;
using System.Configuration;
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
    public partial class CouponOper : SingleTon<CouponOper>
    {
        /// 获取参数
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(Coupon coupon)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (coupon.id != null)
                dict.Add("@id", coupon.id.ToString());
            if (coupon.batch != null)
                dict.Add("@batch", coupon.batch.ToString());
            if (coupon.ids != null)
                dict.Add("@ids", coupon.ids.ToString());
            if (coupon.isUse != null)
                dict.Add("@isUse", coupon.isUse.ToString());
            if (coupon.couponNo != null)
                dict.Add("@couponNo", coupon.couponNo.ToString());
            if (coupon.isDeleted != null)
                dict.Add("@isDeleted", coupon.isDeleted.ToString());
            if (coupon.createTime != null)
                dict.Add("@createTime", coupon.createTime.ToString());
            if (coupon.money != null)
                dict.Add("@money", coupon.money.ToString());
            if (coupon.isRepeat != null)
                dict.Add("@isRepeat", coupon.isRepeat.ToString());
            if (coupon.name != null)
                dict.Add("@name", coupon.name.ToString());

            return dict;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(Coupon coupon)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (coupon.batch != null)
            {
                part1.Append("batch,");
                part2.Append("@batch,");
            }
            if (coupon.ids != null)
            {
                part1.Append("ids,");
                part2.Append("@ids,");
            }
            if (coupon.isUse != null)
            {
                part1.Append("isUse,");
                part2.Append("@isUse,");
            }
            if (coupon.couponNo != null)
            {
                part1.Append("couponNo,");
                part2.Append("@couponNo,");
            }
            if (coupon.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (coupon.createTime != null)
            {
                part1.Append("createTime,");
                part2.Append("@createTime,");
            }
            if (coupon.money != null)
            {
                part1.Append("money,");
                part2.Append("@money,");
            }
            if (coupon.isRepeat != null)
            {
                part1.Append("isRepeat,");
                part2.Append("@isRepeat,");
            }
            if (coupon.name != null)
            {
                part1.Append("name,");
                part2.Append("@name,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into coupon(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(Coupon coupon)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update coupon set ");
            if (coupon.batch != null)
                part1.Append("batch = @batch,");
            if (coupon.ids != null)
                part1.Append("ids = @ids,");
            if (coupon.isUse != null)
                part1.Append("isUse = @isUse,");
            if (coupon.couponNo != null)
                part1.Append("couponNo = @couponNo,");
            if (coupon.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (coupon.createTime != null)
                part1.Append("createTime = @createTime,");
            if (coupon.money != null)
                part1.Append("money = @money,");
            if (coupon.isRepeat != null)
                part1.Append("isRepeat = @isRepeat,");
            if (coupon.name != null)
                part1.Append("name = @name,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }


        /// <summary>
        /// add
        /// </summary>
        /// <param name="Coupon"></param>
        /// <returns></returns>
        public int Add(Coupon model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="Coupon"></param>
        /// <returns></returns>
        public void Update(Coupon model)
        {
            CacheHelper.LockCache("Coupon");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            CacheHelper.ReleaseLock("Coupon");
        }
        /// <summary>
        /// del
        /// </summary>
        /// <param name="Coupon"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            Coupon model = new Coupon();
            model.isDeleted = true;
            model.id = id;
            Update(model);
        }


        public int GetLastCoupon() {
            string str = "select top 1 id from coupon order by id desc";
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str));
        }

        /// <summary>
        /// 获取所有的batch
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,string> GetTs()
        {
            string str = "select distinct batch,name from coupon where isdeleted=0";
            var dt = SqlHelperHere.ExecuteGetDt(str);
            return dt.ConvertToList<Coupon>().Select(p => new { p.batch, p.name }).ToDictionary(p => p.batch, p => p.name);
        }

        public Coupon GetByTs(string ts) {
            var list = CacheHelper.GetByCondition<Coupon>("coupon"," batch='"+ts+"'");
            return list.FirstOrDefault();
        }

        public int[] GetIdsByTs(string ts) {
            string str = "select top 1 ids from coupon where batch=@ts";
            var dict = new Dictionary<string, string>();
            dict.Add("@ts", ts);
            var idsStr = SqlHelperHere.ExecuteScalar(str,dict);
            var r = idsStr.Split(',');
            return Array.ConvertAll(r, int.Parse);
            //return r;
        }

        /// <summary>
        /// 获取同批次第一个记录
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public Coupon GetFirstByTs(string ts) {
            string str = "select top 1 * from coupon where batch=@ts";
            var dict = new Dictionary<string, string>();
            dict.Add("@ts", ts);
            var dt = SqlHelperHere.ExecuteGetDt(str,dict);
            return dt.ConvertToList<Coupon>().FirstOrDefault();
        }

        /// <summary>
        /// 用户是否用过这批次的优惠券
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool isUserUsedCoupon(int userId,string ts) {
            var coupon = GetFirstByTs(ts);
            if ((bool)coupon.isRepeat)//如果允许重复，那就不算他用过
                return false;
            var idsStr = coupon.ids;
            if (idsStr.Length < 1)
                return false;
            var temp = idsStr.Split(',');
       
            var ids  =Array.ConvertAll(temp, int.Parse);
            if (ids.Contains(userId))
                return true;
            return false;
        }

        /// <summary>
        /// 不可重复的兑换码的第一条的ids里加上这个用户id
        /// </summary>
        public void AddCouponIds(int userId, string ts) {
            var coupon = GetFirstByTs(ts);
            var idsStr = coupon.ids;
            Coupon c = new Coupon();
            if (idsStr == "")
                c.ids += userId;
            else
                c.ids += "," + userId;
            c.id = coupon.id;
            Update(c);
        }

        public List<Coupon> GetByCouponNo(string couponNo) {
            return CacheHelper.GetByCondition<Coupon>("coupon", " couponNo='"+couponNo+"'");
            //return list.FirstOrDefault();
        }

        public void DeleteByTs(string ts) {
            string str = "update coupon set isdeleted=1 where batch=@ts";
            var dict = new Dictionary<string, string>();
            dict.Add("@ts", ts);
            SqlHelperHere.ExcuteNon(str,dict);
        }
    }
}
