using System;
using System.Collections.Generic;
using System.Linq;
using Drones.Models;
using GHQualification.Models;

namespace Drones.Common
{
    public class OrdersProcessor
    {
        private readonly DataModel _dataModel;

        public OrdersProcessor(DataModel dataModel)
        {
            _dataModel = dataModel;
        }

        public void Process()
        {
            var output = new List<string>();//comands
            var productQueue = MakeOrderProductQueue(_dataModel.Orders);
            var freeDronesQueue = InitDronesWithCommands(productQueue, _dataModel.GeneralInfo.Drones,
                _dataModel.GeneralInfo.MaxWeight, _dataModel.Warehouses, _dataModel.Orders);
            var droneList = freeDronesQueue.ToList();

            //when drones are inited a command is assigned from commands pool to be executed by drone
            var count = freeDronesQueue.Count();
            for (var i=0; i<count; i++)
            {
                var drone = freeDronesQueue.Dequeue();
                var command = drone.Commands.Dequeue();
                drone.Turns = command.Turns;
                drone.R = command.R;
                drone.C = command.C;
                output.Add(command.CommandString);
            }
            //the simulation starts after this assignment 

            for (var t = 0; t < _dataModel.GeneralInfo.Steps; t++)
            {
                foreach (var drone in droneList)
                {
                    
                    if (drone.HasFinishedCommand())
                    {
                        if (drone.IsCommandQueueEmpty())
                        {
                            freeDronesQueue.Enqueue(drone);
                        }
                        else
                        {
                            var command = drone.Commands.Dequeue();
                            drone.Turns = command.Turns;
                            drone.R = command.R;
                            drone.C = command.C;
                            output.Add(command.CommandString);
                        }
                    }
                    else {
                        drone.Turns--;
                    }
                }


                //command set
                var freeDrones = freeDronesQueue.Count;
                for (var i=0; i<freeDrones;i++)
                {
                    
                    if (productQueue.Count > 0)
                    {
                        var drone = freeDronesQueue.Dequeue();
                        var product = productQueue.First();
                        productQueue.Remove(product.Key);
                        drone.AddLoadCommand(product,_dataModel.Warehouses);
                        drone.AddDeliveryCommand(product,_dataModel.Orders);
                    }
                    
                }

            }
        }
        
        public Queue<Drone> InitDronesWithCommands(Dictionary<OrderDto,Point> productQueue,int dronesNumber, int droneMaxWeight, List<Warehouse> warehouses, List<Order> orders)
        {
            var dronesQueue = new Queue<Drone>();
            for (var i = 0; i < dronesNumber; i++)
            {
                var drone = new Drone()
                {
                    Id = i,
                    R = warehouses[0].R,
                    C = warehouses[0].C,
                    MaxWeight = droneMaxWeight,
                    Turns = 0,
                    Commands = new Queue<Command>()
                };

                var product = productQueue.First();
                productQueue.Remove(product.Key);
                SetDroneCommands(drone, warehouses, product, orders);
                dronesQueue.Enqueue(drone);
            }
            return dronesQueue;
        }
        public Dictionary<OrderDto, Point> MakeOrderProductQueue(List<Order> orders )
        {
            
            var orderId = 0;
            var productQueue = new Dictionary<OrderDto, Point>();
            foreach (var order in orders)
            {

                foreach (var item in order.ProductTypes)
                {
                    productQueue.Add(new OrderDto() { OrderId = orderId, ProductItem = item }, new Point { R = order.R, C = order.C });
                }
                orderId++;
            }
            return productQueue;
        }

        public Drone SetDroneCommands(Drone drone,List<Warehouse> warehouses,KeyValuePair<OrderDto,Point> product,List<Order> orders)
        {
            var warehouse = drone.GetNearestWarehouse(drone.R, drone.C, product.Key.ProductItem, warehouses);
            var order = orders[product.Key.OrderId];
            drone
                .Commands
                .Enqueue(new Command()
                {
                    //CommandString = $"{drone.Id} L {warehouseId} {product.Key.ProductItem} 1",
                    Turns = drone.GetTurns(warehouse.R,warehouse.C),
                    R = warehouse.R,
                    C = warehouse.C
                });
            warehouse.Products[product.Key.ProductItem]--;
            drone.Commands.Enqueue(new Command()
            {
                //CommandString = $"{drone.Id} D {product.Key.OrderId} {product.Key.ProductItem} 1",
                Turns = drone.GetTurns(order.R,order.C),
                R = product.Value.R,
                C = product.Value.C
            });
            return drone;
        }
    }
}
