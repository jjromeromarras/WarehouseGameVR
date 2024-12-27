using Assets.Scripts.Helper;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Order
{
    public string Name { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public List<Task> Tasks { get; set; }
    public OrderType Type { get; set; }
    public string Dock {  get; set; }
    public GameObject DockRef { get; set; }
    public string ContainerClient { get; set; }

    public Order()
    {
        Tasks = new List<Task>();
    }

    public decimal TotalPoints()
    {
        return Tasks.Sum(p => p.Points);
    }

    public decimal TotalErrors()
    {
        return Tasks.Sum(p => p.errors);
    }

    public decimal TotalTime()
    {
        return Tasks.Sum(p => p.totalTime);
    }
}
