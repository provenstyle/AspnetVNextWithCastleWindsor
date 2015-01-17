using System;

namespace WebApplication2.Ioc
{
    using Castle.MicroKernel;
    using Microsoft.AspNet.Mvc;

    public class WindsorControllerFactory : IControllerFactory
    {
        private readonly IKernel _kernel;
        private readonly IControllerActivator _activator;

        public WindsorControllerFactory(IKernel kernel, IControllerActivator activator)
        {
            _kernel    = kernel;
            _activator = activator;
        }

        public object CreateController(ActionContext actionContext)
        {
            var actionDescriptor = (ControllerActionDescriptor) actionContext.ActionDescriptor;
            var type = actionDescriptor.ControllerDescriptor.ControllerTypeInfo.AsType();
            var controller = _kernel.Resolve(type);
            actionContext.Controller = controller;
            _activator.Activate(controller, actionContext);
            return controller;
        }

        public void ReleaseController(object controller)
        {
            _kernel.ReleaseComponent(controller);
        }
    }
}