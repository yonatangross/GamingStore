using System;
using System.Collections.Generic;
using System.Linq;
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

            if (context.Items.Any())
            {
                return;
            }

            var directoryPath =
                AppContext.BaseDirectory.Substring(0,
                    AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));

            #region ItemsSeed

            // Load items if they don't exist.

            //TODO: add different items with properties
            var items = new[]
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
                    Title = "Milano Gaming Chair - Green", Manufacturer = "Arozzi", Price = 799,
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
                    Title = "Milano Gaming Chair - Blue", Manufacturer = "Arozzi", Price = 799,
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
                    Title = "NVIDIA GeForce RTX 3080", Manufacturer = "NVIDIA", Price = 3500, Category = "GPUs",
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
                    Title = "NVIDIA GeForce RTX 3090", Manufacturer = "NVIDIA", Price = 6500, Category = "GPUs",
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
                    Title = "GeForce RTX 2080 SUPER BLACK GAMING", Manufacturer = "EVGA", Price = 4300,
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

            #region CustomersSeed

            var dataCustomers = System.IO.File.ReadAllText(directoryPath + @"\Data\Mock_Data\Customers.json");
            var customers = JsonConvert.DeserializeObject<List<Customer>>(dataCustomers);
            context.Customers.AddRange(customers);
            context.SaveChanges();

            #endregion

            #region StoresSeed

            var dataStores =
                System.IO.File.ReadAllText(directoryPath + @"\Data\Mock_Data\Stores.json");
            var stores = JsonConvert.DeserializeObject<List<Store>>(
                dataStores);
            if (context.Items.Any() && context.Customers.Any())
                GenerateStoreItems(stores, items, customers);

            context.Stores.AddRange(stores);
            context.SaveChanges();

            #endregion

            #region OrdersAndPaymetsSeed

            var orders = GenerateOrders(customers, items.ToList(), stores, out var payments);

            var orderList = orders.ToList();
            foreach (var order in orderList)
                context.Orders.Add(order);
            foreach (var p in payments)
                context.Payments.Add(p);
            context.SaveChanges();

            #endregion
        }

        private static void GenerateStoreItems(IEnumerable<Store> stores, Item[] items,
            IReadOnlyCollection<Customer> customersList)
        {
            var rand = new Random();
            foreach (var store in stores)
            {
                foreach (var item in items)
                {
                    var itemCreated = rand.Next(2) == 1; // 1 - True  - False
                    if (!itemCreated) continue;
                    const float itemsNumberMultiplier = 0.3f;
                    store.StoreItems.Add(new StoreItem()
                    {
                        ItemId = item.Id, StoreId = store.Id,
                        ItemsCount =
                            (uint) rand.Next(1,
                                (int) (customersList.Count * itemsNumberMultiplier)) // customers number times 0.3
                    });
                }
            }
        }

        private static IEnumerable<Order> GenerateOrders(IEnumerable<Customer> customersList,
            IReadOnlyCollection<Item> items,
            IReadOnlyCollection<Store> storesList, out List<Payment> payments)
        {
            payments = new List<Payment>();
            var list = new List<Order>();
            var rand = new Random();
            var shopOpeningDate = new DateTime(2018, 1, 1);
            var range = (DateTime.Today - shopOpeningDate).Days;
            foreach (var customer in customersList)
            {
                var numOfOrdersForCustomer = rand.Next(minValue: 0, maxValue: 5);
                for (var orderNumber = 0; orderNumber < numOfOrdersForCustomer; orderNumber++)
                {
                    const int minItems = 1;
                    const int maxItems = 5;
                    var numItemsOrdered = rand.Next(minItems, maxItems);
                    var store = GenerateRelatedStore(customer, storesList);
                    var order = new Order()
                    {
                        CustomerId = customer.Id, OrderDate = shopOpeningDate.AddDays(rand.Next(range)),
                        State = OrderState.Fulfilled,
                        StoreId = store.Id,
                    };
                    order.OrderItems = GenerateOrderItems(order.Id, items, numItemsOrdered, out var payment);
                    order.Payment = payment;
                    payments.Add(payment);
                    list.Add(order);
                }
            }

            return list;
        }

        private static Store GenerateRelatedStore(Customer customer, IEnumerable<Store> storesList)
        {
            var storesInCustomerCity = storesList.Where(store => store.Address.City == customer.Address.City).ToList();
            var rand = new Random();
            return storesInCustomerCity[rand.Next(storesInCustomerCity.Count)];
        }

        private static ICollection<OrderItem> GenerateOrderItems(int orderId, IEnumerable<Item> items,
            int numItemsOrdered,
            out Payment payment)
        {
            var itemsList = new List<Item>(items); // copy list in order to alter it.
            var rand = new Random();
            var orderItems = new List<OrderItem>();
            for (var orderItemIndex = 0; orderItemIndex < numItemsOrdered; orderItemIndex++)
            {
                var curIndex = rand.Next(itemsList.Count);
                var curItem = itemsList[curIndex];
                itemsList.Remove(curItem);
                var orderItem = new OrderItem()
                {
                    OrderId = orderId,
                    ItemId = curItem.Id,
                    Item = curItem,
                    ItemsCount = (uint) rand.Next(1, 3)
                };
                orderItems.Add(orderItem);
            }

            payment = new Payment()
            {
                ItemsCost = CalculateOrderSum(orderItems), PaymentMethod = (PaymentMethod) rand.Next(0, 3),
                ShippingCost = 0,
                Paid = true
            };


            return orderItems;
        }

        private static int CalculateOrderSum(IEnumerable<OrderItem> orderItems)
        {
            return orderItems.Sum(orderItem => (orderItem.Item.Price * (int) orderItem.ItemsCount));
        }
    }
}