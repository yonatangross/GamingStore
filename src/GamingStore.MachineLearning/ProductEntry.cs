using System;
using Microsoft.ML.Data;

namespace GamingStore.MachineLearning
{
    public class ProductEntry
    {
        [KeyType(count: 262111)]
        public int CustomerNumber { get; set; }

        [KeyType(count: 262111)]
        public int RelatedItemId { get; set; }
    }
}
