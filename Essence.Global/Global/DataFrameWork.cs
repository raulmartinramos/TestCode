using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Essence.Global
{
    public class DataFrameWork
    {
        private string uriomdbapi;
        private string uribackend;
        private string username;
        private string password;
        private CacheProvider imageCache;
        private CacheProvider seriesCache;
        private Func<HttpClient> httpClientFactory;
        


        public DataFrameWork()
        {
            ImageCache = new CacheProvider();
            SeriesCache = new CacheProvider();
        }


        public string Uriomdbapi { get { return uriomdbapi; } set { if (value != null) { uriomdbapi = value; } } }
        public string Uribackend { get { return uribackend; } set { if (value != null) { uribackend = value; } } }
        public string Username { get { return username; } set { if (value != null) { username = value; } } }
        public string Password { get { return password; } set { if (value != null) { password = value; } } }
        public CacheProvider ImageCache { get { return imageCache; } set { if (value != null) { imageCache = value; } } }
        public CacheProvider SeriesCache { get { return seriesCache; } set { if (value != null) { seriesCache = value; } } }
        public Func<HttpClient> HttpClientFactory { get { return httpClientFactory; } set { if (value != null) { httpClientFactory = value; } } }
        

    }
}

