using System;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Event;
using Essence.Dto;
using log4net;



namespace Essence.DataProviders.NHibernate
{
    
    public class NHibernateHelper
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string connectionString;
        private ISessionFactory sessionFactory;

        public ISessionFactory SessionFactory
        {
            get { return sessionFactory ?? (sessionFactory = CreateSessionFactory()); }
        }

        public NHibernateHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private ISessionFactory CreateSessionFactory()
        {
            try
            {
                log.Error("Inicializando NHibernate : " + connectionString);
                return Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008
                    .ConnectionString(connectionString))
                    .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                    .ExposeConfiguration(cfg =>
                    {
                        cfg.EventListeners.PreInsertEventListeners = new IPreInsertEventListener[] { new NHibernateEventListener() };
                        cfg.EventListeners.PreUpdateEventListeners = new IPreUpdateEventListener[] { new NHibernateEventListener() };
                    })
                    .BuildSessionFactory();
            }
            catch (Exception ex)
            {
                //llevar a log
                log.Error("Inicializando NHibernate : " + ex.InnerException);
                return null;
            }
            return null; 
        }

    }
}