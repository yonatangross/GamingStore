using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.Data
{
    public class DbInitializer
    {
        public static void Initialize(StoreContext context)
        {
            context.Database.EnsureCreated();

            // Look for any Customers.
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            var customers = new Customer[]
            {
            new Customer{FirstName="Carson",LastName="Alexander",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Meredith",LastName="Alonso",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Arturo",LastName="Anand",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Gytis",LastName="Barzdukas",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Yan",LastName="Li",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Peggy",LastName="Justice",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Peggy",LastName="Justice",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Laura",LastName="Norman",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Nino",LastName="Olivetto",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Customer{FirstName="Ohad",LastName="Bolilon",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"}
            };
            foreach (var s in customers)
            {
                context.Customers.Add(s);
            }
            context.SaveChanges();

            var items = new Item[]
            {
            new Item{Title="Keyboard",Manufacturer= "Microsoft",Price = 299},
            new Item{Title="Mouse",Manufacturer= "Microsoft",Price = 150},
            new Item{Title="Gaming Chair",Manufacturer= "Razor",Price=799},
            new Item{Title="Mouse Pad",Manufacturer= "Razor",Price=50},
            new Item{Title="Graphic Card",Manufacturer= "Nvidia",Price = 1500},
            new Item{Title="Processor",Manufacturer= "Intel",Price = 1249},
            new Item{Title="Headphones",Manufacturer= "Bose",Price = 499}
            };
            foreach (var c in items)
            {
                context.Items.Add(c);
            }
            context.SaveChanges();

            var stores = new Store[]
            {
            new Store{Id=1,Name="KSP",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Store{Id=2,Name="Zap",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Store{Id=3,Name="Bug",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Store{Id=4,Name="Shimi Gaming Chairs",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Store{Id=5,Name="Ohad GamingWorld",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Store{Id=6,Name="Miranda Mouses",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
            new Store{Id=7,Name="Yoni Keyboards" ,Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"}
            };
            foreach (var e in stores)
            {
                context.Stores.Add(e);
            }
            context.SaveChanges();
        }
    }
}
