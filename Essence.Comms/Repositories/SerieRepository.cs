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
    public class SerieRepository : IDisposable 
    {
        private bool disposed = false;
        private GenericRestfulCrudHttpClient<SerieDTO, int> SerieClient;

        public SerieRepository(string Uri="", string Usuario="", string Password="")
        {
            var c=ServiceContainer.Resolve<DataFrameWork>();
            SerieClient = new GenericRestfulCrudHttpClient<SerieDTO, int>(ServiceContainer.Resolve<DataFrameWork>().Uriomdbapi, "", Usuario, Password);
        }

        public string ResponseError()
        {
            return SerieClient.responseError;
        }
        
        public async Task<IEnumerable<SerieDTO>> GetSerieAsync()
        {
            return await SerieClient.GetManyAsync();
        }

        public async Task<IEnumerable<SerieDTO>> GetSerieAsyncByTitle(string title)
        {
            string Filter = String.Format("?s={0}&Type=series", title);
            return await SerieClient.GetManyAsyncByFilter(Filter);
        }

        public async Task<SerieDTO> GetSerieAsyncById(string imdbID)
        {
            string Filter = String.Format("?i={0}&", imdbID);
            return await SerieClient.GetAsyncByFilter(Filter);
        }

        public async Task<SerieDTO> GetSerieAsyncByTemporada(string title, string season, string episode)
        {
            string Filter = String.Format("?t={0}&Season={1}&Episode={2}", title, season, episode);
            return await SerieClient.GetAsyncByFilter(Filter);
        }

        public async Task<SerieDTO> PostSerieAsync(SerieDTO Serie)
        {
            return await SerieClient.PostAsync(Serie);
        }

        public async Task<SerieDTO> PutSerieAsync(int Id, SerieDTO Serie)
        {
            return await SerieClient.PutAsync(Id, Serie);
        }

        public async Task DeleteMemberAsync(int Id)
        {
            await SerieClient.DeleteAsync(Id);
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
                if (SerieClient != null)
                {
                    var mc = SerieClient;
                    SerieClient = null;
                    mc.Dispose();
                }
                disposed = true;
            }
        }

    }
}
