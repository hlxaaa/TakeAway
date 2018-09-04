using DbOpertion.DBoperation;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    public class riderEnough
    {
        public riderEnough() { }
        public riderEnough(int id, bool flag)
        {
            orderId = id;
            isEnough = flag;
        }
        public int orderId { get; set; }
        public bool isEnough { get; set; }
    }


    public class OrderRes
    {
        public UserAddressForOrderRes userAddress { get; set; }

        public List<foodId_name_amount_price_isMain> listFoods { get; set; }

        public PriceAll priceAll { get; set; }

        public string balance { get; set; }

        public string coupon { get; set; }

        public bool isUseBalance { get; set; }

        public bool isUseCoupon { get; set; }

        public string remarks { get; set; }

        public string deposit { get; set; }
    }

    /// <summary>
    /// 第三方支付金额  第三方支付金额+使用余额
    /// </summary>
    public class PriceAll
    {

        public string payMoney { get; set; }

        public string price { get; set; }
    }

    public class OrderNotPayRes
    {
        public OrderNotPayRes(List<OrderTryJoin> views)
        {
            var view = views.First();
            userName = view.userName;
            areaName = view.areaName;
            createTime = Convert.ToDateTime(view.createTime).ToString("yyyy-MM-dd HH:mm:ss");
            remarks = view.remarks;
            statusType = view.statuType;
            foreach (var item in views.OrderByDescending(p => p.isMain))
            {
                foods += item.foodName + "×" + item.amount + " ";
            }
            address = view.mapAddress + view.addrDetail;
            contactPhone = view.contactPhone;
            orderId = view.orderId;
            status = (int)view.status;
        }
        public int orderId { get; set; }

        public string userName { get; set; }

        public string foods { get; set; }

        public string areaName { get; set; }

        public string createTime { get; set; }

        public string remarks { get; set; }

        public string statusType { get; set; }

        public string address { get; set; }

        public string contactPhone { get; set; }
        public int status { get; set; }
    }

    public class OrderSendingRes
    {
        public OrderSendingRes(List<OrderTryJoin> views,string lat,string lng)
        {
            var view = views.First();
            orderId = view.orderId;
            address = view.riderMapAddress;
            foodNames = new List<string>();
            var priceTemp = 0m;
            foreach (var item in views)
            {
                if ((bool)item.isMain)
                    foodNames.Add(item.foodName);
                priceTemp += (decimal)item.foodPrice;
            }
            priceAll = priceTemp.ToString();
            status = (int)view.status;
            if (status == 2)
            {
                riderName = view.riderName;
                riderPhone = view.riderPhone;

                var starTemp = view.avgStars.ToString();
                var dotIndex = starTemp.IndexOf('.');
                riderStars = Convert.ToDecimal(starTemp.Substring(0, dotIndex + 2));
                sendCount = (int)view.sendCount;

                distance = RiderInfoOper.Instance.GetDistance((int)view.riderId, (int)view.userAddressId,lat,lng);
                if (distance == 0)
                    distance = 2;
            }
        }

        public int orderId { get; set; }

        public List<string> foodNames { get; set; }

        public string priceAll { get; set; }

        public int status { get; set; }

        public string riderName { get; set; }

        public string riderPhone { get; set; }

        public decimal riderStars { get; set; }

        public int sendCount { set; get; }

        public decimal distance { set; get; }

        public RiderPosition riderInfo { get; set; }

        public string address { get; set; }
    }

    public class OrderSendingWebRes
    {
        public OrderSendingWebRes(List<OrderTryJoin> views)
        {
            var view = views.First();
            userName = view.userName;
            remarks = view.remarks;
            statusType = view.statuType;
            foreach (var item in views)
            {
                foods += item.foodName + "×" + item.amount + " ";
            }
            address = view.mapAddress + view.addrDetail;
            contactPhone = view.contactPhone;
            useBalance = (decimal)view.useBalance;
            useCoupon = (decimal)view.useCoupon;
            payTypeAndMoney = view.payType + " " + view.payMoney + "元";
            riderInfo = view.riderName + " " + view.riderPhone;
            orderId = view.orderId;
        }
        public int orderId { get; set; }

        public string userName { get; set; }

        public string foods { get; set; }

        public string remarks { get; set; }

        public string statusType { get; set; }

        public string address { get; set; }

        public string contactPhone { get; set; }

        public decimal useBalance { get; set; }

        public decimal useCoupon { get; set; }

        public string payTypeAndMoney { get; set; }

        public string riderInfo { get; set; }
    }

    public class OrderCancelledRes
    {
        public OrderCancelledRes(List<OrderTryJoin> views)
        {
            var view = views.First();
            userName = view.userName;
            statusType = view.statuType;
            foreach (var item in views)
            {
                foods += item.foodName + "×" + item.amount + " ";
            }
            contactPhone = view.contactPhone;
            endTime = view.endTime.ToString();
            orderId = view.orderId;
        }
        public int orderId { get; set; }

        public string userName { get; set; }

        public string foods { get; set; }

        public string statusType { get; set; }

        public string contactPhone { get; set; }

        public string endTime { get; set; }
    }

    /// <summary>
    /// 订单详情res
    /// </summary>
    public class OrderDetailRes
    {
        public OrderDetailRes(List<OrderTryJoin> viewList)
        {
            var view = viewList.First();
            deposit = view.deposit.ToString();
            timeArea = view.timeArea;

            if (!string.IsNullOrEmpty(view.riderPhone))
                riderPhone = view.riderPhone;
            else
                riderPhone = "";

            userAddress = new userAddress();
            userAddress.address = view.mapAddress;
            userAddress.detail = view.addrDetail;
            userAddress.name = view.contactName;
            userAddress.phone = view.contactPhone;
            listFoods = new List<foodName_Amount_Price_isMain>();
            status = (int)view.status;
            PriceRes price = new PriceRes();
            if (status == 0)
            {
                price.payMoney = (decimal)view.payMoney;
                price.useCoupon = (decimal)view.useCoupon;
                price.useBalance = (decimal)view.useBalance;

            }
            priceAll = price;
            foreach (var item in viewList)
            {
                foodName_Amount_Price_isMain food = new foodName_Amount_Price_isMain();
                food.foodName = item.foodName;
                food.amount = (int)item.amount;
                food.foodPrice = (decimal)item.foodPrice;
                food.isMain = (bool)item.isMain;
                listFoods.Add(food);
            }
            remarks = view.remarks;
            if (view.createTime.Value.AddDays(1) > DateTime.Now)
            {

                var a = (DateTime.Now.Ticks - view.createTime.Value.Ticks) / 10000000;
                var temp = 900 - Convert.ToInt32(a);//-txy 改回900
                restSeconds = temp <= 0 ? -1 : temp;
            }
            else
                restSeconds = -1;
            isActual = (bool)view.isActual;
        }

        public userAddress userAddress { get; set; }

        public int status { get; set; }

        /// <summary>
        /// 押金
        /// </summary>
        //public decimal deposit { get; set; }

        public List<foodName_Amount_Price_isMain> listFoods { get; set; }

        public string remarks { get; set; }

        public PriceRes priceAll { get; set; }

        public int restSeconds { get; set; }

        public string riderPhone { get; set; }

        public string deposit { get; set; }

        public string timeArea { get; set; }

        public bool isActual { get; set; }
    }

    public class userAddress
    {
        public string address { get; set; }

        public string detail { get; set; }

        public string name { get; set; }

        public string phone { get; set; }
    }

    public class OrderDetailForRiderRes
    {

        public OrderDetailForRiderRes(List<OrderTryJoin> viewList)
        {
            var view = viewList.First();
            userAddress = new userAddress();
            userAddress.address = view.mapAddress;
            userAddress.detail = view.addrDetail;
            userAddress.name = view.contactName;
            userAddress.phone = view.contactPhone;
            listFoods = new List<fnapi>();
            status = (int)view.status;
            foreach (var item in viewList)
            {
                fnapi food = new fnapi();
                food.foodName = item.foodName;
                food.amount = (int)item.amount;
                food.foodPrice = item.foodPrice.ToString();
                food.isMain = (bool)item.isMain;
                listFoods.Add(food);
            }
            remarks = view.remarks;
            if (view.arrivalTime != null && view.timeArea != null)
                arriveTime = Convert.ToDateTime(view.arrivalTime).ToString("yyyy-MM-dd") + " " + view.timeArea;
            else
                arriveTime = "";
        }

        public userAddress userAddress { get; set; }
        public int status { get; set; }
        public List<fnapi> listFoods { get; set; }
        public string remarks { get; set; }
        /// <summary>
        /// 判断是否能接单
        /// </summary>
        public bool isEnough { get; set; }

        public string arriveTime { get; set; }
    }


    public class OrderArrivalRes
    {
        public OrderArrivalRes(List<OrderTryJoin> views)
        {
            var view = views.First();
            userName = view.userName;
            remarks = view.remarks;
            statusType = view.statuType;
            foreach (var item in views)
            {
                foods += item.foodName + "×" + item.amount + " ";
            }
            address = view.mapAddress + view.addrDetail;
            contactPhone = view.contactPhone;
            useBalance = (decimal)view.useBalance;
            useCoupon = (decimal)view.useCoupon;
            payTypeAndMoney = view.payType + " " + view.payMoney + "元";
            riderInfo = view.riderName + " " + view.riderPhone;
            endTime = view.endTime.ToString();
            orderId = view.orderId;

        }
        public int orderId { get; set; }

        public string userName { get; set; }

        public string foods { get; set; }

        public string remarks { get; set; }

        public string statusType { get; set; }

        public string address { get; set; }

        public string contactPhone { get; set; }

        public decimal useBalance { get; set; }

        public decimal useCoupon { get; set; }

        public string payTypeAndMoney { get; set; }

        public string riderInfo { get; set; }

        public string endTime { get; set; }


    }

    public class PriceRes
    {
        public decimal useBalance { get; set; }

        public decimal useCoupon { get; set; }

        public decimal payMoney { get; set; }

    }

    public class OrderWaitRes
    {
        public OrderWaitRes(List<OrderTryJoin> views)
        {
            var view = views.First();
            userName = view.userName;
            areaName = view.areaName;
            timeArea = view.timeArea;
            remarks = view.remarks;
            statusType = view.statuType;
            foreach (var item in views)
            {
                foods += item.foodName + "×" + item.amount + " ";
            }
            address = view.mapAddress + view.addrDetail;
            contactPhone = view.contactPhone;
            useBalance = (decimal)view.useBalance;
            useCoupon = (decimal)view.useCoupon;
            payType = view.payType;
            payMoney = (decimal)view.payMoney;
            payTime = Convert.ToDateTime(view.payTime).ToString("yyyy-MM-dd");
            orderId = view.orderId;

            if (view.arrivalTime != null && view.timeArea != null)
                arriveTime = Convert.ToDateTime(view.arrivalTime).ToString("yyyy-MM-dd") + " " + view.timeArea;
            else
                arriveTime = "";
        }

        public int orderId { get; set; }

        public string payTime { get; set; }

        public decimal useBalance { get; set; }

        public decimal useCoupon { get; set; }

        public string payType { get; set; }

        public decimal payMoney { get; set; }

        public string userName { get; set; }

        public string foods { get; set; }

        public string areaName { get; set; }

        public string timeArea { get; set; }

        public string remarks { get; set; }

        public string statusType { get; set; }

        public string address { get; set; }

        public string contactPhone { get; set; }

        public string arriveTime { get; set; }
    }

    public class RiderOrderRes
    {
        public RiderOrderRes(List<OrderTryJoin> views)
        {
            var view = views.First();

            if (view.arrivalTime != null && view.timeArea != null)
                arriveTime = Convert.ToDateTime(view.arrivalTime).ToString("yyyy-MM-dd") + " " + view.timeArea;
            else
                arriveTime = "";
            orderId = view.orderId;
            address = view.mapAddress;
            addrDetail = view.addrDetail;
            status = (int)view.status;
            listFoods = new List<fnapi>();
            var temp2 = new decimal(0);
            foreach (var item in views)
            {
                if (listFoods.Count < 1 && (bool)item.isMain)
                {
                    fnapi temp = new fnapi();
                    temp.foodName = item.foodName;
                    temp.amount = (int)item.amount;
                    temp.foodPrice = item.foodPrice.ToString();
                    temp.isMain = (bool)item.isMain;
                    listFoods.Add(temp);
                }
                temp2 += (int)item.amount * (decimal)item.foodPrice;
            }
            sumPrice = temp2.ToString();
        }

        public RiderOrderRes(List<OrderTryJoin> views, int a)
        {
            var view = views.First();
            orderId = view.orderId;
            address = view.mapAddress;
            addrDetail = view.addrDetail;
            status = (int)view.status;
            listFoods = new List<fnapi>();
            var temp2 = new decimal(0);
            foreach (var item in views)
            {
                if (listFoods.Count < 1 && (bool)item.isMain)
                {
                    fnapi temp = new fnapi();
                    temp.foodName = item.foodName;
                    temp.amount = (int)item.amount;
                    temp.foodPrice = item.foodPrice.ToString();
                    temp.isMain = (bool)item.isMain;
                    listFoods.Add(temp);
                }
                temp2 += (int)item.amount * (decimal)item.foodPrice;
            }
            sumPrice = temp2.ToString();
        }

        public int orderId { get; set; }

        public string address { get; set; }

        public string addrDetail { get; set; }

        public int status { get; set; }

        public List<fnapi> listFoods { get; set; }

        /// <summary>
        /// 判断是否能接单
        /// </summary>
        public bool isEnough { get; set; }

        public string arriveTime { get; set; }

        public string sumPrice { get; set; }
    }

    public class ActiveOrderRes
    {
        /// <summary>
        /// for GetHistoryOrder
        /// </summary>
        /// <param name="orderViews"></param>
        /// <param name="a"></param>
        public ActiveOrderRes(List<OrderTryJoin> orderViews, int a)
        {
            var view = orderViews.First();
            orderId = view.orderId;
            status = (int)view.status;
            listFoods = new List<fnapi>();
            var temp = new decimal();
            foreach (var item in orderViews)
            {
                if ((bool)item.isMain)
                {
                    fnapi food = new fnapi();
                    food.foodName = item.lastFoodName;
                    food.amount = (int)item.amount;
                    food.foodPrice = item.lastFoodPrice.ToString();
                    food.isMain = (bool)item.isMain;
                    listFoods.Add(food);
                }
                temp += (int)item.amount * (decimal)item.foodPrice;
            }
            sumPrice = temp.ToString();
            riderPhone = "";
            restSeconds = -1;
        }

        /// <summary>
        /// for order.status in(0,1,2) 有倒计时
        /// </summary>
        /// <param name="orderViews"></param>
        public ActiveOrderRes(List<OrderTryJoin> orderViews)
        {
            var view = orderViews.First();
            if (view.arrivalTime != null && view.timeArea != null)
                arriveTime = Convert.ToDateTime(view.arrivalTime).ToString("yyyy-MM-dd") + " " + view.timeArea;
            else
                arriveTime = "";
            orderId = view.orderId;
            status = (int)view.status;
            listFoods = new List<fnapi>();
            var temp2 = new decimal();
            foreach (var item in orderViews)
            {
                fnapi food = new fnapi();

                food.foodName = item.foodName;
                food.amount = (int)item.amount;
                food.foodPrice = item.foodPrice.ToString();
                food.isMain = (bool)item.isMain;
                if (food.isMain)
                    listFoods.Add(food);
                temp2 += food.amount * (decimal)item.foodPrice;
            }
            sumPrice = temp2.ToString();
            riderPhone = "";

            if (view.createTime.Value.AddDays(1) > DateTime.Now)
            {
                var a = (DateTime.Now.Ticks - view.createTime.Value.Ticks) / 10000000;
                var temp = 900 - Convert.ToInt32(a);//改成900-txy
                restSeconds = temp <= 0 ? -1 : temp;
            }
            else
                restSeconds = -1;
            //isActual = (bool)view.isActual;
        }

        /// <summary>
        /// for order.status in(0,1,2)  无倒计时
        /// </summary>
        /// <param name="orderViews"></param>
        /// <param name="a"></param>
        public ActiveOrderRes(List<OrderTryJoin> orderViews, string a)
        {
            var view = orderViews.First();
            orderId = view.orderId;
            status = (int)view.status;
            listFoods = new List<fnapi>();
            var temp = new decimal();
            foreach (var item in orderViews)
            {
                fnapi food = new fnapi();
                food.foodName = item.foodName;
                food.amount = (int)item.amount;
                food.foodPrice = item.foodPrice.ToString();
                food.isMain = (bool)item.isMain;
                listFoods.Add(food);
                temp += food.amount * (decimal)item.foodPrice;
            }
            sumPrice = temp.ToString();
            riderPhone = view.riderPhone;
            restSeconds = -1;
        }

        public int orderId { get; set; }

        public string riderPhone { get; set; }

        public int status { get; set; }

        public List<fnapi> listFoods { get; set; }

        public int restSeconds { get; set; }

        public string sumPrice { get; set; }

        public string arriveTime { get; set; }
    }

    public class foodName_Amount_Price_isMain
    {
        public string foodName { get; set; }

        public int amount { get; set; }

        public decimal foodPrice { set; get; }

        public bool isMain { get; set; }
    }

    public class fnapi
    {
        public string foodName { get; set; }

        public int amount { get; set; }

        public string foodPrice { set; get; }

        public bool isMain { get; set; }

    }

    public class CommentRes
    {
        public CommentRes(List<OrderTryJoin> views, string apiHost)
        {
            var view = views.First();
            listFoods = new List<foodId_name_img>();
            foreach (var item in views)
            {
                foodId_name_img fni = new foodId_name_img();
                fni.foodId = (int)item.foodId;
                fni.foodName = item.foodName;
                fni.foodImg =  apiHost + item.foodImg;
                listFoods.Add(fni);
            }
            //foodList = temp;
            riderName = view.riderName;
            riderImg =  apiHost + "/img/riderhead/rider.png";
        }


        public List<foodId_name_img> listFoods { get; set; }

        public string riderName { get; set; }

        public string riderImg { get; set; }
    }

    public class foodId_name_img
    {
        public string foodImg { get; set; }

        public int foodId { get; set; }

        public string foodName { get; set; }
    }

    public class FoodsOnMapRes
    {
        public FoodsOnMapRes(FoodView food, string host)
        {
            foodId = food.id;
            foodName = food.foodName;
            foodPrice = food.foodPrice.ToString();
            foodTagId = (int)food.foodTagId;
            foodImg =  host + food.foodImg;
            if (food.deposit > 0)
                isCycle = true;
            else
                isCycle = false;
            deposit = food.deposit.ToString();
            amount = 0;
            foodStars = Math.Round((decimal)food.star, 1, MidpointRounding.AwayFromZero);//-txy备忘
        }

        public FoodsOnMapRes(FoodView food, string host, int Amount)
        {
            foodId = food.id;
            foodName = food.foodName;
            foodPrice = food.foodPrice.ToString();
            foodTagId = (int)food.foodTagId;
            foodImg =  host + food.foodImg;
            if (food.deposit > 0)
                isCycle = true;
            else
                isCycle = false;
            deposit = food.deposit.ToString();
            amount = Amount;
            foodStars = Math.Round((decimal)food.star, 1, MidpointRounding.AwayFromZero);
        }

        public FoodsOnMapRes(RiderStockJoin food, string host)
        {
            foodId = food.id;
            foodName = food.foodName;
            foodPrice = food.foodPrice.ToString();
            foodTagId = (int)food.tagId;
            foodImg = host + food.foodImg;
            if (food.deposit > 0)
                isCycle = true;
            else
                isCycle = false;
            //isCycle = food.iscycle;
            deposit = food.deposit.ToString();
            amount = 0;
        }

        public FoodsOnMapRes(RiderStockJoin food, string host, int Amount)
        {
            foodId = food.id;
            foodName = food.foodName;
            foodPrice = food.foodPrice.ToString();
            foodTagId = (int)food.tagId;
            foodImg =host + food.foodImg;
            if (food.deposit > 0)
                isCycle = true;
            else
                isCycle = false;
            //isCycle = food.iscycle;
            deposit = food.deposit.ToString();
            amount = Amount;
        }

        /// <summary>
        ///
        /// </summary>
        public Int32 foodId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string foodPrice { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32 foodTagId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String foodImg { get; set; }
        /// <summary>
        ///
        /// </summary>
        //public String foodText { get; set; }
        /// <summary>
        ///
        /// </summary>
        //public Boolean? isDeleted { get; set; }
        /// <summary>
        ///
        /// </summary>
        //public Boolean? isMain { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Boolean? isCycle { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string deposit { get; set; }

        public int amount { get; set; }

        public decimal foodStars { get; set; }
    }

    public class List_FoodsOnMapRes
    {
        public List<FoodsOnMapRes> foods { get; set; }

        public string money { get; set; }
    }

    public class foodId_name_amount_price_isMain
    {
        public foodId_name_amount_price_isMain() { }

        public foodId_name_amount_price_isMain(OrderTryJoin view)
        {
            foodId = (int)view.foodId;
            foodName = view.foodName;
            amount = (int)view.amount;
            foodPrice = view.foodPrice.ToString();
            isMain = (bool)view.isMain;
        }

        public foodId_name_amount_price_isMain(FoodInfo food, int Amount)
        {
            foodId = food.id;
            foodName = food.foodName;
            amount = Amount;
            foodPrice = food.foodPrice.ToString();
            isMain = (bool)food.isMain;
        }

        public foodId_name_amount_price_isMain(FoodView view)
        {
            foodId = view.id;
            foodName = view.foodName;
            foodPrice = view.foodPrice.ToString();
            isMain = (bool)view.isMain;
        }

        public int foodId { get; set; }

        public string foodName { get; set; }

        public int amount { get; set; }

        public string foodPrice { get; set; }

        public bool isMain { get; set; }
    }

    //public class finapi
    //{
    //    public finapi() { }

    //    public finapi(OrderTryJoin view)
    //    {
    //        foodId = (int)view.foodId;
    //        foodName = view.foodName;
    //        amount = (int)view.amount;
    //        foodPrice = view.foodPrice.ToString();
    //        isMain = (bool)view.isMain;
    //    }

    //    public finapi(FoodInfo food, int Amount)
    //    {
    //        foodId = food.id;
    //        foodName = food.foodName;
    //        amount = Amount;
    //        foodPrice = food.foodPrice.ToString();
    //        isMain = (bool)food.isMain;
    //    }

    //    public finapi(FoodView view)
    //    {
    //        foodId = view.id;
    //        foodName = view.foodName;
    //        foodPrice = view.foodPrice.ToString();
    //        isMain = (bool)view.isMain;
    //    }

    //    public int foodId { get; set; }

    //    public string foodName { get; set; }

    //    public int amount { get; set; }

    //    public string foodPrice { get; set; }

    //    public bool isMain { get; set; }
    //}

    public class Price_Deposit
    {
        public decimal price { get; set; }

        public decimal deposit { get; set; }
    }

    public class payRes
    {
        public string payType { get; set; }
        public string payStr { get; set; }
    }
}