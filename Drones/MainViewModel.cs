using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drones.Models;
using GHQualification.Models;

namespace Drones
{
    public class MainViewModel
    {
        public ObservableCollection<Drone> Drones { get; set; }
        public ObservableCollection<Warehouse> Warehouses { get; set; }
    }
}
