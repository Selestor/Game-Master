﻿using System;
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
    private TreeNode treeRoot;

    class TreeNode
    {
        public Vector3 node;

        private TreeNode parent;
        private List<TreeNode> children;

        public TreeNode()
        {
            parent = null;
            node = new Vector3(-9999.0f, -9999.0f, -9999.0f);
            children = new List<TreeNode>();
        }

        public void SetParent(TreeNode parent)
        {
            this.parent = parent;
        }

        public TreeNode GetParent()
        {
            return this.parent;
        }

        public void RemoveChild(TreeNode child)
        {
            TreeNode childToRemove = FindChild(child.node);
            childToRemove = null;
        }

        public void ChangeParent(TreeNode root,TreeNode newParent)
        {
            TreeNode oldParent = root.FindChild(parent.node);
            oldParent.RemoveChild(this);
            parent = newParent;
        }

        public void AddChild(TreeNode child)
        {
            children.Add(child);
        }

        public TreeNode FindChild(Vector3 child)
        {
            TreeNode searchedChild = new TreeNode();

            if (this.node.Equals(child)) searchedChild = this;
            else
            {
                foreach (TreeNode nodeChild in this.children)
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

        treeRoot = new TreeNode();
        treeRoot.node = begining;

        startingPosition = begining;
        destination = end;

        openList.Add(startingPosition);

        do
        {
            CalculateShortestPath(destination);
        } while (openList.Count >= 1);

        TreeNode bottom = new TreeNode();
        bottom = treeRoot.FindChild(destination);
        if (bottom.node.x == -9999)
        {
            bottom.node.x = transform.position.x;
            bottom.node.y = transform.position.y;
            bottom.node.z = transform.position.z;
        }
        SP.Add(bottom.node);
        while (bottom.GetParent() != null)
        {
            bottom = bottom.GetParent();
            SP.Add(bottom.node);
        }
        SP = Reverse(SP);
        SP.RemoveAt(0);
        return SP;
    }

    private void CalculateShortestPath(Vector3 end)
    {
        int[] coordinates = LowestScoreIndex(pathScoring);
        int x = coordinates[0];
        int y = coordinates[1];
        Vector3 current = new Vector3(x, y);

        closedList.Add(current);
        openList.Remove(current);

        if (closedList.Contains(end))
            return;

        List<Vector3> walkableAdjectentSquares = new List<Vector3>();
        walkableAdjectentSquares = AdjecentSquares(current);

        foreach (Vector3 walkableSqare in walkableAdjectentSquares)
        {
            if (closedList.Contains(walkableSqare))
                continue;
            if(!openList.Contains(walkableSqare))
            {
                TreeNode node = new TreeNode();
                node.node = walkableSqare;
                node.SetParent(treeRoot.FindChild(current));
                treeRoot.FindChild(current).AddChild(node);

                pathScoring[Mathf.RoundToInt(walkableSqare.x), Mathf.RoundToInt(walkableSqare.y)]
                    = CalculatePathScore(walkableSqare, current);

                openList.Insert(0, walkableSqare);
            }
            else
            {
                Vector3 newScore = CalculatePathScore(walkableSqare, current);

                float newG = newScore.y;
                float oldH = pathScoring[Mathf.RoundToInt(walkableSqare.x), 
                    Mathf.RoundToInt(walkableSqare.y)].z;
                float oldF = pathScoring[Mathf.RoundToInt(walkableSqare.x), 
                    Mathf.RoundToInt(walkableSqare.y)].x;

                float newF = newG + oldH;
                if (newF < oldF)
                {
                    treeRoot.FindChild(walkableSqare).SetParent(treeRoot.FindChild(current));
                }
            }
        }
    }

    private List<Vector3> AdjecentSquares(Vector3 square)
    {
        List<Vector3> adjecentSquares = new List<Vector3>();

        Vector3 offset;
        for (int i = 0; i < 4; i++)
        {
            offset = new Vector3(1, 0, 0);
            if (i == 1) offset = new Vector3(-1, 0, 0);
            else if (i == 2) offset = new Vector3(0, 1, 0);
            else if (i == 3) offset = new Vector3(0, -1, 0);
            Vector3 adjecent = square + offset;

            int layerMask = 1 << 8;
            RaycastHit2D hit = Physics2D.Linecast(square, adjecent, layerMask);

            if (adjecent.x >= 0 && adjecent.x < columns && adjecent.y >= 0 && adjecent.y < rows && hit.transform == null)
                adjecentSquares.Add(adjecent);
         }
         return adjecentSquares;
    }

    /*private Vector3 CalculatePathScore(TreeNode treeNode)
    {
        int xParent = Mathf.RoundToInt(treeNode.GetParent().node.x);
        int yParent = Mathf.RoundToInt(treeNode.GetParent().node.y);

        int x = Mathf.RoundToInt(treeNode.node.x);
        int y = Mathf.RoundToInt(treeNode.node.y);

        float G = pathScoring[xParent, yParent].y + 1;
        float H = Mathf.Abs(destination.x - x) + Mathf.Abs(destination.y - y); 
        float F = Mathf.RoundToInt(G + H);

        Vector3 score = new Vector3();
        score.x = F;
        score.y = G;
        score.z = H;
        return score;
    }*/

    private Vector3 CalculatePathScore(Vector3 square, Vector3 parent)
    {
        int xParent = Mathf.RoundToInt(parent.x);
        int yParent = Mathf.RoundToInt(parent.y);

        int x = Mathf.RoundToInt(square.x);
        int y = Mathf.RoundToInt(square.y);

        RaycastHit2D puddleHit = Physics2D.Linecast(square, square, 1 << LayerMask.NameToLayer("Obstacle"));

        float G = pathScoring[xParent, yParent].y;
        if (puddleHit.transform == null) G += +1;
        else G += GameManager.instance.puddleCost;

        float H = Mathf.Abs(destination.x - x) + Mathf.Abs(destination.y - y);
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

    List<Vector3> Reverse(List<Vector3> list)
    {
        List<Vector3> reversedList = new List<Vector3>();

        for (int i = list.Count - 1; i >= 0; i--)
            reversedList.Add(list[i]);

        return reversedList;
    }

    public int ReturnPathMovementCost(List<Vector3> path)
    {
        int movementCost = 0;
        Vector3 parent = new Vector3();
        parent = gameObject.transform.position;

        foreach(Vector3 square in path)
        {
            RaycastHit2D puddleHit = Physics2D.Linecast(parent, parent, 1 << LayerMask.NameToLayer("Obstacle"));
            if (puddleHit.transform == null) movementCost++;
            else movementCost += GameManager.instance.puddleCost;
            parent = square;
        }

        return movementCost;
    }
}
