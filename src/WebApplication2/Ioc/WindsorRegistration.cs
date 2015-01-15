using System;
using System.Collections.Generic;
using System.Net;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Windsor;
using WebApplication2.Controllers;

namespace WebApplication2.Ioc
{
    public static class WindsorRegistration
    {
        /// <summary>
        /// Registers all services with the Windsor Container and sets up the fallback provider
        /// </summary>
        /// <param name="container"></param>
        /// <param name="services"></param>
        /// <param name="fallbackProvider"></param>
        public static IServiceProvider Populate(IWindsorContainer container, IEnumerable<IServiceDescriptor> services, IServiceProvider fallbackProvider)
        {
            var fallbackComponentLoader = new FallbackLazyComponentLoader(fallbackProvider);
            container.Register(Component.For<ILazyComponentLoader>().Instance(fallbackComponentLoader));

            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            container.Register(Component.For<IWindsorContainer>().Instance(container));
            container.Register(Component.For<IServiceProvider>().ImplementedBy<WindsorServiceProvider>());
            container.Register(Component.For<IServiceScopeFactory>().ImplementedBy<WindsorServiceScopeFactory>());
            container.Register(Component.For<IControllerFactory>().ImplementedBy<WindsorControllerFactory>());
            container.Install(new KServiceInstaller(services), FromAssembly.This());

            return container.Resolve<IServiceProvider>();
        }

        private class WindsorServiceProvider : IServiceProvider
        {
            private IWindsorContainer _container;

            public WindsorServiceProvider(IWindsorContainer container)
            {
                _container = container;
            }

            public object GetService(Type serviceType)
            {
                return _container.Resolve(serviceType);
            }
        }

        private class WindsorServiceScopeFactory : IServiceScopeFactory
        {
            private readonly IWindsorContainer _container;

            public WindsorServiceScopeFactory(IWindsorContainer container)
            {
                _container = container;
            }

            public IServiceScope CreateScope()
            {
                return new WindsorServiceScope(_container);
            }
        }

        private class WindsorServiceScope : IServiceScope
        {
            private readonly IWindsorContainer _container;
            private readonly IServiceProvider _serviceProvider;
            private readonly IDisposable _scope;

            public WindsorServiceScope(IWindsorContainer container)
            {
                _container = container;
                _scope = _container.BeginScope();
                _serviceProvider = _container.Resolve<IServiceProvider>();
            }

            public IServiceProvider ServiceProvider
            {
                get { return _serviceProvider; }
            }

            public void Dispose()
            {
                _scope.Dispose();
            }
        }

        internal static ComponentRegistration<object> ConfigureLifeCycle(this ComponentRegistration<object> registration, LifecycleKind kind)
        {
            switch (kind)
            {
                case LifecycleKind.Scoped:
                    registration.LifestyleScoped();
                    break;
                case LifecycleKind.Singleton:
                    registration.LifestyleSingleton();
                    break;
                case LifecycleKind.Transient:
                    registration.LifestyleTransient();
                    break;
            }

            return registration;
        }
    }
}