using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Request
{
    public class RiderToken
    {
        //[Required(ErrorMessage = "请填写骑手id", AllowEmptyStrings = false)]
        public string riderId { get; set; }

        //[Required(ErrorMessage = "请填写Token", AllowEmptyStrings = false)]
        public string Token { get; set; }
    }
}