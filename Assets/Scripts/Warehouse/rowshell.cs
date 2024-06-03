using System.Collections.Generic;
using UnityEngine;

public class rowshell : MonoBehaviour
{
    public IEnumerable<shelf> shelfs;
    public int minNumSSCC;
    public int maxNumSSCC;

    public void Awake()
    {
        IEnumerable<pallet> pallets = GetComponentsInChildren<pallet>();
        // Creamos una lista para almacenar los valores asignados
        HashSet<int> assignedValues = new HashSet<int>();

        foreach (var item in pallets)
        {
            int randomValue = GetUniqueRandomValue(minNumSSCC, maxNumSSCC, assignedValues);
            item.ssc = randomValue.ToString().PadLeft(4, '0');
        }

    }



    int GetUniqueRandomValue(int min, int max, HashSet<int> assignedValues)
    {
        int randomValue = UnityEngine.Random.Range(min, max + 1);
        while (assignedValues.Contains(randomValue))
        {
            randomValue = UnityEngine.Random.Range(min, max + 1);
        }
        assignedValues.Add(randomValue);
        return randomValue;
    }

}
