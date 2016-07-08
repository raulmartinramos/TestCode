using System.Linq;
using System;
using NHibernate;

using System.Collections.Generic;

namespace Essence.Repositories
{
    public interface IRepository<T>
        where T: new()
    {
        T GetById(Guid id);
        IEnumerable<T> GetByIds(IEnumerable<Guid> ids);
        ISession GetSession();
        IQueryable<T> GetAll();
        T FindBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression);
        IQueryable<T> FilterBy(System.Linq.Expressions.Expression<System.Func<T, bool>> expression);
        void InsertOnSubmit(T entity);
        void UpdateOnSubmit(T entity);
        void MergeOnSubmit(T entity);
        void InsertUpdateOnSubmit(T entity);
        void DeleteOnSubmit(T entity);
        void DeleteOnSubmit(Guid Id);
        void SubmitChanges();
    }
}
