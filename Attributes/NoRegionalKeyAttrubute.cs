using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace InstagramMVC.Attributes
{
    /// <summary>
    /// Только латинские символы, цифры и "_"
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class NoRegionalKey : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format("Поле \"{0}\" должно содержать только латинские буквы или цифры!", name);
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string FieldValue = value.ToString();
                var regexp = new Regex(@"^[A-Za-z0-9_]+$");
                return regexp.Match(FieldValue).Success;
            }
            return true;
        }   
    }
}