using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Juego 1. Realizar 10 tareas de picking
/// </summary>
public class Game 
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Order> Orders { get; set; }


    public Game(GameObject warehouse, int numorder, int numtareasmax, int level, OrderType type)
    { 
        Orders = new List<Order>();

        for (int i = 0; i < numorder; i++)
        {
            Order order = new Order();
            order.Type = type;
            order.Level = level;
            if (type == OrderType.Shipping)
            {
                order.Dock = "Dock_" + Random.Range(1, 5).ToString();
            } 
            else
            {
                order.Dock = "Dock_" + Random.Range(5, 9).ToString();
            }
            var shelf = warehouse.GetComponentsInChildren<shelf>();
            var shelforder = shelf.OrderBy(s => s.aisle);
            foreach (var shel in shelforder)
            {
                if (Random.Range(0, 10) > 6)
                {
                    Task task = new Task();
                    if (type == OrderType.Shipping)
                    {
                        task = new PickingTask();
                    }
                    

                    var numcontainer = Random.Range(1, 7);
                    if (numcontainer > 3)
                    {
                        task.Location = (shel as shelf).level1.text;
                    }
                    else
                    {
                        task.Location = (shel as shelf).level2.text;
                    }
                    var container = shel.transform.GetChild(numcontainer).GetComponent<pallet>();
                    if (container != null)
                    {
                        task.Container = container.ssc.ToString();
                        if (type == OrderType.Picking && task is PickingTask picking)
                        {
                            picking.Quantity = Random.Range(1, 11);
                            picking.Stock = Enum.GetValues(typeof(Stock)).GetValue(Random.Range(0, 7)).ToString();
                        }
                        order.Tasks.Add(task);
                        if (order.Tasks.Count == numtareasmax)
                        {
                            break;
                        }
                    }


                }
            }
            Orders.Add(order);
        }
    }

    public decimal TotalPoints()
    {
        return Orders.Sum(o => o.TotalPoints());
    }

    public decimal TotalErrors()
    {
        return Orders.Sum(o => o.TotalErrors());
    }
    public decimal TotalTime()
    {
        return Orders.Sum(o => o.TotalTime());
    }
}
