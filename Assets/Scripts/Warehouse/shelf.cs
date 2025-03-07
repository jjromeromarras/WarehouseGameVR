
using TMPro;
using UnityEngine;

public class shelf : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("UI elements")]
    public TextMeshPro level1;

    public TextMeshPro level2;

    public GameObject selectLeve1;
    public GameObject selectLeve2;

    [Header("VALUES")]
    public int aisle;
    public int logicalx;
    public string sideChar;

    void Awake()
    {
        level1.text = aisle.ToString()+sideChar + " " + logicalx.ToString() + " 1";
        level2.text = aisle.ToString()+sideChar + " " + logicalx.ToString() + " 2";
        GameManager.Instance.WriteLog($"[shelf - Awake]: level1.text : {level1.text} - level2.text : {level2.text} ");

    }

    public void SetSelectLevel(string level)
    {
        if(level1.text == level)
            selectLeve1.SetActive(true);
        else
            selectLeve2.SetActive(true);
    
    }
   
    public void UnSelectionShelf()
    {
        selectLeve2.SetActive(false);
        selectLeve1.SetActive(false);
    }
}
