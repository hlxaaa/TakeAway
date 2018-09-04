using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using takeAwayWebApi.Helper;

namespace takeAwayWebApi.Models.Response
{
    public class RiderRes
    {
        public RiderRes(RiderView rider, string tokenStr) {
            riderId = rider.id;
            riderType = rider.riderType;
            riderStatus = rider.riderStatus;
            riderAreaId = rider.riderAreaId;
            areaName = rider.areaName;
            riderNo = rider.riderNo;
            sexType = rider.sexType;
            name = rider.name;
            phone = rider.phone;
            Token = tokenStr;
            var starTemp = rider.avgStars.ToString();
            var dotIndex = starTemp.IndexOf('.');
            stars = starTemp.Substring(0, dotIndex + 2);
            sendCount = (int)rider.sendCount;
        }
        public string Token { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32 riderId { get; set; }
        ///// <summary>
        /////
        ///// </summary>
        //public String riderAccount { get; set; }
        ///// <summary>
        /////
        ///// </summary>
        //public String riderPwd { get; set; }
        /// <summary>
        ///0电瓶车1汽车2自提点
        /// </summary>
        public Int32? riderType { get; set; }
        /// <summary>
        ///0下线1在线
        /// </summary>
        public Boolean? riderStatus { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? riderAreaId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String areaName { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String riderNo { get; set; }
        /// <summary>
        ///true男false女
        /// </summary>
        public Boolean? sexType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String name { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String phone { get; set; }
        /// <summary>
        ///
        /// </summary>
        //public String headImg { get; set; }

        public string stars { get; set; }

        //public int starCount { get; set; }

        public int sendCount { get; set; }

    }

    public class AssignRiderRes
    {
        public AssignRiderRes(RiderInfo rider)
        {
            riderId = rider.id;
            riderName = rider.name;
        }

        public int riderId { get; set; }

        public string riderName { get; set; }

    }

    public class riderId_name
    {
        public riderId_name(RiderInfo rider)
        {
            riderId = rider.id;
            riderName = rider.name;
        }

        public int riderId { get; set; }

        public string riderName { get; set; }
    }

    public class riderId_name2
    {
        public riderId_name2(RiderInfo rider)
        {
            riderId = rider.id;
            name = rider.name;
        }

        public int riderId { get; set; }

        public string name { get; set; }
    }

    public class riderType2 {
        public riderType2(RiderInfo  rider) {
            riderId = rider.id;
            riderName = rider.name;
            address = rider.mapAddress;
            var rp = CacheHelper.GetRiderPosition(riderId);
            lat = rp.lat;
            lng = rp.lng;
        }

        public riderType2(UserAddressView view) {
            riderId = (int)view.riderId;
            riderName = view.riderName;
            address = view.mapAddress;
            lat = view.lat;
            lng = view.lng;
            addressId = view.id;
            phone = view.phone;
        }


        public int riderId { get; set; }

        public string riderName { get; set; }

        public string phone { get; set; }

        public string lat { get; set; }
        public string lng { get; set; }
        public string address { get; set; }
        public int addressId { get; set; }
    }
}