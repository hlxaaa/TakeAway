using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Request
{
    public class UserAddrReq : UserToken
    {
        public string addressId { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        public string address { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        public string detail { get; set; }


    }

    public class AddAddrReq : UserToken
    {
        [Required(ErrorMessage = "请填写联系人", AllowEmptyStrings = false)]
        public string name { get; set; }
        [Required(ErrorMessage = "请填写联系方式", AllowEmptyStrings = false)]
        public string phone { get; set; }
        [Required(ErrorMessage = "请填写地址", AllowEmptyStrings = false)]
        public string address { get; set; }
        [Required(ErrorMessage = "请填写纬度", AllowEmptyStrings = false)]
        public string lat { get; set; }
        [Required(ErrorMessage = "请填写经度", AllowEmptyStrings = false)]
        public string lng { get; set; }
        public string detail { get; set; }
    }

    public class UpdateAddrReq : UserToken
    {
        [Required(ErrorMessage = "地址id错误", AllowEmptyStrings = false)]
        public string addressId { get; set; }

        [Required(ErrorMessage = "请填写联系人", AllowEmptyStrings = false)]
        public string name { get; set; }
        [Required(ErrorMessage = "请填写联系方式", AllowEmptyStrings = false)]
        public string phone { get; set; }
        [Required(ErrorMessage = "请填写地址", AllowEmptyStrings = false)]
        public string address { get; set; }
        [Required(ErrorMessage = "请填写纬度", AllowEmptyStrings = false)]
        public string lat { get; set; }
        [Required(ErrorMessage = "请填写经度", AllowEmptyStrings = false)]
        public string lng { get; set; }
        public string detail { get; set; }


    }
}