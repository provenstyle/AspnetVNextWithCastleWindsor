using Castle.Windsor;
using Microsoft.AspNet.Mvc;

namespace WebApplication2.Controllers
{
    public class WindsorControllerFactory : IControllerFactory
    {
        private readonly IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public object CreateController(ActionContext actionContext)
        {
            var actionDescriptor = (ControllerActionDescriptor) actionContext.ActionDescriptor;
            var type = actionDescriptor.ControllerDescriptor.ControllerTypeInfo.AsType();
            return _container.Kernel.Resolve(type);
        }

        public void ReleaseController(object controller)
        {
            _container.Release(controller);
        }
    }
}