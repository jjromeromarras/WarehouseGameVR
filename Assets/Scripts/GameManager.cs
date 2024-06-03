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
    [SerializeField] public int minlevel;


    public Player player;
    private UnityEngine.AsyncOperation asyncLoad;

    private void Awake()
    {
        if(GameManager.Instance == null)
        {
            player = new Player();
            GameManager.Instance = this;
            this.showayuda = true;
            this.showminimap = true;
            this.penalización = true;
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
}
