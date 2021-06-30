using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Formatting = System.Xml.Formatting;

namespace GamingStore.ProductParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var asin = "N82E16814932329";
            var filesDirectory = @"\\Mac\Home\Desktop\Project\GamingStore\src\GamingStore\wwwroot\images\items";

            var webDriver = new ChromeDriver();
            webDriver.Navigate().GoToUrl($"https://www.newegg.com/global/il-en/p/{asin}");
            IWebElement mainElement = webDriver.FindElementByXPath("//div[@class='product-main display-flex']");

            var title = mainElement.FindElement(By.XPath(".//h1[@class='product-title']")).Text;
            var convertedTitle = title.Replace(" ", string.Empty).Replace("\\", string.Empty).Replace("/", string.Empty).Replace("\"", string.Empty).Substring(0, 40);
            var imagesFolderPath = $"{filesDirectory}\\{convertedTitle}";
            Directory.CreateDirectory(imagesFolderPath);

            try
            {
                var imagesElements = webDriver.FindElementsByXPath(".//img[contains(@src,'ProductImageCompressAll1280')]");
                var imageCounter = 0;

                foreach (IWebElement imagesElement in imagesElements)
                {
                    var imageUrl = imagesElement.GetAttribute("src");
                    using WebClient client = new WebClient();
                    client.DownloadFile(new Uri(imageUrl), $@"{imagesFolderPath}\{++imageCounter}.jpg");

                    if (imageCounter == 3)
                    {
                        break;
                    }
                }


                var description = webDriver.FindElementByXPath(".//div[@id='product-overview']").Text;
                string priceText = webDriver.FindElementByXPath(".//li[@class='price-current']").Text.Replace("₪", string.Empty);
                double price = double.Parse(priceText);

                var item = new Item()
                {
                    Title = title,
                    Description = description,
                    ImageUrl = $"/images/items/{convertedTitle}",
                    Price = price / 4
                };

                string json = JsonConvert.SerializeObject(item, (Newtonsoft.Json.Formatting)Formatting.Indented);
                File.WriteAllText($@"{imagesFolderPath}\item.txt", json);

                Process.Start("explorer.exe", imagesFolderPath);
            }
            catch (Exception e)
            {

            }
        }
    }
}
