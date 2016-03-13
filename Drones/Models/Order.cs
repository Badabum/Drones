using System.Collections.Generic;
using Drones.Models;

namespace GHQualification.Models
{
    /// <summary>
    /// R, C - row and column indexes of order destination
    /// </summary>
    public class Order:IDrawableObject
    {
        public int R { get; set; }
        public int C { get; set; }
        public int ProductItemsCount { get; set; }
        public List<int> ProductTypes { get; set; }
    }
}