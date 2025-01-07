using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ShippingAutomatic : Level
{
    [SerializeField] private shippingpallet[] PS1pallets;
    [SerializeField] private shippingpallet[] PS2pallets;
    [SerializeField] private shippingpallet[] PS3pallets;
    [SerializeField] private shippingpallet[] PS4pallets;
    [SerializeField] private GameObject[] destiny;  
    [SerializeField] private ForkliftPickup forkliftPickup;
    [SerializeField] private AudioClip pickingOK, pickingFail;

    private int currentDestiny;
    private Queue<Task> tasks;
    private Task currentTask;
    private StateGame state;
    private shippingpallet contps1, contps2, contps3, contps4;
    private UnityEngine.Vector3 positionorigpallet;
    private Quaternion rotacionorigpallet;


    // Start is called before the first frame update
    void Start()
    {
        InitLevel();
        game = new Game("ShippingAutomatic", "ShippingAutomatic");
     
        tasks = new Queue<Task>();
        List<shippingpallet> allPallets = new List<shippingpallet>();
        
        allPallets.AddRange(PS1pallets);
        allPallets.AddRange(PS2pallets);
        allPallets.AddRange(PS3pallets);
        allPallets.AddRange(PS4pallets);
        // Mezcla la lista combinada aleatoriamente
        System.Random random = new System.Random();
        allPallets = allPallets.OrderBy(p => random.Next()).ToList();

        foreach (var pallet in allPallets)
        {
            pallet.finishtask = false;
            Task t = new Task();
            t.ContainerRef = pallet;
            t.Container = pallet.ssc;           
            tasks.Enqueue(t);
        }
        

        state = StateGame.ShowBienVenido;
        if (timer != null)
        {
            timer.SetTimeLeft(2400);
        }
        forkliftPickup.onUnloadPallet += onUnloadPallet;
        forkliftPickup.loadPallet += onloadPallet;

        currentTask = tasks.Dequeue();
        var destiny = GetDestiny();
        var origen = (currentTask.ContainerRef as shippingpallet).origen.text;
        if (origen == "PS 1") contps1 = (currentTask.ContainerRef as shippingpallet);
        if (origen == "PS 2") contps2 = (currentTask.ContainerRef as shippingpallet);
        if (origen == "PS 3") contps3 = (currentTask.ContainerRef as shippingpallet);
        if (origen == "PS 4") contps4 = (currentTask.ContainerRef as shippingpallet);

        // Activa el primer elemento de cada lista original si existe
        if (PS1pallets.Count() > 0 && origen != "PS 1")
        {
            contps1 = PS1pallets[0];
            PS1pallets[0].gameObject.SetActive(true);
        }
        if (PS2pallets.Count() > 0 && origen != "PS 2")
        {
            contps2 = PS2pallets[0];
            PS2pallets[0].gameObject.SetActive(true);
        }
        if (PS3pallets.Count() > 0 && origen != "PS 3")
        {
            contps3 = PS3pallets[0];
            PS3pallets[0].gameObject.SetActive(true);
        }
        if (PS4pallets.Count() > 0 && origen != "PS 4")
        {
            contps4 = PS4pallets[0];
            PS4pallets[0].gameObject.SetActive(true);
        }

        rotacionorigpallet = new Quaternion(currentTask.ContainerRef.gameObject.transform.rotation.x,
                                            currentTask.ContainerRef.gameObject.transform.rotation.y,
                                            currentTask.ContainerRef.gameObject.transform.rotation.z,
                                            currentTask.ContainerRef.gameObject.transform.rotation.w);

        positionorigpallet = new Vector3(currentTask.ContainerRef.gameObject.transform.position.x, currentTask.ContainerRef.gameObject.transform.position.y, currentTask.ContainerRef.gameObject.transform.position.z);
        rfcontroller.SetPantallaTxt("UnloadTruckTask", new object[] { (currentTask.ContainerRef as shippingpallet).origen.text, currentTask.ContainerRef.ssc, destiny, tasks.Count + 1, PS1pallets.Length + PS2pallets.Length + PS3pallets.Length + PS4pallets.Length });
      
        if (txtNivel != null)
        {
            txtNivel.SetPantallaTxt("niveldescargarcamion", new object[] { });
        }
        GameManager.Instance.WriteLog($"Iniciar game: ShippingAutomatic");
        GameManager.Instance.WriteLog($"[ShippingAutomatic] - Next Task: Pallet: {currentTask.ContainerRef.ssc} Origen: {(currentTask.ContainerRef as shippingpallet).origen.text} Destiny: {GetDestiny()}");

    }

    private string GetDestiny()
    {
        shippingpallet pallet = currentTask.ContainerRef as shippingpallet;
        if (pallet != null)
        {
            if(pallet.isDockDestiny && pallet.dockDestiny != null)
            {
                forkliftPickup.minYloader = -1;
                forkliftPickup.maxYloader = 0;
                forkliftPickup.hiddenpallet = true;
                forkliftPickup.destiny = pallet.dockDestiny.name;
                return pallet.dockDestiny.gameObject.name;
            }
            else if (pallet.isShelfDestiny && pallet.shelfDestiny != null)
            {
                forkliftPickup.minYloader = 3;
                forkliftPickup.maxYloader = 5;
                forkliftPickup.hiddenpallet = false;
                forkliftPickup.destiny = pallet.shelfDestiny.shelf.text;
                return pallet.shelfDestiny.shelf.text;
            }
            else if (pallet.isStageDestiny && pallet.stageDestiny != null)
            {
                forkliftPickup.minYloader = -1;
                forkliftPickup.maxYloader = 0;
                forkliftPickup.hiddenpallet = true;
                forkliftPickup.destiny = pallet.stageDestiny.name;
                return pallet.stageDestiny.gameObject.name;
            }
        }
        return string.Empty;
    }
    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    showTextoKey("BienvenidaAutomaticTruck");
                    break;
                }
            case StateGame.ObjetivoAutomatic:
                {
                    showTextoKey("ObjetivoAutomaticTruck");
                    break;
                }
            case StateGame.ShowErrorUnloadContainer:
                {
                    showTextoKey("ErrorContainerUnload");
                    break;
                }
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentTask != null)
        {
            currentTask.ContainerRef.gameObject.transform.rotation =
                new Quaternion(rotacionorigpallet.x, rotacionorigpallet.y, rotacionorigpallet.z, rotacionorigpallet.w);
                
            currentTask.ContainerRef.gameObject.transform.position =
                new Vector3(positionorigpallet.x, positionorigpallet.y, positionorigpallet.z);

            forkliftPickup.isCarryingPallet = false;
            forkliftPickup.isOnDestiny = false;
            forkliftPickup.isUnloadLoader = false;
            forkliftPickup.palletobj = null;
            penalizacion += 10;
            GameManager.Instance.player.Score -= 10;

        }
    }

    public override void NextStep()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    setLockPlayer(true);
                    state = StateGame.ObjetivoAutomatic;
                }
                break;
                
            case StateGame.ObjetivoAutomatic:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.LoadContainer;
                    currentTask.ContainerRef.gameObject.SetActive(true);                   
                }
                break;
            case StateGame.ShowErrorUnloadContainer:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.LoadContainer;
                    break;
                }
        }
    }

    private void onloadPallet(string container)
    {
        GameManager.Instance.WriteLog($"[ShippingAutomatic] - onloadPallet: {container}");
        // shippingpallet pallet = currentTask.ContainerRef as shippingpallet;
        // Check paleta a cargar

        //if (pallet != null)
        //{
        //    if (pallet.isDockDestiny && pallet.dockDestiny != null && isFirstDock)
        //    {
        //        pallet.dockDestiny.SetSelected(true); 
        //        isFirstDock = false;
        //    }
        //    else if (pallet.isShelfDestiny && pallet.shelfDestiny != null && isFirstShelf)
        //    {
        //        pallet.shelfDestiny.SetSelected(true);
        //        isFirstShelf = false;
        //    }
        //    else if (pallet.isStageDestiny && pallet.stageDestiny != null && isFirstStage)
        //    {
        //        pallet.stageDestiny.SetSelected(false);
        //        isFirstStage = false;
        //    }
        //}
    }

    private void onUnloadPallet(string container)
    {
        if (container == currentTask.Container)
        {
            GameManager.Instance.WriteLog($"[ShippingAutomatic] - onUnloadPallet: {container} OK");
            bonificacion += 5;
            GameManager.Instance.player.Score += 5;
            (currentTask.ContainerRef as shippingpallet).finishtask = true;
            SoundManager.SharedInstance.PlaySound(pickingOK);
            if (tasks.Count > 0)
            {
                if (contps1 != null) contps1.gameObject.SetActive(false);
                if (contps2 != null) contps2.gameObject.SetActive(false);
                if (contps3 != null) contps3.gameObject.SetActive(false);
                if (contps4 != null) contps4.gameObject.SetActive(false);

                currentTask = tasks.Dequeue();
                currentTask.ContainerRef.gameObject.SetActive(true);
                rotacionorigpallet = new Quaternion(currentTask.ContainerRef.gameObject.transform.rotation.x,
                                          currentTask.ContainerRef.gameObject.transform.rotation.y,
                                          currentTask.ContainerRef.gameObject.transform.rotation.z,
                                          currentTask.ContainerRef.gameObject.transform.rotation.w);

                positionorigpallet = new Vector3(currentTask.ContainerRef.gameObject.transform.position.x, currentTask.ContainerRef.gameObject.transform.position.y, currentTask.ContainerRef.gameObject.transform.position.z);

                var origen = (currentTask.ContainerRef as shippingpallet).origen.text;
                if (origen == "PS 1") contps1 = (currentTask.ContainerRef as shippingpallet);
                if (origen == "PS 2") contps2 = (currentTask.ContainerRef as shippingpallet);
                if (origen == "PS 3") contps3 = (currentTask.ContainerRef as shippingpallet);
                if (origen == "PS 4") contps4 = (currentTask.ContainerRef as shippingpallet);
                if (PS1pallets.Count() > 0 && origen != "PS 1")
                {
                    contps1 = PS1pallets.FirstOrDefault(p=>!p.finishtask);
                    if (contps1 != null)
                    {
                        contps1.gameObject.SetActive(true);
                    }
                }
                if (PS2pallets.Count() > 0 && origen != "PS 2")
                {
                    contps2 = PS2pallets.FirstOrDefault(p => !p.finishtask);
                    if (contps2 != null)
                    {
                        contps2.gameObject.SetActive(true);
                    }
                }
                if (PS3pallets.Count() > 0 && origen != "PS 3")
                {
                    contps3 = PS3pallets.FirstOrDefault(p => !p.finishtask);
                    if (contps3 != null)
                    {
                        contps3.gameObject.SetActive(true);
                    }
                }
                if (PS4pallets.Count() > 0 && origen != "PS 4")
                {
                    contps4 = PS4pallets.FirstOrDefault(p => !p.finishtask);
                    if (contps4 != null)
                    {
                        contps4.gameObject.SetActive(true);
                    }
                }
                if (rfcontroller != null)
                {
                    rfcontroller.SetTitle("TareasAutomaticas");
                }
                rfcontroller.SetPantallaTxt("UnloadTruckTask", new object[] { (currentTask.ContainerRef as shippingpallet).origen.text, currentTask.ContainerRef.ssc, GetDestiny(), tasks.Count + 1, PS1pallets.Length + PS2pallets.Length + PS3pallets.Length + PS4pallets.Length });
                GameManager.Instance.WriteLog($"[ShippingAutomatic] - Next Task: Pallet: {currentTask.ContainerRef.ssc} Origen: {(currentTask.ContainerRef as shippingpallet).origen.text} Destiny: {GetDestiny()}");
            }
            else
            {
                this.setFinishLevel();
            }
        }
        else
        {
            state = StateGame.ShowErrorUnloadContainer;
            SoundManager.SharedInstance.PlaySound(pickingFail);
            penalizacion += 10;
            GameManager.Instance.player.Score -= 10;
            GameManager.Instance.WriteLog($"[ShippingAutomatic] - onUnloadPallet: {container} ERROR");
        }
    }

    internal enum StateGame
    {
        ShowBienVenido,
        ObjetivoAutomatic,
        ShowErrorUnloadContainer,
        LoadContainer,
        FinishLevel
    }
}


