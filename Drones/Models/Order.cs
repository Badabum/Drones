using System.Collections.Generic;
using System.Linq;
using Drones.Models;

namespace GHQualification.Models
{
    /// <summary>
    /// R, C - row and column indexes of order destination
    /// </summary>
    public class Order:IDrawableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int R { get; set; }
        public int C { get; set; }
        public int ProductItemsCount { get; set; }
        public List<int> Products { get; set; }

        public void RemoveProduct(int productId)
        {
            var product = Products.First(p => p == productId);
            Products.Remove(product);
        }
    }
}