using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NHibernate;
using Autofac;
using Autofac.Integration.WebApi;
using Essence.DataProviders.NHibernate;
using Essence.NHibernate;
using Essence.Repositories;

namespace Essence.WebApi
{
    // Nota: para obtener instrucciones sobre cómo habilitar el modo clásico de IIS6 o IIS7, 
    // visite http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        public class Tracker
        {
            public bool IsResolved { get; set; }
        }
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();  
            //MefConfig.RegisterMef();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.Register(x => new NHibernateHelper(System.Configuration.ConfigurationManager.ConnectionStrings["EssenceDb"].ConnectionString).SessionFactory).SingleInstance();
            //builder.Register(x => x.Resolve<NHibernateHelper>().SessionFactory).SingleInstance();
            builder.RegisterType<Tracker>().InstancePerApiRequest();
            builder.Register(x => x.Resolve<ISessionFactory>().OpenSession()).As<ISession>().InstancePerApiRequest();
            builder.RegisterGeneric(typeof(NHibernateRepository<>)).As(typeof(IRepository<>)).InstancePerApiRequest();
            //builder.RegisterGeneric(typeof(NHibernateRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            log4net.Config.XmlConfigurator.Configure();


        }

    }





}