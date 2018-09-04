using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Models.Request
{
    public class HomePageReq:UserToken
    {
        public string tagId { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        public string areaId { get; set; }

        public string orderId { get; set; }

        public string pageIndex { get; set; }
    }

    public class GetAreaReq {
        [LatValidate]
        public string lat { get; set; }
        [LngValidate]
        public string lng { get; set; }
    }

    public class GetRidersReq:GetAreaReq {
        [Required(ErrorMessage="需要区域id",AllowEmptyStrings=false)]
        public string areaId { get; set; }
        [Required(ErrorMessage = "需要菜品id", AllowEmptyStrings = false)]
        public string foodId { get; set; }
    }
}