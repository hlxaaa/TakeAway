using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace takeAwayWebApi.Models.Response
{
    public class AreaRes
    {
        public int areaId { get; set; }

        public string areaName { get; set; }

    }

    public class AreaId_Name
    {
        public AreaId_Name(AreaInfo area)
        {
            areaId = area.id;
            areaName = area.areaName;
        }
        public AreaId_Name() { }

        public int areaId { get; set; }

        public string areaName { get; set; }
    }
}