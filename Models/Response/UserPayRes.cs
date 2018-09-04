using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    public class UserPayRes
    {

    }

    public class UserPayViewRes
    {
        public UserPayViewRes(UserPayView view)
        {
            id = view.id;
            type = (int)view.type;
            money = (decimal)view.money;
            status = (bool)view.status;
            userId = (int)view.userId;
            createTime = Convert.ToDateTime(view.createTime).ToString("yyyy-MM-dd HH:mm:ss");
            takeType = view.takeType;
            takeAccount = view.takeAccount;
            outTradeNo = view.outTradeNo;
            userName = view.userName;
            userPhone = view.userPhone;
            typeName = view.typeName;
            isAdd = (int)view.isAdd;
            takeTypeName = view.takeTypeName;
        }

        public int id { get; set; }
        public int type { get; set; }
        public decimal money { get; set; }
        public bool status { get; set; }
        public int userId { get; set; }
        public string createTime { get; set; }
        public string takeType { get; set; }
        public string takeAccount { get; set; }
        public string outTradeNo { get; set; }
        public string userName { get; set; }
        public string userPhone { get; set; }
        public string typeName { get; set; }
        public int isAdd { get; set; }
        public string takeTypeName { get; set; }
    }

    public class PayRecordRes
    {
        public PayRecordRes(UserPayView view)
        {
            typeName = view.typeName;
            createTime = ((DateTime)view.createTime).ToString("yyyy-MM-dd HH:mm");
            money = view.money.ToString();
            isAdd = Convert.ToBoolean(view.isAdd);
        }

        public string typeName { get; set; }

        public string createTime { get; set; }

        public string money { get; set; }

        public bool isAdd { get; set; }
    }
}