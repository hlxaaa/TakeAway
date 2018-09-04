using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Models.Request
{
    public class TestReq
    {
        
        public int id { get; set; }
        
        public decimal dec { get; set; }

        [Required(AllowEmptyStrings=false)]
        [PwdValidate]
        public string phone { get; set; }
        
        public string bools { get; set; }
        
        public string str { get; set; }

        public int userId { get; set; }

        //public object alipay_trade_pay_response { get; set; }
    }

    //[Serializable]
    public class testrr {
        public string name { get; set; }
        public int sex { get; set; }
    }
    
}
