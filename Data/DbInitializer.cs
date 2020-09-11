using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
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
                    Title = "Cloud Stinger Wired Stereo Gaming Headset", Manufacturer = "HyperX", Price = 200,
                    Category = "Gaming Headsets", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Sound Mode", "Stereo"},
                            {"Connection Type", "Wired"},
                            {"Water Resistant", "No"}
                        }
                },
                new Item
                {
                    Title = "G432 Wired 7.1 Surround Sound Gaming Headset", Manufacturer = "Logitech", Price = 200,
                    Category = "Gaming Headsets", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Built-In Microphone", "Yes"},
                            {"Connection Type", "Wired"},
                            {"Headphone Fit", "Over-the-Ear"}
                        }
                },
                new Item
                {
                    Title = "Milano Gaming Chair - Green", Manufacturer = "Arozzi ", Price = 799,
                    Category = "Gaming Chairs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Office Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Green"}
                        }
                },
                new Item
                {
                    Title = "Milano Gaming Chair - Blue", Manufacturer = "Arozzi ", Price = 799,
                    Category = "Gaming Chairs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Office Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Blue"}
                        }
                },
                new Item
                {
                    Title = " Logitech G440 Hard Gaming", Manufacturer = "Logitech ", Price = 130,
                    Category = "Mouse Pads", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"length", "280mm"},
                            {"width", "340mm"},
                            {"height", "3mm"}
                        }
                },
                new Item
                {
                    Title = "NVIDIA GEFORCE RTX 3080", Manufacturer = "Nvidia", Price = 3500, Category = "GPUs",
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Fan"},
                            {"Boost Clock Speed", "1.71 GHz"},
                            {"GPU Memory Size", "10 GB"}
                        }
                },
                new Item
                {
                    Title = "NVIDIA GEFORCE RTX 3090", Manufacturer = "Nvidia", Price = 6500, Category = "GPUs",
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Fan"},
                            {"Boost Clock Speed", "1.70 GHz"},
                            {"GPU Memory Size", "24 GB"}
                        }
                },
                new Item
                {
                    Title = "GEFORCE RTX 2080 SUPER BLACK GAMING", Manufacturer = "EVGA", Price = 4300,
                    Category = "GPUs",
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Active"},
                            {"Boost Clock Speed", "1815 MHz"},
                            {"GPU Memory Size", "8 GB"}
                        }
                },
                new Item
                {
                    Title = "Intel Core i9-10900KA Comet Lake Box", Manufacturer = "Intel", Price = 2420,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cores", "10"},
                            {"Clock Speed", "3.60GHz - 5.30GHz"},
                            {"GPU", "Intel® UHD Graphics 630"},
                            {"Cache Memory", "20MB"}
                        }
                },
                new Item
                {
                    Title = "Intel Core i9-10850KA Comet Lake Box", Manufacturer = "Intel", Price = 2020,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cores", "10"},
                            {"Clock Speed", "3.60GHz - 5.20GHz"},
                            {"GPU", "Intel® UHD Graphics 630"},
                            {"Cache Memory", "20MB"}
                        }
                },
                new Item
                {
                    Title = "Gaming Headset white combat camouflage", Manufacturer = "Dragon", Price = 499,
                    Category = "Gaming Headsets", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Sound Mode", "Stereo"},
                            {"Connection Type", "Wired"},
                            {"Water Resistant", "No"}
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

            var dataCustomers =
                System.IO.File.ReadAllText(directoryPath + @"\Data\Mock_Data\Customers.json");
            var customersList =
                JsonConvert.DeserializeObject<List<Customer>>(
                    dataCustomers);
            context.Customers.AddRange(customersList);
            context.SaveChanges();
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

            List<Order> ordersList = GenerateOrder(customersList, items);
            var orders = new Order[]
            {
                new Order
                {
                    Customer = customersList[0],
                    OrderDate = DateTime.Now.AddDays(-7),
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            ItemId = 1,
                            ItemsCount = 3
                        },
                        new OrderItem()
                        {
                            ItemId = 2,
                            ItemsCount = 2
                        }
                    },
                    Payment = new Payment()
                    {
                        ItemsCost = items[1].Price * 3 + items[2].Price * 2,
                        Paid = true,
                        PaymentMethod = PaymentMethod.Paypal,
                        ShippingCost = 9
                    },
                    State = OrderState.Fulfilled,
                },
                new Order
                {
                    Customer = customersList[0],
                    OrderDate = DateTime.Now.AddDays(-7),
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            ItemId = 1,
                            ItemsCount = 3
                        },
                        new OrderItem()
                        {
                            ItemId = 2,
                            ItemsCount = 2
                        }
                    },
                    Payment = new Payment()
                    {
                        ItemsCost = items[1].Price * 3 + items[2].Price * 2,
                        Paid = true,
                        PaymentMethod = PaymentMethod.Paypal,
                        ShippingCost = 9
                    },
                    State = OrderState.Fulfilled,
                },
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

        private static List<Order> GenerateOrder(List<Customer> customersList, Item[] items)
        {
            var list = new List<Order>();
            var rand = new Random();
            DateTime start = new DateTime(2018, 1, 1);
            int range = (DateTime.Today - start).Days;
            foreach (var customer in customersList)
            {
                var numOfOrdersForCustomer = rand.Next(minValue: 0, maxValue: 5);
                for (int orderNumber = 0; orderNumber < numOfOrdersForCustomer; orderNumber++)
                {
                    var numOfItems = rand.Next(1, 5);
                    var order = new Order()
                    {
                        Customer = customer, OrderDate = start.AddDays(rand.Next(range)),
                        OrderItems = GenerateOrderItems(items, numOfItems, out var payment), Payment = payment,
                        State = OrderState.Fulfilled, Store = GenerateRelatedStore(customer)
                    };
                    customer.OrderHistory.Add(order);
                }
            }

            return list;
        }

        private static Store GenerateRelatedStore(Customer customer)
        {
            throw new NotImplementedException();
        }

        private static ICollection<OrderItem> GenerateOrderItems(Item[] items, in int numOfItems, out Payment payment)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            payment = new Payment();

            return orderItems;
        }
    }
}