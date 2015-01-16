
namespace WebApplication2.Ioc
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Microsoft.AspNet.Mvc;

    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly().BasedOn<Controller>().WithServiceSelf().LifestyleScoped()
                );
        }
    }
}