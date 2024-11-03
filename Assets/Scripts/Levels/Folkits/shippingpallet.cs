using PlasticPipe.PlasticProtocol.Server.Stubs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class shippingpallet : pallet
{
    [SerializeField] public bool isStageDestiny;
    [SerializeField] public bool isShelfDestiny;
    [SerializeField] public bool isDockDestiny;
    [SerializeField] public TextMeshPro origen;
    [SerializeField] public StageTruck stageDestiny;
    [SerializeField] public ShelfFolkit shelfDestiny;
    [SerializeField] public DockFolkit dockDestiny;
    public bool finishtask;

    
}
