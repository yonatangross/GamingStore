using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GamingStore.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GamingStore.Services.Currency
{
    public class CurrencyConvert
    {
        public string date { get; set; }

        public string @base { get; set; }

        public Currency rates { get; set; }

        /// <summary>
        /// Get currencies converts from USD
        /// </summary>
        /// <param name="symbolsList">EUR,GBP,ILS</param>
        /// <param name="apiKey">default</param>
        public static async Task<List<CurrencyInfo>> GetExchangeRate(List<string> symbolsList, string apiKey = "e90d90b4693847a38101d799814c6ecd")
        {
            string symbols = string.Join(",", symbolsList.ToArray());

            var client = new RestClient($@"https://api.currencyfreaks.com/latest?apikey={apiKey}&symbols={symbols}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);

            var exchangeRate = JsonConvert.DeserializeObject<CurrencyConvert>(response.Content);

            return new List<CurrencyInfo>
            {
                new CurrencyInfo
                {
                    Currency = "ILS",
                    Value = double.Parse(exchangeRate.rates.ILS),
                    Symbol = "₪"
                },
                new CurrencyInfo
                {
                    Currency = "EUR",
                    Value = double.Parse(exchangeRate.rates.EUR),
                    Symbol = "€"
                },
                new CurrencyInfo
                {
                    Currency = "GBP",
                    Value = double.Parse(exchangeRate.rates.GBP),
                    Symbol = "£"
                }
            };
        }
    }
}