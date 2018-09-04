using BeIT.MemCached;
using Common.Helper;
using Common.Result;
using DbOpertion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;
using takeAwayWebApi.Attribute;
using takeAwayWebApi.Helper;
using takeAwayWebApi.Models.Request;



namespace takeAwayWebApi.Controllers
{
    [WebApiException]
    public class test1Controller : ApiController
    {
        ///-txy完成
        ///order37分开。历史订单的增删改查。用户评价的展示。foodstar。用户建议的展示
        string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();
        //string apiHost = ConfigurationManager.AppSettings["takeAwayWebApiHost"].ToString();

        public MemcachedClient cache = MemCacheHelper.GetMyConfigInstance();

        [HttpPost]
        [WebApiException]
        //[ValidateModel]
        public object test(TestReq req)
        {
            var list = new List<testrr> {
                new testrr{ name="txy",sex=1 },
                new testrr{ name="wz",sex=0 },
            };
            cache.Add("test", JsonConvert.SerializeObject(list), DateTime.Now.AddDays(1));

            var r = cache.Get("test");
            //return r;
            var list2 = JsonConvert.DeserializeObject<List<testrr>>(r.ToString());

            return list2;
        }

        public HttpResponseMessage test2(TestReq req)
        {

            var item = new OrderInfo();
            var date = DateTime.Now;
            var price = 1.99m;

            string str = $"On {date}, the price of {item} was {price} per.";
            var b = 0;

            var r = CacheHelper.GetById<OrderInfo>("orderinfo", 1);
            return ControllerHelper.Instance.JsonResultObject(200, new int[0], "");
        }

        public int test7()
        {
            var a = 1; var b = 0;
            return a / b;
        }

        public bool test3()
        {
            return true;
        }

        [HttpPost]
        public string test4(TestReq req)
        {
            return "1";
            //var dict = new Dictionary<string, string>();
            ////dict.Add("@userId", "6");
            //var list = (List<UserInfo>)CacheHelper.GetByCondition<UserInfo>("UserInfo", " id=17", dict);



            var dict = new Dictionary<string, string>();
            dict.Add("@str", "%童晓翊%");
            var list = CacheHelper.GetDistinctCount<OrderTryJoin>("ordertryjoin", " riderName like @str or userName like @str", dict);

            return "";
            //CacheHelper.LockCache("");

            //var list = (List<FoodView>)CacheHelper.GetByCondition<FoodView>("FoodView", " isweek=0");

            //txyInfoOper txyOper = new txyInfoOper();
            //txyInfo txy = new txyInfo();
            //txy.id = 119;
            //txy.ttime = DateTime.Now;
            //txyOper.Update(txy);

            //string isServer = ConfigurationManager.AppSettings.Get("111");
            //var aaaa = isServer == "a";
            //string str = "123456";
            //var a = MD5Helper.StrToMD5_UTF8(str);

            //return a +"  ";
            //var user = userOper.GetById(req.userId);
            //List<string> list = new List<string>();
            //list.Add(user.deviceToken);
            //int id = 999;

            //string key =  StringHelper.Instance.IntToKey(id);
            //var aa =  StringHelper.Instance.Encrypt(id.ToString(), key);

            //return StringHelper.Instance.Decrypt(aa, key);

            //var r = Convert.ToDateTime(DateTime.Now.GetDateTimeFormats('D')[0]);
            //return "";
            ////return AndroidPush.PostMany(list, "测试消息");
            //var str = "123456789,";
            //str = str.Substring(0, str.Length - 1);
            //return str;



            //var a = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            //return a.ToString();

            //return MD5Helper.StrToMD5("fandiapp");

            //var xml = "<xml><return_code><![CDATA[SUCCESS]]></return_code>\n<return_msg><![CDATA[OK]]></return_msg>\n<appid><![CDATA[wxd8482615c33b8859]]></appid>\n<mch_id><![CDATA[1494504712]]></mch_id>\n<nonce_str><![CDATA[6WPU0BTi2HQvv1sQ]]></nonce_str>\n<sign><![CDATA[7C5E95B9554A3F3D74DBCD179679494E]]></sign>\n<result_code><![CDATA[SUCCESS]]></result_code>\n<prepay_id><![CDATA[wx2017122512475225abf6579d0315873400]]></prepay_id>\n<trade_type><![CDATA[APP]]></trade_type>\n</xml>";
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xml);
            //string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            //return json;
            ////return StringHelper.Instance.CreateOrderNo(order);
        }

