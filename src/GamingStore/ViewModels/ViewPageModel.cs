using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Data;
using GamingStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GamingStore.ViewModels
{
    public class ViewPageModel : PageModel
    {
        private readonly StoreContext _context;

        public ViewPageModel(StoreContext context)
        {
            _context = context;
        }

        public int? ItemsInCart { get; set; } = null;

        protected async Task<int> CountItemsInCart(Customer user)
        {
            if (user == null)
            {
                return 0;
            }

            var itemsInCart = 0;

            foreach (Models.Cart itemInCart in _context.Carts.Where(c => c.CustomerId == user.Id))
            {
                itemsInCart += itemInCart.Quantity;
            }

            return itemsInCart;
        }
    }
}
