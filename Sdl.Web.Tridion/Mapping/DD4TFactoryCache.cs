﻿using System.Collections.Generic;
using DD4T.ContentModel.Contracts.Caching;
using DD4T.ContentModel.Contracts.Configuration;
using DD4T.ContentModel.Contracts.Logging;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Resolvers;
using DD4T.ContentModel.Factories;
using DD4T.Factories;
using DD4T.Providers.SDLWeb85.CIL;
using DD4T.Utils;
using DD4T.Utils.Caching;
using Sdl.Web.Common.Configuration;
using Sdl.Web.Legacy;
using Sdl.Web.Tridion.Caching;

namespace Sdl.Web.Tridion.Mapping
{
    /// <summary>
    /// Cache for DD4T Factories; one for each Localization.
    /// </summary>
    internal static class DD4TFactoryCache
    {
        private static readonly IDictionary<string, IPageFactory> _pageFactories = new Dictionary<string, IPageFactory>();
        private static readonly IDictionary<string, IComponentPresentationFactory> _componentPresentationFactories = new Dictionary<string, IComponentPresentationFactory>();
        private static readonly IDictionary<string, IComponentFactory> _componentFactories = new Dictionary<string, IComponentFactory>();
        private static readonly IDictionary<string, IBinaryFactory> _binaryFactories = new Dictionary<string, IBinaryFactory>();

        private static readonly ILogger _logger = new DD4TLoggerAdapter();
        private static readonly IDD4TConfiguration _config = new DD4TConfiguration();

        internal static ICacheAgent CreateDefaultCacheAgent()
        {
            return new DefaultCacheAgent(_config, _logger);
        }

        internal static ICacheAgent CreateCacheAgent()
        {
            return new DD4TCacheAgentAdapter();
        }    

        internal static IPageFactory GetPageFactory(Localization localization)
        {
            lock (_pageFactories)
            {
                IPageFactory pageFactory;
                if (!_pageFactories.TryGetValue(localization.Id, out pageFactory))
                {
                    IPublicationResolver publicationResolver = new PublicationResolver(localization);
                    IProvidersCommonServices providersCommonServices = new ProvidersCommonServices(publicationResolver, _logger, _config);
                    IFactoryCommonServices factoryCommonServices = new FactoryCommonServices(publicationResolver, _logger, _config, CreateCacheAgent());
                    pageFactory = new PageFactory(
                        new TridionPageProvider(providersCommonServices), 
                        GetComponentPresentationFactory(localization), 
                        factoryCommonServices
                        ); 
                    _pageFactories.Add(localization.Id, pageFactory);
                }

                return pageFactory;
            }
        }

        internal static IComponentPresentationFactory GetComponentPresentationFactory(Localization localization)
        {
            lock (_componentPresentationFactories)
            {
                IComponentPresentationFactory componentPresentationFactory;
                if (!_componentPresentationFactories.TryGetValue(localization.Id, out componentPresentationFactory))
                {
                    IPublicationResolver publicationResolver = new PublicationResolver(localization);
                    IProvidersCommonServices providersCommonServices = new ProvidersCommonServices(publicationResolver, _logger, _config);
                    IFactoryCommonServices factoryCommonServices = new FactoryCommonServices(publicationResolver, _logger, _config, CreateCacheAgent());
                    componentPresentationFactory = new ComponentPresentationFactory(
                        new TridionComponentPresentationProvider(providersCommonServices), 
                        factoryCommonServices);
                    _componentPresentationFactories.Add(localization.Id, componentPresentationFactory);
                }

                return componentPresentationFactory;
            }
        }


        internal static IComponentFactory GetComponentFactory(Localization localization)
        {
            lock (_componentFactories)
            {
                IComponentFactory componentFactory;
                if (!_componentFactories.TryGetValue(localization.Id, out componentFactory))
                {
                    IPublicationResolver publicationResolver = new PublicationResolver(localization);
                    IFactoryCommonServices factoryCommonServices = new FactoryCommonServices(publicationResolver, _logger, _config, CreateCacheAgent());
                    componentFactory = new ComponentFactory(
                        GetComponentPresentationFactory(localization),
                        factoryCommonServices );
                    _componentFactories.Add(localization.Id, componentFactory);
                }

                return componentFactory;
            }
        }

        internal static IBinaryFactory GetBinaryFactory(Localization localization)
        {
            lock (_binaryFactories)
            {
                IBinaryFactory binaryFactory;
                if (!_binaryFactories.TryGetValue(localization.Id, out binaryFactory))
                {
                    IPublicationResolver publicationResolver = new PublicationResolver(localization);
                    IProvidersCommonServices providersCommonServices = new ProvidersCommonServices(publicationResolver, _logger, _config);
                    IFactoryCommonServices factoryCommonServices = new FactoryCommonServices(publicationResolver, _logger, _config, CreateCacheAgent());
                    binaryFactory = new BinaryFactory(
                        new TridionBinaryProvider(providersCommonServices),
                        factoryCommonServices);
                    _binaryFactories.Add(localization.Id, binaryFactory);
                }

                return binaryFactory;
            }
        }
    }
}
