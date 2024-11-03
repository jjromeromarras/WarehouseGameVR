using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;

    [SerializeField] public bool showayuda;
    [SerializeField] public bool showminimap;
    [SerializeField] public bool penalización;
    [SerializeField] public bool mandoxbox;
    [SerializeField] public int minlevel;
    [SerializeField] public int maxlevel;


    public Player player;
    private UnityEngine.AsyncOperation asyncLoad;
    private Logger logger;
    private void Start()
    {
        logger = FindObjectOfType<Logger>();
    }
    private void Awake()
    {
        if(GameManager.Instance == null)
        {
            player = new Player();
            GameManager.Instance = this;
            this.showayuda = true;
            this.showminimap = true;
            this.penalización = true;
            this.mandoxbox = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public IEnumerator BackMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
        yield return null;
    }

    public IEnumerator ResetLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Warehouse");
        yield return null;
    }

    public void WriteLog(string msg)
    {
        logger.Log(msg);
    }
}
