using Essence.Dto;
using Essence.Global;
using Essence.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Essence.Comms
{
    public class SearchRepository : IDisposable 
    {
        private bool disposed = false;
        private GenericRestfulCrudHttpClient<SearchDTO, int> SearchClient;

        public SearchRepository(string Uri="", string Usuario="", string Password="")
        {
            SearchClient = new GenericRestfulCrudHttpClient<SearchDTO, int>(ServiceContainer.Resolve<DataFrameWork>().Uriomdbapi, "", Usuario, Password);
        }

        public string ResponseError()
        {
            return SearchClient.responseError;
        }
        
        public async Task<IEnumerable<SearchDTO>> GetSearchAsync()
        {
            return await SearchClient.GetManyAsync();
        }

        public async Task<SearchDTO> GetSearchAsyncByTitle(string title)
        {
            string Filter = String.Format("?s=*{0}*&Type=series", title);
            return await SearchClient.GetAsyncByFilter(Filter);
        }

        public async Task<SearchDTO> PostSearchAsync(SearchDTO Search)
        {
            return await SearchClient.PostAsync(Search);
        }

        public async Task<SearchDTO> PutSearchAsync(int Id, SearchDTO Search)
        {
            return await SearchClient.PutAsync(Id, Search);
        }

        public async Task DeleteMemberAsync(int Id)
        {
            await SearchClient.DeleteAsync(Id);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (SearchClient != null)
                {
                    var mc = SearchClient;
                    SearchClient = null;
                    mc.Dispose();
                }
                disposed = true;
            }
        }

    }
}
