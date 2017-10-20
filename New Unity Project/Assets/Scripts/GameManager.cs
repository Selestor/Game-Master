using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public BoardManager boardScript;
    public TurnManager turnScript;
    public WeaponManager weaponScript;

    [HideInInspector]
    public bool playersTurn = true;
    public Vector3 mousePosition;
    public Vector3 playerPosition;
    public List<Vector3> shortestPath;

    private int whosTurn;
    public int WhosTurn()
    {
        return whosTurn;
    }
    public void SetWhosTurn(int id)
    {
        whosTurn = id;
    }

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        boardScript.SetupScene();

        turnScript = GetComponent<TurnManager>();
        turnScript.RollInitiative();
        turnScript.SetQueueIndex(0);
        turnScript.SetWhosTurn();

        whosTurn = WhosTurn();

        weaponScript = GetComponent<WeaponManager>();
	}



    public void GameOver()
    {
        enabled = false;
    }
}
