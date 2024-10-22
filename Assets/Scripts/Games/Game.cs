using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game 
{
    // Start is called before the first frame update
    public string Name { get; set; }
    public string Description { get; set; }

    public Game(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
