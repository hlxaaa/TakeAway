using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Extend;
using System.ComponentModel.DataAnnotations;

namespace takeAwayWebApi.Attribute
{
    public class DateTimeValidateAttribute : ValidationAttribute
    {
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool AllowEmpty { get; set; }
        public override bool IsValid(object value)
        {
            if (AllowEmpty == true)
            {
                return true;
            }
            if (value == null)
            {
                return false;
            }
            else
            {
                DateTime date = (DateTime)value;
                if (date.IsNullOrEmpty())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}