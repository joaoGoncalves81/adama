﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class CustomizeViewModel
    {
        //public int CategorySelected { get; set; }
        public int? ProductSelected { get; set; }
        [Required(ErrorMessage = "O campo Descrição é obrigatório")]
        public string Description { get; set; }
        public IFormFile UploadFile { get; set; }
        public string Text { get; set; }
        public string Colors { get; set; }
        [Display(Name ="Email")]
        [Required(ErrorMessage = "O campo Email é obrigatório")]
        public string BuyerEmail { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        public string BuyerName { get; set; }
        [Display(Name = "Contato")]
        [Required(ErrorMessage = "O campo Contatos é obrigatório")]
        public string BuyerPhone { get; set; }

        public List<(int,string)> Categories { get; set; }
        public List<CatalogItemViewModel> CatalogItems { get; set; }
        
    }
}
