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
    public class SeriesFavoritesRepository : IDisposable 
    {
        private bool disposed = false;
        private string usuario;
        private string password;

        private GenericRestfulCrudHttpClient<SeriesFavoritesDTO, Guid> SeriesFavoritesClient;

        public SeriesFavoritesRepository(string Uri="", string Usuario="", string Password="")
        {
            SeriesFavoritesClient = new GenericRestfulCrudHttpClient<SeriesFavoritesDTO, Guid>(ServiceContainer.Resolve<DataFrameWork>().Uribackend, "/Pruebas/api/SeriesFavorites/", Usuario, Password);
            usuario = Usuario;
            password = Password;

        }

        public string ResponseError()
        {
            return SeriesFavoritesClient.responseError;
        }
        
        public async Task<IEnumerable<SeriesFavoritesDTO>> GetSeriesFavoritesAsync()
        {
            return await SeriesFavoritesClient.GetManyAsync();
        }


        public async Task<SeriesFavoritesDTO> PostSeriesFavoritesAsync(SeriesFavoritesDTO SeriesFavorites)
        {
            return await SeriesFavoritesClient.PostAsync(SeriesFavorites);
        }

        public async Task<SeriesFavoritesDTO> PutSeriesFavoritesAsync(Guid Id, SeriesFavoritesDTO SeriesFavorites)
        {
            return await SeriesFavoritesClient.PutAsync(Id, SeriesFavorites);
        }

        public async Task DeleteMemberAsync(Guid Id)
        {
            await SeriesFavoritesClient.DeleteAsync(Id);
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
                if (SeriesFavoritesClient != null)
                {
                    var mc = SeriesFavoritesClient;
                    SeriesFavoritesClient = null;
                    mc.Dispose();
                }
                disposed = true;
            }
        }

    }
}
