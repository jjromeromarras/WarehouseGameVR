using System.Collections.Generic;
using UnityEngine;

public class picking : MonoBehaviour
{
    public List<containerpicking> containers;
    // Start is called before the first frame update
   
    public void setContainer(int container, string stock, int cantidad, string ssc)
    {
        if(containers != null && containers.Count>container)
        {
            containers[container].SetStock(stock, cantidad, ssc);
        }
    }

    public void ResetSetSSC()
    {
        for(int i = 0; i < containers.Count; i++)
        {
            containers[i].SetSSCC(string.Empty);
        }
    }
}
