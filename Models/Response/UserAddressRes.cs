using System;

namespace DbOpertion.Models
{
    public class UserAddressRes
    {
        public UserAddressRes(UserAddress ua)
        {
            addressId = ua.id;
            userId = ua.userId;
            name = ua.name;
            phone = ua.phone;
            address = ua.mapAddress;
            lat = ua.lat;
            lng = ua.lng;
            detail = ua.detail;
            isRecently = (bool)ua.isRecently;
        }


        public UserAddressRes() { }
        /// <summary>
        ///
        /// </summary>
        public Int32 addressId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Int32? userId { get; set; }
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
        public String address { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String lat { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String lng { get; set; }
        /// <summary>
        ///
        /// </summary>
        public String detail { get; set; }

        public bool isRecently { get; set; }
}

    public class UserAddressForOrderRes {

        public UserAddressForOrderRes(UserAddress ua) {
            addressId = ua.id;
            name = ua.name;
            phone = ua.phone;
            address = ua.mapAddress;
            detail = ua.detail;
        }

        public UserAddressForOrderRes(UserAddressView view) {
            if (view.riderType != 2)
            {
                addressId = view.id;
                name = view.name;
                phone = view.phone;
                address = view.mapAddress;
                detail = view.detail;
            }
            else
            {
                addressId = view.id;
                name = view.riderName;
                address = view.mapAddress;
                phone = view.phone;
                detail = "";
            }
        }

        public int addressId { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string detail { get; set; }
    }
}
