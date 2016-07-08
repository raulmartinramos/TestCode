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


namespace mImO.MvcHost.Controllers
{
    public class SeriesFavoritesController : ApiController
    {
        private IRepository<SeriesFavoritesDTO> SeriesFavoritesRepository;
        private ISessionFactory SeriesFavoritessession;
        private readonly IComponentContext componentContext;

        public SeriesFavoritesController(IRepository<SeriesFavoritesDTO> customerRepository, IComponentContext componentContext)
        {
            this.SeriesFavoritesRepository = customerRepository;
            this.componentContext = componentContext;
        }

        [ActionName("DefaultAction")]
        public IEnumerable<SeriesFavoritesDTO> GetAllSeriesFavorites()
        {
            var rest = SeriesFavoritesRepository.GetAll().ToList();
            return rest;
        }

        [HttpGet]
        public SeriesFavoritesDTO GetSeriesFavoritesById(Guid Id)
        {
            var rest = SeriesFavoritesRepository.GetById(Id);
            return rest;
        }


        [ActionName("DefaultAction")]
        public HttpResponseMessage PostSeriesFavorites(SeriesFavoritesDTO item)
        {
            try
            {
                ITransaction transaction;
                SeriesFavoritesRepository.GetSession().FlushMode = FlushMode.Auto;
                transaction = SeriesFavoritesRepository.GetSession().BeginTransaction(IsolationLevel.ReadCommitted);
                SeriesFavoritesRepository.InsertOnSubmit(item);
                SeriesFavoritesRepository.SubmitChanges();
                transaction.Commit();
                var response = Request.CreateResponse<SeriesFavoritesDTO>(HttpStatusCode.Created, item);
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
        public HttpResponseMessage PutSeriesFavorites(Guid Id, SeriesFavoritesDTO item)
        {
            try
            {
                ITransaction transaction;
                SeriesFavoritesRepository.GetSession().FlushMode = FlushMode.Auto;
                transaction = SeriesFavoritesRepository.GetSession().BeginTransaction(IsolationLevel.ReadCommitted);
                SeriesFavoritesRepository.UpdateOnSubmit(item);
                SeriesFavoritesRepository.SubmitChanges();
                transaction.Commit();
                var response = Request.CreateResponse<SeriesFavoritesDTO>(HttpStatusCode.Created, item);
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
        public HttpResponseMessage DeleteSeriesFavorites(Guid Id)
        {
            try
            {
                ITransaction transaction;
                HttpResponseMessage response;
                SeriesFavoritesRepository.GetSession().FlushMode = FlushMode.Auto;
                var item = SeriesFavoritesRepository.GetById(Id);
                if (item != null)
                {
                    transaction = SeriesFavoritesRepository.GetSession().BeginTransaction(IsolationLevel.ReadCommitted);
                    SeriesFavoritesRepository.DeleteOnSubmit(item);
                    SeriesFavoritesRepository.SubmitChanges();
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
