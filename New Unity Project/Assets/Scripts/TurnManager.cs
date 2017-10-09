using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    public int whosTurn;

    private List<MovingObject> actorList;
    private List<TurnKeeper> combatQueue;

    public void RollInitiative()
    {
        actorList = new List<MovingObject>();
        combatQueue = new List<TurnKeeper>();
        actorList = GameManager.instance.boardScript.ActorsList();

        foreach (MovingObject actor in actorList)
        {
            TurnKeeper turn = new TurnKeeper();
            turn.actorId = actor.ID;
            turn.actorInitiative = actor.dex + UnityEngine.Random.Range(1, 20);
            combatQueue.Add(turn);
        }

        combatQueue.Sort((p,q) => p.actorInitiative);
    }

    public class TurnKeeper
    {
        public int actorId;
        public int actorInitiative;
    }
}
