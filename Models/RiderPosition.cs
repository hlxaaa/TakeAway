using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models
{
    [Serializable]
    public class RiderPosition
    {
        public RiderPosition() { }

        public RiderPosition(RiderInfo rider) {
            riderId = rider.id;
            lat = rider.lat;
            lng = rider.lng;
            riderType = (int)rider.riderType;
        }
        public Int32 riderId { get; set; }
        public String lat { get; set; }
        public String lng { get; set; }
        /// <summary>
        /// 0电瓶车1汽车2自提点
        /// </summary>
        public int riderType { get; set; }
    }
}