using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public BoardManager boardScript;

    [HideInInspector]
    public bool playersTurn = true;
    public Vector3 mousePosition;
    public Vector3 playerPosition;
    public List<Vector3> shortestPath;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();

        boardScript.SetupScene();
	}

    public void GameOver()
    {
        enabled = false;
    }
}
