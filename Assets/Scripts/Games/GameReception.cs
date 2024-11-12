using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameReception : Game
{
    public List<Order> Orders { get; set; }

    public GameReception(ReceptionData[] data, int numorder, string name) : base(name, string.Empty)
    {
        Orders = new List<Order>();

        for (int i = 0; i < numorder; i++)
        {
            Order order = new Order();
            order.Type = OrderType.Reception;
            order.Level = i + 1;
            order.Dock = data[i].origen.name;
            order.Name = GenerateRandomName();
            var pallets = data[i].palletas;

            foreach (var palet in pallets)
            {
                ReceptionTask task = new ReceptionTask();
                task.parentOrder = order;
                var range = Random.Range(1, 10);
                task.ContainerRef = palet.pallet;
                task.Container = palet.pallet.ssc;
                task.Location = data[i].origen.name;
                task.Quantity = palet.cantidad;
                task.Stock = palet.stock;
                task.isFake = range <= 2;
                order.Tasks.Add(task);
                if (order.Tasks.Count == pallets.Length)
                {
                    task.isLast = true;
                    break;
                }

            }
            Orders.Add(order);
            GameManager.Instance.WriteLog($"Create Game: numorder: {numorder}");

        }

        string GenerateRandomName()
        {
            System.Random random = new System.Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Caracteres alfanuméricos permitidos
            string randomString = new string(Enumerable.Repeat(chars, 5) // Repite 5 veces para generar 5 caracteres
                .Select(s => s[random.Next(s.Length)]) // Selecciona un carácter aleatorio de la cadena
                .ToArray());

            return "REC_" + randomString;
        }
    }
}
