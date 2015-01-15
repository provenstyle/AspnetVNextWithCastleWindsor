using System;
using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Microsoft.AspNet.Mvc;

namespace WebApplication2.Ioc
{
    public class FallbackLazyComponentLoader : ILazyComponentLoader
    {
        private IServiceProvider _fallbackProvider;

        public FallbackLazyComponentLoader(IServiceProvider provider)
        {
            _fallbackProvider = provider;
        }

        public IRegistration Load(string name, Type service, IDictionary arguments)
        {
            System.Console.WriteLine("{0}", service);

            var serviceFromFallback = _fallbackProvider.GetService(service);

            if (serviceFromFallback != null)
            {
                return Component.For(service).Instance(serviceFromFallback);
            }

            return null;
        }
    }
}