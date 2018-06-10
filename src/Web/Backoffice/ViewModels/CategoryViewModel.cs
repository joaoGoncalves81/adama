﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backoffice.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required]        
        [Display(Name = "Ordem")]
        public int Order { get; set; }
       
        [Display(Name = "Posição Web")]
        public string Position { get; set; }

        [Display(Name="Categoria Pai")]
        public int? ParentId { get; set; }
        public CategoryViewModel Parent { get; set; }

        [Display(Name = "Nº Tipos de Produtos")]
        public int NrTypeProducts { get; set; }
        public List<ProductTypeViewModel> ProductTypes { get; set; }
    }
}