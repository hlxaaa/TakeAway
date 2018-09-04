using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Request
{
    public class UserToken
    {
        //[Required(ErrorMessage = "请填写用户id", AllowEmptyStrings = false)]
        public string userId { get; set; }

        //[Required(ErrorMessage = "请填写Token", AllowEmptyStrings = false)]
        public string Token { get; set; }
    }

}