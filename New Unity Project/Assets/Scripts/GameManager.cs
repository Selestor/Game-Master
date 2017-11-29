using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public BoardManager boardScript;
    public TurnManager turnScript;
    public WeaponManager weaponScript;
    public int puddleCost = 4;

    [HideInInspector]
    public bool isAnythingMoving = false;

    public Vector3 mousePosition;
    public Vector3 playerPosition;
    public List<Vector3> shortestPath;

    public GameObject menuPanel;
    public bool paused;

    private int whosTurn;
    public int WhosTurn()
    {
        return whosTurn;
    }
    public void SetWhosTurn(int id)
    {
        whosTurn = id;
    }

    private void Start()
    {
        Unpause();
    }

    // Use this for initialization
    void Awake () {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            FloatingTextController.Initialize();

            weaponScript = GetComponent<WeaponManager>();
            weaponScript.PopulateWeaponList();

            boardScript = GetComponent<BoardManager>();
            boardScript.SetupScene();

            turnScript = GetComponent<TurnManager>();
            turnScript.RollInitiative();
            turnScript.SetQueueIndex(0);
            turnScript.SetWhosTurn();

            whosTurn = WhosTurn();
        }
	}

    public void GameOver()
    {
        Pause();
    }

    public void Quit()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        DetectMousePosition();
        if (Input.GetKeyDown("escape"))
        {
            if (paused == true)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0.0f;
        menuPanel.SetActive(true);
        paused = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
        menuPanel.SetActive(false);
        paused = false;
    }

    private void DetectMousePosition()
    {
        Vector3 camPosition = Camera.main.transform.position;

        Vector3 mp = Input.mousePosition;
        mp.z = -10;
        mousePosition = Camera.main.ScreenToWorldPoint(mp);
        
        mousePosition.x = Mathf.Round(mousePosition.x);
        mousePosition.y = Mathf.Round(mousePosition.y);
        mousePosition.z = 0;
    }
}
