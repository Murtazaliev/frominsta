using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using InstagramMVC.Globals;
using InstagramMVC.Models.NavModel;

namespace InstagramMVC.HtmlHelpers
{
    public static class HtmlHelpers
    {
        public static HtmlString OperationOutput(this HtmlHelper helper, object res)
        {
            if (res == null)
            {
                return new HtmlString("");
            }

            string outputTmpl = "<div class='alert alert-dismissible {1}' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Закрыть'>" +
            "<span aria-hidden='true'>&times;</span></button><strong>{0}</strong></div><script>setTimeout(function(){{$('.alert').fadeOut('slow');}}, 5000)</script>";
            try
            {
                OperationResult opRes = (OperationResult)res;
                switch (opRes.Status)
                {
                    case AppEnums.OperationStatus.Error:
                        return new HtmlString(string.Format(outputTmpl, opRes.Message, "alert-danger"));
                    case AppEnums.OperationStatus.Info:
                        return new HtmlString(string.Format(outputTmpl, opRes.Message, "alert-info"));
                    case AppEnums.OperationStatus.Success:
                        return new HtmlString(string.Format(outputTmpl, opRes.Message, "alert-success"));
                    case AppEnums.OperationStatus.Warning:
                        return new HtmlString(string.Format(outputTmpl, opRes.Message, "alert-warning"));
                    default:
                        return new HtmlString("");
                }
            }
            catch
            {
                return new HtmlString("");
            }            
        }

