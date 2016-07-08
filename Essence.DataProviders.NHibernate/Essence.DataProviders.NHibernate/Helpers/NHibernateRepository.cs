using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using NHibernate.Criterion;
using Essence.Repositories;
using Essence.Dto;


namespace Essence.NHibernate
{

    public class NHibernateRepository<T> : IRepository<T> where T : PersistableBase, new()
    {

        private readonly ISession Session;

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

        public T GetById(Guid id)
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


        public void InsertUpdateOnSubmit(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public void DeleteOnSubmit(T entity)
        {
            Session.Delete(entity);
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
