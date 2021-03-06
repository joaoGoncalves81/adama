﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using ApplicationCore;

namespace DamaWeb.Pages
{
    public class ProductModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _settings;

        public ProductModel(ICatalogService catalogService, 
            IEmailSender emailSender,
            IOptions<EmailSettings> options)
        {
            _catalogService = catalogService;
            _emailSender = emailSender;
            _settings = options.Value;
        }
        public ProductViewModel ProductView { get; set; } = new ProductViewModel();

        [TempData]
        public string StatusMessage { get; set; }

        [ViewData]
        public string MetaDescription { get; set; }
        [ViewData]
        public string Title { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            
            ProductView = await _catalogService.GetCatalogItem(id);

            //Old URL logic
            if (ProductView == null)
            {
                //if category
                var category = await _catalogService.GetCategoryCatalogItems(id, 0, Constants.ITEMS_PER_PAGE);
                if (category != null && !string.IsNullOrEmpty(category.CategoryUrlName))
                {
                    return RedirectToPagePermanent("/Category/Index", new { id = category.CategoryUrlName });
                }
                //if product by sku
                var catalogSlug = await _catalogService.GetSlugFromSkuAsync(id);
                if(!string.IsNullOrEmpty(catalogSlug))
                {
                    return RedirectToPagePermanent("/Product", new { id = catalogSlug });
                }
                return NotFound();
            }

            MetaDescription = ProductView.MetaDescription;
            Title = ProductView.Title;

            ViewData["ProductReferences"] = new SelectList(ProductView.ProductReferences, "Slug", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostSendMessageAsync(string ContactEmailAddress, string Message, string ProductSKU)
        {
            await _emailSender.SendGenericEmailAsync(
                _settings.FromInfoEmail,
                _settings.CCEmails,
                $"Pedido de dúvida do Produto {ProductSKU}",
                $"De: {ContactEmailAddress}<br>" +
                $"Mensagem: {Message}");

            StatusMessage = "A sua mensagem foi enviada com sucesso";
            return RedirectToPage(new { id = ProductSKU });
        }
    }
}
