using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Collections.Generic;
using WebApplication2.Ioc;

namespace Microsoft.Framework.DependencyInjection.Windsor
{
    public class KServiceInstaller : IWindsorInstaller
    {
        private IEnumerable<IServiceDescriptor> _services;

        public KServiceInstaller(IEnumerable<IServiceDescriptor> services)
        {
            _services = services;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            foreach (var service in _services)
            {
                if (service.ImplementationType != null)
                {
                    container.Register(Component.For(service.ServiceType)
                                                .ImplementedBy(service.ImplementationType)
                                                .ConfigureLifeCycle(service.Lifecycle));
                }
                else
                {
                    if(service.ImplementationInstance == null) continue;
                    container.Register(Component.For(service.ServiceType)
                                                .Instance(service.ImplementationInstance)
                                                .ConfigureLifeCycle(service.Lifecycle));
                }
            }
        }
    }
}