        [HttpPost]
        public string test5()
        {
            var a = new int[2];
            a[0] = 1;
            a[1] = 2;
            return string.Join(",", a);
        }

        public bool test6(TestReq req)
        {
            Regex reg = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            string str = @"^[t]?[0-9]+\.[xy]{1,2}$";
            reg = new Regex(str);
            return reg.IsMatch(req.str);
        }

        public object test8(TestReq req)
        {
            var ids = new string[2];
            ids[0] = "1";
            ids[1] = "2";
            var temps = Array.ConvertAll<string, int>(ids, s => int.Parse(s));
            return temps;
        }

        public string test9()
        {
            var arr = new string[1];
            arr[0] = "1";
            int[] arr2 = { 1 };
            return StringHelperHere.Instance.ArrJoin(arr2);

        }

        public HttpResponseMessage getNotify(HttpWebRequest req)
        {
            var a = HttpContext.Current.Request.Form;
            //var a = req;
            return null;
            //return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(c), "");
        }

        /// <summary>
        /// 获取用户token
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetUserToken(TestReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var token = cache.GetModel<Token>("Ta_UserToken_" + userId);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(token), token.GetToken());
        }

        [HttpPost]
        public HttpResponseMessage GetRiderToken(TestReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var token = cache.GetModel<Token>("Ta_RiderToken_" + userId);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(token), token.GetToken());
        }

        [HttpPost]
        public HttpResponseMessage GetCancelCache(TestReq req)
        {
            var userId = Convert.ToInt32(req.userId);
            var cache = CacheHelper.GetCancelCache(userId);
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(cache), "");
        }

        [HttpPost]
        public HttpResponseMessage GetTempFoods(TestReq req)
        {
            //var userId = Convert.ToInt32(req.userId);
            //var r = CacheHelper.GetTempFoods(userId);
            //return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(r), "");
            return null;
        }

        public string SetUserVCode(webReq req)
        {
            return CacheHelper.SetUserVerificationCode(req.phone);
        }

        [HttpPost]
        public string GetPhoneV(TestReq req)
        {

            var phone = req.phone;
            //cache.Get("Ta_VerificationCode_UserPhone=" + phone);
            var r = CacheHelper.GetUserVerificationCode(phone);

            return r;
        }

        /// <summary>
        /// 暂时添加骑手位置
        /// </summary>
        public void AddRiderPosition()
        {
            List<int> ids = new List<int>();
            List<string> lats = new List<string>();
            List<string> lngs = new List<string>();
            List<int> types = new List<int>();
            ids.Add(1);
            lats.Add("30.277673764984936");
            lngs.Add("120.01765251159668");
            types.Add(1);
            ids.Add(2);
            lats.Add("30.277377273725058");
            lngs.Add("120.01833915710449");
            types.Add(0);
            ids.Add(3);
            lats.Add("30.272781544656915");
            lngs.Add("120.02486228942871");
            types.Add(0);
            ids.Add(4);
            lats.Add("30.276395140028825");
            lngs.Add("120.02072095870972");
            types.Add(1);
            ids.Add(5);
            lats.Add("30.274338186666984");
            lngs.Add("120.02338171005249");
            types.Add(0);
            ids.Add(6);
            lats.Add("30.275765086822346");
            lngs.Add("120.01765251159668");
            types.Add(1);
            ids.Add(7);
            lats.Add("30.274708812078295");
            lngs.Add("120.02263069152832");
            types.Add(1);
            ids.Add(8);
            lats.Add("30.275473222569122");
            lngs.Add("120.02153098583221");
            types.Add(0);
            ids.Add(9);
            lats.Add("30.27496361623675");
            lngs.Add("120.02246975898743");
            types.Add(0);
            for (int i = 0; i < ids.Count; i++)
            {
                CacheHelper.SetRiderPosition(ids[i], lats[i], lngs[i], types[i]);
            }
        }

        [HttpPost]
        public HttpResponseMessage wxpayTest()
        {
            string appid = "wxd8482615c33b8859";//
            string mch_id = "1494504712";//微信支付分配的商户号
            string nonce_str = RandHelper.Instance.Str(24, true); ;//随机字符串，不长于32位。推荐随机数生成算法

            string body = "饭的支付测试";//商品描述-腾讯充值中心-QQ会员充值
            string out_trade_no = "thisisWXOrder" + DateTime.Now.ToString("yyyy-MM-dd");//商品订单号只能是数字、大小写字母_-|*@  32位以内
            var fee = new Decimal(0.01);

            string total_fee = (Math.Round((decimal)fee * 100, 0)).ToString();//总金额
            string spbill_create_ip = "192.168.3.2";//终端ip-用户端实际ip-app传给我
            string notify_url = apiHost + "/notify/wxpayNotify";//通知地址
            string trade_type = "APP";//交易类型

            string mch_key = "2BCF8DD9490E328D2FCEDE7B26643231";

            var dict = new Dictionary<string, string>();
            dict.Add("appid", appid);
            //dict.Add("attach", "attach");
            dict.Add("mch_id", mch_id);
            dict.Add("nonce_str", nonce_str);
            //dict.Add("sign", sign);
            dict.Add("body", body);
            dict.Add("out_trade_no", out_trade_no);
            dict.Add("total_fee", total_fee);
            dict.Add("spbill_create_ip", spbill_create_ip);
            dict.Add("notify_url", notify_url);
            dict.Add("trade_type", trade_type);
            var signTemp = WXPayHelperHere.getParamSrc(dict) + "&key=" + mch_key;
            string sign = MD5Helper.Instance.StrToMD5_UTF8(signTemp);//签名-签名生成算法

            var xml = StringHelperHere.Instance.GetWX_XML(appid, "attach", body, mch_id, nonce_str, notify_url, out_trade_no, spbill_create_ip, total_fee, trade_type, sign);
            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            // 要注意的这是这个编码方式，还有内容的Xml内容的编码方式
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            byte[] data = encoding.GetBytes(xml);

            // 准备请求,设置参数
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/xml";

            //request.ContentLength = data.Length;

            outstream = request.GetRequestStream();
            outstream.Write(data, 0, data.Length);
            outstream.Flush();
            outstream.Close();
            //发送请求并获取相应回应数据

            response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            instream = response.GetResponseStream();

            sr = new StreamReader(instream, encoding);
            //返回结果网页(html)代码

            string content = sr.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            //string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);

            //var b = doc.DocumentElement["return_code"].InnerText;

            JObject jo = new JObject();
            foreach (XmlNode item in doc.DocumentElement.ChildNodes)
            {
                jo.Add(item.Name, item.InnerText);
            }
            //return content;
            //var a = 1;
            return ControllerHelper.Instance.JsonResult(200, JsonConvert.SerializeObject(jo), "");
        }

        [HttpPost]
        public HttpResponseMessage alipayTest()
        {
            AlipayHelperHere a = new AlipayHelperHere();
            var b = a.CreateAlipayOrder("0.01", "thisisorder-" + DateTime.Now.ToString("yyyy-MM-dd") + "-" + DateTime.Now.ToString("hh-MM-ss"), "http://" + apiHost + "/notify/alipayNotify");
            return ControllerHelper.Instance.JsonResult(200, "payStr", b, "");
        }

    }
}
