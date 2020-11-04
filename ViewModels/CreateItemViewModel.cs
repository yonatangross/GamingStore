using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using Microsoft.AspNetCore.Http;

namespace GamingStore.ViewModels
{
    public class CreateItemViewModel
    {
        public Item Item { get; set; }
        
        public IFormFile File1 { set; get; }
        
        public IFormFile File2 { set; get; }
        
        public IFormFile File3 { set; get; }

        public Category[] Categories { get; set; }
    }
}
