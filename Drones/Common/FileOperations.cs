using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GHQualification.Models;

namespace Drones.Common
{
    public class FileOperations
    {
        private readonly string _filePath;
        public FileOperations(string filepath)
        {
            _filePath = filepath;
        }
        public  DataModel ReadFileAsync()
        {
            using (var file = new FileStream(_filePath,FileMode.Open))
            {
                using(var streamReader = new StreamReader(file))
                {

                    var generalInfo = streamReader.ReadLine().Split(' ');
                    var genInfo = new GeneralInfo()
                    {
                        Rows = int.Parse(generalInfo[0]),
                        Columns = int.Parse(generalInfo[1]),
                        Drones = int.Parse(generalInfo[2]),
                        Steps = int.Parse(generalInfo[3]),
                        MaxWeight = int.Parse(generalInfo[4]),

                    };
                    var prodCount = int.Parse(streamReader.ReadLine());
                    var productsInfo = new ProductsInfo()
                    {
                        TypesCount = prodCount
                    };

                    var productsWeighs = streamReader.ReadLine().Split(' ');
                    var productsAndWeights = new Dictionary<int, int>();
                    int i = 0;
                    foreach(var weight in productsWeighs)
                    {
                        var tmp = int.Parse(weight);

                        productsAndWeights.Add(i, tmp);
                        i++;
                    }

                    productsInfo.ProductsAndWeights = productsAndWeights;
                    genInfo.Warehouses = int.Parse(streamReader.ReadLine());
                    var wareHouses = new List<Warehouse>(genInfo.Warehouses);
                    for (int j = 0; j < genInfo.Warehouses; j++)
                    {
                        var wareHouse = new Warehouse();
                        var coords = streamReader.ReadLine().Split(' ');
                        wareHouse.R = int.Parse(coords[0]);
                        wareHouse.C = int.Parse(coords[1]);
                        var products = streamReader.ReadLine().Split(' ').Select(p=>int.Parse(p));
                        wareHouse.Products = products.ToList();
                        wareHouse.Id = j;
                        wareHouses.Add(wareHouse);
                        
                    }
                    genInfo.Orders = int.Parse(streamReader.ReadLine());
                    var orders = new List<Order>(genInfo.Orders);
                    for(var k = 0; k < genInfo.Orders; k++)
                    {
                        var order = new Order();
                        var coords = streamReader.ReadLine().Split(' ');
                        order.R = int.Parse(coords[0]);
                        order.C = int.Parse(coords[1]);
                        order.ProductItemsCount = int.Parse(streamReader.ReadLine());
                        order.Products = streamReader.ReadLine().Split(' ').Select(p => int.Parse(p)).ToList();
                        order.Id = k;
                        orders.Add(order);
                        Console.WriteLine(k);
                    }
                    var dataModel = new DataModel()
                    {
                        Orders = orders,
                        Warehouses = wareHouses,
                        GeneralInfo = genInfo,
                        ProducstInfo = productsInfo
                    };
                    return dataModel;
                }

            }
        }
        public void Write(List<string> output)
        {
            using (var file = new FileStream(_filePath.Replace(".in",".out"), FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(file))
                {
                    streamWriter.WriteLine(output.Count);
                    foreach(var item in output)
                    {
                        streamWriter.WriteLine(item);
                    }
                }
            }
        }
    }
}