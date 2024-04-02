using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class picking : MonoBehaviour
{
    public List<containerpicking> containers;
    // Start is called before the first frame update
   
    public void setContainer(int container, string stock, int cantidad)
    {
        if(containers != null && containers.Count>container)
        {
            containers[container - 1].SetStock(stock, cantidad);
        }
    }
}
