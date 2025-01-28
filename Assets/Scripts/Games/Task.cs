public class Task 
{
    public string Location { get; set; }
    public string Container { get; set; }
    public pallet ContainerRef { get; set; }
    public shelf LocationRef { get; set; }      

    public int Points { get; set; }
    public int errors { get; set; }
    public decimal totalTime { get; set; }

    public bool locationScan { get; set; } = false;
    public bool containerScan { get; set; } = false;

    public Order parentOrder { get; set; }

    public bool isLast { get; set; }

    public bool isMulti { get; set; }

}
