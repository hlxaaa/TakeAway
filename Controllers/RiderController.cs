using Aliyun.Acs.Dysmsapi.Model.V20170525;
using AliyunHelper.SendMail;
using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using takeAwayWebApi.Attribute;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers
{
    public class RiderController : BaseController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage Login(RiderLoginReq req)
        {
            #region 密码登录

            var account = req.account;
            var pwd = req.password;
            var password = pwd;
            var list = RiderInfoOper.Instance.GetByAccount(account);
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "账号不存在");
            else
            {
                var riderHere = list.First();
                if (string.IsNullOrEmpty(password))
                {
                    return ControllerHelper.Instance.JsonResult(500, "请先输入密码");
                }
                else if (password != riderHere.riderPwd)
                {
                    return ControllerHelper.Instance.JsonResult(500, "密码错误");
                }
                else
                {
                    var deviceToken = req.deviceToken;
                    if (!string.IsNullOrEmpty(deviceToken))
                    {
                        riderHere.deviceToken = deviceToken;
                        RiderInfoOper.Instance.Update(riderHere);
                    }

                    var token = CacheHelper.SetRiderToken(riderHere);
                    var riderView = RiderInfoOper.Instance.GetViewByAccount(account);
                    if(riderView==null)
                        return ControllerHelper.Instance.JsonResult(500,  "账号不存在或暂未分配区域");
                    RiderRes res = new RiderRes(riderView, token.GetToken());
                    return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
                }

            }

            #endregion

            return ControllerHelper.Instance.JsonResult(500, "意想不到的登录失败");

        }

        /// <summary>
        /// 交通工具设置
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SetType(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var rider = RiderInfoOper.Instance.GetById(riderId);
            if (rider.riderType != 2) {
                if (rider.riderType == 1)
                    rider.riderType = 0;
                else
                    rider.riderType = 1;
            }
            //var temp = rider.riderType + 1;
            //rider.riderType = temp > 2 ? 0 : temp;
           
            try
            {
                var riderHere = new RiderInfo();
                riderHere.id = rider.id;
                riderHere.riderType = rider.riderType;
                RiderInfoOper.Instance.Update(riderHere);
                if (rider.riderType == 2)
                {
                    var ua = UserAddressOper.Instance.GetByRiderId(riderId);
                    if (ua == null)
                    {
                        ua = new UserAddress();
                        ua.riderId = riderId;
                        var rp = CacheHelper.GetRiderPosition(riderId);
                        ua.lat = rp.lat;
                        ua.lng = rp.lng;
                        ua.mapAddress = rider.mapAddress;
                        ua.phone = rider.phone;
                        UserAddressOper.Instance.Add(ua);
                    }
                }
                return ControllerHelper.Instance.JsonResult(200,"riderType",rider.riderType.ToString(), "更换成功");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "更换失败");
            }
        }

        /// <summary>
        /// 骑手获取可选取的区域
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetArea(RiderReq req) {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var r = AreaInfoOper.Instance.GetAreaList();
            if(r!=null)
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(r), "");
            return ControllerHelper.Instance.JsonEmptyArr(200, "暂无可选区域");
        }

        /// <summary>
        /// 骑手设置自己所在的区域
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage SetArea(SetAreaReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var areaId = Convert.ToInt32(req.areaId);
            //rider.riderAreaId = areaId;
            try
            {
                var rider = new RiderInfo();
                rider.id = riderId;
                rider.riderAreaId = areaId;
                RiderInfoOper.Instance.Update(rider);
                return ControllerHelper.Instance.JsonResult(200, "设置成功");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "服务器错误");
            }
        }

        /// <summary>
        /// 提交骑手位置
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage SetPosition(SetPosition req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var rider = RiderInfoOper.Instance.GetById(riderId);
            var lat = req.lat;
            var lng = req.lng;
            var riderType = rider.riderType;

            CacheHelper.SetRiderPosition(riderId, lat, lng,(int)riderType);
            return ControllerHelper.Instance.JsonResult(200, "上传地址成功");
        }

        [HttpPost]
        public HttpResponseMessage ToggleRiderStatus(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var rider = RiderInfoOper.Instance.GetById(riderId);
            if ((bool)rider.riderStatus)
                rider.riderStatus = false;
            else
                rider.riderStatus = true;

            var riderHere = new RiderInfo();
            riderHere.id = riderId;
            riderHere.riderStatus = rider.riderStatus;

            RiderInfoOper.Instance.Update(riderHere);
            JObject jo = new JObject();
            jo.Add("riderStatus", rider.riderStatus);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
        }

        /// <summary>
        /// 获取区域内其他骑手的位置
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetOtherRidersPosition(RiderReq req)
        {
            var riderId = Convert.ToInt32(req.riderId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetRiderToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != riderId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var rider = RiderInfoOper.Instance.GetById(riderId);
            var areaId = (int)rider.riderAreaId;
            var list = RiderInfoOper.Instance.GetRiderPositionInArea(areaId, riderId);
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "无其他骑手");

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(list), "");
        }

        //分割线，以上已写接口文档
        /*————————————————————————————————————————————————*/

        /// <summary>
        /// 留着，不知道需不需要
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public HttpResponseMessage SendMail(UserReq user)
        {
            var phone = user.phone;
            if (true)
            {
                string verification = CacheHelper.SetRiderVerificationCode(phone);
                Enum_SendEmailCode SendEmail;
                SendEmail = Enum_SendEmailCode.UserRegistrationVerificationCode;
                SendSmsResponse Email = AliyunHelper.SendMail.SendMail.Instance.SendEmail(phone, verification, SendEmail);
                //string str = "";
                if (Email.Code.ToUpper() == "OK")
                    return ControllerHelper.Instance.JsonResult(200, "短信验证码发送成功");
                else
                    return ControllerHelper.Instance.JsonResult(500, Email.Message);
            }
            else
            {
                //string str = JsonHelper.JsonMsg(false, "请过段时间重新发送");
                return ControllerHelper.Instance.JsonResult(500, "请过段时间重新发送");
            }
        }

        
    }
}
