using Common.Helper;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using takeAwayWebApi.Controllers.webs;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Controllers.ajax
{
    public class UserAjaxController : WebAjaxController
    {
        public string Get(webReq req)//返回的类型要改一下的
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            string search = req.search == null ? "" : req.search;
            int index = req.index;
            int pages = 0;//总页数
            int size = 12;//一页size条数据
            var time1 = req.time1;
            var time2 = req.time2;

            int pageType = req.pageType;
            #region 用户信息
            if (pageType == 0)
            {
                var condition = " isdeleted=0 and (username like '%" + search + "%' or userphone like '%" + search + "%' )";

                var listAll = CacheHelper.GetDistinctCount<UserInfo>("UserInfo", condition);
                if (listAll.Count < 1)
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);
                }
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);

                var viewList = CacheHelper.GetByCondition<UserInfo>("UserInfo", " id in (" + idsStr + ") order by id desc");

                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;

                var listR = new List<UserWebRes>();
                foreach (var item in viewList)
                {
                    UserWebRes user = new UserWebRes(item);
                    listR.Add(user);
                }
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
            }
            #endregion
            #region 用户使用app信息
            else if (pageType == 1)
            {
                var condition = " 1=1 and ( username like '%" + search + "%') and openTime>'" + time1 + "' and openTime<'" + time2 + "'";
                var listAll = CacheHelper.GetDistinctCount<UserOpenView>("UserOpenView", condition);
                if (listAll.Count < 1)
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);
                }
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);

                var viewList = CacheHelper.GetByCondition<UserOpenView>("UserOpenView", " id in (" + idsStr + ") order by id desc");

                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;

                var listR = new List<UserOpenInfoForWeb>();
                foreach (var item in viewList)
                {
                    UserOpenInfoForWeb temp = new UserOpenInfoForWeb(item);
                    listR.Add(temp);
                }
                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR), index);
                return JsonConvert.SerializeObject(r);
            }
            #endregion
            #region 用户建议
            else if (pageType == 2)
            {
                var condition = " isdeleted=0 and ( content like '%" + search + "%' or  username like '%" + search + "%') and sTime>'" + time1 + "' and sTime<'" + time2 + "'";

                var listAll = CacheHelper.GetDistinctCount<SuggestionView>("SuggestionView", condition);
                if (listAll.Count < 1)
                {
                    r.data = JsonHelperHere.EmptyJson();
                    return JsonConvert.SerializeObject(r);
                }
                //分页
                var ids = listAll.Skip((index - 1) * size).Take(size).Select(p => p.id).ToArray();
                var idsStr = string.Join(",", ids);

                var viewList = CacheHelper.GetByCondition<SuggestionView>("SuggestionView", " id in (" + idsStr + ") order by id desc");

                var count = listAll.Count;
                pages = count / size;
                //总页数
                pages = pages * size == count ? pages : pages + 1;
                var listR = new List<SuggestionRes>();
                foreach (var item in viewList)
                {
                    SuggestionRes t = new SuggestionRes(item);
                    listR.Add(t);
                }

                r.data = JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(listR.OrderByDescending(p => p.id)), index);
                return JsonConvert.SerializeObject(r);
            }
            #endregion
            r.data = JsonHelperHere.EmptyJson();
            return JsonConvert.SerializeObject(r);
        }

        public string Add(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            var phone = req.userPhone;
            var birthday = req.birthday;
            if (ControllerHelper.Instance.IsExists("userinfo", "userphone", phone, "0"))
            {
                r.Message = "已存在该手机号的用户";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
            }
            UserInfo user = new UserInfo();
            user.userName = req.userName;
            var balance = Convert.ToDecimal(req.userBalance);
            var coupon = Convert.ToDecimal(req.coupon);
            if (balance < 0)
            {
                r.Message = "余额不能为负数";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
            }
            if (coupon < 0)
            {
                r.Message = "优惠券不能为负数";
                r.HttpCode = 500;
                return JsonConvert.SerializeObject(r);
            }


            //user.userBalance = balance;
            if (req.userPwd != null)
                user.userPwd = MD5Helper.Instance.StrToMD5(req.userPwd);
            user.userPhone = phone;
            if (!string.IsNullOrEmpty(birthday))
                user.birthday = Convert.ToDateTime(birthday);
            user.coupon = coupon;

            UserInfoOper.Instance.Add(user);
            r.data = "";
            return JsonConvert.SerializeObject(r);
        }

        public string Update(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            UserInfo user = new UserInfo();
            user.userName = req.userName;
            //user.userBalance = Convert.ToDecimal(req.userBalance);
            user.id = Convert.ToInt32(req.userId);
            if (req.userPwd != null)
                user.userPwd = MD5Helper.Instance.StrToMD5(req.userPwd);
            user.userPhone = req.userPhone;

            if (ControllerHelper.Instance.IsExists("userinfo", "userphone", user.userPhone, req.userId.ToString()))
            {
                r.Message = "已存在该手机号的用户";
                r.HttpCode = 400;
                return JsonConvert.SerializeObject(r);
            }

            user.birthday = Convert.ToDateTime(req.birthday);
            user.coupon = Convert.ToDecimal(req.coupon);

            UserInfoOper.Instance.Update(user);
            return JsonConvert.SerializeObject(r);
        }

        public string Delete(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            var id = Convert.ToInt32(req.userId);
            UserInfoOper.Instance.Delete(id);
            return JsonConvert.SerializeObject(r);
        }

        public string BatchDel(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            int[] ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                UserInfoOper.Instance.Delete(ids[i]);
            }
            return JsonConvert.SerializeObject(r);
        }

        public string DelSuggestion(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            var id = req.id;
            SuggestionOper.Instance.Delete(id);
            return JsonConvert.SerializeObject(r);
        }

        public string BatchDelSug(webReq req)
        {
            ResultForWeb r = new ResultForWeb();
            r.HttpCode = 200;
            r.Message = "";
            r.data = "{}";
            if (Session["pfId"] == null)
            {
                r.HttpCode = 300;
                return JsonConvert.SerializeObject(r);
            }
            var ids = req.ids;
            for (int i = 0; i < ids.Length; i++)
            {
                SuggestionOper.Instance.Delete(Convert.ToInt32(ids[i]));
            }
            return JsonConvert.SerializeObject(r);
        }

    }
}
