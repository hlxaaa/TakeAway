using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{

    public class UserRes
    {
        public UserRes() { }

        public UserRes(UserInfo user, string host, string tokenStr)
        {
            userId = user.id;
            name = user.userName;
            balance = user.userBalance.ToString();
            wechat = user.wechat;
            qq = user.qq;
            phone = user.userPhone;
            if (user.birthday == null)
                birthday = "";
            else
                birthday = Convert.ToDateTime(user.birthday).ToString("yyyy-MM-dd");
            coupon = user.coupon.ToString();
            //addresses = user.addresses;
            Token = tokenStr;
            if (!string.IsNullOrEmpty(user.userHead))
                headImg = host + user.userHead;
            else
                headImg = "";
        }

        public String Token { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Int32 userId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String name { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string balance { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String wechat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String qq { get; set; }

        /// <summary>
        ///
        /// </summary>
        public String phone { get; set; }

        /// <summary>
        ///
        /// </summary>
        public String headImg { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String birthday { get; set; }
        /// <summary>
        ///优惠券金额
        /// </summary>
        public string coupon { get; set; }
        ///// <summary>
        /////
        ///// </summary>
        //public String addresses { get; set; }

        public bool hasPwd { get; set; }

    }

    public class UserOpenInfoForWeb
    {
        public UserOpenInfoForWeb(UserOpenView view)
        {
            userName = view.userName;
            lat = view.lat;
            lng = view.lng;
            var temp1 = Convert.ToDateTime(view.openTime).ToString("yyyy-MM-dd HH:mm:ss");
            openTime = view.openTime == null ? "" : temp1;
            var temp2 = Convert.ToDateTime(view.closeTime).ToString("yyyy-MM-dd HH:mm:ss");
            closeTime = view.closeTime == null ? "" : temp2;
            useTime = view.useTime;
            address = view.address;
        }

        public string userName { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string openTime { get; set; }

        public string closeTime { get; set; }
        public int? useTime { get; set; }
    }

    [Serializable]
    public class UserArticleClick
    {
        public UserArticleClick(int id, bool flag)
        {
            userId = id;
            isClicked = flag;
        }

        public int userId { get; set; }
        public bool isClicked { get; set; }
    }

    public class UserInfoRes
    {
        public UserInfoRes(UserView view, string host, string tokenStr)
        {
            Token = tokenStr;
            userId = view.id;
            if (!string.IsNullOrEmpty(view.userHead))
                headImg =host + view.userHead;
            else
                headImg = "";
            name = view.userName;
            if (birthday == null)
                birthday = "";
            else
                birthday = Convert.ToDateTime(view.birthday).ToString("yyyy-MM-dd");
        }

        public UserInfoRes(UserInfo view, string host, string tokenStr)
        {
            Token = tokenStr;
            userId = view.id;
            if (!string.IsNullOrEmpty(view.userHead))
                headImg =  host + view.userHead;
            else
                headImg = "";
            name = view.userName;
            if (birthday == null)
                birthday = "";
            else
                birthday = Convert.ToDateTime(view.birthday).ToString("yyyy-MM-dd");
        }

        public string Token { get; set; }

        public int userId { get; set; }

        public string headImg { get; set; }

        public string name { get; set; }

        public string birthday { get; set; }
    }

    public class UserWebRes
    {
        public UserWebRes(UserInfo user)
        {
            //var arr = user.addresses.Split(',');
            //foreach (var item in arr)
            //{
            //    addresses.Add(item);
            //}
            password = user.userPwd;
            //List<string> list = new List<string>();
            //addresses = addresses == null ? list : addresses;
            userId = user.id;
            userName = user.userName;
            userBalance = user.userBalance;
            wechat = user.wechat;
            qq = user.qq;
            phone = user.userPhone;
            //headImg = user.userHead;
            if (user.birthday == null)
                birthday = "";
            else
                birthday = Convert.ToDateTime(user.birthday).ToString("yyyy-MM-dd");
            coupon = user.coupon;
            headImg = user.userHead;
            if (headImg == "" || headImg == null)
                headImg = "/img/icon/appicon.png";
        }

        /// <summary>
        ///
        /// </summary>
        public Int32 userId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String userName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? userBalance { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String wechat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String qq { get; set; }

        /// <summary>
        ///
        /// </summary>
        public String phone { get; set; }

        ///// <summary>
        /////
        ///// </summary>
        public String headImg { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string birthday { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Decimal? coupon { get; set; }
        /// <summary>
        ///
        /// </summary>
        //public List<string> addresses { get; set; }

        //public String addressStr { get; set; }

        public String password { get; set; }
    }

    public class WalletRes
    {
        public WalletRes(decimal Balance, decimal Coupon)
        {
            balance = Balance.ToString();
            coupon = Coupon.ToString();
        }

        public string balance { get; set; }

        public string coupon { get; set; }
    }
}