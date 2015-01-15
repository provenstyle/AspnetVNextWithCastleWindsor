using System;
using System.Collections.Generic;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Services.Logging.SerilogIntegration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
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

            container.AddFacility<LoggingFacility>(f => f.LogUsing<SerilogFactory>());
            container.Register(Component.For<IServiceProvider>().ImplementedBy<WindsorServiceProvider>());
            container.Register(Component.For<IServiceScopeFactory>().ImplementedBy<WindsorServiceScopeFactory>());
            container.Register(Component.For<IControllerFactory>().ImplementedBy<WindsorControllerFactory>());
            container.Install(FromAssembly.This());

            var serviceId = 1;
            foreach (var serviceDescriptor in services)
                RegisterService(container, serviceDescriptor, String.Format("MVC_{0}", ++serviceId));

            return container.Resolve<IServiceProvider>();
        }

        private static void RegisterService(IWindsorContainer container, IServiceDescriptor serviceDescriptor, string name)
        {
            var service = serviceDescriptor.ServiceType;
            if (serviceDescriptor.ImplementationInstance != null)
            {
                container.Register(Component.For(service)
                         .Instance(serviceDescriptor.ImplementationInstance)
                         .Named(name));
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                var factory = serviceDescriptor.ImplementationFactory;
                container.Register(Component.For(service)
                    .UsingFactoryMethod(kernel =>
                    {
                        var provider = kernel.Resolve<IServiceProvider>();
                        return factory(provider);
                    })
                    .ConfigureLifeCycle(serviceDescriptor.Lifecycle));
            }
            else
            {
                var implementation = serviceDescriptor.ImplementationType;
                container.Register(Component.For(service ?? implementation)
                         .ImplementedBy(implementation)
                         .ConfigureLifeCycle(serviceDescriptor.Lifecycle)
                         .Named(name));
            }
        }

        private class WindsorServiceProvider : IServiceProvider
        {
            private IKernel _kernel;

            public WindsorServiceProvider(IKernel kernel)
            {
                _kernel = kernel;
            }

            public object GetService(Type serviceType)
            {
                return _kernel.Resolve(serviceType);
            }
        }

        private class WindsorServiceScopeFactory : IServiceScopeFactory
        {
            private readonly IKernel _kernel;

            public WindsorServiceScopeFactory(IKernel kernel)
            {
                _kernel = kernel;
            }

            public IServiceScope CreateScope()
            {
                return new WindsorServiceScope(_kernel);
            }
        }

        private class WindsorServiceScope : IServiceScope
        {
            private readonly IKernel _kernel;
            private readonly IDisposable _scope;

            public WindsorServiceScope(IKernel kernel)
            {
                _kernel = kernel;
                _scope = _kernel.BeginScope();
                ServiceProvider = _kernel.Resolve<IServiceProvider>();
            }

            public IServiceProvider ServiceProvider { get; }

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