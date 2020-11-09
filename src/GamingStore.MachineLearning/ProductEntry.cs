using System;
using Microsoft.ML.Data;

namespace GamingStore.MachineLearning
{
    public class ProductEntry
    {
        private const ulong _keyType = 10000;


        [KeyType(_keyType)]
        public uint CustomerNumber { get; set; }

        [KeyType(_keyType)]
        public uint RelatedItemId { get; set; }
    }
}
