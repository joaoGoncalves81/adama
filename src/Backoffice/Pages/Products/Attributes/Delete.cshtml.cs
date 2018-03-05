﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using Infrastructure.Data;
using AutoMapper;
using Backoffice.ViewModels;

namespace Backoffice.Pages.Products.Attributes
{
    public class DeleteModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;

        public DeleteModel(DamaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public ProductAttributeViewModel CatalogAttributeModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ca = await _context.CatalogAttributes
                .Include(c => c.CatalogItem)
                .Include(c => c.ReferenceCatalogItem)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (ca == null)
            {
                return NotFound();
            }

            CatalogAttributeModel = _mapper.Map<ProductAttributeViewModel>(ca);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ca = await _context.CatalogAttributes.FindAsync(id);

            if (ca != null)
            {
                _context.CatalogAttributes.Remove(ca);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Products/Edit", new { id = ca.CatalogItemId });
        }
    }
}