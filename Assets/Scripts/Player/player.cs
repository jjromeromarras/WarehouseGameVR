using System.Collections.Generic;

public class Player
{
    public long Score { get; set; }
    public Dictionary<int, datalevels> Data { get; set; }
    public int Level { get; set; }

    public Player()
    {
        Data = new Dictionary<int, datalevels>();
        Data[0] = new datalevels();
        Data[1] = new datalevels();
        Data[2] = new datalevels();
        Data[3] = new datalevels();
        Data[4] = new datalevels();
    }

}
