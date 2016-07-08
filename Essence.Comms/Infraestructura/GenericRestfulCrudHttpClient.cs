using Essence.IOC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Essence.Global;

namespace Essence.Comms
{
    public class GenericRestfulCrudHttpClient<T, TResourceIdentifier> : IDisposable where T : class, new()
    {
        private bool disposed = false;
        public string responseError;
        public System.Net.HttpStatusCode responseCode;
        private string Username;
        private string Password;
        private HttpClient httpClient;
        private string serviceBaseAddress;
        //private readonly string addressSuffix;
        private string addressSuffix;

        private String proxy;
        private String puerto;
        private String usuario;
        private String password;
        private Boolean activoproxy;


        private readonly string jsonMediaType = "application/json";
        private CancellationTokenSource tokenSource = new CancellationTokenSource();


        public GenericRestfulCrudHttpClient(string serviceBaseAddress, string addressSuffix, string username, string password)
        {
            var _DataFramework = ServiceContainer.Resolve<DataFrameWork>();
            serviceBaseAddress = string.IsNullOrEmpty(serviceBaseAddress) ? _DataFramework.Uriomdbapi : serviceBaseAddress;
            username = string.IsNullOrEmpty(username) ? _DataFramework.Username : username;
            password = string.IsNullOrEmpty(password) ? _DataFramework.Password : password;

            this.serviceBaseAddress = serviceBaseAddress;
            this.addressSuffix = addressSuffix;
            this.Username = username;
            this.Password = password;
            httpClient = MakeHttpClient(serviceBaseAddress);
        }

        public CancellationTokenSource TokenSource { get { return tokenSource; } set { tokenSource = value; } }
        public String AddressSuffix { get { return addressSuffix; } set { addressSuffix = value; } }
        public String ServiceBaseAddress { get { return serviceBaseAddress; } set { serviceBaseAddress = value; } }

        protected virtual HttpClient MakeHttpClient(string serviceBaseAddress)
        {
            //var signingHandler = new HmacSigningHandler(Username, Password, new CanonicalRepresentationBuilder(),
            //                                        new HmacSignatureCalculator());

            //if (signingHandler.SupportsAutomaticDecompression)
            //{
            //    signingHandler.AutomaticDecompression = DecompressionMethods.GZip |
            //                                     DecompressionMethods.Deflate;
            //}


            //if (ProxyConnect.Activo && !string.IsNullOrEmpty(ProxyConnect.Proxy))
            //{
            //    WebProxy p = new WebProxy(ProxyConnect.Proxy, Int32.Parse(ProxyConnect.Puerto));
            //    p.Credentials = new NetworkCredential(ProxyConnect.Usuario, ProxyConnect.Password);
            //    signingHandler.UseProxy = true;
            //    signingHandler.Proxy = p;
            //}


            //httpClient = new HttpClient(new RequestContentMd5Handler()
            //{
            //    InnerHandler = signingHandler
            //});

            //httpClient = new HttpClient();
            //httpClient = HttpClientFactory.Get();
            httpClient = ServiceContainer.Resolve<DataFrameWork>().HttpClientFactory();
            httpClient.BaseAddress = new Uri(serviceBaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(jsonMediaType));
            //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
            //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            return httpClient;
        }



        public async Task<IEnumerable<T>> GetManyAsync()
        {
            
            HttpResponseMessage responseMessage = await httpClient.GetAsync(addressSuffix);
            if (!responseMessage.IsSuccessStatusCode)   
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
            var resultString = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<T>>(resultString);
        }

        public async Task<IEnumerable<T>> GetManyAsyncByFilter(string Filter)
        {
            HttpResponseMessage responseMessage = await httpClient.GetAsync(addressSuffix+Filter);
            if (!responseMessage.IsSuccessStatusCode)
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
            var resultString = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<T>>(resultString);
        }

        public async Task<T> GetAsyncByFilter(string Filter)
        {
            var responseMessage = await httpClient.GetAsync(addressSuffix + Filter);
            if (!responseMessage.IsSuccessStatusCode)
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
            var resultString = responseMessage.Content.ReadAsStringAsync().Result;
            var prueba = JsonConvert.DeserializeObject<T>(resultString);
            return JsonConvert.DeserializeObject<T>(resultString);
        }

        public async Task<T> GetAsync(TResourceIdentifier identifier)
        {
            var responseMessage = await httpClient.GetAsync(addressSuffix + identifier.ToString());
            if (!responseMessage.IsSuccessStatusCode)
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
            var resultString = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(resultString);
        }


        public async Task<T> PostAsync(T model)
        {
            var serializedContent = JsonConvert.SerializeObject(model);
            HttpContent httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.PostAsync(addressSuffix, httpContent);
            if (!responseMessage.IsSuccessStatusCode)
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
            var resultString = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(resultString);
        }

        public async Task<T> PutAsync(TResourceIdentifier identifier, T model)
        {
            var serializedContent = JsonConvert.SerializeObject(model);
            HttpContent httpContent = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.PutAsync(addressSuffix + identifier.ToString(), httpContent);
            if (!responseMessage.IsSuccessStatusCode)
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
            var resultString = responseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(resultString);
        }

        public async Task DeleteAsync(TResourceIdentifier identifier)
        {
            var responseMessage = await httpClient.DeleteAsync(addressSuffix + identifier.ToString());
            if (!responseMessage.IsSuccessStatusCode)
            {
                responseError = responseMessage.Content.ReadAsStringAsync().Result.ToString();
                responseCode = responseMessage.StatusCode;
            }
            responseMessage.EnsureSuccessStatusCode();
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (httpClient != null)
                {
                    var hc = httpClient;
                    httpClient = null;
                    hc.Dispose();
                }
                disposed = true;
            }
        }

        #endregion IDisposable Members
    }
}
