using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace InstagramMVC.Attributes
{
    /// <summary>
    /// Проверка поля на пробелы
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class NoSpaceFieldAttribute : ValidationAttribute//, IClientValidatable
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format("Поле \"{0}\" не должно содержать пробелов!",  name);
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string FieldValue = value.ToString();
                return !FieldValue.Contains(" ");
            }
            return true;
        }        
    }
}