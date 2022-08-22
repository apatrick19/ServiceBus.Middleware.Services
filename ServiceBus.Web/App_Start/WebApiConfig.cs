using ServicBus.Logic.Contracts;
using ServicBus.Logic.Implementations;
using ServicBus.Logic.Implementations.IO.Image;
using ServiceBus.Web.Injection;
using ServiceBus.Custom.Contract;
using ServiceBus.Custom.Implementation;
using ServiceBus.Data.Contracts;
using ServiceBus.Data.Implementation.DataAccess;
using ServiceBus.Logic.Contracts;
using ServiceBus.Logic.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using ServiceBus.Logic;
using manny.ussd.logic.Integration;
using ServiceBus.Logic.Contracts.Service_Contracts;
using ServiceBus.Logic.Service;
using ServiceBus.Logic.Contracts.BankOne;
using ServiceBus.Logic.Integration.BankOne;

namespace ServiceBus.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // config.EnableCors();

            config.MessageHandlers.Add(new CustomLogHandler());

            //DI Container Registration
            var container = new UnityContainer();
            container.RegisterType<IApiPostAndGet, ApiPostAndGet>(new HierarchicalLifetimeManager());
            container.RegisterType<IFileConverter, FileConverter>(new HierarchicalLifetimeManager());
            container.RegisterType<IServiceDapper, ServiceDapper>(new HierarchicalLifetimeManager());

             //validation service 
            container.RegisterType<ILoginService, LoginService>(new HierarchicalLifetimeManager());
            container.RegisterType<IBankOneCoreBankingAuthentication, BankOneCoreBankingAuthentication>(new HierarchicalLifetimeManager());
            container.RegisterType<IAccountValidationService, AccountValidationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPostingIntegrationService, PostingIntegrationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IBankOneCardIntegration, BankOneCardIntegration>(new HierarchicalLifetimeManager());

           

            //New Registration
            container.RegisterType<IAccountByAccountNoIntegration, BankOneAccountByAccountNoIntegration>(new HierarchicalLifetimeManager());
            container.RegisterType<IBankOneAccountCreationIntegration, BankOneAccountCreationIntegration>(new HierarchicalLifetimeManager());
            container.RegisterType<IBankOneBVNValidationIntegration, BankOneBVNValidationIntegration>(new HierarchicalLifetimeManager());
            container.RegisterType<IBankOneTransactionIntegration, BankOneTransactionIntegration>(new HierarchicalLifetimeManager());
            //End

            container.RegisterType<IGenericBaseService, GenericBaseService>(new HierarchicalLifetimeManager());
            container.RegisterType<ICardService, CardService>(new HierarchicalLifetimeManager());
            container.RegisterType<ITransactionService, TransactionService>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserService, UserService>(new HierarchicalLifetimeManager());
            container.RegisterType<IMessengerService, MessengerService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAccountCreationService, AccountCreationService>(new HierarchicalLifetimeManager());
            container.RegisterType<ITransferIntegration, BankOneTransferIntegration>(new HierarchicalLifetimeManager());
            container.RegisterType<IBillerLogicService, BillerLogicService>(new HierarchicalLifetimeManager());
            container.RegisterType<ITransferLogicService, TransferLogicService>(new HierarchicalLifetimeManager());
            container.RegisterType<ITransferBaseService, TransferBaseService>(new HierarchicalLifetimeManager());
            container.RegisterType<IBillingService, BillingService>(new HierarchicalLifetimeManager());
            container.RegisterType<ILoginService, LoginService>(new HierarchicalLifetimeManager());
            container.RegisterType<IAccountService, AccountService>(new HierarchicalLifetimeManager());


            //New Services 
            container.RegisterType<IAccountGenericService, AccountGenericService>(new HierarchicalLifetimeManager());
            container.RegisterType<ITransactionGenericService, TransactionGenericService>(new HierarchicalLifetimeManager());

            config.DependencyResolver = new UnityResolver(container);
        }
    }
}
