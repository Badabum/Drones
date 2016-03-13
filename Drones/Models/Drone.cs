using System;
using System.Collections.Generic;
using Drones.Common;
using GHQualification.Models;

namespace Drones.Models
{
    public class Drone:IDrawableObject
    {
        public int Id { get; set; }
        private int _currentWeight;
        public int MaxWeight { get; set; }
        public int Turns { get; set; }
        public int R { get; set; }
        public int C { get; set; }

        public string Name => $"d{Id}";

        public bool HasFinishedCommand()
        {
            return Turns == 0;
        }

        public bool IsCommandQueueEmpty()
        {
            return Commands.Count == 0;
        }
        public int GetTurns(int destinationR, int destinationC)
        {
            return MathFunctions.CalculateDistance(this.R, this.C, destinationR, destinationC) + 1;
        }

        public void AddLoadCommand(KeyValuePair<OrderDto,Point> product,List<Warehouse> warehouses )
        {
            var warehouse = GetNearestWarehouse(R, C, product.Key.ProductItem, warehouses);
            Commands.Enqueue(new Command()
                            {
                                //CommandString = $"{drone.Id} L {warehouseId} {product.Key.ProductItem} 1",
                                Turns = GetTurns(warehouse.R, warehouse.C),
                                R = warehouse.R,
                                C = warehouse.C
                            });
            warehouse.Products[product.Key.ProductItem]--;
        }

        public void AddDeliveryCommand(KeyValuePair<OrderDto, Point> product, List<Order> orders)
        {
            var order = orders[product.Key.OrderId];
            Commands.Enqueue(new Command()
            {
                //CommandString = $"{Id} D {product.Key.OrderId} {product.Key.ProductItem} 1",
                Turns = GetTurns(order.R, order.C),
                R = product.Value.R,
                C = product.Value.C
            });
        }
        public Queue<Command> Commands { get; set; }
        public Warehouse GetNearestWarehouse(int r, int c, int productTypeId, List<Warehouse> warehouses)
        {
            var minDistance = 999999999;
            var warehouseId = 0;
            for (var i = 0; i < warehouses.Count; i++)
            {
                if ((warehouses[i].Products[productTypeId]) > 0)
                {
                    var tmp = MathFunctions.CalculateDistance(r, c, warehouses[i].R, warehouses[i].C);
                    if (tmp >= minDistance) continue;
                    minDistance = tmp;
                    warehouseId = i;
                }
            }
            return warehouses[warehouseId];
        }
        public int CurrentWeight
        {
            get {
                return _currentWeight;
            }
            set {
                if (value >= MaxWeight)
                {
                    _currentWeight = MaxWeight;
                    //Free = true;
                }
            }
        }
        
    }
}