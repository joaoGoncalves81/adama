﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backoffice.ViewModels
{
    public class OrderItemViewModel
    {
        [Display(Name = "Id")]
        public int ProductId { get; set; }
        [Display(Name = "SKU")]
        public string ProductSku { get; set; }
        [Display(Name = "Nome")]
        public string ProductName { get; set; }
        [Display(Name = "Imagem")]
        public string PictureUri { get; set; }
        [Display(Name = "Preço Unitário")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Quantidade")]
        public int Units { get; set; }
        public int? CatalogAttribute1 { get; set; }
        public int? CatalogAttribute2 { get; set; }
        public int? CatalogAttribute3 { get; set; }
        [Display(Name = "Atributos")]
        public List<OrderItemAttributeViewModel> Attributes { get; set; } = new List<OrderItemAttributeViewModel>();
    }

    public class OrderItemAttributeViewModel
    {
        [Display(Name = "Tipo de Atributo")]
        public AttributeType AttributeType { get; set; }
        [Display(Name = "Valor")]
        public string AttributeName { get; set; }
    }
}
