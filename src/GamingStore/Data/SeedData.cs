using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Extensions;
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
        public static async Task Initialize(IServiceProvider serviceProvider, string adminPassword)
        {
            await using var context = new StoreContext(serviceProvider.GetRequiredService<DbContextOptions<StoreContext>>());
            var userManager = serviceProvider.GetService<UserManager<Customer>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            await CreateRolesAndUsers(context, userManager, roleManager, adminPassword);

            SeedDatabase(context);


        }

        private static async Task CreateRolesAndUsers(StoreContext context, UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, string adminPassword)
        {
            const string admin = "Admin";
            const string customer = "Customer";

            bool roleExists = await roleManager.RoleExistsAsync(admin);

            if (!roleExists)
            {
                // first we create Admin roll    
                var role = new IdentityRole { Name = admin };
                await roleManager.CreateAsync(role);

                //Here we create a Admin super users who will maintain the website                   
                await AddAdmins(userManager, adminPassword);
            }

            // creating Creating Employee role     
            roleExists = await roleManager.RoleExistsAsync(customer);
            if (!roleExists)
            {
                var role = new IdentityRole { Name = customer };
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
                    Category = "GamingHeadsets",
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
                    Category = "GamingHeadsets", PropertiesList =
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
                    Category = "GamingChairs", PropertiesList =
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
                    Category = "GamingChairs", PropertiesList =
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
                    Title = "Logitech G440 Hard Gaming", Manufacturer = "Logitech", Price = 130,
                    Category = "MousePads", PropertiesList =
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
                    Category = "GPUs",
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
                    Title = "NVIDIA GeForce RTX 3090", Manufacturer = "NVIDIA", Price = 1500, Category ="GPUs",
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
                    Category = "GPUs",
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
                    Category = "CPUs", PropertiesList =
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
                    Category = "CPUs", PropertiesList =
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
                    Category = "GamingHeadsets", PropertiesList =
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
                    Category = "Monitors", PropertiesList =
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
                    Category = "CPUs", PropertiesList =
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
                    Category = "Monitors", PropertiesList =
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
                    Category = "Monitors", PropertiesList =
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
                    Category = "Monitors", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Size", "27'"},
                            {"Technology", "NVIDIA G-SYNC"},
                            {"Resolution", "QHD Wide 1440p"},
                            {"Response Time", "1ms"}
                        },
                    ImageUrl = "images/items/LG27GL83A-B",
                    Description = "The 27” Ultra Gear QHD IPS 1ms 144Hz monitor is G-Sync Compatible and has a 3-Side Virtually Borderless bezel. Other features includes: Tilt / Height / Pivot Adjustable Stand."
                },
                new Item
                {
                    Title = "Logitech G PRO Mechanical Gaming Keyboard", Manufacturer = "Logitech", Price = 118.99,
                    Category = "Keyboards", PropertiesList =
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
                    Category = "Keyboards", PropertiesList =
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
                    Category = "Mouses", PropertiesList =
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
                    Category = "Mouses", PropertiesList =
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
                    Category = "Mouses", PropertiesList =
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
                    Category = "Mouses", PropertiesList =
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
                    Category = "Mouses", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wired"},
                            {"Weight", "82g"},
                            {"Programmable Buttons", "8"}
                        },
                    ImageUrl = "images/items/RazerDeathAdderv2Ergonomic",
                    Description="With over 10 million Razer DeathAdders sold, the most celebrated and awarded gaming mouse in the world has earned its popularity through its exceptional ergonomic design. Perfectly suited for a palm grip, it also works well with claw and fingertip styles. The Razer DeathAdder V2 continues this legacy, retaining its signature shape while shedding more weight for quicker handling to improve your gameplay. Going beyond conventional office ergonomics, the optimized design also provides greater comfort for gaming—important for those long raids or when you’re grinding your rank on ladder."
                },
                //START HERE.................
                new Item
                {
                    Title = "AMD Ryzen 5 3600XT 6-core, 12-threads unlocked", Manufacturer = "AMD", Price = 233.30,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "4.5 GHz"},
                            {"Processor Socket", "Socket AM4"},
                            {"TDP", "95W"},
                            {"GameCache", "35MB"}
                        },
                    ImageUrl = "images/items/AMDRyzen53600XT",
                    Description="The AMD Ryzen 5 3600XT. Light Years Ahead. Featuring award winning performance and optimized technology for gamers, for creators, for everyone."
                },
                new Item
                {
                    Title = "Intel Core i5-8400 Desktop Processor 6 Cores", Manufacturer = "Intel", Price = 205,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "2.8 GHz"},
                            {"Processor Socket", "LGA 1151"},
                            {"UHD Graphics", "630"},
                            {"Smart Cache", "9MB"}
                        },
                    ImageUrl = "images/items/IntelCorei5-8400",
                    Description="Intel Core i5-8400 Processor (9M Cache, up to 2.80 GHz)"},

                new Item
                {
                    Title = "Intel Core i3-9100F Desktop Processor 4 Core", Manufacturer = "Intel", Price = 91.95,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "3.6 GHz"},
                            {"Processor Socket", "LGA 1151"},
                            {"TDP", "65W"},
                            {"Smart Cache", "6MB"}
                        },
                    ImageUrl = "images/items/IntelCorei3-9100F",
                    Description="9th Gen Intel Core i3-9100f desktop processor without processor graphics."
                },

                new Item
                {
                    Title = "Intel Core i3-10100 Desktop Processor 4 Cores", Manufacturer = "Intel", Price = 149.69,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "4.3 GHz"},
                            {"Processor Socket", "LGA 1200"},
                            {"TDP", "85W"},
                            {"Smart Cache", "9MB"}
                        },
                    ImageUrl = "images/items/IntelCorei3-10100",
                    Description="10th Gen Intel Core i3-10100 desktop processor optimized for everyday computing. Cooler included in the box. ONLY compatible with 400 series chipset based motherboard. 65W."
                },

                new Item
                {
                    Title = "Intel Core i3-8100 Desktop Processor 4", Manufacturer = "Intel", Price = 123.80,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "3.6 GHz"},
                            {"Processor Socket", "LGA 1156"},
                            {"UHD Graphics", "630"},
                            {"Smart Cache", "6MB"}
                        },
                    ImageUrl = "images/items/IntelCorei3-8100",
                    Description="Intel Core i3-8100 Desktop Processor 4 Cores up to 3.6 GHz Turbo Unlocked LGA1151 300 Series 95W"},

                new Item
                {
                    Title = "AMD Ryzen 3 3200G 4-Core", Manufacturer = "AMD", Price = 130,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "4.0 GHz"},
                            {"Processor Socket", "Socket AM4"},
                            {"TDP", "65W"},
                            {"Cache", "6MB"}
                        },
                    ImageUrl = "images/items/AMDRyzen33200G",
                    Description="AMD Ryzen 3 3200G, With Wraith Stealth C."
                },
                new Item
                {
                    Title = "Intel Core i7-9700 Desktop Processor 8 Cores", Manufacturer = "Intel", Price = 292.60,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "3.0 GHz"},
                            {"Processor Socket", "LGA 1151"},
                            {"TDP", "75W"},
                            {"Cache", "9MB"}
                        },
                    ImageUrl = "images/items/IntelCorei7-9700",
                    Description="9th Gen Intel Core i7-9700 desktop processor with Intel Turbo Boost Technology 2.0 and Intel vPro Technology."
                },
                new Item
                {
                    Title = "AMD Ryzen 7 3700X 8-Core", Manufacturer = "AMD", Price = 304.99,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "4.4 GHz"},
                            {"Processor Socket", "Socket AM4"},
                            {"TDP", "65W"},
                            {"Cache", "6MB"}
                        },
                    ImageUrl = "images/items/AMDRyzen73700X",
                    Description="AMD Ryzen 7 3700X 8 core, 16 thread unlocked desktop processor with Wraith Prism LED cooler. Base Clock - 3.6GHz.Default TDP / TDP: 65W."},
                //START HERE.................
                new Item
                {
                    Title = "Intel Core i5-9400 Desktop Processor 6 Cores", Manufacturer = "Intel", Price = 164.70,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "2.9 GHz"},
                            {"Processor Socket", "LGA 1151"},
                            {"TDP", "75W"},
                            {"Cache", "9MB"}
                        },
                    ImageUrl = "images/items/IntelCorei5-9400",
                    Description="Intel Core i5-9400 Desktop Processor 6 Cores 2. 90 GHz up to 4. 10 GHz Turbo LGA1151 300 Series 65W Processors BX80684I59400"
                },

                new Item
                {
                    Title = "Intel Core i9-9900K Desktop Processor 8 Cores", Manufacturer = "Intel", Price = 399.99,
                    Category = "CPUs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Processor Speed", "5.0 GHz"},
                            {"Processor Socket", "LGA 1151"},
                            {"TDP", "95W"},
                            {"Cache", "12MB"}
                        },
                    ImageUrl = "images/items/IntelCorei9-9900K",
                    Description="Intel Core i9-9900K Desktop Processor 8 Cores up to 5.0 GHz Turbo unlocked LGA1151 300 Series 95W"

                },
                new Item
                {
                    Title = "AOC C24G1A 24 Curved Frameless Gaming Monitor, FHD 1920x1080", Manufacturer = "AOC", Price = 399.99,
                    Category = "Monitors", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Display Size", "24 Inches"},
                            {"Resolution", "FHD 1920x1080"},
                            {"Hardware Interface", "VGA, HDMI"},
                            {"Display Technology", "LED"}
                        },
                    ImageUrl = "images/items/AOC_black_monitor",
                    Description="AOC CQ32G1 31.5 Curved Frameless Gaming Monitor, Quad HD 2560x1440 ,4 ms Response Time, 144Hz, FreeSync, DisplayPort/HDMI/VGA, VESA, Black"


                },

                new Item
                {
                    Title = "Hbada Gaming Chair Racing", Manufacturer = "Hbada", Price = 159.99,
                    Category = "GamingChairs", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Chair Style", "Gaming chair"},
                            {"Headrest Features", "Padded"},
                            {"Swivel Angle", "360 degrees"},
                            {"Color", "Black"}
                        },
                    ImageUrl = "images/items/Hbada_Gaiming_Chair_Style",
                    Description="High Back Computer Chair with Height Adjustment, Headrest and Lumbar Support E-Sports Swivel Chair"


                },

                new Item
                {
                    Title = "PHILIPS RGB Wired Gaming Mouse", Manufacturer = "PHILIPS", Price = 29.99,
                    Category = "Mouses", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wired"},
                            {"Weight", "56g"},
                            {"Water Resistant", "No"}
                        },
                    ImageUrl = "images/items/PHILIPS_Gaming_MICE",
                    Description="Gaming Mouse, 7 Programmable Buttons, Adjustable DPI, Comfortable Grip Ergonomic Optical PC Computer Gamer Mice"



                },
                new Item
                {
                    Title = "Corsair Harpoon PRO", Manufacturer = "Corsair", Price = 49.99,
                    Category = "Mouses", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Mouse Shape", "Right-Handed"},
                            {"Connection Type", "Wired"},
                            {"Weight", "85g"},
                            {"Water Resistant", "yes"}
                        },
                    ImageUrl = "images/items/Corsair_mice",
                    Description="RGB Gaming Mouse - Lightweight Design - 12,000 DPI Optical Sensor"


                },
                new Item
                {
                    Title = "AORUS Gaming Monitor 240Hz 1080P Adaptive Sync",
                    Manufacturer = "AORUS",
                    Price = 429.99,
                    Category = "Monitors",
                    ImageUrl = "images/items/AORUSFI25F24.5240Hz1080PAdaptiveSyncGami",
                    Description="FHD WITH 240HZ Supports Adaptive-Sync (FreeSync Premium) Technology 24.5” FHD panel (1920 x 1080 resolution)"
                },
                new Item
                {
                    Title = "Asus Gaming Monitor Full HD 1920 x 1080",
                    Manufacturer = "Asus",
                    Price = 344.99,
                    Category = "Monitors",
                    ImageUrl = "images/items/ASUSTUFGamingVG279QM27FullHD1920x10801ms",
                    Description="1 ms 280Hz (Overclocking)"
                },
                new Item
                {
                    Title = "Acer Gaming Monitor 27 inch QHD 2560 x 1440",
                    Manufacturer = "Acer",
                    Price = 211.99,
                    Category = "Monitors",
                    ImageUrl = "images/items/AcerKA272UbiipxUM.HX2AA.00427QHD2560x144",
                    Description="DisplayPort AMD RADEON FreeSync Technology Gaming Monitor"
                },
                new Item
                {
                    Title = "MSI Gaming Monitor 3440 x 1440 UWHD 144Hz",
                    Manufacturer = "MSI",
                    Price = 689.99,
                    Category = "Monitors",
                    ImageUrl = "images/items/MSIOptixMPG341CQR343440x1440UWHD144Hz1ms",
                    Description="1ms 2xHDMI DisplayPort USB Type-C AMD FreeSync HDR"
                },
                new Item
                {
                    Title = "Samsung Gaming Monitor 3840 x 1080 1ms 144Hz",
                    Manufacturer = "Samsung",
                    Price = 789.99,
                    Category = "Monitors",
                    ImageUrl = "images/items/SamsungCHG90SeriesC49HG90493840x10801ms1",
                    Description="2x HDMI DisplayPort Mini-DisplayPort HDR AMD FreeSync USB"
                },
                new Item
                {
                    Title = "MSI GeForce GTX 1660 Ti",
                    Manufacturer = "MSI",
                    Price = 242.99,
                    Category = "GPUs",
                    ImageUrl = "images/items/MSIGeForceGTX1660TiDirectX12GTX1660TIVEN",
                    Description="DirectX 12 GTX 1660 TI VENTUS XS 6G OC 6GB 192-Bit GDDR6"

                },
                new Item
                {
                    Title = "GIGABYTE Radeon RX 5700 ",
                    Manufacturer = "GIGABYTE",
                    Price = 368.99,
                    Category = "GPUs",
                    ImageUrl = "images/items/GIGABYTERadeonRX5700XTDirectX12GV-R57XTG",
                    Description="Powered by AMD Radeon 8GB 256-Bit GDDR6"
                },
                new Item
                {
                    Title = "MSI GeForce GTX 1050 Ti",
                    Manufacturer = "MSI",
                    Price = 138.65,
                    Category = "GPUs",
                    ImageUrl = "images/items/MSIGeForceGTX1050TiDirectX12GTX1050TiGAM",
                    Description="GAMING X 4G 4GB 128-Bit GDDR5"
                },
                new Item
                {
                    Title = "Intel Core i9-10900K",
                    Manufacturer = "Intel",
                    Price = 484.87,
                    Category = "CPUs",
                    ImageUrl = "images/items/IntelCorei9-10900K10-Core3.7GHzLGA120012",
                    Description="10th Gen Intel Core Desktop Processor"
                },
                new Item
                {
                    Title = "Corsair Gaming Mouse M65 RGB ELITE",
                    Manufacturer = "Corsair",
                    Price = 54.89,
                    Category = "Mouses",
                    ImageUrl = "images/items/CorsairM65RGBELITETunableFPSGamingMouse,",
                    Description="gaming mouse takes your FPS gameplay to the next level"
                },
                new Item
                {
                    Title = "Glorious Glossy Black Gaming Mouse",
                    Manufacturer = "Glorious ",
                    Price = 36.99,
                    Category = "Mouses",
                    ImageUrl = "images/items/GloriousModelOMinusGOM-GBLACKGlossyBlack",
                    Description="58mm width at grip, 63mm with at back, 58mm at front, 120mm long"
                },

                new Item
                {
                    Title = "Corsair Gaming Headset HS70",
                    Manufacturer = "Corsair",
                    Price = 45.55 ,
                    Category = "GamingHeadsets",
                    ImageUrl = "images/items/CorsairHS70PROWIRELESSUSBConnectorCircum",
                    Description="Play with the freedom of up to 40ft of wireless range and up to 16 hours of battery life"
                },

                new Item
                {
                    Title = "Mad Catz Gaming Headset 7.1",
                    Manufacturer = "Mad Catz",
                    Price =41.45 ,
                    Category = "GamingHeadsets",
                    ImageUrl = "images/items/MadCatzF.R.E.Q.4USBConnectorCircumauralG",
                    Description="Fully immerse yourself in your game with dynamic, full-range sound pumped from super-sized 50mm neodymium drivers. Its 7.1 virtual sound lets you hear directional movement to gain a competitive edge, and its noise-canceling mic delivers crystal-clear audio without annoying background noise. This USB gaming headset also features brilliant RGB lighting, ergonomic comfort, and easy-access inline control for volume adjustment and mic mute."
                },

                new Item
                {
                    Title ="Corsair Gaming Headset",
                    Manufacturer = "Corsair",
                    Price =49.55 ,
                    Category = "GamingHeadsets",
                    ImageUrl = "images/items/CorsairGamingVOIDPRORGBWirelessPremiumGa",
                    Description="Maximum audio performance with DTS Headphone: X 7.1 Surround for "
                },

                new Item
                {
                    Title = "Logitech Gaming Headset ",
                    Manufacturer = "Logitech",
                    Price = 39.89,
                    Category = "GamingHeadsets",
                    ImageUrl = "images/items/Logitech981-000681G4337.1WiredGamingHead",
                    Description="Extremely lightweight for maximum comfort"
                },

                new Item
                {
                    Title = "Razer Gaming Keyboard",
                    Manufacturer = "Razer",
                    Price =220.45 ,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerHuntsmanEliteOpto-MechanicalSwitch",
                    Description="The New Razer Opto-Mechanical Switch - Actuation at the speed of light"
                },

                new Item
                {
                    Title = "Razer Pokemon Pikachu Edition Gaming Keyboard",
                    Manufacturer = "Razer",
                    Price =199.99,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerPokemonPikachuEditionGamingKeyboard",
                    Description="This is the special Pokemon Pikachu Edition China Exclusive by Razer."
                },

                new Item
                {
                    Title = "Razer Overwatch Razer BlackWidow Chroma Mechanical Gaming Keyboard",
                    Manufacturer = "Razer",
                    Price = 239.99,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerOverwatchRazerBlackWidowChromaMecha",
                    Description="Chroma customizable backlighting - With 16.8 Million color options,Exclusive Overwatch design"
                },

                new Item
                {
                    Title = "Razer Ornata Expert",
                    Manufacturer = "Razer ",
                    Price =119.99 ,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerOrnataExpert-RevolutionaryMecha-Mem",
                    Description="Designed from the ground up, the all-new Razer Mecha-Membrane combines the soft cushioned touch of a membrane rubber dome with the crisp tactile click of a mechanical switch"
                },

                new Item
                {
                    Title = "Razer BlackWidow X Tournament Edition Chroma",
                    Manufacturer = "Razer ",
                    Price =229.99 ,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerBlackWidowXTournamentEditionChroma-",
                    Description="Express your individuality and get the leg-up in games with Chroma backlighting and over 16.8 million color options"
                },

                new Item
                {
                    Title = "KLIM Puma - USB Gamer Headset with Mic", Manufacturer = "KLIM Puma", Price = 44.99,
                    Category = "GamingHeadsets", PropertiesList =
                        new Dictionary<string, string>()
                        {
                            {"Sound Mode", "Stereo"},
                            {"Connection Type", "Wired"},
                            {"Water Resistant", "Yes"}
                        },
                    ImageUrl = "images/items/KLIM_headset",
                    Description = "7.1 Surround Sound Audio - Integrated Vibrations - Perfect for PC and PS4 Gaming - New 2020 Version - Black"

                },
                new Item
                {
                    Title = "GeForce RTX 3090 DirectX 12 RTX",
                    Manufacturer = "MSI",
                    Price =459.99 ,
                    Category = "GPUs",
                    ImageUrl = "images/items/MSIGeForceRTX3090DirectX12RTX3090VENTUS3",
                    Description="DirectX 12 RTX 3090 VENTUS 3X 24G OC 24GB 384-Bit GDDR6X PCI Express 4.0 HDCP Ready SLI Support Video Card"
                },


                new Item
                {
                    Title = "Logitech Black Blueto Gaming Mouse",
                    Manufacturer = "Logitech",
                    Price =83.99 ,
                    Category = "Mouses",
                    ImageUrl = "images/items/LogitechMXAnywhere3910-005987BlackBlueto",
                    Description="USB-C quick charging - up to 70 days of power per full charge; up to 3 hours of power with one-minute charge"
                },
                new Item
                {
                    Title = "Razer DeathAdder RZ01 Gaming Mouse",
                    Manufacturer = "Razer",
                    Price =25.99,
                    Category = "Mouses",
                    ImageUrl = "images/items/RAZERDeathAdderRZ01-00151400-R3U1BlackWi",
                    Description="Black Wired Optical Precision Gaming Mouse - 3.5G Infrared Sensor"
                },
                new Item
                {
                    Title = "Creative Gaming Headset EV03",
                    Manufacturer = "Creative",
                    Price =99.99 ,
                    Category = "Headset",
                    ImageUrl = "images/items/CreativeSoundBlasterEVO3.5mmUSBConnector",
                    Description="Meet the Sound Blaster EVO, a headset that's amazing for audio"
                },
                new Item
                {
                    Title = "Asus Gaming Headset H5",
                    Manufacturer = "Asus",
                    Price =115.65,
                    Category = "Headset",
                    ImageUrl = "images/items/ASUSTUFGamingH53.5mmUSBConnectorCircumau",
                    Description="USB 2.0 or 3.5mm connector gaming headset with Onboard 7.1 Virtual Surround Sound"
                },
                new Item
                {
                    Title = "Creative Gaming Headset HS-720",
                    Manufacturer = "Creative",
                    Price =46.99 ,
                    Category = "Headset",
                    ImageUrl = "images/items/CreativeChatMaxHS-72051EF0410AA004USBCon",
                    Description="USB Connector USB Headset for Online Chats and PC Gaming"
                },
                new Item
                {
                    Title = "Logitech Gaming Headset H820e ",
                    Manufacturer = "Logitech",
                    Price =209.99 ,
                    Category = "Headset",
                    ImageUrl = "images/items/LogitechH820eUSBConnectorSupra-auralWire",
                    Description="USB Connector Supra-aural Wireless Headset"
                },
                new Item
                {
                    Title = "Logitech Gaming Headset G533",
                    Manufacturer = "Logitech",
                    Price =59.99,
                    Category = "Headset",
                    ImageUrl = "images/items/LogitechG533WirelessDTS7.1SurroundSoundG",
                    Description="Wireless DTS 7.1 Surround Sound"
                },
                new Item
                {
                    Title = "Razer Battlefield 4 BlackWidow Gaming Keyboard",
                    Manufacturer = "Razer",
                    Price =699.99 ,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerBattlefield4BlackWidowUltimateMecha",
                    Description="Never miss a key in the dark with backlit keys that give you the tactical edge, allowing you to launch assaults and flank your foes even under low light conditions."
                },
                new Item
                {
                    Title = "Logitech Gaming Keyboard G613",
                    Manufacturer = "Logitech",
                    Price =109.99 ,
                    Category = "Keyboards",
                    ImageUrl = "images/items/LogitechG613WirelessMechanicalGamingKeyb",
                    Description="Six programmable G-keys - Put custom macro sequences and in-app commands at your fingertips. Customize G-key profiles individually for each app"
                },
                
                new Item
                {
                    Title = "Corsair K55 Gaming Keyboard ",
                    Manufacturer = "Corsair",
                    Price = 24.99,
                    Category = "Keyboards",
                    ImageUrl = "images/items/CorsairGamingK55RGBKeyboard,BacklitRGBLE",
                    Description="6 programmable macro keys enable powerful actions, key remaps and combos"
                },
                new Item
                {
                    Title = "Razer Blackwidow Ultimate 2016 Gaming Keyboard",
                    Manufacturer = "Razer",
                    Price =389.99 ,
                    Category = "Keyboards",
                    ImageUrl = "images/items/RazerBlackwidowUltimate2016MechanicalGam",
                    Description="Individually backlit keys with Dynamic lighting effects"
                },
                new Item
                {
                    Title = "Logitech G903 Lightspeed Gaming Mouse ",
                    Manufacturer = "Logitech ",
                    Price =204.99 ,
                    Category = "Mouses",
                    ImageUrl = "images/items/LogitechG903LIGHTSPEEDWirelessGamingMous",
                    Description="Wireless Gaming Mouse with HERO 16K Sensor, 140+ Hour with Rechargeable Battery, LIGHTSYNC RGB, POWERPLAY"
                },
                new Item
                {
                    Title = "Logitech Lightspeed G604 Gaming Mouse",
                    Manufacturer = "Logitech",
                    Price =189.99 ,
                    Category = "Mouses",
                    ImageUrl = "images/items/LogitechG604LIGHTSPEEDWirelessGamingMous",
                    Description="15 Programmable Controls, Dual Wireless Connectivity Modes, and HERO 16K Sensor"
                },
                //new Item
                //{
                //    Title = "",
                //    Manufacturer = "",
                //    Price = ,
                //    Category = "",
                //    ImageUrl = "images/items/",
                //    Description=""
                //},
                //new Item
                //{
                //    Title = "",
                //    Manufacturer = "",
                //    Price = ,
                //    Category = "",
                //    ImageUrl = "",
                //    Description=""
                //},
                //new Item
                //{
                //    Title = "",
                //    Manufacturer = "",
                //    Price = ,
                //    Category = "",
                //    ImageUrl = "",
                //    Description=""
                //},
                //new Item
                //{
                //    Title = "",
                //    Manufacturer = "",
                //    Price = ,
                //    Category = "",
                //    ImageUrl = "",
                //    Description=""
                //},
                //new Item
                //{
                //    Title = "",
                //    Manufacturer = "",
                //    Price = ,
                //    Category = "",
                //    ImageUrl = "",
                //    Description=""
                //},






            };
            foreach (var item in items)
            {
                item.Manufacturer = item.Manufacturer.Trim().FirstCharToUpper();
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
            List<Store> stores = JsonConvert.DeserializeObject<List<Store>>(dataStores);

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
                        ItemId = item.Id,
                        StoreId = store.Id,
                        ItemsCount =
                            (uint)random.Next(1,
                                (int)(customersList.Count * itemsNumberMultiplier)) // customers number times 0.3
                    });
                }
            }
        }

        private static IEnumerable<Order> GenerateOrders(IEnumerable<Customer> customersList,
            IReadOnlyCollection<Item> items,
            IReadOnlyCollection<Store> storesList, out List<Payment> paymentsList)
        {
            paymentsList = new List<Payment>();
            var orderList = new List<Order>();
            var rand = new Random();
            var shopOpeningDate = new DateTime(2018, 1, 1);
            int range = (DateTime.Today - shopOpeningDate).Days;

            try
            {
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
                        paymentsList.Add(payment);
                        orderList.Add(order);
                    }
                }

                return orderList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static Store GenerateRelatedStore(Customer customer, IEnumerable<Store> stores)
        {
            try
            {
                List<Store> storeList = stores.ToList();
                List<Store> storesInCustomerCity = storeList.Where(store => store.Address.City == customer.Address.City).ToList();
                var rand = new Random();

                bool relatedStores = storesInCustomerCity.Count != 0;
                var randRelatedStoreIndex = rand.Next(relatedStores ? storesInCustomerCity.Count : storeList.Count);

                var generatedRelatedStore = relatedStores ? storesInCustomerCity[randRelatedStoreIndex]: storeList[randRelatedStoreIndex];

                return generatedRelatedStore;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static ICollection<OrderItem> GenerateOrderItems(string orderId, IEnumerable<Item> items, int numItemsOrdered, out Payment payment)
        {
            var itemsList = new List<Item>(items); // copy list in order to alter it.
            var rand = new Random();
            var orderItems = new List<OrderItem>();

            try
            {
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
                        ItemsCount = rand.Next(1, 3)
                    };

                    orderItems.Add(orderItem);
                }

                var orderSum = CalculateOrderSum(orderItems);
                var shippingCost = 0;

                payment = new Payment
                {
                    ItemsCost = orderSum,
                    PaymentMethod = (PaymentMethod)rand.Next(0, 3),
                    ShippingCost = shippingCost,
                    Paid = true,
                    Total = orderSum+ shippingCost
                };

                return orderItems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static double CalculateOrderSum(IEnumerable<OrderItem> orderItems)
        {
            return orderItems.Sum(orderItem => (orderItem.Item.Price * (double)orderItem.ItemsCount));
        }
    }
}