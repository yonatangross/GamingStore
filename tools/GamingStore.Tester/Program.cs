using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamingStore.Contracts.ML;
using GamingStore.MachineLearning;
using GamingStore.Services.Currency;

namespace GamingStore.Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var exchangeRate = await CurrencyConvert.GetExchangeRate(new List<string> {"EUR", "GBP", "ILS"});
        }
    }
}
