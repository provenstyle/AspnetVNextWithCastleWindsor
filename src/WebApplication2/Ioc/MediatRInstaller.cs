
namespace WebApplication2.Ioc
{
    using System;
    using System.IO;
    using System.Net.NetworkInformation;
    using Castle.Windsor;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using CommonServiceLocator.WindsorAdapter;
    using MediatR;
    using Microsoft.Practices.ServiceLocation;

    public class MediatRInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var childContainer         = new WindsorContainer();
            var serviceLocator         = new WindsorServiceLocator(childContainer);
            var serviceLocatorProvider = new ServiceLocatorProvider(() => serviceLocator);
            childContainer.Kernel.AddHandlersFilter(new ContravariantFilter());
            childContainer.Register(
                Component.For<ServiceLocatorProvider>().Instance(serviceLocatorProvider),
                Classes.FromAssemblyContaining<IMediator>().Pick().WithServiceAllInterfaces(),
                Classes.FromThisAssembly()
                       .BasedOn(typeof (IRequestHandler<,>))
                       .OrBasedOn(typeof (IAsyncRequestHandler<,>))
                       .OrBasedOn(typeof (INotificationHandler<>))
                       .OrBasedOn(typeof (IAsyncNotificationHandler<>))
                       .WithServiceBase());
            container.AddChildContainer(childContainer);

            var mediator = childContainer.Resolve<IMediator>();
            container.Register(Component.For<IMediator>().Instance(mediator));
        }
    }
}