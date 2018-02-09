﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.ProductType
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public EditModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public ProductTypeViewModel ProductTypeModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.CatalogTypes
                .Include(p => p.Categories)
                 .ThenInclude(c => c.Category)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (productType == null)
            {
                return NotFound();
            }
            ProductTypeModel = _mapper.Map<ProductTypeViewModel>(productType);            
            if (productType.Categories?.Count > 0)
            {
                ProductTypeModel.CategoriesId.AddRange(productType.Categories.Select(x => x.CategoryId));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //check if code exists
            if (_context.CatalogTypes.Any(x => x.Code.ToUpper() == ProductTypeModel.Code.ToUpper() && x.Id != ProductTypeModel.Id))
            {
                ModelState.AddModelError("", $"O nome do Tipo do Produto '{ProductTypeModel.Code}' já existe!");
                return Page();
            }

            //Get entity
            var productTypeEntity = await _context.CatalogTypes
                .Include(x => x.Categories)
                .SingleOrDefaultAsync(x => x.Id == ProductTypeModel.Id);

            if(productTypeEntity != null)
            {
                productTypeEntity.Code = ProductTypeModel.Code;
                productTypeEntity.Description = ProductTypeModel.Description;

                //Remove
                var to_remove = productTypeEntity.Categories.Where(c => !ProductTypeModel.CategoriesId.Any(c2 => c2 == c.CategoryId));
                foreach (var item in to_remove)
                {
                    _context.Entry(item).State = EntityState.Deleted;
                }
                //Add
                var to_add = ProductTypeModel.CategoriesId.Where(c => !productTypeEntity.Categories.Any(c2 => c2.CategoryId == c));
                foreach (var item in to_add)
                {
                    productTypeEntity.Categories.Add(new CatalogTypeCategory { CategoryId = item });
                }                
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return RedirectToPage("./Index");
        }
    }
}
