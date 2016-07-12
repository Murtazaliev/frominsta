using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using InstagramMVC.Models.InstagramModels;
using InstagramMVC.Models.InstagramModels.Responses;
using Newtonsoft.Json;

namespace InstagramMVC.InstagramEngine
{
    public class Instagram
    {        
        private string search_by_recent_tag_pattern = ConfigurationManager.AppSettings["instagram.apiuri"].ToString() + "tags/{0}/media/recent?access_token={1}";
        private string search_by_max_tag_id_pattern = ConfigurationManager.AppSettings["instagram.apiuri"].ToString() + "tags/{0}/media/recent?access_token={1}&min_tag_id={2}";
        private string access_token;

        public Instagram(string AccessToken)
        {
            access_token = AccessToken;
        }

        /// <summary>
        /// Загрузка содержимого url и десериализация в JSON
        /// </summary>
        private async Task<MediasResponse> DownloadMediaResponseAsync(string url)
        {
            MediasResponse res = new MediasResponse();
            using (var client = new WebClient())
            {
                try
                {
                    var result = await client.DownloadDataTaskAsync(url);
                    var response = System.Text.Encoding.Default.GetString(result);

                    res = JsonConvert.DeserializeObject<MediasResponse>(response);
                }
                catch
                {}
            }

            return res;
        }

        private MediasResponse DownloadMediaResponse(string url)
        {
            MediasResponse res = new MediasResponse();
            using (var client = new WebClient())
            {
                try
                {
                    //****************************************************
                    //********* убрать при комп-ии на сервере ************
                    //****************************************************
                    //client.Proxy = new WebProxy("192.168.116.99", 8080);
                    var result = client.DownloadData(url);
                    var response = System.Text.Encoding.Default.GetString(result);

                    res = JsonConvert.DeserializeObject<MediasResponse>(response);
                }
                catch
                { }
            }

            return res;
        }

        /// <summary>
        /// Поиск по тегу
        /// </summary>
        /// <param name="Tag">Название тега</param>
        /// <param name="TotalResults">Отобразить кол-во результатов (0 - все)</param>
        /// <returns></returns>
        public async Task<List<Media>> SearchByTagAsync(string Tag, int TotalResults = 20)
        {
            //ограничить на 100 результатов поиска
            TotalResults = TotalResults == 0 ? 30 : TotalResults;

            List<Media> res = new List<Media>();

            string tagSearchUrl = string.Format(search_by_recent_tag_pattern, Tag, access_token);
            try
            {

                int ItemsLoaded = 0;
                do
                {
                    var mr = await DownloadMediaResponseAsync(tagSearchUrl);
                    ItemsLoaded += mr.Data.Count;

                    if (ItemsLoaded > TotalResults)
                    {
                        res.AddRange(mr.Data.Take(TotalResults));
                        tagSearchUrl = string.Empty;
                    }
                    else
                    { 
                        res.AddRange(mr.Data);
                        tagSearchUrl = mr.Pagination.NextUrl;
                    }

                    TotalResults -= ItemsLoaded;

                                        
                } while (!string.IsNullOrEmpty(tagSearchUrl));
            }
            catch (Exception e)
            { 
            
            }
            
            return res;
        }

        public List<Media> SearchByTag(string Tag, int TotalResults, int min_tag_id = 0)
        {
            //ограничить на 30 результатов поиска
            //https://api.instagram.com/v1/tags/test/media/recent?max_tag_id=1136838388667352749&client_id=b59fbe4563944b6c88cced13495c0f49&callback=? - эта хуйня работает
            TotalResults = TotalResults == 0 ? 30 : TotalResults;
            //next_max_id = string.Empty;
            //max_tag_id - до
            //min_tag_id - нихуя не работает
            //TODO deleted загружаются снова
            List<Media> res = new List<Media>();
            string tagSearchUrl = string.Format(search_by_recent_tag_pattern, HttpUtility.UrlDecode(Tag, System.Text.Encoding.GetEncoding("utf-8")), access_token);
            DateTime last_tag_create_time = DateTime.Now;
            if (min_tag_id > 0)
            {
                InstagramMVC.Models.MediaTag mediatag = InstagramMVC.DataManagers.HashTagManager.GetMediaTagByID(min_tag_id);
                if (mediatag != null)
                {
                    last_tag_create_time = mediatag.INSTAGRAM_MEDIA_CREATED_TIME;
                    TotalResults = 200;
                }
                mediatag = null;
            }
            //string.Format(search_by_max_tag_id_pattern, HttpUtility.UrlDecode(Tag, System.Text.Encoding.GetEncoding("utf-8")), access_token, max_tag_id)
            try
            {
                int ItemsLoaded = 0;
                do
                {
                    var mr = DownloadMediaResponse(tagSearchUrl);
                    if (mr.Data.Count > 0)
                    {
                        int min_tag_idx = mr.Data.FindIndex(x => x.CreatedTime <= last_tag_create_time);
                        if (min_tag_idx > 0)
                        {
                            res.AddRange(mr.Data.Take(min_tag_idx));
                            break;
                        }
                        ItemsLoaded += mr.Data.Count;

                        if (ItemsLoaded > TotalResults)
                        {
                            res.AddRange(mr.Data.Take(TotalResults - res.Count));
                            tagSearchUrl = string.Empty;
                        }
                        else
                        {
                            res.AddRange(mr.Data);
                            tagSearchUrl = mr.Pagination.NextUrl;
                        }

                        //TotalResults -= mr.Data.Count;
                    }
                    else
                    {
                        break;
                    }

                    //next_max_id = mr.Pagination.NextMaxId;
                } while (!string.IsNullOrEmpty(tagSearchUrl));
                //обратитить элементы, т.к. Instagram выдает фото отсорт по времени DESCENDING, т.е. 1-е фото самое новое
                res.Reverse();
            }
            catch (Exception e)
            {

            }

            return res;
        }
    }
}