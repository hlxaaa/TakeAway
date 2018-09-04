using Aliyun.Acs.Dysmsapi.Model.V20170525;
using AliyunHelper.SendMail;
using Common.Helper;
using Common.Result;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using takeAwayWebApi.Attribute;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers
{
    public class UserController : BaseController
    {
        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();
        int size = 10;
        //int couponDays = 7;//兑换码过期天数

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage SendMail(UserReq req)
        {
            var phone = req.phone;
            if (true)
            {
                string verification = CacheHelper.SetUserVerificationCode(phone); //SetUserVerificationCode;
                Enum_SendEmailCode SendEmail;
                SendEmail = Enum_SendEmailCode.UserRegistrationVerificationCode;
                SendSmsResponse Email = AliyunHelper.SendMail.SendMail.Instance.SendEmail(phone, verification, SendEmail);
                //string str = "";
                if (Email.Code.ToUpper() == "OK")
                    return ControllerHelper.Instance.JsonResult(200, "短信验证码发送成功");
                else
                    return ControllerHelper.Instance.JsonResult(500, "请过段时间重新发送");
            }
            else
            {
                //string str = JsonHelper.JsonMsg(false, "请过段时间重新发送");
                return ControllerHelper.Instance.JsonResult(500, "请过段时间重新发送");
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateModel]
        public HttpResponseMessage Login(UserReq req)
        {
            var userId = req.userId;
            var qq = req.qq;
            var wechat = req.wechat;
            var phoneNumber = req.phone;
            var code = req.verificationCode;
            var tokenString = req.Token;
            var mode = req.LoginMode;
            var userinfo = new UserInfo();
            Token tokenTemp = new Token();

            bool hasPwd = true;

            #region 短信验证登录
            if (mode == "Mail")
            {
                if (string.IsNullOrEmpty(code))
                {
                    return ControllerHelper.Instance.JsonResult(500, "验证码为空");
                }
                else
                {
                    var codeHere = CacheHelper.GetUserVerificationCode(phoneNumber);
                    if (codeHere == null)
                    {
                        return ControllerHelper.Instance.JsonResult(500, "请重新发送验证码");
                    }
                    else if (codeHere != code)
                    {
                        return ControllerHelper.Instance.JsonResult(500, "验证码错误");
                    }
                    else
                    {
                        var list = UserInfoOper.Instance.GetByPhone(phoneNumber);
                        if (list.Count < 1)
                        { //第一次用这个手机号登录
                            userinfo.userPhone = phoneNumber;
                            userinfo.userHead = "";
                            //userinfo.userPwd ="";
                            var id = UserInfoOper.Instance.Add(userinfo);
                            userinfo = UserInfoOper.Instance.GetById(id);
                            userinfo.birthday = null;
                            //hasPwd = false;
                        }
                        else
                        { //手机登录过，token失效
                            userinfo = list.First();
                        }
                    }
                }
            }
            #endregion

            #region 密码登录
            else if (mode == "Password")
            {
                var password = MD5Helper.Instance.StrToMD5(req.password);
                var list = UserInfoOper.Instance.GetByPhone(phoneNumber);
                if (list.Count < 1)
                {//数据库里没这个手机号，要先注册
                    return ControllerHelper.Instance.JsonResult(500, "该手机号尚未注册");
                }
                else
                {//手机号注册过了
                    userinfo = list.First();
                    if (string.IsNullOrEmpty(password))
                    {
                        return ControllerHelper.Instance.JsonResult(500, "请先输入密码");
                    }
                    else if (password != userinfo.userPwd)
                    {
                        return ControllerHelper.Instance.JsonResult(500, "密码错误");
                    }
                }
            }
            #endregion

            #region QQ登录
            else if (mode == "QQ")
            {
                if (string.IsNullOrEmpty(qq))
                {
                    return ControllerHelper.Instance.JsonResult(500, "QQ号码为空");
                }
                else
                {
                    var list = UserInfoOper.Instance.GetByQQ(qq);
                    if (list.Count < 1)
                    { //第一次用这个QQ登录
                        return ControllerHelper.Instance.JsonResult(200, "phone", "", "");
                    }
                    else
                    { //这个qq登录过
                        userinfo = list.First();
                    }
                }
            }
            #endregion

            #region 微信登录
            else if (mode == "WeChat")
            {
                if (string.IsNullOrEmpty(wechat))
                {
                    return ControllerHelper.Instance.JsonResult(500, "微信号码为空");
                }
                else
                {
                    var list = UserInfoOper.Instance.GetByWechat(wechat);
                    if (list.Count < 1)
                    { //第一次用这个微信登录
                        return ControllerHelper.Instance.JsonResult(200, "phone", "", "");
                    }
                    else
                    { //这个微信登录过
                        userinfo = list.First();
                    }
                }
            }
            #endregion
            var deviceToken = req.deviceToken;
            if (!string.IsNullOrEmpty(deviceToken))
            {
                var user = new UserInfo();
                user.id = userinfo.id;
                user.deviceToken = deviceToken;
                user.deviceType = req.deviceType;
                UserInfoOper.Instance.Update(user);
            }

            tokenTemp = CacheHelper.SetUserToken(userinfo);
            UserRes userR = new UserRes(userinfo, apiHost, tokenTemp.GetToken());
            if (string.IsNullOrEmpty(userinfo.userPwd))
                hasPwd = false;
            userR.hasPwd = hasPwd;
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(userR), "");

            //return ControllerHelper.Instance.JsonResult(500, "意想不到的登录失败");

        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public HttpResponseMessage Register(RegisterReq req)
        {
            string phone = req.phone;
            string vCode = req.verificationCode;
            var pwd = req.password;

            var users = UserInfoOper.Instance.GetByPhone(phone);
            if (users.Count != 0)
            {
                return ControllerHelper.Instance.JsonResult(500, "该手机号已注册");
            }
            else
            {
                var code = CacheHelper.GetUserVerificationCode(phone);
                if (code == null)//缓存中没有这个手机的验证码了，正式时保留5分钟
                {
                    return ControllerHelper.Instance.JsonResult(500, "请重新发送验证码");
                }
                else if (code != vCode)
                {
                    return ControllerHelper.Instance.JsonResult(500, "验证码错误");
                }
                else
                {//可以注册了
                    UserInfo userHere = new UserInfo();
                    userHere.userPhone = phone;
                    userHere.userPwd = MD5Helper.Instance.StrToMD5(pwd);
                    userHere.userHead = "";
                    var id = UserInfoOper.Instance.Add(userHere);

                    userHere.id = id;
                    userHere.userName = "";
                    //userHere.birthday = null;
                    Token token = CacheHelper.SetUserToken(userHere);
                    //userHere.Token = token.GetToken();
                    UserRes userRes = new UserRes(userHere, apiHost, token.GetToken());
                    return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(userRes), "");
                    //var str = JsonHelper.JsonPlus(true, JsonConvert.SerializeObject(userRes), "");
                    //return ControllerHelper.Instance.JsonResult(str);
                    //目前是返回了新的user记录了，移动端要什么再改-txy
                }
            }
        }

        /// <summary>
        /// qq或微信登录状态，绑定手机号接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns>status</returns>
        public HttpResponseMessage BindPhone(UserReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var qq = req.qq;
            var wechat = req.wechat;
            var phoneNumber = req.phone;
            var code = req.verificationCode;
            var bindMode = req.BindMode;
            bool hasPwd = true;

            var cache_code = CacheHelper.GetUserVerificationCode(phoneNumber);

            var userinfo = new UserInfo();
            if (cache_code == null)
            {
                return ControllerHelper.Instance.JsonResult(500, "请重新获取验证码");
            }
            else if (cache_code != code)
            {
                return ControllerHelper.Instance.JsonResult(500, "验证码错误");
            }
            else
            {
                #region QQ登录绑定手机
                if (bindMode == "QQ")
                {
                    var list = UserInfoOper.Instance.GetByPhone(phoneNumber);
                    if (list.Count > 0)
                    { //已存在这个手机号码
                        userinfo = list.First();
                        if (!string.IsNullOrEmpty(userinfo.qq))//且这个号码下有QQ
                        {
                            return ControllerHelper.Instance.JsonResult(500, "该手机号已绑定QQ");
                        }
                        else
                        {//账号未绑定QQ
                            userinfo.qq = qq;
                            UserInfoOper.Instance.Update(userinfo);
                        }
                    }
                    else
                    { //不存在这个手机号
                        userinfo.userPhone = phoneNumber;
                        userinfo.qq = qq;
                        var id = UserInfoOper.Instance.Add(userinfo);
                        userinfo = UserInfoOper.Instance.GetById(id);
                    }
                }
                #endregion

                #region 微信登录绑定手机
                if (bindMode == "WeChat")
                {
                    var list = UserInfoOper.Instance.GetByPhone(phoneNumber);
                    if (list.Count > 0)
                    { //已存在这个手机号码
                        userinfo = list.First();
                        if (!string.IsNullOrEmpty(userinfo.wechat))
                        {
                            return ControllerHelper.Instance.JsonResult(500, "该手机号已绑定微信");
                        }
                        else
                        {
                            userinfo.wechat = wechat;
                            UserInfoOper.Instance.Update(userinfo);
                        }
                    }
                    else
                    {  //不存在这个手机号
                        userinfo.userPhone = phoneNumber;
                        userinfo.wechat = wechat;
                        var id = UserInfoOper.Instance.Add(userinfo);
                        userinfo = UserInfoOper.Instance.GetById(id);
                    }
                }
                #endregion
            }
            var deviceToken = req.deviceToken;
            if (!string.IsNullOrEmpty(deviceToken))
            {
                userinfo.deviceToken = deviceToken;
                userinfo.deviceType = req.deviceType;
                UserInfoOper.Instance.Update(userinfo);
            }

            var tokenTemp = CacheHelper.SetUserToken(userinfo);
            UserRes r = new UserRes(userinfo, apiHost, tokenTemp.GetToken());
            if (string.IsNullOrEmpty(userinfo.userPwd))
                hasPwd = false;
            r.hasPwd = hasPwd;
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(r), "");

        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [ValidateModel]
        public HttpResponseMessage SetPassword(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var pwd = MD5Helper.Instance.StrToMD5(req.password);

            var userHere = new UserInfo();
            userHere.id = userId;
            userHere.userPwd = pwd;
            try
            {
                UserInfoOper.Instance.Update(userHere);
                return ControllerHelper.Instance.JsonResult(200, "设置成功");
            }
            catch (Exception e)
            {
                return ControllerHelper.Instance.JsonResult(200, "服务器错误");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [ValidateModel]
        public HttpResponseMessage ReSetPwd(ResetPwdReq req)
        {
            var pwd = MD5Helper.Instance.StrToMD5(req.password);
            var newPwd = MD5Helper.Instance.StrToMD5(req.newPassword);
            var userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var user = UserInfoOper.Instance.GetById(userId);
            if (pwd == newPwd)
            {
                return ControllerHelper.Instance.JsonResult(500, "不能与原密码相同");
            }
            else
                if (user.userPwd != pwd)
            {
                return ControllerHelper.Instance.JsonResult(500, "当前密码不正确");
            }
            else
            {
                //user.userPwd = newPwd;
                try
                {
                    var userHere = new UserInfo();
                    userHere.id = userId;
                    userHere.userPwd = newPwd;
                    UserInfoOper.Instance.Update(userHere);
                    return ControllerHelper.Instance.JsonResult(200, "修改密码成功");
                }
                catch (Exception e)
                {
                    //string eStr = JsonHelper.OneToJson(false, "", "", "出问题了");
                    //return ControllerHelper.Instance.JsonResult(eStr);
                    return ControllerHelper.Instance.JsonResult(500, "出问题了");
                }
            }


        }

        /// <summary>
        /// 密码找回
        /// </summary>
        /// <param name="req">只传要修改的密码</param>
        /// <returns></returns>
        [ValidateModel]
        public HttpResponseMessage FindPassword(FindPwdReq req)
        {
            var phone = req.phone;
            var vCode = req.verificationCode;
            var pwd = req.newPassword;
            //var vPwd = user.vPassword;

            var list = UserInfoOper.Instance.GetByPhone(phone);
            if (list.Count < 1)
            {
                return ControllerHelper.Instance.JsonResult(500, "该手机号尚未注册");
            }
            else
            {
                var code = CacheHelper.GetUserVerificationCode(phone);
                if (code == null)//缓存中没有这个手机的验证码了，正式时保留5分钟
                {
                    return ControllerHelper.Instance.JsonResult(500, "请重新发送验证码");
                }
                else if (code != vCode)
                {
                    return ControllerHelper.Instance.JsonResult(500, "验证码错误");
                }
                else
                {
                    var user = list.First();
                    var userHere = new UserInfo();
                    userHere.id = user.id;
                    userHere.userPwd = MD5Helper.Instance.StrToMD5(pwd);
                    UserInfoOper.Instance.Update(userHere);
                    return ControllerHelper.Instance.JsonResult(200, "重置成功");
                }
            }

        }

        /// <summary>
        /// 解绑QQ或微信号
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public HttpResponseMessage RemoveContact(UserReq req)
        {
            var mode = req.RemoveMode;
            var userId = Convert.ToInt32(req.userId);

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            if (mode == "QQ")
            {

                //user.qq = "";
                var user = new UserInfo();
                user.id = userId;
                user.qq = "";
                UserInfoOper.Instance.Update(user);
                return ControllerHelper.Instance.JsonResult(200, "与QQ账号的绑定已解除");
            }
            else
            {

                //user.wechat = "";
                var user = new UserInfo();
                user.id = userId;
                user.wechat = "";
                UserInfoOper.Instance.Update(user);
                return ControllerHelper.Instance.JsonResult(200, "与微信账号的绑定已解除");
            }


        }

        /// <summary>
        /// 社交账号中 绑定QQ或微信
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public HttpResponseMessage BuildContact(UserReq req)
        {
            var mode = req.BindMode;
            var userId = Convert.ToInt32(req.userId);
            var qq = req.qq;
            var wechat = req.wechat;

            var tokenStr = req.Token;
            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");


            if (mode == "QQ")
            {
                var list = UserInfoOper.Instance.GetByQQ(qq);
                if (list.Count > 0)
                {
                    return ControllerHelper.Instance.JsonResult(500, "该QQ已绑定其他账号");
                }
                else
                {

                    //user.qq = qq;
                    var user = new UserInfo();
                    user.id = userId;
                    user.qq = qq;
                    UserInfoOper.Instance.Update(user);
                    return ControllerHelper.Instance.JsonResult(200, "QQ账号绑定成功");
                }
            }
            else
            {
                var list = UserInfoOper.Instance.GetByQQ(wechat);
                if (list.Count > 0)
                {
                    return ControllerHelper.Instance.JsonResult(500, "该微信已绑定其他账号");
                }
                else
                {

                    //user.wechat = wechat;
                    var user = new UserInfo();
                    user.id = userId;
                    user.wechat = wechat;
                    UserInfoOper.Instance.Update(user);
                    return ControllerHelper.Instance.JsonResult(200, "微信账号绑定成功");
                }
            }
        }

        /// <summary>
        /// 获取用户个人信息（头像、昵称、生日）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetUserInfo(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var user = UserInfoOper.Instance.GetViewByUserId(userId);
            UserInfoRes res = new UserInfoRes(user, apiHost, tokenStr);

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 设置用户头像、昵称、生日
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SetUserInfo(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var user = new UserInfo();
            user.id = userId;

            var headImgStr = req.headImg;
            var imgUrl = "";
            if (!string.IsNullOrEmpty(headImgStr))
                imgUrl = ControllerHelper.Instance.SaveHeadImg(headImgStr, apiHost, userId);
            var name = req.name;
            if (!string.IsNullOrEmpty(name))
                user.userName = name;

            var birthday = req.birthday;
            if (!string.IsNullOrEmpty(birthday))
                user.birthday = Convert.ToDateTime(birthday);

            if (!string.IsNullOrEmpty(imgUrl))
            {
                user.userHead = imgUrl;
                var newImgName = imgUrl.Substring(imgUrl.LastIndexOf('/') + 1);
                ControllerHelper.Instance.DeleteOtherUserHeadImg(userId, newImgName);
            }

            UserInfoOper.Instance.Update(user);

            var view = UserInfoOper.Instance.GetViewByUserId(userId);
            if (view == null)
                return ControllerHelper.Instance.JsonResult(500, "用户不存在");
            UserInfoRes res = new UserInfoRes(view, apiHost, tokenStr);

            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "修改成功");
        }

        /// <summary>
        /// 用户提交建议
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Suggest(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var content = req.suggestion;
            Suggestion sug = new Suggestion();
            sug.userId = userId;
            sug.content = content;
            sug.sTime = DateTime.Now;
            SuggestionOper.Instance.Add(sug);
            return ControllerHelper.Instance.JsonResult(200, "建议成功");
        }

        /// <summary>
        /// 获取钱包中的余额和优惠券
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetWallet(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var user = UserInfoOper.Instance.GetById(userId);
            WalletRes res = new WalletRes((Decimal)user.userBalance, (Decimal)user.coupon);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(res), "");
        }

        /// <summary>
        /// 收支明细
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetPayRecord(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");


            var pageIndex = Convert.ToInt32(req.pageIndex);

            var list = CacheHelper.GetByConditionPaging<UserPayView>("UserPayView", " userid=" + userId, pageIndex, size, " order by createTime desc");

            if (list.Count < 1)
                return ControllerHelper.Instance.JsonEmptyArr(200, "暂无记录");
            var listR = new List<PayRecordRes>();
            foreach (var item in list)
            {
                PayRecordRes pr = new PayRecordRes(item);
                listR.Add(pr);
            }
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(listR), "");
        }

        /// <summary>
        /// 提现
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage TakeCash(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var takeType = req.takeType;
            var takeAccount = req.takeAccount;
            var money = Convert.ToDecimal(req.money);

            var user = UserInfoOper.Instance.GetById(userId);
            if (money > user.userBalance)
                return ControllerHelper.Instance.JsonResult(500, "超过现有的余额");
            var userHere = new UserInfo();
            userHere.id = user.id;
            userHere.userBalance = user.userBalance - money;
            UserInfoOper.Instance.Update(userHere);

            UserPay up = new UserPay();
            up.type = 99;
            up.money = money;
            up.userId = userId;
            up.status = false;
            up.createTime = DateTime.Now;
            up.takeType = takeType;
            up.takeAccount = takeAccount;
            UserPayOper.Instance.Add(up);

            return ControllerHelper.Instance.JsonResult(200, "1-3个工作日提现成功");
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Charge(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");


            var payType = req.payType;
            var payMoney = Convert.ToDecimal(req.payMoney);
            var Spbill_create_ip = req.spbill_create_ip;
            var out_trade_no = StringHelperHere.Instance.GetTimeStamp() + StringHelperHere.Instance.GetLastFiveStr(userId.ToString());
            UserPay up = new UserPay();
            up.type = 0;
            up.status = false;
            up.userId = userId;
            up.createTime = DateTime.Now;
            up.takeType = payType;
            up.outTradeNo = out_trade_no;
            up.money = payMoney;
            UserPayOper.Instance.Add(up);

            if (payType == "alipay")
            {
                //var str = alipay.CreateAlipayOrder(chargeMoney, out_trade_no, "http://" + apiHost + "/user/chargeByAlipayNotify");

                AlipayHelperHere a = new AlipayHelperHere();
                var b = a.CreateAlipayOrder(req.payMoney, out_trade_no, "https://fan-di.com/notify/chargeByAlipayNotify");

                JObject jo = new JObject();
                jo.Add("payType", "alipay");
                jo.Add("payStr", b);

                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");

            }
            else
            {
                WXPayHelperHere w = new WXPayHelperHere();
                var b = w.CreateWXOrder(payMoney, out_trade_no, "https://fan-di.com/notify/ChargeByWxNotify", Spbill_create_ip);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(b);
                if (doc.DocumentElement["return_code"].InnerText != "SUCCESS")
                {
                    txyInfo txy = new txyInfo();
                    txy.content = JsonConvert.SerializeObject(doc);
                    //txyInfoOper txyOper = new txyInfoOper();
                    txyInfoOper.Instance.Add(txy);
                    return ControllerHelper.Instance.JsonResult(500, "网络不稳定，请稍后");
                }

                JObject jo = new JObject();
                jo.Add("payType", "wxpay");
                foreach (XmlNode item in doc.DocumentElement.ChildNodes)
                {
                    if (item.Name != "sign")
                        jo.Add(item.Name, item.InnerText);
                }

                var timestamp = StringHelperHere.Instance.GetTimeStamp();
                jo.Add("timestamp", timestamp);
                var dict = new Dictionary<string, string>();

                foreach (var item in jo)
                {
                    if (item.Key == "prepay_id")
                        dict.Add("prepayid", item.Value.ToString());
                    if (item.Key == "nonce_str")
                        dict.Add("noncestr", item.Value.ToString());
                }

                dict.Add("appid", "wxd8482615c33b8859");
                dict.Add("partnerid", "1494504712");
                dict.Add("package", "Sign=WXPay");
                dict.Add("timestamp", timestamp);

                string sign = StringHelperHere.Instance.GetWXSign(dict, "2BCF8DD9490E328D2FCEDE7B26643231");
                jo.Add("sign", sign);
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
            }
        }

        /// <summary>
        /// 用户打开app时记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UploadUserLocation(UserReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var lat = req.lat;
            var lng = req.lng;
            var address = req.address;
            var now = DateTime.Now;
            if (userId != 0)
            {
                UserOpenInfo uo = new UserOpenInfo();
                uo.lat = lat;
                uo.lng = lng;
                uo.openTime = now;
                uo.address = address;
                uo.userId = userId;
                UserOpenInfoOper.Instance.Add(uo);
                return ControllerHelper.Instance.JsonResult(200, "已记录");
            }
            return ControllerHelper.Instance.JsonResult(200, "没登陆不记录");
        }

        /// <summary>
        /// 用户关闭app时记录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UploadUserClose(UserReq req)
        {
            if (req != null)
            {
                var userId = Convert.ToInt32(req.userId);
                var uo = UserOpenInfoOper.Instance.GetLastByUserId(userId);
                uo.closeTime = DateTime.Now;
                UserOpenInfoOper.Instance.Update(uo);
                return ControllerHelper.Instance.JsonResult(200, "已记录");
            }
            return ControllerHelper.Instance.JsonResult(200, "没登陆不记录");
        }

        /// <summary>
        /// 兑换优惠券
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ConvertCoupon(UserReq req)
        {
            var tokenStr = req.Token;
            var userId = Convert.ToInt32(req.userId);

            Token token = CacheHelper.GetUserToken(tokenStr);
            if (token == null)
                return ControllerHelper.Instance.JsonResult(400, "token失效");
            if (token.Payload.UserID != userId)
                return ControllerHelper.Instance.JsonResult(400, "token错误");

            var couponNo = req.couponNo;
            var list = CouponOper.Instance.GetByCouponNo(couponNo);
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "兑换码不存在");

            list = list.Where(p => p.isUse == false).ToList();
            if (list.Count < 1)
                return ControllerHelper.Instance.JsonResult(500, "兑换码已被使用");

            var coupon = list.First();
            var now = DateTime.Now;
            var set = new SettingRes();
            var couponDays = Convert.ToInt32(set.couponSaveDays);
            if (Convert.ToDateTime(coupon.createTime).AddDays(couponDays) < now)
                return ControllerHelper.Instance.JsonResult(500, "兑换码已过期");
            //if ((bool)coupon.isUse)
            //    return ControllerHelper.Instance.JsonResult(500, "兑换码已被使用");


            var ts = coupon.batch;
            if (CouponOper.Instance.isUserUsedCoupon(userId, ts))
                return ControllerHelper.Instance.JsonResult(500, "已使用过同类型兑换码");

            coupon.isUse = true;
            if (!(bool)coupon.isRepeat)
            {
                CouponOper.Instance.AddCouponIds(userId, ts);
            }
            CouponOper.Instance.Update(coupon);

            var user = UserInfoOper.Instance.GetById(userId);
            var userHere = new UserInfo();
            userHere.id = user.id;
            userHere.coupon = user.coupon + coupon.money;
            UserInfoOper.Instance.Update(userHere);

            UserPay up = new UserPay();
            up.type = 1;
            up.money = coupon.money;
            up.userId = userId;
            up.createTime = DateTime.Now;
            UserPayOper.Instance.Add(up);

            return ControllerHelper.Instance.JsonResult(200, "兑换成功");
        }

        /// <summary>
        /// 判断用户是否点过消息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage IsUserClicked(UserReq req)
        {
            var jo = new JObject();

            var count = ArticleInfoOper.Instance.GetArticleCount();
            if (count == 0)
            {
                jo.Add("isClicked", true);
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
            }

            if (req == null)
            {
                jo.Add("isClicked", false);
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
            }
            var userId = Convert.ToInt32(req.userId);
            if (userId == 0)
            {
                jo.Add("isClicked", false);
                return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
            }

            var flag = CacheHelper.IsUserArticleClick(userId);
            jo.Add("isClicked", flag);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");

        }

        /// <summary>
        /// 用户点击的时候传过来记录一下
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SetUserClicked(UserReq req)
        {
            if (req == null)
                return ControllerHelper.Instance.JsonResult(200, "没登录");
            var userId = Convert.ToInt32(req.userId);
            if (userId == 0)
                return ControllerHelper.Instance.JsonResult(200, "没登录");
            CacheHelper.SetUserArticleClick(userId);
            return ControllerHelper.Instance.JsonResult(200, "已记录");
        }

        //分割线，以上已写接口文档
        /*————————————————————————————————————————————————*/

    }
}
