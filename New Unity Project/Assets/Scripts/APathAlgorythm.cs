using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APathAlgorythm : MonoBehaviour {
    private int columns;
    private int rows;

    private Vector3 startingPosition;
    private Vector3 destination;

    private Vector3[,] pathScoring;
    private List<Vector3> openList;
    private List<Vector3> closedList;
    private TreeNode<Vector3> treeRoot;

    class TreeNode<Vector3>
    {
        public Vector3 node;

        private TreeNode<Vector3> parent;
        private List<TreeNode<Vector3>> children;

        public TreeNode()
        {
            parent = null;
            node = default(Vector3);
            children = new List<TreeNode<Vector3>>();
        }

        public void SetParent(TreeNode<Vector3> parent)
        {
            this.parent = parent;
        }

        public TreeNode<Vector3> GetParent()
        {
            return this.parent;
        }

        public void AddChild(TreeNode<Vector3> child)
        {
            children.Add(child);
            child.parent = this;
        }

        public TreeNode<Vector3> FindChild(Vector3 child)
        {
            TreeNode<Vector3> searchedChild = new TreeNode<Vector3>();

            if (this.node.Equals(child)) searchedChild = this;
            else
            {
                foreach (TreeNode<Vector3> nodeChild in this.children)
                {
                    searchedChild = nodeChild.FindChild(child);
                    if (searchedChild.node.Equals(child)) return searchedChild;
                }
            }
            return searchedChild;
        }

    }


    public List<Vector3> ReturnShortestPath(Vector3 begining, Vector3 end)
    {
        List<Vector3> SP = new List<Vector3>();

        columns = GameManager.instance.GetComponent<BoardManager>().columns;
        rows = GameManager.instance.GetComponent<BoardManager>().rows;
        closedList = new List<Vector3>();
        openList = new List<Vector3>();
        pathScoring = new Vector3[columns, rows];

        treeRoot = new TreeNode<Vector3>();
        treeRoot.node = begining;

        startingPosition = begining;
        destination = end;

        CalculateShortestPath(begining, end);

        TreeNode<Vector3> bottom = treeRoot.FindChild(end);
        SP.Add(bottom.node);
        while (bottom.GetParent() != null)
        {
            bottom = bottom.GetParent();
            SP.Add(bottom.node);
        }

        return SP;
    }

    private void CalculateShortestPath(Vector3 begining, Vector3 end)
    {
        closedList.Add(begining);
        AddAdjecentToOpenList(begining, end);

        int[] coordinates = LowestScoreIndex(pathScoring);
        int x = coordinates[0];
        int y = coordinates[1];

        Vector3 S = new Vector3(x, y);
        openList.Remove(S);

        if (S != end) CalculateShortestPath(S, end);
        else
        {
            TreeNode<Vector3> lastNode = new TreeNode<Vector3>();
            lastNode.node = S;
            lastNode.SetParent(treeRoot.FindChild(begining));
            treeRoot.FindChild(begining).AddChild(lastNode);
            closedList.Add(S);
        }
    }

    private void AddAdjecentToOpenList(Vector3 current, Vector3 target)
    {
        Vector3 offset;
        for (int i = 0; i < 4; i++)
        {
            offset = new Vector3(1, 0, 0);
            if (i == 1) offset = new Vector3(-1, 0, 0);
            else if (i == 2) offset = new Vector3(0, 1, 0);
            else if (i == 3) offset = new Vector3(0, -1, 0); 
            Vector3 adjecent = current + offset;

            int layerMask = 1 << 8;
            RaycastHit2D hit = Physics2D.Linecast(current, adjecent, layerMask);

            if (adjecent.x >= 0 && adjecent.x < columns && adjecent.y >= 0 && adjecent.y < rows && hit.transform == null)
            {
                TreeNode<Vector3> child = new TreeNode<Vector3>();
                child.node = adjecent;
                child.SetParent(treeRoot.FindChild(current));
                treeRoot.FindChild(current).AddChild(child);

                if (!closedList.Contains(adjecent))
                {
                    Vector3 score = new Vector3();
                    score = CalculatePathScore(adjecent, current);
                    if (!openList.Contains(adjecent))
                    {
                        openList.Add(adjecent);
                        pathScoring[Mathf.RoundToInt(adjecent.x), Mathf.RoundToInt(adjecent.y)] = score;
                    }
                    else
                    {
                        float oldF = pathScoring[Mathf.RoundToInt(adjecent.x), Mathf.RoundToInt(adjecent.y)].x;
                        if (score.x < oldF)
                        {
                            openList.Remove(adjecent);
                            openList.Add(adjecent);
                            pathScoring[Mathf.RoundToInt(adjecent.x), Mathf.RoundToInt(adjecent.y)] = score;
                        }
                    }
                }
            }
        }
    }

    private Vector3 CalculatePathScore(Vector3 field, Vector3 previous)
    {
        int x = Mathf.RoundToInt(previous.x);
        int y = Mathf.RoundToInt(previous.y);

        float G = pathScoring[x, y].y + 1;
        float H = Mathf.Abs(destination.x - field.x) + Mathf.Abs(destination.y - field.y); 
        float F = Mathf.RoundToInt(G + H);
        Vector3 score = new Vector3();
        score.x = F;
        score.y = G;
        score.z = H;
        return score;
    }

    private int[] LowestScoreIndex(Vector3[,] scoreBoard)
    {
        int[] lowestScoreIndex = new int[2];
        float min = scoreBoard[Mathf.RoundToInt(openList[openList.Count - 1].x), Mathf.RoundToInt(openList[openList.Count - 1].y)].x;
        lowestScoreIndex[0] = Mathf.RoundToInt(openList[openList.Count - 1].x);
        lowestScoreIndex[1] = Mathf.RoundToInt(openList[openList.Count - 1].y);

        foreach (Vector3 field in openList)
        {
            if (scoreBoard[Mathf.RoundToInt(field.x), Mathf.RoundToInt(field.y)].x < min)
            {
                min = scoreBoard[Mathf.RoundToInt(field.x), Mathf.RoundToInt(field.y)].x;
                lowestScoreIndex[0] = Mathf.RoundToInt(field.x);
                lowestScoreIndex[1] = Mathf.RoundToInt(field.y);
            }
        }

        return lowestScoreIndex;
    }
}
