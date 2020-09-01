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

            var Customers = new Customer[]
            {
            new Customer{FirstName="Carson",LastName="Alexander"},
            new Customer{FirstName="Meredith",LastName="Alonso"},
            new Customer{FirstName="Arturo",LastName="Anand"},
            new Customer{FirstName="Gytis",LastName="Barzdukas"},
            new Customer{FirstName="Yan",LastName="Li"},
            new Customer{FirstName="Peggy",LastName="Justice"},
            new Customer{FirstName="Peggy",LastName="Justice"},
            new Customer{FirstName="Laura",LastName="Norman"},
            new Customer{FirstName="Nino",LastName="Olivetto"}
            };
            foreach (var s in Customers)
            {
                context.Customers.Add(s);
            }
            context.SaveChanges();

            var Items = new Item[]
            {
            new Item{Id= 1050,Title="Chemistry"},
            new Item{Id=4022,Title="Microeconomics3"},
            new Item{Id=4041,Title="Macroeconomics"},
            new Item{Id=1045,Title="Calculus"},
            new Item{Id=3141,Title="Trigonometry"},
            new Item{Id=2021,Title="Composition"},
            new Item{Id=2042,Title="Literature"}
            };
            foreach (var c in Items)
            {
                context.Items.Add(c);
            }
            context.SaveChanges();

            var Stores = new Store[]
            {
            new Store{Id=1,Name="a"},
            new Store{Id=2,Name="b"},
            new Store{Id=3,Name="c"},
            new Store{Id=4,Name="d"},
            new Store{Id=5,Name="e"},
            new Store{Id=6,Name="f"},
            new Store{Id=7,Name="g" }
            };
            foreach (var e in Stores)
            {
                context.Stores.Add(e);
            }
            context.SaveChanges();
        }
    }
}
