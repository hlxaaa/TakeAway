using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Request
{
    public class HomeOrderReq:UserToken
    {
        public string addressId { get; set; }
        public string listFoods { get; set; }

        /// <summary>
        /// 是否使用余额
        /// </summary>
        public string isUseBalance { get; set; }

        /// <summary>
        /// 是否使用礼金券
        /// </summary>
        public string isUseCoupon { get; set; }

        public string foodId { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        public string areaId { get; set; }

        public string riderId { get; set; }

        public string remarks { get; set; }

        public string timeArea { get; set; }

    }

    [Serializable]
    public class foodId_Amount {

        public foodId_Amount() { }

        public foodId_Amount(OrderFoods ofs) {
            foodId = (int)ofs.foodId;
            amount = (int)ofs.amount;
        }

        public foodId_Amount(int FoodId, int Amount) {
            foodId = FoodId;
            amount = Amount;
        }

        public int foodId { get; set; }

        public int amount { get; set; }

    }

    [Serializable]
    public class AreaId_foodId_amount {
        public int areaId { get; set; }
        public int foodId { get; set; }
        public int amount { get; set; }
    }


    [Serializable]
    public class foodId_Name {
        public foodId_Name(int FoodId,string FoodName) {
            foodId = FoodId;
            foodName = FoodName;
        }

        public foodId_Name(FoodInfo food) {
            foodId = food.id;
            foodName = food.foodName;
        }

        public int foodId { get; set; }
        public string foodName { get; set; }
    }
}