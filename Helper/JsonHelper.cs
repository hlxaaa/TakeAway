using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Helper
{
    public class JsonHelperHere
    {
        /// <summary>
        /// 给json添加status和message
        /// </summary>
        /// <param name="status"></param>
        /// <param name="json"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string JsonPlus(bool status, string json, string message) {
            return "{\"status\":\"" + status + "\",\"data\":" + json + ",\"message\":\"" + message + "\"}";
        }

        ///// <summary>
        ///// 给json添加status和message
        ///// </summary>
        ///// <param name="status"></param>
        ///// <param name="json"></param>
        ///// <param name="message"></param>
        ///// <returns></returns>
        //public static string JsonPlus(bool status, string json, string message)
        //{
        //    return "{\"status\":\"" + status + "\",\"data\":" + json + ",\"message\":\"" + message + "\"}";
        //}

        /// <summary>
        /// 一个数据转为json格式，附加status、message
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string OneToJson(bool status, string name, string value, string message)
        {
            string str = "{\"" + name + "\":\"" + value + "\"}";
            return JsonPlus(status, str, message);
        }

        public static string JsonAddPage(int pages, string json, int index)
        {
            return "{\"pages\":\"" + pages + "\",\"data\":" + json + ",\"index\":\"" + index + "\"}";
        }

        public static string JsonResult(int pages, object o, int index)
        {
            ResultDataForWeb r = new ResultDataForWeb(pages,o,index);
            return JsonConvert.SerializeObject(r);
            //return "{\"pages\":\"" + pages + "\",\"data\":" + o + ",\"index\":\"" + index + "\"}";
        }

        public static string EmptyJson()
        {
            return "{\"data\":\"\"}";
        }

        ///// <summary>
        ///// 一个数据转为json格式，附加status、message
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string OneToJson(bool status,string name, string value,string message) {
        //    string str =  "{\"" + name + "\":\""+value+"\"}";
        //    return JsonPlus(status, str, message);
        //}

        ///// <summary>
        ///// 一个数据转为json格式，附加status、message
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static string OneToJson(string status, string name, string value, string message)
        //{
        //    string str = "{\"" + name + "\":\"" + value + "\"}";
        //    return "{\"status\":\"" + status + "\",\"data\":" + str + ",\"message\":\"" + message + "\"}";
        //}

        /// <summary>
        /// 不发送数据，只发送消息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string JsonMsg(bool status, string msg) {
            var jo = new JObject();
            jo.Add("status", status);
            jo.Add("message", msg);
            return JsonConvert.SerializeObject(jo);
            //return "{\"status\":\"" + status + "\",\"message\":\"" + msg + "\"}";
        }
    }
}