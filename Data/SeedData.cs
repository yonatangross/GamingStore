using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GamingStore.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider,string adminPassword)
        {
            await using var context = new StoreContext(serviceProvider.GetRequiredService<DbContextOptions<StoreContext>>());
            var userManager = serviceProvider.GetService<UserManager<Customer>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            await CreateRolesAndUsers(context,userManager, roleManager,adminPassword);

            SeedDatabase(context);
        }

        private static async Task CreateRolesAndUsers(StoreContext context,UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager,string adminPassword)
        {
            const string admin = "Admin";
            const string storeManager = "StoreManager";
            const string customer = "Customer";

            bool roleExists = await roleManager.RoleExistsAsync(admin);

            if (!roleExists)
            {
                // first we create Admin roll    
                var role = new IdentityRole {Name = admin};
                await roleManager.CreateAsync(role);

                //Here we create a Admin super users who will maintain the website                   
                await AddAdmins(userManager, adminPassword);
            }

            // creating Creating Manager role     
            roleExists = await roleManager.RoleExistsAsync(storeManager);
            if (!roleExists)
            {
                var role = new IdentityRole {Name = storeManager};
                await roleManager.CreateAsync(role);
            }

            // creating Creating Employee role     
            roleExists = await roleManager.RoleExistsAsync(customer);
            if (!roleExists)
            {
                var role = new IdentityRole {Name = customer};
                await roleManager.CreateAsync(role);
            }
            await context.SaveChangesAsync();
        }

        private static async Task AddAdmins(UserManager<Customer> userManager, string adminPassword)
        {
            var admins = new List<Customer>
            {
                new Customer
                {
                    UserName = "yonatan2gross@gmail.com",
                    Email = "yonatan2gross@gmail.com",
                    FirstName = "Yonatan",
                    LastName = "Gross",
                },
                new Customer
                {
                    UserName = "matan18061806@gmail.com",
                    Email = "matan18061806@gmail.com",
                    FirstName = "Matan",
                    LastName = "Hassin"
                },
                new Customer
                {
                    UserName = "aviv943@gmail.com",
                    Email = "aviv943@gmail.com",
                    FirstName = "Aviv",
                    LastName = "Miranda"
                },
                new Customer
                {
                    UserName = "ohad338@gmail.com",
                    Email = "ohad338@gmail.com",
                    FirstName = "Ohad",
                    LastName = "Cohen"
                }
            };

            foreach (Customer newAdmin in admins)
            {
                IdentityResult result = await userManager.CreateAsync(newAdmin, adminPassword);

                //Add default User to Role Admin    
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }

        public static void SeedDatabase(StoreContext context)
        {
            if (context.Items.Any())
            {
                return;
            }

            string directoryPath =
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
                    Category = Category.GamingHeadsets,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Sound Mode", "Stereo"},
                            {"Connection Type", "Wired"},
                            {"Water Resistant", "No"}
                        },
                    ImageUrl = "images/items/items/CloudStingerWiredStereoGamingHeadset.jpg"
                },
                new Item
                {
                    Title = "G432 Wired 7.1 Surround Sound Gaming Headset", Manufacturer = "Logitech", Price = 200,
                    Category = Category.GamingHeadsets, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Built-In Microphone", "Yes"},
                            {"Connection Type", "Wired"},
                            {"Headphone Fit", "Over-the-Ear"}
                        },
                    ImageUrl = "images/items/items/G432Wired7dot1SurroundSoundGamingHeadset.jpg"
                },
                new Item
                {
                    Title = "Milano Gaming Chair - Green", Manufacturer = "Arozzi", Price = 799,
                    Category = Category.GamingChairs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Office Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Green"}
                        },
                    ImageUrl = "images/items/MilanoGamingChair-Green.jpg"
                },
                new Item
                {
                    Title = "Milano Gaming Chair - Blue", Manufacturer = "Arozzi", Price = 799,
                    Category = Category.GamingChairs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Office Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Blue"}
                        },
                    ImageUrl = "images/items/MilanoGamingChair-Blue.jpg"
                },
                new Item
                {
                    Title = "Logitech G440 Hard Gaming", Manufacturer = "Logitech ", Price = 130,
                    Category = Category.MousePads, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"length", "280mm"},
                            {"width", "340mm"},
                            {"height", "3mm"}
                        },
                    ImageUrl = "images/items/LogitechG440HardGaming.jpg"
                },
                new Item
                {
                    Title = "NVIDIA GeForce RTX 3080", Manufacturer = "NVIDIA", Price = 3500,
                    Category = Category.GPUs,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Fan"},
                            {"Boost Clock Speed", "1.71 GHz"},
                            {"GPU Memory Size", "10 GB"}
                        },
                    ImageUrl = "images/items/NVIDIAGeForceRTX3080.jpg"
                },
                new Item
                {
                    Title = "NVIDIA GeForce RTX 3090", Manufacturer = "NVIDIA", Price = 6500, Category =Category.GPUs,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Fan"},
                            {"Boost Clock Speed", "1.70 GHz"},
                            {"GPU Memory Size", "24 GB"}
                        },
                    ImageUrl = "images/items/NVIDIAGeForceRTX3090.jpg"
                },
                new Item
                {
                    Title = "GeForce RTX 2080 SUPER BLACK GAMING", Manufacturer = "EVGA", Price = 4300,
                    Category = Category.GPUs,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Active"},
                            {"Boost Clock Speed", "1815 MHz"},
                            {"GPU Memory Size", "8 GB"}
                        },
                    ImageUrl = "images/items/GeForceRTX2080SUPERBLACKGAMING.jpg"
                },
                new Item
                {
                    Title = "Intel Core i9-10900KA Comet Lake Box", Manufacturer = "Intel", Price = 2420,
                    Category = Category.CPUs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cores", "10"},
                            {"Clock Speed", "3.60GHz - 5.30GHz"},
                            {"GPU", "Intel® UHD Graphics 630"},
                            {"Cache Memory", "20MB"}
                        },
                    ImageUrl = "images/items/IntelCorei9-10850KACometLakeBox.jpg"
                },
                new Item
                {
                    Title = "Intel Core i9-10850KA Comet Lake Box", Manufacturer = "Intel", Price = 2020,
                    Category = Category.CPUs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cores", "10"},
                            {"Clock Speed", "3.60GHz - 5.20GHz"},
                            {"GPU", "Intel® UHD Graphics 630"},
                            {"Cache Memory", "20MB"}
                        },
                    ImageUrl = "images/items/IntelCorei9-10900KACometLakeBox.jpg"
                },
                new Item
                {
                    Title = "Gaming Headset white combat camouflage", Manufacturer = "Dragon", Price = 499,
                    Category = Category.GamingHeadsets, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Sound Mode", "Stereo"},
                            {"Connection Type", "Wired"},
                            {"Water Resistant", "No"}
                        },
                    ImageUrl = "images/items/gamingheadsetwhitecombatcamouflage.jpg"
                }
            };
            foreach (Item item in items)
            {
                context.Items.Add(item);
            }

            context.Items.AsNoTracking();
            context.SaveChanges();
            #endregion

            #region CustomersSeed
            string dataCustomers = System.IO.File.ReadAllText($@"{directoryPath}\Data\Mock_Data\CustomersMin.json");
            var customers = JsonConvert.DeserializeObject<List<Customer>>(dataCustomers);
            context.Customers.AddRange(customers);
            context.Customers.AsNoTracking();
            context.SaveChanges();
            #endregion

            #region StoresSeed
            string dataStores = System.IO.File.ReadAllText(directoryPath + @"\Data\Mock_Data\Stores.json");
            var stores = JsonConvert.DeserializeObject<List<Store>>(dataStores);

            if (context.Items.Any() && context.Customers.Any())
            {
                GenerateStoreItems(stores, items, customers);
            }

            context.Stores.AddRange(stores);
            context.SaveChanges();
            #endregion

            #region OrdersAndPaymentsSeed

            try
            {
                IEnumerable<Order> orders = GenerateOrders(customers, items.ToList(), stores, out var payments);

                List<Order> orderList = orders.ToList();
                foreach (Order order in orderList)
                {
                    context.Orders.Add(order);
                }

                foreach (Payment payment in payments)
                {
                    context.Payments.Add(payment);
                }

                context.Orders.AsNoTracking();
                context.SaveChanges();
            }
            catch (Exception e)
            {
                
            }
            #endregion

            #region CartsSeed

            //try
            //{
            //    Customer firstCustomer = context.Customers.First();
            //    var cart = new Cart(firstCustomer.Id)
            //    {
            //        ShoppingCart = new Dictionary<int, uint>()
            //        {
            //            {1, 2}
            //        }
            //    };

            //    context.Carts.AsNoTracking();
            //    context.Carts.Update(cart);
            //    context.SaveChanges();
            //}
            //catch (Exception e)
            //{
                
            //}
            #endregion
        }

        private static void GenerateStoreItems(IEnumerable<Store> stores, Item[] items, IReadOnlyCollection<Customer> customersList)
        {
            var random = new Random();
            
            foreach (Store store in stores)
            {
                foreach (Item item in items)
                {
                    bool itemCreated = random.Next(2) == 1; // 1 - True  - False
                    
                    if (!itemCreated)
                    {
                        continue;
                    }

                    const float itemsNumberMultiplier = 0.3f;
                    
                    store.StoreItems.Add(new StoreItem
                    {
                        ItemId = item.Id, StoreId = store.Id,
                        ItemsCount =
                            (uint) random.Next(1,
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
            int range = (DateTime.Today - shopOpeningDate).Days;

            foreach (Customer customer in customersList)
            {
                int numOfOrdersForCustomer = rand.Next(minValue: 0, maxValue: 5);
                
                for (var orderNumber = 0; orderNumber < numOfOrdersForCustomer; orderNumber++)
                {
                    const int minItems = 1;
                    const int maxItems = 5;
                    int numItemsOrdered = rand.Next(minItems, maxItems);
                    Store store = GenerateRelatedStore(customer, storesList);
                    
                    var order = new Order
                    {
                        CustomerId = customer.Id,
                        OrderDate = shopOpeningDate.AddDays(rand.Next(range)),
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
            List<Store> storesInCustomerCity = storesList.Where(store => store.Address.City == customer.Address.City).ToList();
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
                int curIndex = rand.Next(itemsList.Count);
                Item curItem = itemsList[curIndex];
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