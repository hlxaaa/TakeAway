using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Models
{
    public class PlatformRes
    {
        public PlatformRes(Platform p) {
            account = p.account;
            pwd = p.pwd;
            level = (int)p.level;
            id = p.id;
        }
        public int id { get; set; }
        public string account { get; set; }
        public string pwd { get; set; }
        public int level { get; set; }
    }

    public class TipsSettingsRes {
        public TipsSettingsRes(string csd, string rct, string rbt, string ost, string oat, string bgt, string sart,string dis)
        {
            couponSaveDays = csd;
            riderCancelTips = rct;
            recoveryBoxTips = rbt;
            orderSendingTips = ost;
            orderArrivelTips = oat;
            boxGetTips = bgt;
            serverAssignRiderTips = sart;
            discount = dis;
        }

        public TipsSettingsRes(SettingRes set)
        {
            couponSaveDays = set.couponSaveDays ;
            riderCancelTips = set.riderCancelTips;
            recoveryBoxTips = set.recoveryBoxTips;
            orderSendingTips = set.orderSendingTips;
            orderArrivelTips = set.orderArrivelTips;
            boxGetTips = set.boxGetTips;
            serverAssignRiderTips = set.serverAssignRiderTips;
            discount = set.discount;
        }

        public string couponSaveDays { get; set; }

        public string riderCancelTips { get; set; }

        public string recoveryBoxTips { get; set; }

        public string orderSendingTips { get; set; }

        public string orderArrivelTips { get; set; }

        public string boxGetTips { get; set; }

        public string serverAssignRiderTips { get; set; }
        public string discount { get; set; }

    }
}