namespace WebApplication2.Ioc
{
    using System;
    using System.Collections;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers;

    public class FallbackLazyComponentLoader : ILazyComponentLoader
    {
        private readonly IServiceProvider _fallbackProvider;

        public FallbackLazyComponentLoader(IServiceProvider provider)
        {
            _fallbackProvider = provider;
        }

        public IRegistration Load(string name, Type service, IDictionary arguments)
        {
            var serviceFromFallback = _fallbackProvider.GetService(service);
            return serviceFromFallback != null
                 ? Component.For(service).Instance(serviceFromFallback)
                 : null;
        }
    }
}