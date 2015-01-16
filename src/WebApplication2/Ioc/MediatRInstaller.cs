
using System.Linq;
using Castle.Core.Internal;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using WebApplication2.IoC;

namespace WebApplication2.Ioc
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
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
            childContainer.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel, true));

            childContainer.Register(
                Component.For<ServiceLocatorProvider>().Instance(serviceLocatorProvider),
                Component.For<IMediator>().ImplementedBy<Mediator>(),
                //Component.For(typeof(IRequestHandler<,>)).ImplementedBy(typeof(MediatorPipeline<,>)).IsDefault(),
                Classes.FromThisAssembly()
                       .BasedOn(typeof (IRequestHandler<,>))
                       .OrBasedOn(typeof (IAsyncRequestHandler<,>))
                       .OrBasedOn(typeof (INotificationHandler<>))
                       .OrBasedOn(typeof (IAsyncNotificationHandler<>))
                       .WithServiceBase());

            (from t in this.GetType().Assembly.GetTypes()
             let i = t.GetInterfaces().SingleOrDefault(i => i.IsGenericType
                   && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
             where t.IsClass && !t.IsAbstract && i != null && !typeof(MediatorPipeline<,>).IsAssignableFrom(t)
             select i).ForEach(handler => childContainer.Register(Component
                 .For(handler)
                 .ImplementedBy((typeof(MediatorPipeline<,>)).MakeGenericType(handler.GetGenericArguments()))
                 .IsDefault()
                 ));

            container.AddChildContainer(childContainer);

            var mediator = childContainer.Resolve<IMediator>();
            container.Register(Component.For<IMediator>().Instance(mediator));
        }
    }
}