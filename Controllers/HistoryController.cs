using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IO;

namespace history.Controllers
{
    [Route("history")]
    public class HistoryController : Controller
    {
        private IMemoryCache _memoryCache;
        public HistoryController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        // GET: api/users
        [HttpGet]
        public IActionResult Get()
        {
            var date = DateTime.Now;
            var fileName = string.Format("./files/{0}-{1}.json", date.Month, date.Day);
            try
            {
                string cacheKey = string.Format("historycache-{0}{1}", date.Month, date.Day);
                var data =_memoryCache.Get(cacheKey) as string;
                if(data == null)
                {
                    System.Console.WriteLine("file not in memory cache.");
                    System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fs))
                    {
                        data = sr.ReadToEnd();
                    }

                    if(!string.IsNullOrEmpty(data))
                        _memoryCache.Set(cacheKey, data);
                }

                return Ok(data);
            }
            catch(Exception ex)
            {
                return Ok(new {message = string.Format("can't find {0} file.", fileName)});
            }
        }

    }
}