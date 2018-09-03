﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DamaNoJornal.Core.Models.Catalog;
using DamaNoJornal.Core.Services.RequestProvider;
using DamaNoJornal.Core.Extensions;
using System.Collections.Generic;
using DamaNoJornal.Core.Services.FixUri;
using DamaNoJornal.Core.Services.Settings;

namespace DamaNoJornal.Core.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestProvider _requestProvider;
        private readonly IFixUriService _fixUriService;
        private readonly string ApiUrlBaseFormat = "api/{0}/catalog";
        private readonly string ApiUrlBase;

        public CatalogService(IRequestProvider requestProvider, IFixUriService fixUriService, ISettingsService settingsService)
        {
            _requestProvider = requestProvider;
            _fixUriService = fixUriService;
            if (settingsService.PlaceId == GlobalSetting.GroceryPlace.Id.ToString())
                ApiUrlBase = string.Format(ApiUrlBaseFormat,"grocery");
            else
                ApiUrlBase = string.Format(ApiUrlBaseFormat, "v1");

        }

        public async Task<ObservableCollection<CatalogItem>> FilterAsync(int? catalogBrandId, int? catalogTypeId)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);   
            if(catalogTypeId.HasValue && catalogBrandId.HasValue)
                builder.Path = $"{ApiUrlBase}/items/type/{catalogTypeId}/category/{catalogBrandId}";
            else if (catalogBrandId.HasValue && !catalogTypeId.HasValue)
                builder.Path = $"{ApiUrlBase}/items/category/{catalogBrandId}";
            else
                builder.Path = $"{ApiUrlBase}/items/type/{catalogTypeId}";
            string uri = builder.ToString();

            CatalogRoot catalog = await _requestProvider.GetAsync<CatalogRoot>(uri);

            if (catalog?.Data != null)
                return catalog?.Data.ToObservableCollection();
            else
                return new ObservableCollection<CatalogItem>();
        }

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = $"{ApiUrlBase}/items";
            string uri = builder.ToString();

            CatalogRoot catalog = await _requestProvider.GetAsync<CatalogRoot>(uri);

            if (catalog?.Data != null)
            {
                _fixUriService.FixCatalogItemPictureUri(catalog?.Data);
                return catalog?.Data.ToObservableCollection();
            }
            else
                return new ObservableCollection<CatalogItem>();
        }

        public async Task<ObservableCollection<CatalogBrand>> GetCatalogCategoryAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = $"{ApiUrlBase}/catalogcategories";
            string uri = builder.ToString();

            IEnumerable<CatalogBrand> brands = await _requestProvider.GetAsync<IEnumerable<CatalogBrand>>(uri);

            if (brands != null)
                return brands?.ToObservableCollection();
            else
                return new ObservableCollection<CatalogBrand>();
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync(int? catalogCategoryId = null)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            if(!catalogCategoryId.HasValue)
                builder.Path = $"{ApiUrlBase}/catalogtypes";
            else
                builder.Path = $"{ApiUrlBase}/catalogtypes/{catalogCategoryId}";
            string uri = builder.ToString();

            IEnumerable<CatalogType> types = await _requestProvider.GetAsync<IEnumerable<CatalogType>>(uri);

            if (types != null)
                return types.ToObservableCollection();
            else
                return new ObservableCollection<CatalogType>();
        }
    }
}
