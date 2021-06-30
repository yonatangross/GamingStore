﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Extensions
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            input = input.ToLower();
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        }
    }
}
