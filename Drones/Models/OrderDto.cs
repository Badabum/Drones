using GHQualification.Models;

namespace Drones.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int ProductItem { get; set; }
        public Order Order { get; set; }
    }
}