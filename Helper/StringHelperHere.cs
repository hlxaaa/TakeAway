using Common;
using Common.Helper;
using DbOpertion.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace takeAwayWebApi.Helper
{
    public class StringHelperHere : SingleTon<StringHelperHere>
    {
        /// <summary>
        /// 正则：6~16位英文字母、数字
        /// </summary>
        public string regPwd = @"^[0-9a-zA-z]{6,16}$";

        public string regAccount = @"^[0-9a-zA-z]{5,16}$";

        public bool IsPwdValidate(string pwd)
        {
            Regex reg = new Regex(regPwd);
            return reg.IsMatch(pwd);
        }

        public bool IsAccountValidate(string pwd)
        {
            Regex reg = new Regex(regAccount);
            return reg.IsMatch(pwd);
        }

        /// <summary>
        /// 生成微信需要的xml字符串
        /// </summary>
        /// <returns></returns>
        public string GetWX_XML(string appid, string attach, string body, string mch_id, string nonce_str, string notify_url, string out_trade_no, string spbill_create_ip, string total_fee, string trade_type, string sign)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            sb.Append("<appid>" + appid + "</appid>");
            sb.Append("<attach>" + attach + "</attach>");
            sb.Append("<body>" + body + "</body>");
            sb.Append("<mch_id>" + mch_id + "</mch_id>");
            sb.Append("<nonce_str>" + nonce_str + "</nonce_str>");
            sb.Append("<notify_url>" + notify_url + "</notify_url>");
            sb.Append("<out_trade_no>" + out_trade_no + "</out_trade_no>");
            sb.Append("<spbill_create_ip>" + spbill_create_ip + "</spbill_create_ip>");
            sb.Append("<total_fee>" + total_fee + "</total_fee>");
            sb.Append("<trade_type>" + trade_type + "</trade_type>");
            sb.Append("<sign>" + sign + "</sign>");
            sb.Append("</xml>");

            return sb.ToString();
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 获取字符串的最后五位，少则前方补0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string GetLastFiveStr(string str)
        {
            do
            {
                str = "0" + str;
            } while (str.Length < 5);
            return str.Substring(str.Length - 5, 5);
        }

        /// <summary>
        /// 微信计算签名
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="mchKey"></param>
        /// <returns></returns>
        public string GetWXSign(Dictionary<string, string> dict, string mchKey)
        {
            var signTemp = WXPayHelperHere.getParamSrc(dict) + "&key=" + mchKey;
            return MD5Helper.Instance.StrToMD5_UTF8(signTemp);
        }

        /// <summary>
        /// 数组join成字符串，加'的那种
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public string ArrJoin(Array arr)
        {
            var str = "";
            foreach (var item in arr)
            {
                if (item != null)
                    str += "'" + item + "',";
            }
            return RemoveLastOne(str);
        }

        /// <summary>
        /// 去掉字符串最后一个字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string RemoveLastOne(string str)
        {
            if (str.Length < 1)
                return "字符串长度小于1";
            return str.Substring(0, str.Length - 1);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="word">被加密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后字符串</returns>
        public string Encrypt(string word, string key)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-zA-Z]*$"))
            {
                throw new Exception("key 必须由字母组成");
            }
            key = key.ToLower();
            //逐个字符加密字符串
            char[] s = word.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                char a = word[i];
                char b = key[i % key.Length];
                s[i] = EncryptChar(a, b);
            }
            return new string(s);
        }

        /// <summary>
        /// 加密单个字符
        /// </summary>
        /// <param name="a">被加密字符</param>
        /// <param name="b">密钥</param>
        /// <returns>加密后字符</returns>
        private char EncryptChar(char a, char b)
        {
            int c = a + b - 'a';
            if (a >= '0' && a <= '9') //字符0-9的转换
            {
                while (c > '9') c -= 10;
            }
            else if (a >= 'a' && a <= 'z') //字符a-z的转换
            {
                while (c > 'z') c -= 26;
            }
            else if (a >= 'A' && a <= 'Z') //字符A-Z的转换
            {
                while (c > 'Z') c -= 26;
            }
            else return a; //不再上面的范围内，不转换直接返回
            return (char)c; //返回转换后的字符
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="word">被解密字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后字符串</returns>
        public string Decrypt(string word, string key)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-zA-Z]*$"))
            {
                throw new Exception("key 必须由字母组成");
            }
            key = key.ToLower();
            //逐个字符解密字符串
            char[] s = word.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                char a = word[i];
                char b = key[i % key.Length];
                s[i] = DecryptChar(a, b);
            }
            return new string(s);
        }

        /// <summary>
        /// 解密单个字符
        /// </summary>
        /// <param name="a">被解密字符</param>
        /// <param name="b">密钥</param>
        /// <returns>解密后字符</returns>
        private char DecryptChar(char a, char b)
        {
            int c = a - b + 'a';
            if (a >= '0' && a <= '9') //字符0-9的转换
            {
                while (c < '0') c += 10;
            }
            else if (a >= 'a' && a <= 'z') //字符a-z的转换
            {
                while (c < 'a') c += 26;
            }
            else if (a >= 'A' && a <= 'Z') //字符A-Z的转换
            {
                while (c < 'A') c += 26;
            }
            else return a; //不再上面的范围内，不转换直接返回
            return (char)c; //返回转换后的字符
        }

        /// <summary>
        /// 数字转字符串
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public string IntToKey(string no)
        {
            string str = no;
            char[] s = str.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = IntToKeyChar(s[i]);
            }
            return new string(s);
        }

        /// <summary>
        /// 字符串转数字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string KeyToInt(string key)
        {
            string str = key;
            char[] s = str.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = CharToInt(s[i]);
            }
            return new string(s);
        }

        /// <summary>
        /// 数字字符转英文字符
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public char IntToKeyChar(char a)
        {
            switch (a)
            {
                case '0':
                    return 'l';
                case '1':
                    return 'y';
                case '2':
                    return 'e';
                case '3':
                    return 's';
                case '4':
                    return 'a';
                case '5':
                    return 'w';
                case '6':
                    return 'c';
                case '7':
                    return 'q';
                case '8':
                    return 'b';
                case '9':
                    return 'j';
                default:
                    return 'n';
            }
        }

        /// <summary>
        /// 英文字符转数字字符
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public char CharToInt(char a)
        {
            switch (a)
            {
                case 'l':
                    return '0';
                case 'y':
                    return '1';
                case 'e':
                    return '2';
                case 's':
                    return '3';
                case 'a':
                    return '4';
                case 'w':
                    return '5';
                case 'c':
                    return '6';
                case 'q':
                    return '7';
                case 'b':
                    return '8';
                case 'j':
                    return '9';
                default:
                    return 'n';
            }
        }

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string CreateOrderNo(string userId)
        {
            var str = DateTime.Now.ToString("yyyyMMddHHmmss") + GetLastFiveStr(userId) + RandHelper.Instance.Number(3);
            return str;
        }

        public decimal KeepDecimal(decimal d)
        {
            string result = d.ToString("#0.00");
            return Convert.ToDecimal(result);
        }
    }

}
