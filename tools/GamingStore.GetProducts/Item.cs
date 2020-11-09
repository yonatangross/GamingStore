using System;
using System.Collections.Generic;
using System.Text;

namespace GamingStore.ProductParser
{
    class Item
    {
        public string Title { get; set; }

        public string Manufacturer { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; }
    }
}
