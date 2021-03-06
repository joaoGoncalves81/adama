﻿namespace DamaWeb.ViewModels
{

    public class CatalogItemViewModel
    {
        public int CatalogItemId { get; set; }
        public string CatalogItemName { get; set; }
        public string PictureUri { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceBeforeDiscount { get; set; }
        //public string ProductSku { get; set; }
        public string ProductSlug { get; set; }
        public string PictureHighUri { get; set; }
        public bool IsUnavailable { get; set; }
    }
}
