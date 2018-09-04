using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using takeAwayWebApi.Attribute;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers
{
    public class UserAddressController : BaseController
    {

        //UserAddressOper oper = new UserAddressOper();

        /// <summary>
        /// 获取用户地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetAddress(UserAddrReq req)
        {
            var userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var list = UserAddressOper.Instance.GetAddrByUserId(userId);
            if (list.Count > 0)
            {
                List<UserAddressRes> list_r = new List<UserAddressRes>();
                foreach (var item in list)
                {
                    list_r.Add(new UserAddressRes(item));
                }
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(list_r), "");
            }
            else
            {
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无地址");
            }
        }

        /// <summary>
        /// 新增地址接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage AddAddress(AddAddrReq req)
        {
            UserAddress ua = new UserAddress();
            ua.userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != ua.userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            ua.name = req.name;
            ua.phone = req.phone;
            ua.mapAddress = req.address;
            ua.detail = req.detail;
            ua.lat = req.lat;
            ua.lng = req.lng;

            try
            {
                var id = UserAddressOper.Instance.Add(ua);
                return ControllerHelper.Instance.JsonResult(200, "addressId", id.ToString(), "添加地址成功");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "网络不稳定");
            }
        }

        /// <summary>
        /// 更新地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage UpdateAddress(UpdateAddrReq req)
        {
            UserAddress ua = new UserAddress();
            ua.userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != ua.userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            ua.name = req.name;
            ua.phone = req.phone;
            ua.mapAddress = req.address;
            ua.detail = req.detail;
            ua.lat = req.lat;
            ua.lng = req.lng;
            ua.id = Convert.ToInt32(req.addressId);

            try
            {
                UserAddressOper.Instance.Update(ua);
                return ControllerHelper.Instance.JsonResult(200, "addressId", req.addressId.ToString(), "更新地址成功");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(500, "出问题了");
            }
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteAddress(UserAddrReq req)
        {
            var userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");


            var addressId = Convert.ToInt32(req.addressId);
            UserAddressOper.Instance.Delete(addressId);
            return ControllerHelper.Instance.JsonResult(200, "删除地址成功");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ChooseAddress(UserAddrReq req)
        {
            var userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var addressId = Convert.ToInt32(req.addressId);
            try
            {
                UserAddressOper.Instance.ChangeRecentlyAddr(userId, addressId);
                return ControllerHelper.Instance.JsonResult(200, "addressId", addressId.ToString(), "选择成功");
            }
            catch (Exception)
            {
                return ControllerHelper.Instance.JsonResult(500, "服务器错误");
            }
        }
        //分割线，以上已写接口文档
        /*————————————————————————————————————————————————*/


    }
}
