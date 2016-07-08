using Essence.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Essence.Comms
{
    public class PostsRepository : IDisposable 
    {
        private bool disposed = false;
        private GenericRestfulCrudHttpClient<PostsDTO, int> PostsClient;

        public PostsRepository(string Uri="", string Usuario="", string Password="")
        {
            PostsClient = new GenericRestfulCrudHttpClient<PostsDTO, int>(Uri, "/posts/", Usuario,Password );
        }


        public string ResponseError()
        {
            return PostsClient.responseError;
        }
        
        public async Task<IEnumerable<PostsDTO>> GetPostsAsync()
        {
            return await PostsClient.GetManyAsync();
        }

        public async Task<PostsDTO> GetPostsAsync(int Id)
        {
            PostsClient.AddressSuffix = "/posts/posts/";
            return await PostsClient.GetAsync(Id);
        }

        public async Task<PostsDTO> PostPostsAsync(PostsDTO Posts)
        {
            return await PostsClient.PostAsync(Posts);
        }

        public async Task<PostsDTO> PutPostsAsync(int Id, PostsDTO Posts)
        {
            return await PostsClient.PutAsync(Id, Posts);
        }

        public async Task DeleteMemberAsync(int Id)
        {
            await PostsClient.DeleteAsync(Id);
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
                if (PostsClient != null)
                {
                    var mc = PostsClient;
                    PostsClient = null;
                    mc.Dispose();
                }
                disposed = true;
            }
        }

    }
}
