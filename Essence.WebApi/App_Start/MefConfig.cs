using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace Essence.WebApi
{
    public static class MefConfig
    {
        public static void RegisterMef()
        {
            var container = ConfigureContainer();

            ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(container));

            var dependencyResolver = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver;
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new MefDependencyResolver(container);
        }

        private static CompositionContainer ConfigureContainer()
        {
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var container = new CompositionContainer(assemblyCatalog);

            return container;
        }
    }

    public class MefDependencyResolver : IDependencyResolver
    {
        private readonly CompositionContainer _container;

        public MefDependencyResolver(CompositionContainer container)
        {
            _container = container;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            var export = _container.GetExports(serviceType, null, null).SingleOrDefault();

            return null != export ? export.Value : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var exports = _container.GetExports(serviceType, null, null);
            var createdObjects = new List<object>();

            if (exports.Any())
            {
                foreach (var export in exports)
                {
                    createdObjects.Add(export.Value);
                }
            }

            return createdObjects;
        }

        public void Dispose()
        {
            ;
        }
    }

    public class MefControllerFactory : DefaultControllerFactory
    {
        private readonly CompositionContainer _compositionContainer;

        public MefControllerFactory(CompositionContainer compositionContainer)
        {
            _compositionContainer = compositionContainer;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            var export = _compositionContainer.GetExports(controllerType, null, null).SingleOrDefault();

            IController result;

            if (null != export)
            {
                result = export.Value as IController;
            }
            else
            {
                result = base.GetControllerInstance(requestContext, controllerType);
                _compositionContainer.ComposeParts(result);
            }

            return result;
        }
    }
}