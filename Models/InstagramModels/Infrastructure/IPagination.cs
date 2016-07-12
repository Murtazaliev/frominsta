using System.Collections.Generic;
using InstagramMVC.Models.InstagramModels;

namespace InstagramMVC.Models.Infrastructure
{
    public interface IPagination<T>
    {
        Pagination Pagination { get; set; }
        List<T> Data { get; set; }
    }
}
