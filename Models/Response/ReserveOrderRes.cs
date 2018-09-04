using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    /// <summary>
    /// 返回预订单支付页的数据
    /// </summary>
    public class ReserveOrderRes
    {
        public UserAddressForOrderRes userAddress { get; set; }

        public List<foodId_name_amount_price_isMain> listFoods { get; set; }

        public PriceAll priceAll { get; set; }

        public string balance { get; set; }

        public string coupon { get; set; }

        public bool isUseBalance { get; set; }

        public bool isUseCoupon { get; set; }

        public string deposit { get; set; }

        public string timeArea { get; set; }

        public string remarks { get; set; }
    }

    public class ReserveFoodRes
    {
        public ReserveFoodRes(FoodView view, string host)
        {
            foodId = view.id;
            foodName = view.foodName;
            foodPrice = view.foodPrice.ToString();
            secondTag = view.secondTagName;
            foodImg =  host + view.foodImg;
        }

        public int foodId { get; set; }

        public string foodName { get; set; }

        public string foodPrice { get; set; }

        public string secondTag { get; set; }

        public string foodImg { get; set; }
    }
}