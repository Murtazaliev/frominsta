using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//https://gist.github.com/chaoaretasty/1671586

namespace InstagramMVC.Binders
{
    public class DateTimeBinder : IModelBinder
    {
        //private DateTime? GetValue(ModelBindingContext bindingContext, string key)
        //{
        //    if (string.IsNullOrEmpty(key))
        //    {
        //        return null;
        //    }

        //    string ModelName = bindingContext.ModelName;
        //    ValueProviderResult vpr = bindingContext.ValueProvider.GetValue(string.Format("{0}.{1}" ,ModelName, key));
        //}

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("BindingContext is needed!!!");
            }
            string ModelName = bindingContext.ModelName;

            var DatePartKey = ModelName + ".Date";
            var HourPartKey = ModelName + ".Hour";
            var MinPartKey = ModelName + ".Minute";

            ValueProviderResult vprDate = bindingContext.ValueProvider.GetValue(DatePartKey);
            ValueProviderResult vprHour = bindingContext.ValueProvider.GetValue(HourPartKey);
            ValueProviderResult vprMin = bindingContext.ValueProvider.GetValue(MinPartKey);

            var DatePart = System.DateTime.Now;
            var HourPart = 0;
            var MinPart = 0;


            if (vprDate == null || vprDate.AttemptedValue == "")
            {
                //если DateTime не отображено через шаблон DateAndTime.cshtml, то брать из названия модели, т.е. имя поля
                try
                {
                    vprDate = bindingContext.ValueProvider.GetValue(ModelName);
                }
                catch 
                { 
                    //если ни что не помогает выставить тек. дату
                    vprHour = new ValueProviderResult(DateTime.Now, DateTime.Now.ToString(), System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                }

                if (vprDate == null)
                {
                    //значит пустое значение ввели, нодо вызвать ModelStateError как-то или наследовать от DefaultModelBinder(!скорей всего так)
                    vprDate = new ValueProviderResult(DateTime.Now, DateTime.Now.ToString(), System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                }
                
            }

            if (vprHour == null || vprHour.AttemptedValue == "")
            {
                vprHour = new ValueProviderResult(0, "0", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (vprMin == null || vprMin.AttemptedValue == "")
            {
                vprMin = new ValueProviderResult(0, "0", null);
            }

            //сохранить занчения для прорисовки формы по-новой 
            bindingContext.ModelState.SetModelValue(DatePartKey, vprDate);
            bindingContext.ModelState.SetModelValue(HourPartKey, vprHour);
            bindingContext.ModelState.SetModelValue(MinPartKey, vprMin);

            DatePart = Convert.ToDateTime(vprDate.AttemptedValue);//, new CultureInfo("ru-RU", true)
            HourPart = Convert.ToInt32(vprHour.AttemptedValue);//, new CultureInfo("ru-RU", true)
            MinPart = Convert.ToInt32(vprMin.AttemptedValue);//, new CultureInfo("ru-RU", true)

            var res = new DateTime(DatePart.Year, DatePart.Month, DatePart.Day, HourPart, MinPart, 0);

            return res;
        }
    }
}