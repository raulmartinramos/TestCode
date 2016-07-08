using System;
using System.Data;
using NHibernate;
using mImO.Repositories;


namespace mImO.DataProviders.NHibernate
{
    public interface INHibSessionProvider
    {
        ISession Session { get; }
    }



    public class NHibernateUnitOfWork : IUnitOfWork, INHibSessionProvider
    {
	    private ITransaction transaction;
	    public ISession Session { get; private set; }

        public NHibernateUnitOfWork(Func<ISessionFactory> sessionFactory)
	    {
            Session = sessionFactory().OpenSession();
            Session.FlushMode = FlushMode.Auto;
            this.transaction = Session.BeginTransaction(IsolationLevel.ReadCommitted);
	    }


	    public void Dispose()
	    {
		    if(Session.IsOpen)
		    {
			    Session.Close();
		    }
	    }

	    public void Commit()
	    {
		    if(!transaction.IsActive)
		    {
			    throw new InvalidOperationException("No active transation");
		    }
		    transaction.Commit();
	    }

	    public void Rollback()
	    {
		    if(transaction.IsActive)
		    {
			    transaction.Rollback();
		    }
	    }
    }
}
