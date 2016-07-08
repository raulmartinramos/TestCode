using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using mImO.Repositories;
using mImO.Contracts;
using mImO.Contracts.DTOs;
using System.Net.Http;
using Dapper;
using System.Collections.Generic;
using NHibernate.Criterion;


namespace mImO.DataProviders.NHibernate
{

    public class NHibernateRepository<T> : IRepository<T>, IDisposable  where T : PersistableBase, new()
    {

        private readonly ISession Session;
        private readonly HttpRequestMessage Request;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (Request != null)
                    Request.Dispose();
        }
        ~NHibernateRepository()
        {
            Dispose(false);
        }
        //public NHibernateRepository(Func<ISession> session)
        //public NHibernateRepository(ISession session, HttpRequestMessage requestMessage)
        //{
        //    Session = session;
        //    Request = requestMessage;
        //}

        public NHibernateRepository(ISession session)
        {
            Session = session;
        }

        public ISession GetSession()
        {
            return Session;
        }

        public T GetById(string id)
        {
            return Session.Load<T>(id);
        }

        public IEnumerable<T> GetByIds(System.Collections.Generic.IEnumerable<Guid> ids)
        {
            //var myCriteria = Session.CreateCriteria<T>().Add(Restrictions.In("Id", ids.ToArray()));
            var myCriteria = Session.CreateCriteria<T>().Add(Restrictions.In(Projections.Property("Id"), ids.ToArray()));
            
            return myCriteria.List<T>();
            //return myCriteria.List<T>().ToList<T>();
        }

        public T GetById( Guid  id)
        {
            return Session.Load<T>(id);
        }

        public IQueryable<T> GetAll()
        {
            return Session.Query<T>();
        }


        public T FindBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression)
        {
            return FilterBy(expression).Single();
        }

        public IQueryable<T> FilterBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression)
        {
            return GetAll().Where(expression).AsQueryable();

        }


        public void InsertOnSubmit(T entity)
        {
            Session.Save(entity);
        }


        public void UpdateOnSubmit(T entity)
        {
            Session.Update(entity);
        }

        public void MergeOnSubmit(T entity)
        {
            Session.Merge(entity);
        }

        public void InsertUpdateCopyOnSubmit(T entity)
        {
            Session.SaveOrUpdateCopy(entity);
        }

        public void InsertUpdateOnSubmit(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public void DeleteOnSubmit(T entity) 
        {
            Session.Delete(entity);

            //Grabamos un historico de todos los registros eliminados
            T var = new T();
            string tablename = var.GetType().Name.Substring(0, var.GetType().Name.Length - 3);
            if (Enum.IsDefined(typeof(TiposTablas), tablename))
            {
                TiposTablas tipotabla = (TiposTablas)System.Enum.Parse(typeof(TiposTablas), tablename);
                PersistableBase item = ((PersistableBase)entity);
                Session.Save(new Entidades_EliminadasDTO() { IdEntidad = item.Id, IdEPsCreador = item.IdEPsCreador, IdCreador = item.IdCreador, TipoTabla = tipotabla });
            }
        }

        public void DeleteOnSubmit(Guid Id)
        {
            Session.Delete(Id);
        }

        public void SubmitChanges()
        {
            Session.Flush();
        }
    }
}
