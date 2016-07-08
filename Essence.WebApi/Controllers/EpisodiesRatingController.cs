using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Essence.Dto;
using Essence.Repositories;
using Autofac;
using NHibernate;
using System.Linq.Expressions;


namespace mImO.MvcHost.Controllers
{
    public class EpisodiesRatingController : ApiController
    {
        private IRepository<EpisodiesRatingDTO> EpisodiesRatingRepository;
        private ISessionFactory EpisodiesRatingsession;
        private readonly IComponentContext componentContext;

        public EpisodiesRatingController(IRepository<EpisodiesRatingDTO> customerRepository, IComponentContext componentContext)
        {
            this.EpisodiesRatingRepository = customerRepository;
            this.componentContext = componentContext;
        }

        [ActionName("DefaultAction")]
        public IEnumerable<EpisodiesRatingDTO> GetAllEpisodiesRating()
        {
            var rest = EpisodiesRatingRepository.GetAll().ToList();
            return rest;
        }

        [HttpGet]
        public EpisodiesRatingDTO GetEpisodiesRatingById(Guid Id)
        {
            var rest = EpisodiesRatingRepository.GetById(Id);
            return rest;
        }


        [ActionName("DefaultAction")]
        public HttpResponseMessage PostEpisodiesRating(EpisodiesRatingDTO item)
        {
            try
            {

                EpisodiesRatingRepository.GetSession().FlushMode = FlushMode.Auto;
                using (ITransaction transaction = EpisodiesRatingRepository.GetSession().BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        Expression<Func<EpisodiesRatingDTO, bool>> filter = x => x.imdbID == item.imdbID;
                        var EpisodiesRatingDB = EpisodiesRatingRepository.FilterBy(filter).ToList().FirstOrDefault();

                        if (EpisodiesRatingDB != null)
                        {
                            if (item.Rating == 0)
                                EpisodiesRatingRepository.DeleteOnSubmit(EpisodiesRatingDB);
                            else
                            {
                                EpisodiesRatingDB.Rating = item.Rating;
                                EpisodiesRatingRepository.UpdateOnSubmit(EpisodiesRatingDB);
                            }
                        }
                        else
                        {
                            if (item.Rating > 0)
                            {
                                EpisodiesRatingDB = new EpisodiesRatingDTO();
                                EpisodiesRatingDB.imdbID = item.imdbID;
                                EpisodiesRatingDB.Rating = item.Rating;
                                EpisodiesRatingRepository.InsertOnSubmit(EpisodiesRatingDB);
                            }
                        }
                        EpisodiesRatingRepository.SubmitChanges();
                        transaction.Commit();
                        var response = Request.CreateResponse<EpisodiesRatingDTO>(HttpStatusCode.Created, EpisodiesRatingDB);
                        string uri = Url.Link("DefaultApi", new { id = item.Id });
                        response.Headers.Location = new Uri(uri);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return response;
            }
        }

        [ActionName("DefaultAction")]
        public HttpResponseMessage PutEpisodiesRating(Guid Id, EpisodiesRatingDTO item)
        {
            try
            {
                ITransaction transaction;
                EpisodiesRatingRepository.GetSession().FlushMode = FlushMode.Auto;
                transaction = EpisodiesRatingRepository.GetSession().BeginTransaction(IsolationLevel.ReadCommitted);
                Expression<Func<EpisodiesRatingDTO, bool>> filter = x => x.imdbID == item.imdbID;
                var EpisodiesRatingDB = EpisodiesRatingRepository.FilterBy(filter).ToList().FirstOrDefault();
                EpisodiesRatingRepository.UpdateOnSubmit(item);
                EpisodiesRatingRepository.SubmitChanges();
                transaction.Commit();
                var response = Request.CreateResponse<EpisodiesRatingDTO>(HttpStatusCode.Created, item);
                string uri = Url.Link("DefaultApi", new { id = item.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (Exception ex)
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return response;
            }
        }


        [ActionName("DefaultAction")]
        public HttpResponseMessage DeleteEpisodiesRating(Guid Id)
        {
            try
            {
                ITransaction transaction;
                HttpResponseMessage response;
                EpisodiesRatingRepository.GetSession().FlushMode = FlushMode.Auto;
                var item = EpisodiesRatingRepository.GetById(Id);
                if (item != null)
                {
                    transaction = EpisodiesRatingRepository.GetSession().BeginTransaction(IsolationLevel.ReadCommitted);
                    EpisodiesRatingRepository.DeleteOnSubmit(item);
                    EpisodiesRatingRepository.SubmitChanges();
                    transaction.Commit();
                    response = Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                return response;
            }
            catch (Exception ex)
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, ex.InnerException == null ? ex.Message : ex.InnerException.Message);
                return response;
            }
        }
    }
}
