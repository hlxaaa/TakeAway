using Common;
using DbOpertion.DBoperation;
using DbOpertion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using takeAwayWebApi.Models.Response;

namespace takeAwayWebApi.Helper
{
    public class ControllerHelper : SingleTon<ControllerHelper>
    {
        /// <summary>
        /// 清除一天前的缓存图片
        /// </summary>
        /// <param name="path">temp文件夹路径</param>
        public  void ClearTempPic(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            {
                DateTime a = file.LastWriteTime;
                int b = DateTime.Now.Subtract(a).Days;
                if (b > 0)
                    File.Delete(path + "\\" + file);
            }
        }

        /// <summary>
        /// 清除n天前的二维码图片
        /// </summary>
        /// <param name="path">temp文件夹路径</param>
        public void ClearTempQCode(string path,int days)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            {
                DateTime a = file.LastWriteTime;
                if(a.AddDays(days)<DateTime.Now)
                    File.Delete(path + "\\" + file);
            }

            foreach (DirectoryInfo file in dir.GetDirectories())
            {
                DateTime a = file.LastWriteTime;
                if (a.AddDays(days) < DateTime.Now)
                    Directory.Delete(path + "\\" + file,true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">不用处理，直接返回json字符串</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="pages"></param>
        /// <returns></returns>
        public  string Paging2<T>(List<T> list, int index, int size) where T : class ,new()
        {
            int pages = list.Count / size;
            pages = pages * size == list.Count ? pages : pages + 1;//总页数
            List<T> list2 = list.Skip((index - 1) * size).Take(size).ToList();
            return JsonHelperHere.JsonAddPage(pages, JsonConvert.SerializeObject(list2), index);
        }

        /// <summary>
        /// 判断某个字段的某个值在表中是否存在,传0就不跟自己比较了(IsDeleted=0)
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public  bool IsExists(string tableName, string fieldName, string value, string id)
        {
            string str = "select * from " + tableName + " where isDeleted='False' and id!=" + id + " and " + fieldName + " = @value";
            SqlCommand com = new SqlCommand(str);
            com.Parameters.AddWithValue("@value", value);
            var r = SqlHelperHere.ExecuteScalar(com);
            if (r == null)
                return false;
            return true;
        }

        public  void DeleteFile(string url)
        {
            if (File.Exists(url))
                File.Delete(url);
        }

        public HttpResponseMessage JsonResult(int httpcode, string msg)
        {
            JObject jo = new JObject();
            jo.Add("httpcode", httpcode);
            jo.Add("message", msg);
            return new HttpResponseMessage { Content = new StringContent(jo.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        public HttpResponseMessage JsonResult(int httpcode, string name, string value, string msg)
        {
            string json = "{\"" + name + "\":\"" + value + "\"}";
            JObject jo = new JObject();
            jo.Add("httpcode", httpcode);
            var str1 = JsonConvert.SerializeObject(jo);
            var str = str1.Substring(0, str1.Length - 1) + ",\"data\":" + json + ",\"message\":\"" + msg + "\"}";
            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        public HttpResponseMessage JsonResult(int httpcode, string json, string msg)
        {
            JObject jo = new JObject();
            jo.Add("httpcode", httpcode);
            var str1 = JsonConvert.SerializeObject(jo);
            var str = str1.Substring(0, str1.Length - 1) + ",\"data\":" + json + ",\"message\":\"" + msg + "\"}";
            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        //可能是最终形态
        public HttpResponseMessage JsonResultObject(int httpcode, object d, string msg)
        {
            Result r = new Result(httpcode, d, msg);
            var str = JsonConvert.SerializeObject(r);
            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /// <summary>
        /// data传空数组
        /// </summary>
        /// <param name="httpCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public HttpResponseMessage JsonEmptyArr(int httpCode, string msg)
        {
            JObject jo = new JObject();
            jo.Add("httpcode", httpCode);
            var str1 = JsonConvert.SerializeObject(jo);
            var str = str1.Substring(0, str1.Length - 1) + ",\"data\":[],\"message\":\"" + msg + "\"}";

            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /// <summary>
        /// data:""
        /// </summary>
        /// <param name="httpCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public HttpResponseMessage JsonEmptyStr(int httpCode, string msg)
        {
            JObject jo = new JObject();
            jo.Add("httpcode", httpCode);
            var str1 = JsonConvert.SerializeObject(jo);
            var str = str1.Substring(0, str1.Length - 1) + ",\"data\":\"\",\"message\":\"" + msg + "\"}";

            return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /// <summary>
        /// 计算礼金券，余额，第三方支付金额
        /// </summary>
        public PriceRes GetThreePrice(decimal balance, decimal coupon, decimal sumMoney)
        {
            var discount = new decimal(0.1);
            PriceRes res = new PriceRes();
            var newCoupon = sumMoney * discount;
            res.useCoupon = newCoupon > coupon ? coupon : newCoupon;
            var newBalance = sumMoney - res.useCoupon;
            res.useBalance = newBalance > balance ? balance : newBalance;
            var newPay = sumMoney - res.useBalance - res.useCoupon;
            if (newPay > 0)
                res.payMoney = newPay;
            else
                res.payMoney = 0;
            return res;
        }

        /// <summary>
        /// 有押金时计算支付的金额,-txy改动了。本来使用余额，且余额充足的时候，押金不扣的，现在扣了。
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="coupon"></param>
        /// <param name="sumMoney"></param>
        /// <param name="deposit"></param>
        /// <returns></returns>
        public PriceRes GetThreePriceWithDeposit(decimal balance, decimal coupon, decimal sumMoney, decimal deposit)
        {
            var discountTemp = new SettingRes().discount;

            var balanceTemp = balance > deposit ? balance - deposit : balance;
            var depositForPayMoney = balance > deposit ? 0 : deposit;
            var depositForUseBalance = balance > deposit ? deposit:0;
            var discount = Convert.ToDecimal(discountTemp)/100;
            PriceRes res = new PriceRes();
            var newCoupon = sumMoney * discount;
            res.useCoupon = newCoupon > coupon ? coupon : newCoupon;
            var newBalance = sumMoney - res.useCoupon;
            res.useBalance = (newBalance > balanceTemp ? balanceTemp : newBalance) + depositForUseBalance;
            var newPay = sumMoney - res.useBalance - res.useCoupon;
            if (newPay > 0)
                res.payMoney = newPay + depositForPayMoney;
            else
                res.payMoney = 0;
            res.payMoney = StringHelperHere.Instance.KeepDecimal(res.payMoney);
            res.useBalance = StringHelperHere.Instance.KeepDecimal(res.useBalance);
            return res;
        }

        /// <summary>
        /// 保存用户头像图片，返回url
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="serverPath"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string SaveHeadImg(string base64, string serverPath, int userId)
        {
            base64 = base64.Substring(base64.IndexOf(',') + 1);
            byte[] byteArray = Convert.FromBase64String(base64);

            string saveFileName = DateTime.Now.ToFileTime().ToString();
            string shortPath = "/img/userhead/userId" + userId + "-" + saveFileName + ".jpg";
            string path = HttpContext.Current.Server.MapPath(shortPath);

            Stream s = new FileStream(path, FileMode.Append);
            s.Write(byteArray, 0, byteArray.Length);

            s.Close();
            return shortPath;
        }

        /// <summary>
        /// 更换头像后，把该用户之前的头像删掉
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newImgName"></param>
        public void DeleteOtherUserHeadImg(int userId, string newImgName)
        {
            string path = HttpContext.Current.Server.MapPath("/img/userhead/");
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            {
                var name = file.Name;
                if (name.Contains("userId" + userId + "-") && name != newImgName)
                    file.Delete();
            }
        }

        public  void UpdateImg(string[] oldImgNames, string[] imgNames)
        {
            for (int i = 0; i < oldImgNames.Length; i++)
            {
                bool isExist = false;
                for (int j = 0; j < imgNames.Length; j++)
                {
                    if (oldImgNames[i] == imgNames[j])
                        isExist = true;
                }
                if (!isExist)
                    DeleteFile(oldImgNames[i]);
            }
        }

        /// <summary>
        /// 如果目标地址不存在，复制文件
        /// </summary>
        /// <param name="sourceUrl"></param>
        /// <param name="targetUrl"></param>
        public void CopyFile(string sourceUrl, string targetUrl)
        {
            if (!File.Exists(targetUrl))
                File.Copy(sourceUrl, targetUrl);
        }

        public string JsonWebResult(int pages, string json, int index)
        {
            JObject jo = new JObject();
            jo.Add("pages", pages);
            //jo.Add("data", json);
            var str1 = JsonConvert.SerializeObject(jo);
            var str = str1.Substring(0, str1.Length - 1) + ",\"data\":" + json + ",\"index\":\"" + index + "\"}";
            return str;
            //return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
        }

        /*————————————————————————————————————————————*/

        //jsonHelper里很多改到这了，可以用这边的-txy

        /*————————————————————————————————————————————*/

        /// <summary>
        /// 将Base64字符串转换为图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public Bitmap Base64ToImage(string base64)
        {
            byte[] arr = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(arr);
            Bitmap bmp = new Bitmap(ms);
            ms.Close();
            return bmp;
        }

    }
}