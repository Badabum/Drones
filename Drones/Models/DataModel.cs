using System.Collections.Generic;

namespace GHQualification.Models
{
    public class DataModel
    {
        public GeneralInfo GeneralInfo{ get; set; }
        public List<Order> Orders { get; set; }
        public List<Warehouse> Warehouses { get; set; }
        public ProductsInfo ProducstInfo { get; set; }
        
    }


}
