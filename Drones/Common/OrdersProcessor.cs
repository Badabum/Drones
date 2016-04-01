using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Drones.Models;
using GHQualification.Models;

namespace Drones.Common
{
    public class OrdersProcessor
    {
        private DataModel _dataModel;

        //drones redrawind event
        public event Action<List<Drone>> onDronesChanged;
        //orders redrawind event
        public event Action<List<Order>> onOrdersChanged;

        //for drawind chart
        public event Action<int, int> onOrdersCountChanged;

        public event Action<int, int> onOrdersCountChangedImproved;

        public event Action<int, int> onOrdersCountChangedBest;

        public event Action<long> onSimulationFinished;

        public OrdersProcessor(DataModel dataModel)
        {
            _dataModel = dataModel;
        }

        public void SetDataModel(DataModel model)
        {
            _dataModel = model;
        }
        public void Process(int steps,int refreshStep)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var output = new List<string>();//comands
            var orders = _dataModel.Orders.Select(o => o.Clone() as Order).ToList();
            var productQueue = MakeOrderProductQueue(orders);
            var freeDronesQueue = InitDronesWithCommands(productQueue, _dataModel.GeneralInfo.Drones,
                _dataModel.GeneralInfo.MaxWeight, _dataModel.Warehouses, orders);
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

