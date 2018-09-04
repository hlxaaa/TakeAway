using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Models.Request
{
    public class UserReq : UserToken
    {
        public string address { get; set; }

        public string couponNo { get; set; }

        public string spbill_create_ip { get; set; }

        public string payType { get; set; }

        public string money { get; set; }

        public string takeAccount { get; set; }

        //public string userId { get; set; }

        public string qq { get; set; }

        public string wechat { get; set; }

        public string phone { get; set; }

        public string verificationCode { get; set; }

        //public string Token { get; set; }

        /// <summary>
        /// 登录方式(Mail、WeChat、QQ、Token、Password)
        /// </summary>
        public string LoginMode { get; set; }

        //[Required(ErrorMessage = "密码不能为空", AllowEmptyStrings = false)]
        [PwdValidate]
        public string password { get; set; }

        public string vPassword { get; set; }

        public string newPassword { get; set; }

        /// <summary>
        /// 绑定方式（QQ，WeChat）
        /// </summary>
        public string BindMode { get; set; }

        /// <summary>
        /// 解绑方式（QQ，WeChat）
        /// </summary>
        public string RemoveMode { get; set; }

        public string name { get; set; }

        public string birthday { get; set; }

        public string headImg { get; set; }

        public string suggestion { get; set; }

        public string payMoney { get; set; }

        public string deviceToken { get; set; }

        /// <summary>
        /// 设备类型ios或android
        /// </summary>
        public string deviceType { get; set; }

        public string pageIndex { get; set; }

        public string takeType { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }
    }

    public class RegisterReq
    {
        [Required(ErrorMessage = "请填写手机号", AllowEmptyStrings = false)]
        public string phone { get; set; }

        [Required(ErrorMessage = "请填写验证码", AllowEmptyStrings = false)]
        public string verificationCode { get; set; }

        [Required(ErrorMessage = "请填写密码", AllowEmptyStrings = false)]
        public string password { get; set; }
    }

    public class FindPwdReq
    {

        [PwdValidate]
        public string newPassword { get; set; }

        public string phone { get; set; }

        public string verificationCode { get; set; }
    }

    public class ResetPwdReq : UserToken
    {
        [PwdValidate]
        public string password { get; set; }

        [PwdValidate]
        public string newPassword { get; set; }

    }
}