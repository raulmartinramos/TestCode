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
    public class EpisodiesRatingRepository : IDisposable 
    {
        private bool disposed = false;
        private string usuario;
        private string password;

        private GenericRestfulCrudHttpClient<EpisodiesRatingDTO, Guid> EpisodiesRatingClient;

        public EpisodiesRatingRepository(string Uri="", string Usuario="", string Password="")
        {
            EpisodiesRatingClient = new GenericRestfulCrudHttpClient<EpisodiesRatingDTO, Guid>(ServiceContainer.Resolve<DataFrameWork>().Uribackend, "/Pruebas/api/EpisodiesRating/", Usuario, Password);
            usuario = Usuario;
            password = Password;

        }

        public string ResponseError()
        {
            return EpisodiesRatingClient.responseError;
        }
        
        public async Task<IEnumerable<EpisodiesRatingDTO>> GetEpisodiesRatingAsync()
        {
            return await EpisodiesRatingClient.GetManyAsync();
        }


        public async Task<EpisodiesRatingDTO> PostEpisodiesRatingAsync(EpisodiesRatingDTO EpisodiesRating)
        {
            return await EpisodiesRatingClient.PostAsync(EpisodiesRating);
        }

        public async Task<EpisodiesRatingDTO> PutEpisodiesRatingAsync(Guid Id, EpisodiesRatingDTO EpisodiesRating)
        {
            return await EpisodiesRatingClient.PutAsync(Id, EpisodiesRating);
        }

        public async Task DeleteMemberAsync(Guid Id)
        {
            await EpisodiesRatingClient.DeleteAsync(Id);
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
                if (EpisodiesRatingClient != null)
                {
                    var mc = EpisodiesRatingClient;
                    EpisodiesRatingClient = null;
                    mc.Dispose();
                }
                disposed = true;
            }
        }

    }
}
