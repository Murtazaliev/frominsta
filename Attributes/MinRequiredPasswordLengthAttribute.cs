using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using InstagramMVC.DataManagers;

namespace InstagramMVC.Attributes
{
    //аттрибут проверки мин длины пароля. длина пароля берется из GS_Const.MinPasswordLength
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class MinRequiredPasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _minimumLength = Convert.ToInt32(UtilManager.GetVarValue("app.minpasswordlength"));
        public override string FormatErrorMessage(string name)
        {
            //return String.Format(System.Globalization.CultureInfo.CurrentCulture, ErrorMessageString, name, _minimumLength);
            return string.Format("Минимальная длина пароля {0} символов!", _minimumLength);
        }
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string password = value.ToString();
                return password.Length >= this._minimumLength;
            }

            return true;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minimumLength, int.MaxValue)
            };
        }
    }
}