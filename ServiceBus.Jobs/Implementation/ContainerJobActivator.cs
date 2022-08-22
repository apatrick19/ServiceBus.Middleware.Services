//using Hangfire;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ServiceBus.Jobs.Implementation
//{
//    public class ContainerJobActivator : JobActivator
//    {
//        private IUnityContainer _container;

//        public ContainerJobActivator(UnityContainer container)
//        {
//            _container = container;
//        }

//        public override object ActivateJob(Type type)
//        {
//            return _container.Resolve(type);
//        }
//    }
//}
