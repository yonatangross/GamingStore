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
            if (context.Items.Any())
            {
                return;   // DB has been seeded
            }


            //TODO: add different items with properties
            var items = new Item[]
            {
                new Item{Title="Keyboard",Manufacturer= "Microsoft",Price = 299,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }},
                new Item{Title="Mouse",Manufacturer= "Microsoft",Price = 150,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }},
                new Item{Title="Gaming Chair",Manufacturer= "Razor",Price=799,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }},
                new Item{Title="Mouse Pad",Manufacturer= "Razor",Price=50,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }},
                new Item{Title="Graphic Card",Manufacturer= "Nvidia",Price = 1500,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }},
                new Item{Title="Processor",Manufacturer= "Intel",Price = 1249,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }},
                new Item{Title="Headphones",Manufacturer= "Bose",Price = 499,PropertiesList =new Dictionary<string, string>()  {
                    {"diameter", "3"},
                    { "width", "15"},
                    { "height", "4"} }}
            };
            foreach (var i in items)
            {
                context.Items.Add(i);
            }
            context.SaveChanges();

            //TODO: add different orders with items list,store,state and payments at least.
            var orders = new Order[]
            {
                new Order{Customer =new Customer{FirstName="samir",LastName="Alonso",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
                    OrderDate = DateTime.Now.AddDays(-7),Items = new Dictionary<Item, uint>(){{  new Item{
                            Title="Processor",
                            Manufacturer= "Intel",
                            Price = 1249,
                            PropertiesList =new Dictionary<string, string>()  {
                            {"diameter", "3"},
                            { "width", "15"},
                            { "height", "4"} }},1},{
                        new Item{Title="Headphones",Manufacturer= "Bose",Price = 499,PropertiesList =new Dictionary<string, string>()  {
                            {"diameter", "3"},
                            { "width", "15"},
                            { "height", "4"} }} ,2}}},
                new Order{Customer =new Customer{FirstName="shimon",LastName="Alonso",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474"},
                    OrderDate = DateTime.Now.AddDays(-7),Items = new Dictionary<Item, uint>(){{  new Item{
                        Title="Processor",
                        Manufacturer= "Intel",
                        Price = 1249,
                        PropertiesList =new Dictionary<string, string>()  {
                            {"diameter", "3"},
                            { "width", "15"},
                            { "height", "4"} }},1},{
                        new Item{Title="Headphones",Manufacturer= "Bose",Price = 499,PropertiesList =new Dictionary<string, string>()  {
                            {"diameter", "3"},
                            { "width", "15"},
                            { "height", "4"} }} ,2}}}
            };
            foreach (var o in orders)
            {
                context.Orders.Add(o);
            }
            context.SaveChanges();

            //todo: add payments with all parameters.
            var payments = new Payment[]
            {

            };
            foreach (var p in payments)
            {
                context.Payments.Add(p);
            }
            context.SaveChanges();

            //todo: add customers with all parameters including complex objects.
            var customers = new Customer[]
            {
            new Customer{FirstName="Carson",LastName="Alexander",Email = "yonatan2gross@gmail.com",PhoneNumber = "0506656474",OrderHistory = new List<Order>(){}},
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
            foreach (var c in customers)
            {
                context.Customers.Add(c);
            }
            context.SaveChanges();


            //todo: add stores with all parameters including complex objects.
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
            foreach (var s in stores)
            {
                context.Stores.Add(s);
            }
            context.SaveChanges();

        }
    }
}
