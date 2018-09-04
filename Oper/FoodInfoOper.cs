
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DbOpertion.Models;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using System;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Response;
using Common;

namespace DbOpertion.DBoperation
{
    public partial class FoodInfoOper : SingleTon<FoodInfoOper>
    {
        /// <summary>
        /// add
        /// </summary>
        /// <param name="FoodInfo"></param>
        /// <returns></returns>
        public int Add(FoodInfo model)
        {
            var str = GetInsertStr(model) + " select @@identity";
            var dict = GetParameters(model);
            return Convert.ToInt32(SqlHelperHere.ExecuteScalar(str, dict));
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="FoodInfo"></param>
        /// <returns></returns>
        public void Update(FoodInfo model)
        {
            CacheHelper.LockCache("FoodInfo");
            var str = GetUpdateStr(model);
            var dict = GetParameters(model);
            SqlHelperHere.ExcuteNon(str, dict);
            CacheHelper.ReleaseLock("FoodInfo");
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="foodinfo"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetParameters(FoodInfo foodinfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (foodinfo.id != null)
                dict.Add("@id", foodinfo.id.ToString());
            if (foodinfo.foodName != null)
                dict.Add("@foodName", foodinfo.foodName.ToString());
            if (foodinfo.foodPrice != null)
                dict.Add("@foodPrice", foodinfo.foodPrice.ToString());
            if (foodinfo.foodTagId != null)
                dict.Add("@foodTagId", foodinfo.foodTagId.ToString());
            if (foodinfo.foodImg != null)
                dict.Add("@foodImg", foodinfo.foodImg.ToString());
            if (foodinfo.foodText != null)
                dict.Add("@foodText", foodinfo.foodText.ToString());
            if (foodinfo.isDeleted != null)
                dict.Add("@isDeleted", foodinfo.isDeleted.ToString());
            if (foodinfo.isMain != null)
                dict.Add("@isMain", foodinfo.isMain.ToString());
            if (foodinfo.isThisWeek != null)
                dict.Add("@isThisWeek", foodinfo.isThisWeek.ToString());
            if (foodinfo.deposit != null)
                dict.Add("@deposit", foodinfo.deposit.ToString());
            if (foodinfo.secondTag != null)
                dict.Add("@secondTag", foodinfo.secondTag.ToString());
            if (foodinfo.star != null)
                dict.Add("@star", foodinfo.star.ToString());
            if (foodinfo.starCount != null)
                dict.Add("@starCount", foodinfo.starCount.ToString());
            if (foodinfo.isOn != null)
                dict.Add("@isOn", foodinfo.isOn.ToString());
            if (foodinfo.FoodTime != null)
                dict.Add("@FoodTime", foodinfo.FoodTime.ToString());

            return dict;
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="foodinfo"></param>
        /// <returns>是否成功</returns>
        public string GetInsertStr(FoodInfo foodinfo)
        {
            StringBuilder part1 = new StringBuilder();
            StringBuilder part2 = new StringBuilder();

            if (foodinfo.foodName != null)
            {
                part1.Append("foodName,");
                part2.Append("@foodName,");
            }
            if (foodinfo.foodPrice != null)
            {
                part1.Append("foodPrice,");
                part2.Append("@foodPrice,");
            }
            if (foodinfo.foodTagId != null)
            {
                part1.Append("foodTagId,");
                part2.Append("@foodTagId,");
            }
            if (foodinfo.foodImg != null)
            {
                part1.Append("foodImg,");
                part2.Append("@foodImg,");
            }
            if (foodinfo.foodText != null)
            {
                part1.Append("foodText,");
                part2.Append("@foodText,");
            }
            if (foodinfo.isDeleted != null)
            {
                part1.Append("isDeleted,");
                part2.Append("@isDeleted,");
            }
            if (foodinfo.isMain != null)
            {
                part1.Append("isMain,");
                part2.Append("@isMain,");
            }
            if (foodinfo.isThisWeek != null)
            {
                part1.Append("isThisWeek,");
                part2.Append("@isThisWeek,");
            }
            if (foodinfo.deposit != null)
            {
                part1.Append("deposit,");
                part2.Append("@deposit,");
            }
            if (foodinfo.secondTag != null)
            {
                part1.Append("secondTag,");
                part2.Append("@secondTag,");
            }
            if (foodinfo.star != null)
            {
                part1.Append("star,");
                part2.Append("@star,");
            }
            if (foodinfo.starCount != null)
            {
                part1.Append("starCount,");
                part2.Append("@starCount,");
            }
            if (foodinfo.isOn != null)
            {
                part1.Append("isOn,");
                part2.Append("@isOn,");
            }
            if (foodinfo.FoodTime != null)
            {
                part1.Append("FoodTime,");
                part2.Append("@FoodTime,");
            }
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into foodinfo(").Append(part1.ToString().Remove(part1.Length - 1)).Append(") values (").Append(part2.ToString().Remove(part2.Length - 1)).Append(")");
            return sql.ToString();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="foodinfo"></param>
        /// <returns>是否成功</returns>
        public string GetUpdateStr(FoodInfo foodinfo)
        {
            StringBuilder part1 = new StringBuilder();
            part1.Append("update foodinfo set ");
            if (foodinfo.foodName != null)
                part1.Append("foodName = @foodName,");
            if (foodinfo.foodPrice != null)
                part1.Append("foodPrice = @foodPrice,");
            if (foodinfo.foodTagId != null)
                part1.Append("foodTagId = @foodTagId,");
            if (foodinfo.foodImg != null)
                part1.Append("foodImg = @foodImg,");
            if (foodinfo.foodText != null)
                part1.Append("foodText = @foodText,");
            if (foodinfo.isDeleted != null)
                part1.Append("isDeleted = @isDeleted,");
            if (foodinfo.isMain != null)
                part1.Append("isMain = @isMain,");
            if (foodinfo.isThisWeek != null)
                part1.Append("isThisWeek = @isThisWeek,");
            if (foodinfo.deposit != null)
                part1.Append("deposit = @deposit,");
            if (foodinfo.secondTag != null)
                part1.Append("secondTag = @secondTag,");
            if (foodinfo.star != null)
                part1.Append("star = @star,");
            if (foodinfo.starCount != null)
                part1.Append("starCount = @starCount,");
            if (foodinfo.isOn != null)
                part1.Append("isOn = @isOn,");
            if (foodinfo.FoodTime != null)
                part1.Append("FoodTime = @FoodTime,");
            int n = part1.ToString().LastIndexOf(",");
            part1.Remove(n, 1);
            part1.Append(" where id= @id  ");
            return part1.ToString();
        }

        public List<FoodInfo> GetFoods()
        {
            return CacheHelper.GetByCondition<FoodInfo>("FoodInfo", " isdeleted=0 ");
        }

        public Price_Deposit GetFoodPriceAndDepositByAFA(List<AreaId_foodId_amount> listafa, int areaId)
        {
            Price_Deposit result = new Price_Deposit();
            listafa = listafa.Where(p => p.areaId == areaId).ToList();
            if (listafa.Count < 1)
            {
                result.price = 0;
                result.deposit = 0;
            }
            else
            {
                var listFoods = new List<foodId_Amount>();
                foreach (var item in listafa)
                {
                    foodId_Amount fa = new foodId_Amount();
                    fa.foodId = item.foodId;
                    fa.amount = item.amount;
                    listFoods.Add(fa);
                }
                result = GetFoodPriceAndDeposit(listFoods);
            }
            return result;
        }

        /// <summary>
        /// 获取菜品总价和总押金
        /// </summary>
        /// <param name="listFoods"></param>
        /// <returns></returns>
        public Price_Deposit GetFoodPriceAndDeposit(List<foodId_Amount> listFoods)
        {
            Price_Deposit result = new Price_Deposit();
            var ids = listFoods.Select(p => p.foodId).ToArray();
            var idsStr = string.Join(",", ids);

            var list = CacheHelper.GetByCondition<FoodInfo>("FoodInfo", " id in (" + idsStr + ")");

            var r = new decimal();
            foreach (var item in listFoods)
            {
                var price = list.Where(p => p.id == item.foodId).Select(p => p.foodPrice).First();
                r += item.amount * (decimal)price;
            }
            result.price = r;

            var sum = new decimal(0);
            foreach (var item in listFoods)
            {
                var food = list.Where(p => p.id == item.foodId).First();
                var amount = item.amount;
                var deposit = (decimal)food.deposit;
                sum += amount * deposit;
            }
            result.deposit = sum;
            return result;
        }

        /// <summary>
        /// 获取周菜品中的附加品（配菜）
        /// </summary>
        /// <returns></returns>
        public List<FoodView> GetReserveNotMainFood()
        {
            return CacheHelper.GetByCondition<FoodView>("FoodView", " isdeleted=0 and  isweek=1 and ismain=0");
        }

        /// <summary>
        /// 获取订单中的菜品列表
        /// </summary>
        /// <returns></returns>
        public List<foodId_name_amount_price_isMain> GetFoodINAPI(List<foodId_Amount> listFoods)
        {
            var ids = listFoods.Select(p => p.foodId).ToArray();
            var idsStr = string.Join(",", ids);
            var list = CacheHelper.GetByCondition<FoodInfo>("FoodInfo", " id in(" + idsStr + ")");
            List<foodId_name_amount_price_isMain> listR = new List<foodId_name_amount_price_isMain>();
            foreach (var item in listFoods)
            {
                var food = list.Where(p => p.id == item.foodId).First();
                foodId_name_amount_price_isMain fa = new foodId_name_amount_price_isMain(food, item.amount);
                listR.Add(fa);
            }
            return listR;
        }

        /// <summary>
        /// 只要菜品列表里的一个主菜信息
        /// </summary>
        /// <param name="listFoods"></param>
        /// <returns></returns>
        public FoodView GetMainFood(List<foodId_Amount> listFoods)
        {
            var ids = GetFoodIds(listFoods);
            var idsStr = string.Join(",", ids);
            var list = CacheHelper.GetByCondition<FoodView>("foodview", " isdeleted=0 and  id in (" + idsStr + ") and ismain=1 ");
            return list.FirstOrDefault();
        }

        public int[] GetFoodIds(List<foodId_Amount> listFoods)
        {
            return listFoods.Select(p => p.foodId).Distinct().ToArray();
        }

        /// <summary>
        /// 获取所有实时菜品的id
        /// </summary>
        /// <returns></returns>
        public List<int> GetFoodIds()
        {
            var list = CacheHelper.GetByCondition<FoodView>("FoodView", " isdeleted=0 and  isweek=0");
            return list.Select(p => p.id).ToList();
        }

        /// <summary>
        /// 更新菜品的星级
        /// </summary>
        /// <param name="listFoods"></param>
        public void UpdateFoodStars(List<FoodComment> listFoods)
        {
            var ids = listFoods.Select(p => p.foodId).ToArray();
            var idsStr = string.Join(",", ids);
            var list = CacheHelper.GetByCondition<FoodInfo>("foodinfo", " id in (" + idsStr + ")");
            foreach (var item in list)
            {
                FoodInfo food = new FoodInfo();
                var newStar = listFoods.Where(p => p.foodId == item.id).First().foodStars;
                var oldStar = item.star;
                var starCount = item.starCount;
                food.star = (oldStar * starCount + newStar) / (starCount + 1);
                food.starCount += 1;
                food.id = item.id;
                Update(food);
            }
        }


        public Dictionary<int, string> GetFoodDict()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var list = CacheHelper.GetByCondition<FoodView>("FoodView", " isdeleted=0 and  isweek=0");
            foreach (var item in list)
            {
                dict.Add(item.id, item.foodName);
            }

            //List<FoodView> list = (List<FoodView>)CacheHelper.Get<FoodView>("FoodView", "Ta_FoodView");
            //list = list.Where(p => p.isWeek == false).ToList();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    dict.Add(list[i].id, list[i].foodName);
            //}
            return dict;
        }

        /// <summary>
        /// 不管IsDeleted
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetFoodDict2()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var list = CacheHelper.GetByCondition<FoodInfo>("FoodInfo", " 1=1");
            foreach (var item in list)
            {
                dict.Add(item.id, item.foodName);
            }
            return dict;
        }

        public FoodInfo GetById(int foodId)
        {
            return CacheHelper.GetById<FoodInfo>("FoodInfo", foodId);
        }

        /// <summary>
        /// 清除过期的预定菜品
        /// </summary>
        public void ClearWeekFood()
        {
            var now = Convert.ToInt32(DateTime.Now.DayOfWeek);
            now = now == 0 ? 7 : now;
            var dt = DateTime.Now.AddDays((7 - now) - 14);
            string str = "select id from foodview where isWeek=0 and foodTime<'" + dt.ToString("yyyy-MM-dd") + "'";
            var list = SqlHelperHere.ExecuteGetList<FoodView>(str);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var food = new FoodInfo();
                    food.id = item.id;
                    food.isDeleted = true;
                    food.isOn = false;
                    FoodInfoOper.Instance.Update(food);
                }

            }
        }
    }
}
