using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using takeAwayWebApi.Helper;

namespace takeAwayWebApi.Attribute
{
    /// <summary>
    /// 密码
    /// </summary>
    public class PwdValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            this.ErrorMessage = "密码应为6~16位英文字母、数字";
            if (value != null)
            {
                var RegexStr = StringHelperHere.Instance.regPwd;
                if (Regex.IsMatch(value.ToString(), RegexStr))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 纬度
    /// </summary>
    public class LatValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            this.ErrorMessage = "纬度应为0-90度";
            if (value != null)
            {
                var temp = Convert.ToDecimal(value);
                if (temp >= 0 && temp <= 90)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }

    /// <summary>
    /// 经度
    /// </summary>
    public class LngValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            this.ErrorMessage = "经度应为0-180度";
            if (value != null)
            {
                var temp = Convert.ToDecimal(value);
                if (temp >= 0 && temp <= 180)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public class IdNotZeroValidateAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var temp = Convert.ToInt32(value);
                if (temp > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 评价星级
    /// </summary>
    public class StarsValidateAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                var temp = Convert.ToInt32(value);
                if (temp >= 0 && temp <= 5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}