        public static HtmlString WaitingScreen(this HtmlHelper helper, string imgLink)
        { 
            return new HtmlString(string.Format(
@"<div class='modal' id='waiting' role='dialog'>
    <div class='modal-dialog'>
        <div class='modal-content'>
            <div class='modal-body' style='text-align: center;'>
                <div class='row'>
                    <div>
                        <strong>Идет загрузка страницы ...</strong>
                    </div>
                    <div style='height: 50px'>
                        <img src='{0}' />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>", imgLink));
        }

        public static IDisposable BeginProfilePanel(this HtmlHelper helper, TabPanel tabPanel, int activeTabID)
        {
            return new ProfilePanel(helper, tabPanel, activeTabID);
        }

        class ProfilePanel : IDisposable
        {
            private HtmlHelper _helper;
            private TabPanel _tabPanel;
            private int _activeTabID;

            public ProfilePanel(HtmlHelper helper, TabPanel tabPanel, int activeTabID)
            {
                _helper = helper;
                _tabPanel = tabPanel;
                _activeTabID = activeTabID;

                if (this._tabPanel != null)
                {
                    StringBuilder HtmlOutput = new StringBuilder();
                    //this._helper.ViewContext.Writer.Write("<ul class=\"nav nav-tabs\">");
                    HtmlOutput.AppendLine("<ul class=\"nav nav-tabs\">");
                    string user_role = InstagramMVC.DataManagers.UserManager.GetUserRole(this._helper.ViewContext.HttpContext.User.Identity.Name);

                    foreach (var item in tabPanel.Tabs.OrderBy(x => x.TabOrderPos))
                    {
                        string[] allowed_roles = item.TabGroupIDs.Split(',');
                        if (allowed_roles.Contains(user_role))
                        {
                            if (item.TabOrderPos == activeTabID)
                            {
                                //this._helper.ViewContext.Writer.Write(string.Format("<li class='active'><a href='#curtab' data-toggle='tab'>{0}</a></li>", item.TabCaption));
                                HtmlOutput.AppendLine(string.Format("<li class='active'><a href='#curtab' data-toggle='tab'>{0}</a></li>", item.TabCaption));
                                item.TabActive = true;
                            }
                            else
                            {
                                //this._helper.ViewContext.Writer.Write(string.Format("<li><a href='{0}'>{1}</a></li>", item.TabLinkURL, item.TabCaption));
                                HtmlOutput.AppendLine(string.Format("<li><a href='{0}'>{1}</a></li>", item.TabLinkURL, item.TabCaption));
                            }
                        }                        
                    }

                    //this._helper.ViewContext.Writer.Write("</ul>");
                    HtmlOutput.AppendLine("</ul>");
                    if (!tabPanel.Tabs.Any(x => x.TabActive))
                    {
                        HtmlOutput.Clear();
                    }
                    //this._helper.ViewContext.Writer.Write("<div class='tab-content'>");
                    HtmlOutput.AppendLine("<div class='tab-content'>");
                    //this._helper.ViewContext.Writer.Write("<div class='tab-pane active' id='curtab'>");
                    HtmlOutput.AppendLine("<div class='tab-pane active' id='curtab'>");
                    this._helper.ViewContext.Writer.Write(HtmlOutput.ToString());
                }                
            }

            public void Dispose()
            {
                if (this._tabPanel != null)
                {
                    this._helper.ViewContext.Writer.Write("</div></div>");
                }                
            }
        }

        public static MvcHtmlString Pager(this HtmlHelper htmlHelper, int curPage, int totalPages, Func<int, string> PageURL)
        {
            //диапозон видимых ссылок на страницы
            int VisibleLinksCount = 3;
            int offset = 1;
            var Links = new Dictionary<string, string>();

            if (totalPages < VisibleLinksCount)
            {
                VisibleLinksCount = totalPages;
            }

            if (curPage > totalPages)
            {
                curPage = totalPages;
            }


            if (curPage < VisibleLinksCount)
            {
                for (int i = 1; i <= VisibleLinksCount; i++)
                {
                    if (i == curPage)
                    {
                        //если тек. стр., то не добавляем ссылку
                        Links.Add(i.ToString(), "");
                    }
                    else
                    {
                        Links.Add(i.ToString(), PageURL(i));
                    }
                }

                if (VisibleLinksCount < totalPages)
                {
                    Links.Add(" ... ", "");
                    Links.Add(totalPages.ToString(), PageURL(totalPages));
                }
            }
            else
            {
                //1 ... 3 4 5 6 7 ... 21
                
                if (totalPages <= curPage + offset)
                {
                    if (curPage - VisibleLinksCount > 0)
                    {
                        Links.Add("1", PageURL(1));
                        Links.Add(" ...", "");
                    }

                    for (int i = totalPages - VisibleLinksCount + 1; i <= totalPages; i++)
                    {
                        if (i == curPage)
                        {
                            //если тек. стр., то не добавляем ссылку
                            Links.Add(i.ToString(), "");
                        }
                        else
                        {
                            Links.Add(i.ToString(), PageURL(i));
                        }
                    }
                }
                else
                {
                    Links.Add("1", PageURL(1));
                    Links.Add(" ...", "");

                    for (int i = curPage - offset; i <= curPage + offset; i++)
                    {
                        if (i == curPage)
                        {
                            //если тек. стр., то не добавляем ссылку
                            Links.Add(i.ToString(), "");
                        }
                        else
                        {
                            Links.Add(i.ToString(), PageURL(i));
                        }
                    }

                    Links.Add("... ", "");
                    Links.Add(totalPages.ToString(), PageURL(totalPages));
                }
            }

            var res = new StringBuilder();
            res.AppendLine("<div class=\"container text-center\">");
            res.AppendLine("<nav>");
            res.AppendLine("<ul class=\"pagination\">");
            foreach (var key in Links.Keys)
            {
                if (string.IsNullOrEmpty(Links[key]))
                {
                    if (key.Contains("..."))
                    {
                        res.AppendFormat("<li class=\"disabled\"><a href=\"#\">{0} <span class=\"sr-only\">(current)</span></a></li>", key.Trim());
                    }
                    else
                    {
                        res.AppendFormat("<li class=\"active\"><a href=\"#\">{0} <span class=\"sr-only\">(current)</span></a></li>", key);
                    }
                }
                else
                {
                    res.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", Links[key], key);
                }
            }
            res.AppendLine("<ul>");
            res.AppendLine("</nav>");
            res.AppendLine("</div>");

            return MvcHtmlString.Create(res.ToString());
        }
    }
}