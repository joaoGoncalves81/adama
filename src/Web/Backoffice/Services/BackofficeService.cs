﻿using ApplicationCore.DTOs;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Interfaces;
using AutoMapper;
using Backoffice.Extensions;
using Backoffice.Interfaces;
using Backoffice.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.Services
{
    public class BackofficeService : IBackofficeService
    {
        private readonly DamaContext _db;
        private readonly IMapper _mapper;
        private readonly ISageService _sageService;

        public BackofficeService(DamaContext context, IMapper mapper, ISageService sageService)
        {
            _db = context;
            _mapper = mapper;
            this._sageService = sageService;
        }
        public bool CheckIfFileExists(string fullpath, string fileName)
        {
            return System.IO.File.Exists(Path.Combine(fullpath,fileName));
        }

        public void DeleteFile(string fullpath, string fileName)
        {
            if (System.IO.File.Exists(Path.Combine(fullpath, fileName)))
                System.IO.File.Delete(Path.Combine(fullpath, fileName));
        }

        public async Task<string> SaveFileAsync(IFormFile formFile, string fullPath, string uriPath, string addToFilename)
        {
            var filename = formFile.GetFileName();

            if (!string.IsNullOrEmpty(addToFilename))
            {                
                var name = filename.Substring(0, filename.LastIndexOf('.')) + $"-{addToFilename}";
                filename = name + filename.Substring(filename.LastIndexOf('.'));
            }

            var filePath = Path.Combine(
                fullPath,
                filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return uriPath + filename; 
        }

        public async Task SaveFileAsync(byte[] bytes, string fullPath, string filename)
        {
            var filePath = Path.Combine(
                fullPath,
                filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

        }

        public async Task<string> GetSku(int typeId, int illustationId, int? attributeId = null)
        {
            var type = await _db.CatalogTypes                
                .Where(x => x.Id == typeId)
                .Select(x => x.Code)
                .SingleAsync();
            var illustration = await _db.CatalogIllustrations
                .Include(x => x.IllustrationType)
                .SingleOrDefaultAsync(x => x.Id == illustationId);
                        
            string sku = $"{type}_{illustration.Code}_{illustration.IllustrationType.Code}";
            if (attributeId.HasValue)
                sku += $"_{attributeId}";
            return sku;
        }

        public async Task<bool> CheckIfSkuExists(string sku)
        {
            return (await _db.CatalogItems.SingleOrDefaultAsync(x => x.Sku == sku)) != null;
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            var orders = await _db.Orders
                .Include(x => x.OrderItems)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
                
            return _mapper.Map<List<OrderViewModel>>(orders);
        }

        public async Task<OrderViewModel> GetOrder(int id)
        {
            var order = await _db.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.Details)
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.ItemOrdered)
                .SingleOrDefaultAsync(x => x.Id == id);

            var orderViewModel = _mapper.Map<OrderViewModel>(order);
            orderViewModel.Items = _mapper.Map<List<OrderItemViewModel>>(order.OrderItems);
            orderViewModel.Items.ForEach(async x => x.ProductSku = await GetSkuAsync(x.ProductId));
            return orderViewModel;
        }

        private async Task<string> GetSkuAsync(int catalogItemId)
        {
            var item = await _db.CatalogItems.FindAsync(catalogItemId);
            if (item != null)
                return item.Sku;
            return string.Empty;
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync(int productTypeId)
        {
            var type = await _db.CatalogTypes
                .Include(x => x.Categories)
                    .ThenInclude(c => c.Category)
                .SingleOrDefaultAsync(x => x.Id == productTypeId);
            if(type != null)
                return _mapper.Map<List<CategoryViewModel>>(type.Categories.Select(x => x.Category).ToList());
            return null;
        }

        public async Task<IList<CustomizeOrderViewModel>> GetCustomizeOrdersAsync()
        {
            var orders = await _db.CustomizeOrders
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return _mapper.Map<List<CustomizeOrderViewModel>>(orders);
        }

        public async Task<CustomizeOrderViewModel> GetCustomizeOrderAsync(int id)
        {
            var order = await _db.CustomizeOrders.FindAsync(id);
            return _mapper.Map<CustomizeOrderViewModel>(order);
        }

        public async Task<SageResponseDTO> RegisterInvoiceAsync(int id)
        {
            var order = await _db.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(i => i.ItemOrdered)
                .SingleOrDefaultAsync(x => x.Id == id);

            List<OrderItem> items = new List<OrderItem>();
            foreach (var item in order.OrderItems)
            {
                items.Add(new OrderItem(item.ItemOrdered, item.UnitPrice, item.Units));
            }

            SageResponseDTO response;
           
            if (order.TaxNumber.HasValue)
            {
                response = await _sageService.CreateInvoiceWithTaxNumber(
                    items,
                    order.BillingToAddress.Name,
                    order.TaxNumber.Value.ToString(),
                    order.BillingToAddress.Street,
                    order.BillingToAddress.PostalCode,
                    order.BillingToAddress.City,
                    order.Id);
            }
            else
                response = await _sageService.CreateAnonymousInvoice(items, order.Id);

            if(response.InvoiceId.HasValue)
            {
                order.SalesInvoiceId = response.InvoiceId.Value;
                order.SalesInvoiceNumber = response.InvoiceNumber;                
                await _db.SaveChangesAsync();

                //Payment
                await _sageService.InvoicePayment(order.SalesInvoiceId.Value, order.Total());
                
            }
            
            return response;
        }

        public Task<byte[]> GetInvoicePDF(int invoiceId)
        {
            return _sageService.GetPDFInvoice(invoiceId);
        }
    }
}
