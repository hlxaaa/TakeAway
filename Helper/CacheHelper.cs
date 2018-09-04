using BeIT.MemCached;
using Common.Helper;
using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;


namespace takeAwayWebApi.Helper
{
    public class CacheHelper
    {
        //现在引用的memcache.dll不是这个项目里的。

        /// <summary>
        /// token有效时间
        /// </summary>
        public static double TokenInvalidOutTimeDays = 20;

        public static string connStr = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        public static MemcachedClient cache = MemCacheHelper.GetMyConfigInstance();

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="listName">list名</param>
        /// <param name="isHasDeleteField">记录是否软删除</param>
        public static void UpdateCache<T>(string tableName, string listName) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                MemcachedClient MyCache = MemCacheHelper.GetMyConfigInstance();
                string selectAll = "select * from " + tableName;
                SqlDataAdapter da = new SqlDataAdapter(selectAll, conn);
                DataTable table = new DataTable();
                da.Fill(table);
                var list = table.ConvertToList<T>();
                MyCache.Set(listName, list);

                var list2 = MyCache.GetModel<List<DbOpertion.Models.UserInfo>>("List_UserInfo");
            }
        }

        /// <summary>
        /// 记得名字带id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="listName"></param>
        /// <param name="id"></param>
        public static void UpdateCacheById<T>(string tableName, string listName, string id) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                MemcachedClient MyCache = MemCacheHelper.GetMyConfigInstance();
                string selectAll = "select * from " + tableName + " where id = " + id;
                SqlDataAdapter da = new SqlDataAdapter(selectAll, conn);
                DataTable table = new DataTable();
                da.Fill(table);
                var list = table.ConvertToList<T>();
                MyCache.Set(listName + "_" + id, list);

                //var list2 = MyCache.GetModel<List<DbOpertion.Models.UserInfo>>("List_UserInfo");
            }
        }

        /// <summary>
        /// 从缓存取，若没有就更新缓存再取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        public static object Get<T>(string tableName, string listName) where T : class, new()
        {
            if (false)//前期改动多，每次都要update-txy
            {
                List<T> a = (List<T>)cache.Get(listName);
                if (a == null)
                {
                    UpdateCache<T>(tableName, listName);
                    a = (List<T>)cache.Get(listName);
                }
                return a;
            }
            UpdateCache<T>(tableName, listName);
            List<T> b = (List<T>)cache.Get(listName);
            return b;
        }

        /// <summary>
        /// 可能不需要了
        /// </summary>
        public static void GetAllCache()
        {
            UpdateCache<UserInfo>("UserInfo", "List_UserInfo");
            UpdateCache<RiderInfo>("RiderInfo", "List_RiderInfo");
            //UpdateCache<FoodTypeInfo>("FoodTypeInfo", "List_FoodTypeInfo");
            //UpdateCache<FoodInfo>("FoodInfo", "List_FoodInfo");
            //UpdateCache<Drinks>("Drinks", "List_Drinks");
            UpdateCache<AreaInfo>("AreaInfo", "List_AreaInfo");
        }

        public static void Set(string key, string value, TimeSpan ts)
        {
            cache.Set(key, value, ts);
        }

        /// <summary>
        /// 用户注册验证码是否能重置
        /// </summary>
        /// <param name="Phone">用户手机</param>
        /// <returns></returns>
        public static bool GetUserResetVerificationCode(string Phone)
        {
            var phone = cache.Get("Ta_VerificationCode_UserPhone=" + Phone);
            return phone == null;
        }

        /// <summary>
        /// 设置用户注册验证码
        /// </summary>
        /// <param name="Phone">用户手机</param>
        /// <returns></returns>
        public static string SetUserVerificationCode(string Phone)
        {
            var code = cache.Get("Ta_VerificationCode_UserPhone=" + Phone);
            if (code == null)
            {
                string VerificationCode = RandHelperHere.Instance.Number(6);
                CacheHelper.Set("Ta_VerificationCode_UserPhone=" + Phone, VerificationCode, new TimeSpan(0, 5, 0));//TimeSpan(0, 5, 0)保留5分钟,测试暂时保留一天-txy
                return VerificationCode;
            }
            return (string)code;
        }

        /// <summary>
        /// 获取手机号对应验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>验证码或null</returns>
        public static string GetUserVerificationCode(string phone)
        {
            return (string)cache.Get("Ta_VerificationCode_UserPhone=" + phone);
        }

        /// <summary>
        /// 设置用户Token,存入缓存
        /// </summary>
        /// <param name="user">用户模型</param>
        /// <returns></returns>
        public static Token SetUserToken(UserInfo user)
        {
            Token token = new Token
            {
                Payload = new Payload
                {
                    exp = DateTime.Now.AddDays(TokenInvalidOutTimeDays).ToString(),
                    UserID = user.id,
                    UserName = user.userName
                }
            };
            cache.Set("Ta_UserToken_" + token.Payload.UserID, token, DateTime.Now.AddDays(TokenInvalidOutTimeDays));
            return token;
        }

        /// <summary>
        /// 从缓存中获取用户Token
        /// </summary>
        /// <param name="tokenString">tooken字符串</param>
        /// <returns></returns>
        public static Token GetUserToken(string tokenString)
        {
            if (tokenString == null)
                return null;
            Token token = new Token();
            if (token.Validate(tokenString))
            {
                var tokenModel = cache.GetModel<Token>("Ta_UserToken_" + token.Payload.UserID);
                if (tokenModel != null && tokenString == tokenModel.GetToken())
                {
                    return tokenModel;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取骑手账号token
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns></returns>
        public static Token GetRiderToken(string tokenString)
        {
            if (tokenString == null)
                return null;
            Token token = new Token();
            if (token.Validate(tokenString))
            {
                var tokenModel = cache.GetModel<Token>("Ta_RiderToken_" + token.Payload.UserID);
                if (tokenModel != null && tokenString == tokenModel.GetToken())
                {
                    return tokenModel;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置骑手账号token
        /// </summary>
        /// <param name="rider"></param>
        /// <returns></returns>
        public static Token SetRiderToken(RiderInfo rider)
        {
            Token token = new Token
            {
                Payload = new Payload
                {
                    exp = DateTime.Now.AddDays(TokenInvalidOutTimeDays).ToString(),
                    UserID = rider.id,
                    UserName = rider.name
                }
            };
            cache.Set("Ta_RiderToken_" + token.Payload.UserID, token, DateTime.Now.AddDays(TokenInvalidOutTimeDays));
            return token;
        }

        public static void SetUserArticleClick(int userId)
        {
            var uacList = cache.GetModel<List<UserArticleClick>>("Ta_UserArticleClick");
            if (uacList == null)
            {
                UserArticleClick temp = new UserArticleClick(userId, true);
                List<UserArticleClick> list = new List<UserArticleClick>
                {
                    temp
                };
                cache.Set("Ta_UserArticleClick", list);
                return;
            }
            var list2 = uacList.Where(p => p.userId == userId).ToList();
            if (list2.Count < 1)
            {
                UserArticleClick temp = new UserArticleClick(userId, true);
                uacList.Add(temp);
                cache.Set("Ta_UserArticleClick", uacList);
                return;
            }
            uacList.Where(p => p.userId == userId).First().isClicked = true;
            cache.Set("Ta_UserArticleClick", uacList);
            return;
        }

        public static bool IsUserArticleClick(int userId)
        {
            var uacList = cache.GetModel<List<UserArticleClick>>("Ta_UserArticleClick");
            if (uacList == null)
                return false;
            var list = uacList.Where(p => p.userId == userId).ToList();
            if (list.Count() < 1)
            {
                return false;
            }
            if (list.First().isClicked)
                return true;
            return false;
        }

        /// <summary>
        /// 发布新文章后，设所有用户都没点过文章
        /// </summary>
        public static void SetAllUserArticleNotClick()
        {
            var list = cache.GetModel<List<UserArticleClick>>("Ta_UserArticleClick");
            if (list == null)
                return;
            foreach (var item in list)
            {
                item.isClicked = false;
            }
            cache.Set("Ta_UserArticleClick", list);
        }

        /// <summary>
        /// 骑手端设置验证码
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static string SetRiderVerificationCode(string Phone)
        {
            var code = cache.Get("Ta_VerificationCode_RiderPhone=" + Phone);
            if (code == null)
            {
                string VerificationCode = RandHelperHere.Instance.Number(6);
                CacheHelper.Set("Ta_VerificationCode_RiderPhone=" + Phone, VerificationCode, new TimeSpan(3, 0, 5, 0));//TimeSpan(0, 5, 0)保留5分钟,测试暂时保留一天-txy
                return VerificationCode;
            }
            return (string)code;
        }

        /// <summary>
        /// 获取骑手端手机号对应验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns>验证码或null</returns>
        public static string GetRiderVerificationCode(string phone)
        {
            return (string)cache.Get("Ta_VerificationCode_RiderPhone=" + phone);
        }

        /// <summary>
        /// 设置骑手坐标
        /// </summary>
        /// <param name="riderId"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public static void SetRiderPosition(int riderId, string lat, string lng, int riderType)
        {
            RiderPosition rp = new RiderPosition
            {
                riderId = riderId,
                lat = lat,
                lng = lng,
                riderType = riderType
            };
            cache.Set("riderPosition_" + riderId, rp, DateTime.Now.AddDays(TokenInvalidOutTimeDays));
        }

        /// <summary>
        /// 获取骑手坐标
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public static RiderPosition GetRiderPosition(int riderId)
        {
            var r = cache.GetModel<RiderPosition>("riderPosition_" + riderId);
            if (r == null)
            {
                var rider = RiderInfoOper.Instance.GetById(riderId);
                AddRiderPosition(riderId, (int)rider.riderType);
                return cache.GetModel<RiderPosition>("riderPosition_" + riderId);
            }
            return r;
        }

        /// <summary>
        /// 获取骑手坐标
        /// </summary>
        /// <param name="riderId"></param>
        /// <returns></returns>
        public static RiderPosition GetRiderPosition(int riderId, int riderType)
        {
            var r = cache.GetModel<RiderPosition>("riderPosition_" + riderId);
            if (r == null)
            {
                AddRiderPosition(riderId, riderType);
                return cache.GetModel<RiderPosition>("riderPosition_" + riderId);
            }
            return r;
        }

        /// <summary>
        /// 缓存锁
        /// </summary>
        /// <returns></returns>
        public static void LockCache(string name)
        {
            var flag = false;
            while (!flag)
            {
                flag = cache.Add("L_C_Ta_" + name, "123", new TimeSpan(0, 0, 0, 30));
            }
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        public static void ReleaseLock(string name)
        {
            cache.Delete("L_C_Ta_" + name);
        }

        //public static void ClearAllCache() {
        //    cache.FlushAll();
        //}

        /// <summary>
        /// 设置某用户点过的菜
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listFoods"></param>
        public static void SetTempFoods(int userId, List<foodId_Amount> listFoods)
        {
            cache.Set("Ta_HPFoods_userId_" + userId, listFoods, DateTime.Now.AddDays(TokenInvalidOutTimeDays));
        }

        public static void SetTempFoods2(int userId, List<AreaId_foodId_amount> listFoods)
        {
            cache.Set("Ta_HPFoods2_userId_" + userId, listFoods, DateTime.Now.AddDays(TokenInvalidOutTimeDays));
        }

        /// <summary>
        /// 获取用户点过的菜
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<foodId_Amount> GetTempFoods(int userId)
        {
            var r = cache.GetModel<List<foodId_Amount>>("Ta_HPFoods_userId_" + userId);
            if (r == null)
                return new List<foodId_Amount>();
            return r;
        }

        public static List<AreaId_foodId_amount> GetTempFoods2(int userId)
        {
            var r = cache.GetModel<List<AreaId_foodId_amount>>("Ta_HPFoods2_userId_" + userId);
            if (r == null)
                return new List<AreaId_foodId_amount>();
            return r;
        }

        /// <summary>
        /// 删除某用户点过的菜
        /// </summary>
        /// <param name="userId"></param>
        public static void DelTempFoods(int userId)
        {
            cache.Delete("Ta_HPFoods_userId_" + userId);
        }

        public static void DelTempFoods2(int userId)
        {
            cache.Delete("Ta_HPFoods2_userId_" + userId);
        }

        /// <summary>
        /// 防止 缓存锁 死循环 用这个清一波
        /// </summary>
        public static void ClearFlagStrs()
        {
            var flagStrs = cache.GetModel<List<string>>("Ta_flagStrs");
            foreach (var item in flagStrs)
            {
                cache.Delete(item);
            }
        }

        public static CancelCache GetCancelCache(int userId)
        {
            CancelCache cc = cache.GetModel<CancelCache>("Ta_CancelCache_userId_" + userId);
            if (cc == null)
                return null;
            return cc;
        }

        public static void SetCancelCache(CancelCache cc)
        {
            cache.Set("Ta_CancelCache_userId_" + cc.userId, cc, new TimeSpan(1, 0, 0, 0));
        }

        /// <summary>
        /// 给骑手位置添加假数据
        /// </summary>
        /// <param name="riderId">骑手id</param>
        /// <param name="type">骑手类型0电瓶车1汽车2自提点</param>
        public static void AddRiderPosition(int riderId, int type)
        {
            List<int> ids = new List<int>();
            List<string> lats = new List<string>();
            List<string> lngs = new List<string>();
            List<int> types = new List<int>();
            ids.Add(riderId);
            var temp = RandHelper.Instance.Number(8, true);
            var temp2 = RandHelper.Instance.Number(8, true);
            lats.Add("30.27" + temp);
            lngs.Add("120.01" + temp2);
            types.Add(type);
            for (int i = 0; i < ids.Count; i++)
            {
                CacheHelper.SetRiderPosition(ids[i], lats[i], lngs[i], types[i]);
            }
        }

        //————————————————————————————————————
        public static Token checkToken(int orderId)
        {
            return cache.GetModel<Token>("Ta_UserToken_" + orderId);
        }

        //———————————————以下不操作缓存———————————————————

        /// <summary>
        /// 根据id直接从数据库获取数据,防注入了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="listName">比如userInfo 下划线自带</param>
        /// <param name="id">就是id</param>
        /// <returns></returns>
        public static T GetById<T>(string tableName, int id) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string selectAll = "select * from " + tableName + " where id =@id ";
                var dict = new Dictionary<string, string>();
                dict.Add("@id", id.ToString());
                var table = SqlHelperHere.ExecuteGetDt(selectAll, dict);
                return table.ConvertToList<T>().FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static List<T> GetByCondition<T>(string tableName, string condition) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from " + tableName + " where " + condition);
                SqlDataAdapter da = new SqlDataAdapter(sb.ToString(), conn);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        /// <summary>
        /// 根据id集合来查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<T> GetByIds<T>(string tableName, string ids) where T : class, new()
        {
            return CacheHelper.GetByCondition<T>(tableName, " id in (" + ids + ")");
        }

        /// <summary>
        /// 根据id集合来查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<T> GetByIds<T>(string tableName, int[] ids) where T : class, new()
        {
            var str = string.Join(",", ids);
            return CacheHelper.GetByIds<T>(tableName, str);
        }

        /// <summary>
        /// 根据条件查询,防注入版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static List<T> GetByCondition<T>(string tableName, string condition, Dictionary<string, string> dict) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from " + tableName + " where " + condition);

                SqlCommand sqlCom = new SqlCommand(sb.ToString(), conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        /// <summary>
        /// 倒序的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<T> GetByConditionPaging<T>(string tableName, string condition, int index, int size) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from " + tableName + " where " + condition);

                int x = (index - 1) * size;
                string str = "select top " + size + " * from (" + sb.ToString() + ") r where id not in (select top " + x + " id from (" + sb.ToString() + ") r order by id desc) order by id desc";

                SqlDataAdapter da = new SqlDataAdapter(str, conn);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        /// <summary>
        /// 倒序的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<T> GetByConditionPaging<T>(string tableName, string condition, int index, int size, string order) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from " + tableName + " where " + condition);

                int x = (index - 1) * size;
                string str = "select top " + size + " * from (" + sb.ToString() + ") r where id not in (select top " + x + " id from (" + sb.ToString() + ") r " + order + ") " + order;

                SqlDataAdapter da = new SqlDataAdapter(str, conn);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        //下面的不需要分页，因为web上要显示总共多少页，我需要把所有符合条件的都select出来，算页数

        /// <summary>
        /// 条件复杂的时候，查询出所有符合的id,已经倒序了(如果需要按多个字段来排序的话，就自己写sql吧)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<T> GetDistinctCount<T>(string tableName, string condition) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select distinct id from " + tableName + " where " + condition + " order by id desc");

                SqlDataAdapter da = new SqlDataAdapter(sb.ToString(), conn);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

        /// <summary>
        /// 条件复杂的时候，查询出所有符合的id,防注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<T> GetDistinctCount<T>(string tableName, string condition, Dictionary<string, string> dict) where T : class, new()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select distinct id from " + tableName + " where " + condition + " order by id desc");

                SqlCommand sqlCom = new SqlCommand(sb.ToString(), conn);
                foreach (var item in dict)
                {
                    sqlCom.Parameters.AddWithValue(item.Key, item.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlCom);
                DataTable table = new DataTable();
                da.Fill(table);
                return table.ConvertToList<T>();
            }
        }

    }
}