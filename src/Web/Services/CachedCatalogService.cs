﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Interfaces;
using Web.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Specifications;

namespace Web.Services
{
    public class CachedCatalogService : ICatalogService
    {
        private readonly IMemoryCache _cache;
        private readonly CatalogService _catalogService;
        private static readonly string _brandsKey = "brands";
        private static readonly string _typesKey = "types";
        private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}-{3}";
        private static readonly string _categoryItemsKeyTemplate = "items-{0}";
        private static readonly string _categoryItemKeyTemplate = "item-{0}";
        private static readonly string _categoryAttrKeyTemplate = "attr-{0}";
        private static readonly string _itemsByTagKeyTemplate = "tag-{0}-{1}";
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromSeconds(30);

        public CachedCatalogService(IMemoryCache cache,
            CatalogService catalogService)
        {
            _cache = cache;
            _catalogService = catalogService;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            return await _cache.GetOrCreateAsync(_brandsKey, async entry =>
                    {
                        entry.SlidingExpiration = _defaultCacheDuration;
                        return await _catalogService.GetBrands();
                    });
        }

        public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int? itemsPage, int? brandID, int? typeId)
        {
            string cacheKey = String.Format(_itemsKeyTemplate, pageIndex, itemsPage, brandID, typeId);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItems(pageIndex, itemsPage, brandID, typeId);
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            return await _cache.GetOrCreateAsync(_typesKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetTypes();
            });
        }

        public async Task<CatalogIndexViewModel> GetCategoryCatalogItems(int categoryId)
        {
            string cacheKey = String.Format(_categoryItemsKeyTemplate, categoryId);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCategoryCatalogItems(categoryId);
            });
        }

        public async Task<ProductViewModel> GetCatalogItem(string id)
        {
            string cacheKey = String.Format(_categoryItemKeyTemplate, id);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItem(id);
            });
        }

        public async Task<AttributeViewModel> GetAttributeDetails(int attributeId)
        {
            string cacheKey = String.Format(_categoryAttrKeyTemplate, attributeId);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetAttributeDetails(attributeId);
            });
        }

        public async Task<CatalogIndexViewModel> GetCatalogItemsByTag(string tagName, TagType? tagType)
        {
            string cacheKey = String.Format(_itemsByTagKeyTemplate, tagName, tagType);
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _catalogService.GetCatalogItemsByTag(tagName,tagType);
            });
        }
    }
}
