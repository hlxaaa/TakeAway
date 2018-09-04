using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using takeAwayWebApi.Attribute;

namespace takeAwayWebApi.Controllers
{
   
    //[ValidateModel]
    [WebApiException]
    public class BaseController : ApiController
    {
        
    }
}
