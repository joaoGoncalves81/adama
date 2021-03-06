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
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using ApplicationCore;

namespace Backoffice.Pages.ProductType
{
    public class EditModel : PageModel
    {
        private readonly Infrastructure.Data.DamaContext _context;
        protected readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public EditModel(Infrastructure.Data.DamaContext context, IMapper mapper, IBackofficeService service, IOptions<BackofficeSettings> backofficeSettings)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = backofficeSettings.Value;
        }

        [BindProperty]
        public ProductTypeViewModel ProductTypeModel { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _context.CatalogTypes
                .Include(p => p.Categories)
                 .ThenInclude(c => c.Category)
                 .Include(p => p.PictureTextHelpers)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

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

            if (ProductTypeModel.Picture?.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem? O máximo é 2MB, obrigado! Ass.: O seu amor!");
                return Page();
            }

            if (ProductTypeModel.FormFileTextHelpers?.Count > 0 && ProductTypeModel.FormFileTextHelpers.Any(x => x.Length > 2097152))
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho das imagens da localização do nome? O máximo é 2MB, obrigado! Ass.: O seu amor!");
                return Page();
            }

            ProductTypeModel.Slug = Utils.URLFriendly(ProductTypeModel.Slug);
            if ((await CheckIfSlugExistsAsync(ProductTypeModel.Id, ProductTypeModel.Slug)))
            {
                ModelState.AddModelError("ProductTypeModel.Slug", "Este slug já existe!");
                return Page();
            }

            //Get entity
            var productTypeEntity = await _context.CatalogTypes
                .Include(x => x.Categories)
                .Include(x => x.PictureTextHelpers)
                .SingleOrDefaultAsync(x => x.Id == ProductTypeModel.Id);

            //Save Image
            if (ProductTypeModel?.Picture?.Length > 0)
            {
                if(!string.IsNullOrEmpty(productTypeEntity.PictureUri))
                {
                    _service.DeleteFile(_backofficeSettings.WebProductTypesPictureV2FullPath, Utils.GetFileName(productTypeEntity.PictureUri));
                }
                ProductTypeModel.PictureUri = _service.SaveFile(ProductTypeModel.Picture, _backofficeSettings.WebProductTypesPictureV2FullPath, _backofficeSettings.WebProductTypesPictureV2Uri, ProductTypeModel.Id.ToString(), true, 300).PictureUri;
            }

            //Save Images Text Helpers
            if (ProductTypeModel?.FormFileTextHelpers?.Count > 0)
            {
                //Delete All
                foreach (var item in productTypeEntity.PictureTextHelpers)
                {
                    _service.DeleteFile(item.Location);
                    _context.Entry(item).State = EntityState.Deleted;
                }

                foreach (var item in ProductTypeModel.FormFileTextHelpers)
                {
                    var lastId = _context.FileDetails.Count() > 0 ? GetLastFileDetailsId() : 0;
                    var pictureInfo = _service.SaveFile(item, _backofficeSettings.WebProductTypesPictureV2FullPath, _backofficeSettings.WebProductTypesPictureV2Uri, (++lastId).ToString(), true, 150);
                    productTypeEntity.PictureTextHelpers.Add(new FileDetail
                    {
                        PictureUri = pictureInfo.PictureUri,
                        Extension = pictureInfo.Extension,
                        FileName = pictureInfo.Filename,
                        Location = pictureInfo.Location
                    });
                }
            }

            if (productTypeEntity != null)
            {
                productTypeEntity.Update(ProductTypeModel.Code,
                                         ProductTypeModel.Description,
                                         ProductTypeModel.DeliveryTimeMin,
                                         ProductTypeModel.DeliveryTimeMax,
                                         ProductTypeModel.DeliveryTimeUnit,
                                         ProductTypeModel.Price,
                                         ProductTypeModel.AdditionalTextPrice,
                                         ProductTypeModel.Weight,
                                         ProductTypeModel.MetaDescription,
                                         ProductTypeModel.Title,
                                         ProductTypeModel.Slug);

                if (!string.IsNullOrEmpty(ProductTypeModel.PictureUri))
                    productTypeEntity.UpdatePicture(ProductTypeModel.PictureUri);

                // //Remove
                // var to_remove = productTypeEntity.Categories.Where(c => !ProductTypeModel.CategoriesId.Any(c2 => c2 == c.CategoryId));
                // foreach (var item in to_remove)
                // {
                //     _context.Entry(item).State = EntityState.Deleted;
                // }
                // //Add
                // var to_add = ProductTypeModel.CategoriesId.Where(c => !productTypeEntity.Categories.Any(c2 => c2.CategoryId == c));
                // foreach (var item in to_add)
                // {
                //     productTypeEntity.AddCategory(new CatalogTypeCategory(item));
                // }
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

        public async Task<IActionResult> OnGetRemoveAdditionalImagesAsync(int id)
        {
            var productTypeEntity = await _context.CatalogTypes
               .Include(x => x.PictureTextHelpers)
               .SingleOrDefaultAsync(x => x.Id == id);

            if(productTypeEntity != null)
            {
                foreach (var item in productTypeEntity.PictureTextHelpers)
                {
                    _context.Entry(item).State = EntityState.Deleted;
                }
                await _context.SaveChangesAsync();
            }
            StatusMessage = "As imagens foram removidas";
            return RedirectToPage(new { id });
        }

        private int GetLastFileDetailsId()
        {
            return _context.FileDetails
                .AsEnumerable()
                .Last()
                .Id;
        }

        private async Task<bool> CheckIfSlugExistsAsync(int id, string slug)
        {
            return await _context.CatalogTypes.AnyAsync(x => x.Id != id && x.Slug == slug);
        }
    }
}