            for (var t = 0; t < steps; t++)
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
                        drone.AddDeliveryCommand(product,orders);
                        
                    }
                    
                }
                var completedOrdersCount = GetCompletedOrdersCount(orders);
                if (productQueue.Count <= 0)
                {
                    UpdateChart(onOrdersCountChanged, 0, t, refreshStep,true);
                    break;
                }
                if (completedOrdersCount > 0)
                {
                    //CallUpdateEvent(onOrdersChanged, completedOrders, 500, t);
                    UpdateChart(onOrdersCountChanged, _dataModel.Orders.Count - completedOrdersCount, t, refreshStep);
                }
                CallUpdateEvent(onDronesChanged,droneList,500,t);
                //if (t%500 == 0)
                //{
                //    onDronesChanged?.Invoke(droneList);
                //    Thread.Sleep(500);

                //}
            }
            watch.Stop();
            onSimulationFinished(watch.ElapsedMilliseconds);
        }

        public void InmprovedAlgorithm(int steps,int refreshStep)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //sort orders by count of items
            var orders = _dataModel.Orders.Select(o=>o.Clone() as Order).OrderBy(o=>o.ProductItemsCount).ToList();
      
            var output = new List<string>();//comands
            var productQueue = MakeOrderProductQueue(orders);
            var freeDronesQueue = InitDronesWithCommands(productQueue, _dataModel.GeneralInfo.Drones,
                _dataModel.GeneralInfo.MaxWeight, _dataModel.Warehouses, orders);
            var droneList = freeDronesQueue.ToList();

            //when drones are inited a command is assigned from commands pool to be executed by drone
            var count = freeDronesQueue.Count();
            for (var i = 0; i < count; i++)
            {
                var drone = freeDronesQueue.Dequeue();
                var command = drone.Commands.Dequeue();
                drone.Turns = command.Turns;
                drone.R = command.R;
                drone.C = command.C;
                output.Add(command.CommandString);
            }
            //the simulation starts after this assignment 

            for (var t = 0; t < steps; t++)
            {
                if (t == 0)
                {
                    UpdateChart(onOrdersCountChangedImproved, orders.Count, 1, 1);
                }
               
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
                for (var i = 0; i < freeDrones; i++)
                {

                    if (productQueue.Count > 0)
                    {
                        var drone = freeDronesQueue.Dequeue();
                        var product = productQueue.First();
                            productQueue.Remove(product.Key);
                            drone.AddLoadCommand(product, _dataModel.Warehouses);
                            drone.AddDeliveryCommand(product, orders);

                    }

                }

                var completedOrdersCount = GetCompletedOrdersCount(orders);
                if (completedOrdersCount > 0)
                {
                    //CallUpdateEvent(onOrdersChanged, completedOrders, 500, t);
                    UpdateChart(onOrdersCountChangedImproved, _dataModel.Orders.Count - completedOrdersCount, t, refreshStep);
                }
                if (productQueue.Count <= 0)
                {
                    UpdateChart(onOrdersCountChangedImproved, 0, t,refreshStep,true);
                    break;
                }

                //CallUpdateEvent(onDronesChanged, droneList, 500, t);
                //if (t%500 == 0)
                //{
                //    onDronesChanged?.Invoke(droneList);
                //    Thread.Sleep(500);

                //}
            }
            watch.Stop();
            onSimulationFinished(watch.ElapsedMilliseconds);


        }


        public void InmprovedAlgorithmBestOrderFinding(int steps, int refreshStep)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //sort orders by count of items
            var orders = _dataModel.Orders.Select(o => o.Clone() as Order).OrderBy(o => o.ProductItemsCount).ToList();

            var output = new List<string>();//comands
            var productQueue = MakeOrderProductQueue(orders);
            var freeDronesQueue = InitDronesWithCommands(productQueue, _dataModel.GeneralInfo.Drones,
                _dataModel.GeneralInfo.MaxWeight, _dataModel.Warehouses, orders);
            var droneList = freeDronesQueue.ToList();

            //when drones are inited a command is assigned from commands pool to be executed by drone
            var count = freeDronesQueue.Count();
            for (var i = 0; i < count; i++)
            {
                var drone = freeDronesQueue.Dequeue();
                var command = drone.Commands.Dequeue();
                drone.Turns = command.Turns;
                drone.R = command.R;
                drone.C = command.C;
                output.Add(command.CommandString);
            }
            //the simulation starts after this assignment 

            for (var t = 0; t < steps; t++)
            {
                if (t == 0)
                {
                    UpdateChart(onOrdersCountChangedBest, orders.Count, 1, 1);
                }

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
                for (var i = 0; i < freeDrones; i++)
                {

                    if (productQueue.Count > 0)
                    {

                        var drone = freeDronesQueue.Dequeue();
                        var bestOrderId = drone.GetBestOrder(productQueue);
                        var product = productQueue.First(p => p.Key.OrderId == bestOrderId);
                        productQueue.Remove(product.Key);
                        drone.AddLoadCommand(product, _dataModel.Warehouses);
                        drone.AddDeliveryCommand(product, orders);

                    }

                }

                var completedOrdersCount = GetCompletedOrdersCount(orders);
                if (completedOrdersCount > 0)
                {
                    //CallUpdateEvent(onOrdersChanged, completedOrders, 500, t);
                    UpdateChart(onOrdersCountChangedBest, _dataModel.Orders.Count - completedOrdersCount, t, refreshStep);
                }
                if (productQueue.Count <= 0)
                {
                    UpdateChart(onOrdersCountChangedBest, 0, t, refreshStep, true);
                    break;
                }

                //CallUpdateEvent(onDronesChanged, droneList, 500, t);
                //if (t%500 == 0)
                //{
                //    onDronesChanged?.Invoke(droneList);
                //    Thread.Sleep(500);

                //}
            }
            watch.Stop();
            onSimulationFinished(watch.ElapsedMilliseconds);


        }




        private void UpdateChart(Action<int, int> action, int count, int iteration, int updateInterval,bool isLastIteration=false)
        {
            if (iteration % updateInterval == 0)
            {
                action?.Invoke(count,iteration);
                //Thread.Sleep(500);
            }
            if (isLastIteration)
            {
                action?.Invoke(count, iteration);
            }
        }
        private void CallUpdateEvent<T>(Action<List<T>> action,List<T> items,  int updateInterval, int currentStep)
        {
            if (currentStep % updateInterval == 0)
            {
                action?.Invoke(items);
                Thread.Sleep(500);
            }
        }

        private int GetCompletedOrdersCount(List<Order> allOrders)
        {
            var completedOrdersCount = allOrders.Count(order => order.Completed());
            return completedOrdersCount;
        }
        
        private Queue<Drone> InitDronesWithCommands(Dictionary<OrderDto,Point> productQueue,int dronesNumber, int droneMaxWeight, List<Warehouse> warehouses, List<Order> orders)
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

        private Dictionary<OrderDto, Point> MakeOrderProductQueue(List<Order> orders )
        {
            
            var orderId = 0;
            var productQueue = new Dictionary<OrderDto, Point>();
            foreach (var order in orders)
            {

                foreach (var item in order.Products)
                {
                    productQueue.Add(new OrderDto() { OrderId = order.Id, ProductItem = item,Order = order}, new Point { R = order.R, C = order.C });
                }
                orderId++;
            }
            return productQueue;
        }

        private Drone SetDroneCommands(Drone drone,List<Warehouse> warehouses,KeyValuePair<OrderDto,Point> product,List<Order> orders)
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
