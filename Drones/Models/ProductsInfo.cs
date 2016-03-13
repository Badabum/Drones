using System.Collections.Generic;

namespace GHQualification.Models
{
    public class ProductsInfo
    {
        public int TypesCount { get; set; }
        public Dictionary<int,int> ProductsAndWeights { get; set; }
    }
}