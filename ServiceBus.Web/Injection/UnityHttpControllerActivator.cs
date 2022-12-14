using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

using Unity;

namespace ServiceBus.Web.Injection
{
    public class UnityHttpControllerActivator: System.Web.Http.Dispatcher.IHttpControllerActivator
    {
        private IUnityContainer _container;

        public UnityHttpControllerActivator(IUnityContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpControllerContext controllerContext, Type controllerType)
        {
            return (IHttpController)_container.Resolve(controllerType);
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return (IHttpController)_container.Resolve(controllerType);
        }
    }
}