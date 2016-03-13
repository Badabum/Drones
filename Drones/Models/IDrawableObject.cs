using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drones.Models
{
    interface IDrawableObject
    {
         string Name { get; }
         int Id { get; set; }
         int R { get; set; }
         int C { get; set; }
    }
}
