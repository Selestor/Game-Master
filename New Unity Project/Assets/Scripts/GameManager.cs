using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void GameOver()
    {
        enabled = false;
    }
}
