using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PAthfinding : MonoBehaviour
{
    [SerializeField] Grid _grid = default;

    [SerializeField] Transform _start = default, _goal = default;

    [SerializeField] bool _manhatten;

     List<Node> _finalPath = new List<Node>();

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        if(_grid.GetGrid == null)
        {
            Debug.LogError("No grid found!");
            return;
        }
        FindPath(_start.position, _goal.position);
    }
    private void Update()
    {
        FindPath(_start.position, _goal.position);
    }
    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        //check each surrounding node from a starting point
        //compare the nodes f cost
        //go with the node that has the smallest f cost
        Node startNode = _grid.NodeFromWorldPosition(startPos);
        Node endNode = _grid.NodeFromWorldPosition(endPos);

        List<Node> openList = new List<Node>();
        HashSet<Node> closeList = new HashSet<Node>();
        foreach (Node n in _grid.GetGrid)
        {
            n.ResetCosts();
        }
        openList.Add(startNode);//add the starting node to openList
        int listrunIndex = 0;
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if(openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost) //compare the costs
                {
                    if(openList[i].HCost < currentNode.HCost)
                    {
                        currentNode = openList[i];
                    }
                }
            }
            openList.Remove(currentNode);
            closeList.Add(currentNode);
            if(currentNode == endNode) //alternatively check if positions are the same?
            {
                GetFinalPath(startNode, endNode);
                return;
            }
            List<Node> neighbourNodes = _manhatten ? _grid.ManhattenNeighbours(currentNode) : _grid.GetNeighbours(currentNode);

                foreach (Node neighbourNode in neighbourNodes)
                {
                    if (!neighbourNode.Walkeable || closeList.Contains(neighbourNode))
                    {
                        continue;
                    }

                    int newMovCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode);
                    if (newMovCostToNeighbour < neighbourNode.GCost || !openList.Contains(neighbourNode))
                    {
                        neighbourNode.SetGCost(newMovCostToNeighbour);
                        neighbourNode.SetHCost(GetDistance(neighbourNode, endNode));
                        neighbourNode.SetParent(currentNode);
                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
        }
    }
    void GetFinalPath(Node startNode, Node endNode)
    {
        //List<Node> finalPath = new List<Node>();
        //Node currentNode = endNode;
        //while(currentNode != startNode)
        //{
        //    finalPath.Add(currentNode);
        //    currentNode = currentNode.Parent;
        //}
        //finalPath.Reverse();
        //this.finalPath = finalPath;
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        this._finalPath = path;
        _grid.SetPath(_finalPath);
    }
    //List<Node> NeighbourNodes(Node neighbourNode)
    //{
    //    List<Node> neighbouringNode = new List<Node>();
    //    int xCheck;
    //    int yCheck;

    //    //right side
    //    xCheck = neighbourNode.PosX + 1;
    //    yCheck = neighbourNode.PosY;
    //    if(xCheck >= 0 && xCheck < gridSizeX)
    //    {
    //        if(yCheck >= 0 && yCheck < gridSizeY)
    //        {
    //            neighbouringNode.Add(grid[xCheck, yCheck]);
    //        }
    //    }
    //    //left side
    //    xCheck = neighbourNode.PosX - 1;
    //    yCheck = neighbourNode.PosY;
    //    if (xCheck >= 0 && xCheck < gridSizeX)
    //    {
    //        if (yCheck >= 0 && yCheck < gridSizeY)
    //        {
    //            neighbouringNode.Add(grid[xCheck, yCheck]);
    //        }
    //    }
    //    //top side
    //    xCheck = neighbourNode.PosX;
    //    yCheck = neighbourNode.PosY + 1;
    //    if (xCheck >= 0 && xCheck < gridSizeX)
    //    {
    //        if (yCheck >= 0 && yCheck < gridSizeY)
    //        {
    //            neighbouringNode.Add(grid[xCheck, yCheck]);
    //        }
    //    }
    //    //bottom side
    //    xCheck = neighbourNode.PosX;
    //    yCheck = neighbourNode.PosY - 1;
    //    if (xCheck >= 0 && xCheck < gridSizeX)
    //    {
    //        if (yCheck >= 0 && yCheck < gridSizeY)
    //        {
    //            neighbouringNode.Add(grid[xCheck, yCheck]);
    //        }
    //    }
    //    return neighbouringNode;
    //}
        
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
        if(dstX > dstY)
        {
            return 14*dstY+10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

