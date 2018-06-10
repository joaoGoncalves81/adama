﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using Backoffice.ViewModels;
using AutoMapper;

namespace Backoffice.Pages.Category
{
    public class DetailsModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;

        public DetailsModel(Infrastructure.Data.DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public CategoryViewModel Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(x => x.Parent)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            //Prodcut Types
            var types = await _context.CatalogTypeCategories
                .Include(x => x.CatalogType)
                .Where(x => x.CategoryId == id)
                .Select(ct => ct.CatalogType)
                .ToListAsync();

            Category = _mapper.Map<CategoryViewModel>(category);
            Category.ProductTypes = _mapper.Map<List<ProductTypeViewModel>>(types);
            
            return Page();
        }
    }
}