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
using AutoMapper;
using Backoffice.ViewModels;
using Backoffice.Interfaces;
using Microsoft.Extensions.Options;
using Backoffice.Extensions;
using ApplicationCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backoffice.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly DamaContext _context;
        private readonly IMapper _mapper;
        private readonly IBackofficeService _service;
        private readonly BackofficeSettings _backofficeSettings;

        public EditModel(DamaContext context, IMapper mapper, IOptions<BackofficeSettings> settings, IBackofficeService service)
        {
            _context = context;
            _mapper = mapper;
            _service = service;
            _backofficeSettings = settings.Value;
        }

        [BindProperty]
        public ProductEditViewModel ProductModel { get; set; }

        [BindProperty]
        public List<CatalogCategoryViewModel> CatalogCategoryModel { get; set; } = new List<CatalogCategoryViewModel>();

        public class ProductEditViewModel
        {
            public int Id { get; set; }
            [Required]
            [StringLength(100)]
            [Display(Name = "Nome")]
            public string Name { get; set; }
            [Required]
            [StringLength(100)]
            public string Slug { get; set; }
            [Display(Name = "Descrição")]
            public string Description { get; set; }
            [Display(Name = "Preço")]
            public decimal? Price { get; set; }
            [Display(Name = "Ilustração")]
            [Required]
            public int CatalogIllustrationId { get; set; }
            [Required]
            [Display(Name = "Tipo de Produto")]
            public int CatalogTypeId { get; set; }
            [Display(Name = "SKU")]
            public string Sku { get; set; }
            [Display(Name = "Loja")]
            public bool ShowOnShop { get; set; }
            [Display(Name = "Novidade")]
            public bool IsNew { get; set; }
            [Display(Name = "Destaque")]
            public bool IsFeatured { get; set; }
            [Display(Name = "Personalizar")]
            public bool CanCustomize { get; set; }
            [Display(Name = "Indisponível")]
            public bool IsUnavailable { get; set; }
            [Display(Name = "Imagem Principal")]            
            public IFormFile Picture { get; set; }
            [Display(Name = "URL da Imagem Principal")]
            public string PictureUri { get; set; }
            [Display(Name = "Imagens do Produto")]
            public List<IFormFile> OtherPictures { get; set; }
            public int Stock { get; set; }
            [Display(Name = "Meta Description")]
            [StringLength(160)]
            public string MetaDescription { get; set; }
            [Display(Name = "Title")]
            [StringLength(43)]
            public string Title { get; set; }
            public string CatalogTypeMeta { get; set; }
            [Display(Name = "Desconto")]
            public decimal? Discount { get; set; }

            public IList<ProductAttributeViewModel> Attributes { get; set; } = new List<ProductAttributeViewModel>();
            [Display(Name = "Imagens do Produto")]
            public List<ProductPictureViewModel> Pictures { get; set; } = new List<ProductPictureViewModel>();
            [Display(Name = "Categorias")]
            public IList<CatalogCategoryViewModel> Categories { get; set; } = new List<CatalogCategoryViewModel>();
            public IList<CatalogReference> References { get; set; } = new List<CatalogReference>();
            public string PictureHighUri { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductModel = _mapper.Map<ProductEditViewModel>(
                await _context.CatalogItems
                .Include(p => p.Categories)
                .Include(p => p.CatalogIllustration)
                .Include(p => p.CatalogType)
                .Include(p => p.Attributes)
                .Include(p => p.Pictures)
                .Include(p => p.References)
                .ThenInclude(cr => cr.ReferenceCatalogItem)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id));

            ProductModel.Pictures.RemoveAll(x => x.IsMain);

            if (ProductModel == null)
            {
                return NotFound();
            }
            await PopulateLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateLists();
                return Page();
            }
           
            //Validade Pictures
            if (!ValidatePictures())
            {
                await PopulateLists();
                return Page();
            }

            //Fix Slug
            ProductModel.Slug = Utils.URLFriendly(ProductModel.Slug);

            if ((await SlugExistsAsync(ProductModel.Id, ProductModel.Slug)))
            {
                ModelState.AddModelError("ProductModel.Slug", "Já existe um slug com o mesmo nome!");
                await PopulateLists();
                return Page();
            }

            //Validate SKU
            ProductModel.Sku = await _service.GetSku(ProductModel.CatalogTypeId, ProductModel.CatalogIllustrationId) + "_" + ProductModel.Id; 
            
            //Main Picture
            if (ProductModel.Picture != null && ProductModel.Picture.Length > 0)
            {
                if (!string.IsNullOrEmpty(ProductModel.PictureUri))
                {
                    _service.DeleteFile(_backofficeSettings.WebProductsPictureV2FullPath, Utils.GetFileName(ProductModel.PictureUri));
                }
                var pictureInfo = _service.SaveFile(ProductModel.Picture, _backofficeSettings.WebProductsPictureV2FullPath, _backofficeSettings.WebProductsPictureV2Uri, ProductModel.Id.ToString(), true, 700, 700);
                ProductModel.PictureUri = pictureInfo.PictureUri;
                ProductModel.PictureHighUri = pictureInfo.PictureHighUri;

            }

            ////Update images            
            //foreach (var item in ProductModel.CatalogPictures.Where(x => x.Picture != null && !x.ToRemove).ToList())
            //{
            //    _service.DeleteFile(_backofficeSettings.WebProductsPictureFullPath, Utils.GetFileName(item.PictureUri));
            //    item.PictureUri = (await _service.SaveFileAsync(item.Picture, _backofficeSettings.WebProductsPictureFullPath, _backofficeSettings.WebProductsPictureUri, item.Id.ToString())).PictureUri;
            //}
            //Save other images
            if (ProductModel.OtherPictures != null)
            {
                var order = ProductModel.Pictures.Count == 0 ? 0 : ProductModel.Pictures.Max(x => x.Order);
                var lastCatalogPictureId = _context.CatalogPictures.Count() > 0 ? GetLastCatalogPictureId() : 0;
                foreach (var item in ProductModel.OtherPictures)
                {
                    var info = _service.SaveFile(item, _backofficeSettings.WebProductsPictureV2FullPath, _backofficeSettings.WebProductsPictureV2Uri, (++lastCatalogPictureId).ToString(), true, 700, 700);
                    ProductModel.Pictures.Add(new ProductPictureViewModel
                    {
                        IsActive = true,
                        Order = ++order,
                        IsMain = false,
                        PictureUri = info.PictureUri,
                        PictureHighUri = info.PictureHighUri
                    });
                }
            }

            //Save Changes    
            ProductModel.Price = ProductModel.Price == 0 ? default(decimal?) : ProductModel.Price;
            //var prod = _mapper.Map<CatalogItem>(ProductModel);
            var prod = await _context.CatalogItems
                .Include(p => p.Categories)                
                .Include(p => p.Pictures)
                .SingleOrDefaultAsync(m => m.Id == ProductModel.Id);
            
            prod.UpdateCatalogItem(ProductModel.Name,ProductModel.Slug,ProductModel.Description,ProductModel.CatalogIllustrationId,ProductModel.CatalogTypeId,ProductModel.PictureUri,ProductModel.Price,ProductModel.Discount,ProductModel.IsFeatured,ProductModel.IsNew,ProductModel.ShowOnShop,ProductModel.CanCustomize,ProductModel.IsUnavailable,ProductModel.Sku,ProductModel.Stock,ProductModel.MetaDescription,ProductModel.Title);
            
            //Main Picture
            if(!string.IsNullOrEmpty(ProductModel.PictureUri))
            {
                var mainPic = prod.Pictures.SingleOrDefault(x => x.IsMain);
                if (mainPic == null)
                    prod.AddPicture(new CatalogPicture(true,true,ProductModel.PictureUri,0,ProductModel.PictureHighUri));
                else
                    prod.Pictures.SingleOrDefault(x => x.IsMain).UpdatePictureUri(ProductModel.PictureUri);
            }

            //Other pictutes
            foreach (var item in ProductModel.Pictures.Where(x => !x.IsMain).ToList())
            {
                var otherPicture = item.Id != 0 ? prod.Pictures.SingleOrDefault(x => x.Id == item.Id) : null;

                if (item.ToRemove && otherPicture != null)
                {
                    _context.Entry(otherPicture).State = EntityState.Deleted;
                    _service.DeleteFile(_backofficeSettings.WebProductsPictureV2FullPath, Utils.GetFileName(item.PictureUri));
                }
                else if(otherPicture != null)
                {                    
                    otherPicture.UpdatePictureInfo(item.IsActive,item.Order,item.PictureUri,item.PictureHighUri);
                }
                else
                {
                    prod.AddPicture(new CatalogPicture(item.IsActive,false,item.PictureUri,item.Order,item.PictureHighUri));
                }
            }

            //Categorias dos Produtos
            var catalogCategoriesDb = await _context.CatalogCategories.Where(x => x.CatalogItemId == prod.Id).ToListAsync();
            
            //Novos
            foreach (var item in CatalogCategoryModel.Where(x => x.Selected).ToList())
            {
                if(catalogCategoriesDb == null || !catalogCategoriesDb.Any(x => x.CategoryId == item.CategoryId))
                {
                    prod.AddCategory(item.CategoryId);
                }
                foreach (var child in item.Childs.Where(x => x.Selected).ToList())
                {
                    if (catalogCategoriesDb == null || !catalogCategoriesDb.Any(x => x.CategoryId == child.CategoryId))
                    {
                        prod.AddCategory(child.CategoryId);
                    }
                }
            }
            //Remover
            foreach (var item in CatalogCategoryModel.Where(x => x.Id != 0).ToList())
            {
                if (!item.Selected)
                    _context.CatalogCategories.Remove(_context.CatalogCategories.Find(item.Id));

                foreach (var child in item.Childs.Where(x => x.Id != 0 && !x.Selected).ToList())
                {
                    _context.CatalogCategories.Remove(_context.CatalogCategories.Find(child.Id));
                }

            }

            //_context.Attach(prod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAddAttributeAsync()
        {
            await PopulateLists();
            ProductModel.Attributes.Add(new ProductAttributeViewModel
            {
                CatalogItemId = ProductModel.Id
            });
            return Page();
        }
        private int GetLastCatalogPictureId()
        {
            return _context.CatalogPictures
                .AsNoTracking()
                .AsEnumerable()
                .Last()
                .Id;
        }

        private async Task<bool> SlugExistsAsync(int catalogItemId, string slug)
        {
            return await _context.CatalogItems.AnyAsync(x => x.Id != catalogItemId && x.Slug == slug);
        }
        private bool ValidateAttributesModel()
        {
            foreach (var item in ProductModel.Attributes)
            {
                if (!item.ToRemove)
                {
                    //Validate
                    //if (string.IsNullOrEmpty(item.Code))
                    //    ModelState.AddModelError("", "O código do atributo é obrigatório");
                    if (string.IsNullOrEmpty(item.Name))
                        ModelState.AddModelError("", "O nome do atributo é obrigatório");
                    if (!ModelState.IsValid)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private async Task PopulateLists()
        {
            var illustrationsDb = await _context.CatalogIllustrations
                .Include(x => x.IllustrationType)
                .AsNoTracking()
                .ToListAsync();
            var illustrations = illustrationsDb
                .Select(s => new { s.Id, Name = $"{s.Code} - {s.IllustrationType.Code} - {s.Name}" })
                .OrderBy(x => x.Name)
                .ToList();
            var types = await _context.CatalogTypes
                .AsNoTracking()
                .ToListAsync();
            ViewData["IllustrationId"] = new SelectList(illustrations, "Id", "Name");

            ViewData["ProductTypeId"] = new SelectList(types.Select(x => new { x.Id, Name = $"{x.Code} - {x.Name}" }), "Id", "Name");
            await SetCatalogCategoryModel();
        }

        private bool ValidatePictures()
        {
            // if (string.IsNullOrEmpty(ProductModel.PictureUri) && ProductModel.Picture == null)
            // {
            //     ModelState.AddModelError("", "A imagem principal é obrigatória!");
            // }

            if (ProductModel.Picture != null && ProductModel.Picture.Length > 2097152)
            {
                ModelState.AddModelError("", "A menina quer por favor diminuir o tamanho da imagem principal? O máximo é 2MB, obrigado! Ass.: O seu amor!");
            }

            //check if file exits but not the same
            //if (ProductModel.Picture != null && 
            //    ProductModel.Picture.GetFileName() != Utils.GetFileName(ProductModel.PictureUri) && 
            //    _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, ProductModel.Picture.GetFileName()))
            //{
            //    ModelState.AddModelError("", $"O nome da imagem {ProductModel.Picture.GetFileName()} já existe, por favor escolha outro nome!");
            //}
            
            //foreach (var item in ProductModel.CatalogPictures)
            //{
            //    if (item.Picture != null && item.Picture.Length > 2097152)
            //        ModelState.AddModelError("", $"A imagem {item.Picture.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");               
            //}

            if (ProductModel.OtherPictures != null)
            {
                foreach (var item in ProductModel.OtherPictures)
                {
                    if (item.Length > 2097152)
                        ModelState.AddModelError("", $"A imagem {item.GetFileName()} está muito grande amor, O máximo é 2MB, obrigado!");

                    //if (item != null &&
                    //    _service.CheckIfFileExists(_backofficeSettings.WebProductsPictureFullPath, item.GetFileName()))
                    //    ModelState.AddModelError("", $"O nome da imagem {item.GetFileName()} já existe, por favor escolha outro nome!");
                }
            }

            return ModelState.IsValid;
        }

        private async Task SetCatalogCategoryModel()
        {
            //Catalog Categories            
            var allCats = await _context.Categories
                .Include(x => x.Parent)
                .AsNoTracking()
                .ToListAsync();
            foreach (var item in allCats.Where(x => x.Parent == null).ToList())
            {
                var catalogCategory = ProductModel.Categories.SingleOrDefault(x => x.CategoryId == item.Id);
                CatalogCategoryViewModel parent = new CatalogCategoryViewModel
                {
                    Id = catalogCategory?.Id ?? 0,
                    CategoryId = item.Id,
                    Label = item.Name,
                    Selected = catalogCategory != null ? true : false,
                    Childs = new List<CatalogCategoryViewModel>()
                };
                parent.Childs.AddRange(allCats.Where(x => x.ParentId == item.Id).Select(s => new CatalogCategoryViewModel
                {
                    Id = ProductModel.Categories.SingleOrDefault(x => x.CategoryId == s.Id)?.Id ?? 0,
                    CategoryId = s.Id,
                    Label = s.Name,
                    Selected = ProductModel.Categories.Any(x => x.CategoryId == s.Id),
                }));
                CatalogCategoryModel.Add(parent);
            }
        }
    }
}
