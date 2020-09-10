using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;
using Newtonsoft.Json;

namespace GamingStore.Data
{
    public class DbInitializer
    {
        public static void Initialize(StoreContext context)
        {
            context.Database.EnsureCreated();

            // Look for any items.
            if (context.Items.Any())
            {
                return; // DB has been seeded
            }

            var directoryPath =
                AppContext.BaseDirectory.Substring(0,
                    AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));

            #region ItemsSeed

            //TODO: add different items with properties
            var items = new Item[]
            {
                new Item
                {
                    Title = "Keyboard", Manufacturer = "Microsoft", Price = 299, Category = "hardware", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                },
                new Item
                {
                    Title = "Mouse", Manufacturer = "Microsoft", Price = 150, Category = "hardware", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                },
                new Item
                {
                    Title = "Gaming Chair", Manufacturer = "Razor", Price = 799, Category = "other", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                },
                new Item
                {
                    Title = "Mouse Pad", Manufacturer = "Razor", Price = 50, Category = "hardware", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                },
                new Item
                {
                    Title = "Graphic Card", Manufacturer = "Nvidia", Price = 1500, Category = "GPUs hardware",
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                },
                new Item
                {
                    Title = "Processor", Manufacturer = "Intel", Price = 1249, Category = "CPU", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                },
                new Item
                {
                    Title = "Headphones", Manufacturer = "Bose", Price = 499, Category = "hardware", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"diameter", "3"},
                            {"width", "15"},
                            {"height", "4"}
                        }
                }
            };
            foreach (var i in items)
            {
                context.Items.Add(i);
            }

            context.SaveChanges();

            #endregion

            #region StoresSeed

            if (!context.Stores.Any())
            {
                var dataStores =
                    System.IO.File.ReadAllText(directoryPath + @"\Data\Mock_Data\Stores.json");
                List<Store> storesList =
                    JsonConvert.DeserializeObject<List<Store>>(
                        dataStores);
                context.Stores.AddRange(storesList);
                context.SaveChanges();
            }
            //var stores = new Store[]
            //{
            //    new Store
            //    {
            //        Id = 0,
            //        Name = "KSP", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474",
            //        Address = new Address()
            //        {
            //            Address1 = "Dizingoff Center - Second Floor",
            //            City = "Beer Sheva",
            //            Country = "Israel",
            //            PostalCode = "1234567"
            //        },
            //        OpeningHours = new OpeningHours[7]
            //        {
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Sunday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Monday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Tuesday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Wednesday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Thursday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Friday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(14, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Saturday,
            //                OpeningTime = new TimeSpan(19, 30, 00),
            //                ClosingTime = new TimeSpan(22, 00, 00),
            //            }
            //        },
            //    },
            //    new Store {Id = 1, Name = "Zap", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"},
            //    new Store {Id = 2, Name = "Bug", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"},
            //    new Store
            //    {
            //        Id = 3, Name = "Shimi Gaming Chairs", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store
            //    {
            //        Id = 4, Name = "Ohad GamingWorld", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store
            //    {
            //        Id = 5, Name = "Miranda Mouses", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store
            //    {
            //        Id = 6, Name = "Yoni Keyboards", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store()
            //    {
            //        Id = 7,
            //        Name = "Gaming Store - Tel Aviv",
            //        PhoneNumber = "0506656474",
            //        Email = "yonatan2gross@gmail.com",
            //        Address = new Address()
            //        {
            //            Address1 = "Dizingoff Center - Second Floor",
            //            City = "Tel Aviv",
            //            Country = "Israel",
            //            PostalCode = "1234567"
            //        },
            //        OpeningHours = new OpeningHours[7]
            //        {
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Sunday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Monday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Tuesday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Wednesday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Thursday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(20, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Friday,
            //                OpeningTime = new TimeSpan(8, 30, 00),
            //                ClosingTime = new TimeSpan(14, 00, 00),
            //            },
            //            new OpeningHours()
            //            {
            //                DayOfWeek = DayOfWeek.Saturday,
            //                OpeningTime = new TimeSpan(19, 30, 00),
            //                ClosingTime = new TimeSpan(22, 00, 00),
            //            }
            //        },
            //    },
            //    new Store
            //    {
            //        Id = 8, Name = "Yoni Headphones", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store {Id = 9, Name = "Matan GPUs ", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"},
            //    new Store
            //    {
            //        Id = 10, Name = "GamingStore", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store
            //    {
            //        Id = 11, Name = "KeyboardStore", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store
            //    {
            //        Id = 12, Name = " MouseStore", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store {Id = 13, Name = "Alam", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"},
            //    new Store
            //    {
            //        Id = 14, Name = "Mahsany Hashmal", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Store
            //    {
            //        Id = 15, Name = "BolilonGamingStore", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474"
            //    },
            //};
            //foreach (var s in stores)
            //{
            //    if (s.Id != 7)
            //        context.Stores.Add(s);
            //    else
            //        context.Stores.Add(s);
            //}

            //context.SaveChanges();

            #endregion

            #region CustomersSeed

            // Look for any items.
            if (!context.Customers.Any())
            {
                var dataCustomers =
                    System.IO.File.ReadAllText(directoryPath + @"\Data\Mock_Data\Customers.json");
                List<Customer> customersList =
                    JsonConvert.DeserializeObject<List<Customer>>(
                        dataCustomers);
                context.Customers.AddRange(customersList);
                context.SaveChanges();
            }
            //var customers = new Customer[]
            //{
            //    new Customer
            //    {
            //        FirstName = "Carson", LastName = "Alexander", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Shimon Peres 22",
            //            City = "Holon",
            //            Address2 = "Shimon Peres 22 Apartment 3",
            //            Country = "Israel",
            //            PostalCode = "463722",
            //            State = "Shipped"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Meredith", LastName = "Alonso", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Shimon Peres 10",
            //            City = "Rishon Lezion",
            //            Address2 = "Shimon Peres 10 Apartment 4",
            //            Country = "Israel",
            //            PostalCode = "132134",
            //            State = "On the way"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Arturo", LastName = "Anand", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Shimon 15",
            //            City = "Bat-Yam",
            //            Address2 = "Shimon 15 Apartment 20",
            //            Country = "Israel",
            //            PostalCode = "203941",
            //            State = "Delivered"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Gytis", LastName = "Barzdukas", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Peres 53",
            //            City = "Hedera",
            //            Address2 = "Peres 22 Apartment 53",
            //            Country = "Israel",
            //            PostalCode = "549382",
            //            State = "Shipped"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Carson", LastName = "Alexander", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Meredith", LastName = "Alonso", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Arturo", LastName = "Anand", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Gytis", LastName = "Barzdukas", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Yan", LastName = "Li", Email = "yonatan2gross@gmail.com", PhoneNumber = "0506656474",
            //        Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Peggy", LastName = "Justice", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Peggy", LastName = "Justice", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Laura", LastName = "Norman", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Nino", LastName = "Olivetto", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Ohad", LastName = "Bolilon", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474", Address = new Address()
            //        {
            //            Address1 = "Ben Yehuda 179", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
            //        }
            //    },
            //    new Customer
            //    {
            //        FirstName = "Matan", LastName = "Bolilon", Email = "bolilon@gmail.com", PhoneNumber = "0506656474"
            //    },
            //    new Customer
            //        {FirstName = "Almog", LastName = "Tfi", Email = "anubis@gmail.com", PhoneNumber = "0506656474"},
            //    new Customer
            //        {FirstName = "Aviv", LastName = "Bolila", Email = "Bil954@gmail.com", PhoneNumber = "0506656474"},
            //    new Customer
            //    {
            //        FirstName = "Rick", LastName = "Holio", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474"
            //    },
            //    new Customer
            //    {
            //        FirstName = "Matan", LastName = "Hamor", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474"
            //    },
            //    new Customer
            //    {
            //        FirstName = "Yoni", LastName = "Bolilon", Email = "yonatan2gross@gmail.com",
            //        PhoneNumber = "0506656474"
            //    }
            //};
            //foreach (var c in customers)
            //{
            //    context.Customers.Add(c);
            //}

            //context.SaveChanges();

            #endregion

            #region OrdersSeed

            var orders = new Order[]
            {
                new Order
                {
                    Customer = new Customer
                    {
                        FirstName = "samir", LastName = "Alonso", Email = "yonatan2gross@gmail.com",
                        PhoneNumber = "0506656474"
                    },
                    OrderDate = DateTime.Now.AddDays(-7),
                    //OrderItems = new List<OrderItem>()
                    //{
                    //    new OrderItem()
                    //    {
                    //        Item =
                    //            new Item
                    //            {
                    //                Title = "Processor i7",
                    //                Manufacturer = "Intel",
                    //                Price = 3450,
                    //                PropertiesList = new Dictionary<string, string>()
                    //                {
                    //                    {"diameter", "3"},
                    //                    {"width", "15"},
                    //                    {"height", "4"}
                    //                }
                    //            },
                    //        ItemsCount = 3
                    //    },
                    //    new OrderItem()
                    //    {
                    //        Item = new Item
                    //        {
                    //            Title = "Headphones",
                    //            Manufacturer = "Bose",
                    //            Price = 499,
                    //            PropertiesList = new Dictionary<string, string>()
                    //            {
                    //                {"diameter", "3"},
                    //                {"width", "15"},
                    //                {"height", "4"}
                    //            }
                    //        },
                    //        ItemsCount = 2
                    //    }
                    //},
                    //Payment = new Payment()
                    //{
                    //    ItemsCost = (3450 * 3) + (499 * 2),
                    //    Paid = true,
                    //    PaymentMethod = PaymentMethod.Paypal,
                    //    ShippingCost = 9
                    //},
                    State = OrderState.Fulfilled,
                },
                new Order
                {
                    Customer = new Customer
                    {
                        FirstName = "Yonatan", LastName = "Chips", Email = "yonatan2gross@gmail.com", Address =
                            new Address()
                            {
                                Address1 = "Yirmiyahu 48", City = "Tel Aviv", Country = "Israel", PostalCode = "1234567"
                            },
                        PhoneNumber = "0506656474"
                    },
                    OrderDate = DateTime.Now.AddDays(-7),
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Item =
                                new Item
                                {
                                    Title = "Processor i5",
                                    Manufacturer = "Intel",
                                    Price = 1450,
                                    PropertiesList = new Dictionary<string, string>()
                                    {
                                        {"diameter", "3"},
                                        {"width", "15"},
                                        {"height", "4"}
                                    }
                                },
                            ItemsCount = 2
                        },
                        new OrderItem()
                        {
                            Item = new Item
                            {
                                Title = "QCII",
                                Manufacturer = "Bose",
                                Price = 799,
                                PropertiesList = new Dictionary<string, string>()
                                {
                                    {"diameter", "3"},
                                    {"width", "15"},
                                    {"height", "4"}
                                }
                            },
                            ItemsCount = 1
                        }
                    },
                    Payment = new Payment()
                    {
                        ItemsCost = (1450 * 2) + (799 * 1),
                        Paid = true,
                        PaymentMethod = PaymentMethod.Paypal,
                        ShippingCost = 39
                    },
                    State = OrderState.Fulfilled,
                }
            };
            foreach (var o in orders)
            {
                context.Orders.Add(o);
            }

            context.SaveChanges();

            #endregion

            #region PaymentsSeed

            //todo: add payments with all parameters.
            var payments = new Payment[]
            {
                new Payment
                {
                    OrderForeignKey = 1, ItemsCost = 543, ShippingCost = 20, Paid = true,
                    PaymentMethod = PaymentMethod.Paypal
                },
                new Payment
                {
                    OrderForeignKey = 2, ItemsCost = 250, ShippingCost = 20, Paid = true,
                    PaymentMethod = PaymentMethod.Cash
                },
                //new Payment
                //{
                //    OrderForeignKey = 3, ItemsCost = 6043, ShippingCost = 0, Paid = false,
                //    PaymentMethod = PaymentMethod.CreditCard
                //},
                //new Payment
                //{
                //    OrderForeignKey = 4, ItemsCost = 300, ShippingCost = 30, Paid = true,
                //    PaymentMethod = PaymentMethod.Cash
                //},
                //new Payment
                //{
                //    OrderForeignKey = 5, ItemsCost = 987, ShippingCost = 30, Paid = false,
                //    PaymentMethod = PaymentMethod.CreditCard
                //},
                //new Payment
                //{
                //    OrderForeignKey = 6, ItemsCost = 100, ShippingCost = 50, Paid = true,
                //    PaymentMethod = PaymentMethod.Cash
                //}
            };
            foreach (var p in payments)
            {
                context.Payments.Add(p);
            }

            context.SaveChanges();

            #endregion
        }
    }
}