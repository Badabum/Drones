using System.Collections.Generic;
using Drones.Models;

namespace GHQualification.Models
{
    /// <summary>
    /// R,C - warehouse position,
    /// Products - list of available products in warehouse
    /// </summary>
    public class Warehouse :IDrawableObject
    {
        public int R { get; set; }
        public int C { get; set; }
        public List<int> Products { get; set; }
    }
}