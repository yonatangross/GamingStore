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
                    Title = "Cloud Stinger Wired Stereo Gaming Headset", Manufacturer = "HyperX", Price = 29.78,
                    Category = Category.GamingHeadsets,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Sound Mode", "Stereo"},
                            {"Connection Type", "Wired"},
                            {"Water Resistant", "No"}
                        },
                    ImageUrl = "images/items/CloudStingerWiredStereoGamingHeadset",
                    Description = "HyperX Cloud Stinger is the ideal headset for gamers looking for lightweight comfort, superior sound quality and added convenience. At just 275 grams, it’s comfortable on your neck and its ear cups rotate in a 90-degree angle for a better fit. HyperX signature memory foam also provides ultimate comfort around the ears for prolonged gaming sessions."
                },
                new Item
                {
                    Title = "G432 Wired 7.1 Surround Sound Gaming Headset", Manufacturer = "Logitech", Price = 39.95,
                    Category = Category.GamingHeadsets, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Built-In Microphone", "Yes"},
                            {"Connection Type", "Wired"},
                            {"Headphone Fit", "Over-the-Ear"}
                        },
                    ImageUrl = "images/items/G432Wired7dot1SurroundSoundGamingHeadset",
                    Description = "Logitech G432 7. 1 surround sound gaming headset is enhanced with advanced Soundscape technology. Hear more of the game with huge 50 mm drivers that deliver a big sound. For maximum immersion, DTS Headphone: X 2. 0 surround sound creates precise in-game positional awareness."
                },
                new Item
                {
                    Title = "Milano Gaming Chair - Green", Manufacturer = "Arozzi", Price = 227,
                    Category = Category.GamingChairs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Office Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Green"}
                        },
                    ImageUrl = "images/items/MilanoGamingChair-Green",
                    Description = "No matter where you game, work or even just read and relax, doing it in supreme comfort allows you to do it better, longer and with greater enthusiasm. That’s the inspiration for Milano, Arozzi’s gaming chair which provides both maximum comfort and maximum value."
                },
                new Item
                {
                    Title = "Milano Gaming Chair - Red", Manufacturer = "Arozzi", Price = 219.99,
                    Category = Category.GamingChairs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Office Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Blue"}
                        },
                    ImageUrl = "images/items/MilanoGamingChair-Red",
                    Description = "No matter where you game, work or even just read and relax, doing it in supreme comfort allows you to do it better, longer and with greater enthusiasm. That’s the inspiration for Milano, Arozzi’s gaming chair which provides both maximum comfort and maximum value."
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
                    ImageUrl = "images/items/LogitechG440HardGaming",
                    Description = "G440 features a low friction, hard polymer surface ideal for high DPI gaming, improving mouse control and precise cursor placement."
                },
                new Item
                {
                    Title = "NVIDIA GeForce RTX 3080", Manufacturer = "NVIDIA", Price = 719,
                    Category = Category.GPUs,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Fan"},
                            {"Boost Clock Speed", "1.71 GHz"},
                            {"GPU Memory Size", "10 GB"}
                        },
                    ImageUrl = "images/items/NVIDIAGeForceRTX3080",
                    Description = "NVIDIA Ampere Streaming Multiprocessors 2nd Generation RT Cores 3rd Generation Tensor Cores Powered by GeForce RTX 3080 Integrated with 10GB GDDR6X 320-bit memory interface WINDFORCE 3X Cooling System with alternate spinning fans RGB Fusion 2.0 Protection metal back plate Clock Core: 1755"
                },
                new Item
                {
                    Title = "NVIDIA GeForce RTX 3090", Manufacturer = "NVIDIA", Price = 1500, Category =Category.GPUs,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Fan"},
                            {"Boost Clock Speed", "1.70 GHz"},
                            {"GPU Memory Size", "24 GB"}
                        },
                    ImageUrl = "images/items/NVIDIAGeForceRTX3090",
                    Description = "The GeForce RTXTM 3090 is a big ferocious GPU (BFGPU) with TITAN class performance. It’s powered by Ampere—NVIDIA’s 2nd gen RTX architecture—doubling down on ray tracing and AI performance with enhanced RT Cores, Tensor Cores, and new streaming multiprocessors. Plus, it features a staggering 24 GB of G6X memory"
                },
                new Item
                {
                    Title = "GeForce RTX 2080 SUPER BLACK GAMING", Manufacturer = "EVGA", Price = 430,
                    Category = Category.GPUs,
                    PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Cooling System", "Active"},
                            {"Boost Clock Speed", "1815 MHz"},
                            {"GPU Memory Size", "8 GB"}
                        },
                    ImageUrl = "images/items/GeForceRTX2080SUPERBLACKGAMING",
                    Description = "The EVGA GeForce RTX K-series graphics cards are powered by the all-New NVIDIA Turing architecture to give you incredible New levels of gaming realism, speed, power efficiency, and immersion. With the EVGA GeForce RTX K-series gaming cards you get the best gaming experience with the next generation graphics performance"
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
                    ImageUrl = "images/items/IntelCorei9-10850KACometLakeBox",
                    Description = "Intel BX80684I99900KF Intel Core i9-9900KF Desktop Processor 8 Cores up to 5. 0 GHz Turbo Unlocked Without Processor Graphics LGA1151 300 Series 95W. Memory Types: DDR4-2666,Max Memory Bandwidth: 41.6 GB/s, Scalability: 1S Only,PCI Express Configurations: Up to 1x16, 2x8, 1x8+2x4"
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
                    ImageUrl = "images/items/IntelCorei9-10900KACometLakeBox",
                    Description = "Intel BX80684I99900KF Intel Core i9-9900KF Desktop Processor 8 Cores up to 5. 0 GHz Turbo Unlocked Without Processor Graphics LGA1151 300 Series 95W. Memory Types: DDR4-2666,Max Memory Bandwidth: 41.6 GB/s, Scalability: 1S Only,PCI Express Configurations: Up to 1x16, 2x8, 1x8+2x4"
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
                    ImageUrl = "images/items/gamingheadsetwhitecombatcamouflage",
                    Description = "High precision 50mm magnetic neodymium drivers deliver high-quality stereo sound, From clear high-frequency and mid-range playback to deep bass surround bass. provide you a immerse game experience, Let you quickly hear footsteps and distant gunshots from different direction in Fortnight, PUBG or CS: go etc"
                },
                new Item
                {
                    Title = "Sceptre 24' Curved 75Hz Gaming LED Monitor", Manufacturer = "Sceptre", Price = 129,
                    Category = Category.Monitors, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Display Size", "24 Inches"},
                            {"Resolution", "FHD 1080p"},
                            {"Hardware Interface", "VGA, HDMI"},
                            {"Display Technology", "LED"}
                        },
                    ImageUrl = "images/items/Sceptre24Curved",
                    Description = "24' Diagonal viewable curved screen HDMI, VGA & PC audio in ports Windows 10 compatible Contemporary sleek metal design with the C248W-1920RN, a slender 1800R screen curvature yields wide-ranging images that seemingly surround you. Protection and comfort are the hallmarks of this design as the metal pattern brush fi₪h is smooth and pleasing to the touch."
                },
                new Item
                {
                    Title = "AMD Ryzen 9 3900X 12-core, 24-thread processor", Manufacturer = "AMD", Price = 429,
                    Category = Category.CPUs, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "4.6 GHz"},
                            {"Processor Socket", "Socket AM4"}
                        },
                    ImageUrl = "images/items/AMDRyzen3900X",
                    Description = "AMD Ryzen 9 3900X 12 core, 24 thread unlocked desktop processor with Wraith Prism LED cooler."
                },
                new Item
                {
                    Title = "Acer Predator XB271HU 27' WQHD Monitor", Manufacturer = "Acer", Price = 510.9,
                    Category = Category.Monitors, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Size", "27'"},
                            {"Technology", "NVIDIA G-SYNC"},
                            {"Resolution", "2560 x 1440 WQHD"},
                            {"Refresh Rate", "144Hz (OverClocking to 165Hz)"},
                            {"Panel Type", "IPS"},
                            {"Response Time", "4ms"},
                            {"Ports", "1 x DP, 1 x HDMI & 4 x USB 3.0"}
                        },
                    ImageUrl = "images/items/AcerPredatorXB271HU",
                    Description = "Fasten your seatbelt: Acer's Predator XB271HU WQHD display is about to turbocharge your gaming experience. This monitor combines jaw dropping specs, IPS panel that supports 144Hz refresh rate, delivering an amazing gaming experience."
                },
                new Item
                {
                    Title = "Asus VG278QR 27” Gaming Monitor", Manufacturer = "Asus", Price = 336.9,
                    Category = Category.Monitors, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Size", "27'"},
                            {"Technology", "NVIDIA G-SYNC"},
                            {"Resolution", "FHD 1080p Ultra Wide"},
                            {"Refresh Rate", "165Hz"},
                            {"Response Time", "0.5ms"},
                            {"Ports", "DisplayPort, HDMI"}
                        },
                    ImageUrl = "images/items/AsusVG278QR27GamingMonitor",
                    Description = "Make every millisecond count with the 27” VG278QR gaming monitor featuring a 165Hz refresh rate and 0 5ms response time with Asus’ elmb technology to reduce motion blur with free Sync and G-SYNC compatibility Turn any desk into a marathon battle station with vg278qr’s ergonomic adjustable stand and eye Care technology."
                },
                new Item
                {
                    Title = "LG 27GL83A-B 27 Inch Ultragear QHD IPS 1ms", Manufacturer = "LG", Price = 379.99,
                    Category = Category.Monitors, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Size", "27'"},
                            {"Technology", "NVIDIA G-SYNC"},
                            {"Resolution", "QHD Wide 1440p"},
                            {"Response Time", "1ms"}
                        },
                    ImageUrl = "images/items/AsusVG278QR27GamingMonitor",
                    Description = "The 27” Ultra Gear QHD IPS 1ms 144Hz monitor is G-Sync Compatible and has a 3-Side Virtually Borderless bezel. Other features includes: Tilt / Height / Pivot Adjustable Stand."
                },
                new Item
                {
                    Title = "Logitech G PRO Mechanical Gaming Keyboard", Manufacturer = "Logitech", Price = 118.99,
                    Category = Category.Keyboards, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"LIGHTSPEED Wireless", "No"},
                            {"Mechanical Switches", "GX Blue clicky"},
                            {"Connectivity", "USB Keyboard + USB Passthrough"},
                            {"Dedicated Media Control", "No - Integrated F-keys"}
                        },
                    ImageUrl = "images/items/LogitechGPROMechanicalGamingKeyboard",
                    Description = "Built for pros from the bottom up. A compact tenkeyless design frees up table space for low-sens mousing. Pro-grade Clicky switches give you an audible feedback bump. Programmable LIGHTSYNC RGB and onboard memory lets you customize and store a lighting pattern for tournaments"
                },
                new Item
                {
                    Title = "Corsair Strafe RGB MK.2 Mechanical Gaming Keyboard", Manufacturer = "Corsair", Price = 127.71,
                    Category = Category.Keyboards, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Connections", "USB"}
                        },
                    ImageUrl = "images/items/CorsairStrafe",
                    Description = "The next-generation CORSAIR STRAFE RGB MK.2 mechanical keyboard features 100% CHERRY MX Silent RGB keyswitches for key presses that are up to 30% quieter, alongside and 8MB onboard profile storage to take your gaming profiles with you."
                },
                new Item
                {
                    Title = "Mouse M325 Lemon", Manufacturer = "Logitech", Price = 29.95,
                    Category = Category.Mouses, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wireless"},
                            {"Weight", "125g"},
                            {"Max battery life", "36 Months"}
                        },
                    ImageUrl = "images/items/LogitechM325MouseLemon",
                    Description="Logitech Wireless Mouse M325 Lemon Yellow With micro-precise scrolling, ultra-smooth cursor control and super-long-and-reliable battery power, the compact Logitech Wireles Mouse M325 screams control—and personal style in your choice of sweet color combinations."
                },
                new Item
                {
                    Title = "M325 Mouse Watermelon", Manufacturer = "Logitech", Price = 20.58,
                    Category = Category.Mouses, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wireless"},
                            {"Weight", "140g"},
                            {"Max battery life", "36 Months"}
                        },
                    ImageUrl = "images/items/LogitechM325MouseWatermelon",
                    Description="Logitech M325 Wireless Mouse With micro-precise scrolling, ultra-smooth cursor control and super-long-and-reliable battery power, the compact Logitech Wireles Mouse M325 screams control—and personal style in your choice of sweet color combinations."
                },
                new Item
                {
                    Title = "M525 Mouse", Manufacturer = "Logitech", Price = 20.58,
                    Category = Category.Mouses, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wireless"},
                            {"Weight", "156g"},
                            {"Max battery life", "36 Months"}
                        },
                    ImageUrl = "images/items/LogitechM525",
                    Description="With micro-precise scrolling, ultra-smooth cursor control and super-long-and-reliable battery power, the compact Logitech Wireles Mouse M525 screams control—and personal style in your choice of sweet color combinations."
                },
                new Item
                {
                    Title = "Basilisk Essentiy", Manufacturer = "Razer", Price = 45.55,
                    Category = Category.Mouses, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wired"},
                            {"Weight", "127g"},
                            {"Programmable Buttons", "7"}
                        },
                    ImageUrl = "images/items/RazerBasiliskEssentiy",
                    Description="The Razer Basilisk Essential is the gaming mouse with customizable features for an edge in battle. The multi-function paddle offers extended controls such as push-to-talk, while the Razer mechanical mouse switches give fast response times and are durable for up to 20 million clicks."
                },
                new Item
                {
                    Title = "DeathAdder v2 Ergonomic", Manufacturer = "Razer", Price = 69.99,
                    Category = Category.Mouses, PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wired"},
                            {"Weight", "82g"},
                            {"Programmable Buttons", "8"}
                        },
                    ImageUrl = "images/items/RazerDeathAdderv2Ergonomic",
                    Description="With over 10 million Razer DeathAdders sold, the most celebrated and awarded gaming mouse in the world has earned its popularity through its exceptional ergonomic design. Perfectly suited for a palm grip, it also works well with claw and fingertip styles. The Razer DeathAdder V2 continues this legacy, retaining its signature shape while shedding more weight for quicker handling to improve your gameplay. Going beyond conventional office ergonomics, the optimized design also provides greater comfort for gaming—important for those long raids or when you’re grinding your rank on ladder."
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
                    order.PaymentId = payment.Id;
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

        private static ICollection<OrderItem> GenerateOrderItems(string orderId, IEnumerable<Item> items, int numItemsOrdered, out Payment payment)
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

            payment = new Payment
            {
                ItemsCost = CalculateOrderSum(orderItems), PaymentMethod = (PaymentMethod) rand.Next(0, 3),
                ShippingCost = 0,
                Paid = true
            };

            return orderItems;
        }

        private static double CalculateOrderSum(IEnumerable<OrderItem> orderItems)
        {
            return orderItems.Sum(orderItem => (orderItem.Item.Price * (double) orderItem.ItemsCount));
        }
    }
}