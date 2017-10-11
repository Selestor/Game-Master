using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    public class TurnKeeper
    {
        public int actorId;
        public int actorInitiative;
    }

    private List<MovingObject> actorList;
    public List<TurnKeeper> combatQueue;

    private int whosTurn_queueIndex;
    private int whosTurn_id = -1;

    public void SetQueueIndex(int i)
    {
        whosTurn_queueIndex = i;
    }

    public void SetWhosTurn()
    {
        whosTurn_id = combatQueue[whosTurn_queueIndex].actorId;
        GameManager.instance.SetWhosTurn(whosTurn_id);
    }

    public void EndTurn()
    {
        int howManyLeft = combatQueue.Count;
        if (whosTurn_queueIndex < howManyLeft - 1)
            whosTurn_queueIndex++;
        else whosTurn_queueIndex = 0;
        SetWhosTurn();
    }

    public void RollInitiative()
    {
        actorList = new List<MovingObject>();
        combatQueue = new List<TurnKeeper>();
        actorList = GameManager.instance.boardScript.ActorsList();

        foreach (MovingObject actor in actorList)
        {
            TurnKeeper turn = new TurnKeeper();
            turn.actorId = actor.id;
            turn.actorInitiative = actor.dex + UnityEngine.Random.Range(1, 20);
            combatQueue.Add(turn);
        }

        combatQueue.Sort((p, q) => p.actorInitiative.CompareTo(q.actorInitiative));
        combatQueue.Reverse();
    }

    public void RemoveFromQueue(int id)
    {
        TurnKeeper x = combatQueue.Find(r => r.actorId == id);
        combatQueue.Remove(x);
    }
}
