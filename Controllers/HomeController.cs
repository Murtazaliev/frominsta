using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using InstagramMVC.Models;
using InstagramMVC.Models.InstagramModels.Responses;
using InstagramMVC.InstagramEngine;
using InstagramMVC.Models.InstagramModels;

namespace InstagramMVC.Controllers
{
    public class HomeController : Controller
    {
        private string _access_token;

        public HomeController()
        {
            _access_token = InstagramMVC.DataManagers.UtilManager.GetVarValue("instagram.accesstoken");            
        }

        public ActionResult Index()
        {
            //ПОПРОБОВАТЬ В ПОНЕДЕЛЬНИК С ТЕМ ЖЕ accesstoken, чтобы убедиться сколько он действует, вызвав http://localhost:37264/Home/searchByTag
            //попробовать сохранять accesstoken с выгруженным профилем в инстаграм и делать с ним запросы
            //сохраненный accesstoken работает(медленно может из-за async, поробовать без async), 
            //но не известно какое время, поэтому в Admin сделать п-т "Обновить accesstoken" и прописывать его в базу
            //if (Session["Instagram"] == null)
            //{
            //    return RedirectToAction("LoginToInstagram");
            //}
            //return View("SearchByTag");
            return View();
        }

        public PartialViewResult UserInfoVidget()
        {
            if (Session["Instagram"] != null)
            {
                UserInfo userinfo = ((InstagramAuthResponse)Session["Instagram"]).user;
                return PartialView(userinfo);
            }

            return PartialView();
        }

        public ActionResult SearchByTag()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SearchByTag(string search_tag)
        {
            //InstagramAuthResponse iar = (InstagramAuthResponse)Session["Instagram"];
            var insta = new Instagram(_access_token);//iar.access_token
            List<Media> res = await insta.SearchByTagAsync(search_tag);
            return View(res);

            if (Session["Instagram"] != null)
            {
                ViewBag.Tag = search_tag;

                //**************************************************************************
                //InstagramAuthResponse iar = (InstagramAuthResponse)Session["Instagram"];
                //var insta = new Instagram(iar.access_token);
                //List<Media> res = await insta.SearchByTagAsync(search_tag);
                //return View(res);
                //***************************************************************************

                //var tagSearchUrl = string.Format("https://api.instagram.com/v1/tags/{0}/media/recent?access_token={1}", search_tag, iar.access_token);

                //WebClient client = new WebClient();
                //Task<byte[]> ds = client.DownloadDataTaskAsync(tagSearchUrl);
                //var result = await ds;
                //var response = System.Text.Encoding.Default.GetString(result);

                //List<Media> ii = new List<Media>();
                //MediasResponse mr = JsonConvert.DeserializeObject<MediasResponse>(response);
                //int max_iteration = 2;
                //while (max_iteration > 0 && !string.IsNullOrEmpty(mr.Pagination.NextUrl))
                //{
                //    ii.AddRange(mr.Data);
                //    tagSearchUrl = mr.Pagination.NextUrl;
                //    ds = client.DownloadDataTaskAsync(tagSearchUrl);
                //    result = await ds;
                //    response = System.Text.Encoding.Default.GetString(result);
                //    mr = JsonConvert.DeserializeObject<MediasResponse>(response);

                //    --max_iteration;
                //}
                //return View(ii);


                //ii.AddRange(mr.Data);

                //dynamic jsResult = JsonConvert.DeserializeObject(response);
                //foreach (var data in jsResult.data)
                //{
                //    ii.Add(new Image
                //    {
                //        LowResolution = new Resolution { Url = data.images.low_resolution.url, Height = data.images.low_resolution.height, Width = data.images.low_resolution.width },
                //        StandardResolution = new Resolution { Url = data.images.standard_resolution.url, Height = data.images.standard_resolution.height, Width = data.images.standard_resolution.width },
                //        Thumbnail = new Resolution { Url = data.images.thumbnail.url, Height = data.images.thumbnail.height, Width = data.images.thumbnail.width }
                //    }); 
                //}

                
            }
            return View();
        }

        public ActionResult Demo(string tag = "test")
        {
            return View((object)tag);
        }

        [AllowAnonymous]
        public JsonResult DemoTags(string tag)
        {            
            List<Media> res = new List<Media>();
            var insta = new Instagram(_access_token);
            res = insta.SearchByTag(tag, 5);//HttpUtility.UrlEncode(tag, System.Text.Encoding.GetEncoding("utf-8"))

            if (res.Count == 0)
            {
                res.Add(new Media
                {
                    User = new User { Username = "Извините, ничего не найдено!", ProfilePicture = Url.Content("~/Content/img/show_logo.png") },
                    Images = new Image
                    {
                        StandardResolution = new Resolution
                        {
                            Url = Url.Content("~/Content/img/wedding.jpg"),
                            Height = 1014
                        }
                    },
                    Caption = new Caption() { Text = "Извините, ничего не найдено!" }
                });
            }
            
            return Json(res.Select(x => new { User = x.User.Username,
                                              UserProfileUrl = x.User.ProfilePicture, 
                                              Url = x.Images.StandardResolution.Url, 
                                              Caption = x.Caption.Text,
                                              Width = x.Images.StandardResolution.Width, 
                                              Height = x.Images.StandardResolution.Height }), JsonRequestBehavior.AllowGet);
        }
    }
}
