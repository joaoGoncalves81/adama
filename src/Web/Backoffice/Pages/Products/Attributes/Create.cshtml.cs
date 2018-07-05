﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;
using Backoffice.Interfaces;

namespace Backoffice.Pages.Products.Attributes
{
    public class CreateModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;

        public CreateModel(Infrastructure.Data.DamaContext context, IMapper mapper, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            this._service = service;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var prod = await _context.CatalogItems.FindAsync(id);
            if (prod == null)
                return NotFound();

            CatalogAttributeModel.CatalogItemId = id;
            CatalogAttributeModel.CatalogItem = _mapper.Map<ProductViewModel>(prod);
            
            ViewData["ReferenceCatalogItemId"] = new SelectList(_context.CatalogItems, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public ProductAttributeViewModel CatalogAttributeModel { get; set; } = new ProductAttributeViewModel();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            CatalogAttributeModel.CatalogItem = null;
            var attribute = _mapper.Map<CatalogAttribute>(CatalogAttributeModel);
            _context.CatalogAttributes.Add(attribute);
            await _context.SaveChangesAsync();

            //var price = attribute.CatalogItem.Price;
            //if (attribute.Price.HasValue)
            //    price += attribute.Price.Value;
            //await _service.AddOrUpdateCatalogPrice(attribute.CatalogItemId, attribute.Id, price);

            return RedirectToPage("/Products/Edit", new { id = CatalogAttributeModel.CatalogItemId });
        }
    }
}