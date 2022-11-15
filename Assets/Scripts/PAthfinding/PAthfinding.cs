using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PAthfinding : MonoBehaviour
{
    [SerializeField] Grid _grid = default;

    [SerializeField] Transform _start = default, _goal = default;

    [SerializeField] public int _fontSize;

    public List<Node> finalPath = new List<Node>();

    public bool drawGizmos;
    public bool showGCost, showHCost, showFCost;
    public bool showLine;

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
    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        //check each surrounding node from a starting point
        //compare the nodes f cost
        //go with the node that has the smallest f cost
        Node startNode = _grid.NodeFromWorldPosition(startPos);
        Node endNode = _grid.NodeFromWorldPosition(endPos);

        List<Node> openList = new List<Node>();
        HashSet<Node> closeList = new HashSet<Node>();
        //startNode.SetGCost(0); 
        openList.Add(startNode);//add the starting node to openList
        int listrunIndex = 0;
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                //int gCost = openList[i].GCost;
                //Debug.Log("Current node's gCost=" + openList[i].GCost);
                //int hCost = openList[i].HCost;
                if(openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost) //compare the costs
                {
                    if(openList[i].HCost < currentNode.HCost)
                    {
                        currentNode = openList[i];
                    }
                    //currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closeList.Add(currentNode);
            if(currentNode == endNode) //alternatively check if positions are the same?
            {
                GetFinalPath(startNode, endNode);
                return;
            }
            foreach (Node neighbourNode in _grid.NeighbourNodes(currentNode))
            {
                if (!neighbourNode.Walkeable || closeList.Contains(neighbourNode))
                {
                    continue;
                }
                //Vector2 difference = currentNode.Position - neighbourNode.Position;
                //float xF = difference.x;
                //float yF = difference.y;
                //xF = Mathf.Abs(xF);
                //int xPoint = Mathf.FloorToInt(xF);
                //yF = Mathf.Abs(yF);
                //int yPoint = Mathf.FloorToInt(yF);
                //int neighBourCost = xPoint + yPoint;
                //int moveCost = currentNode.GCost + neighBourCost;
                //neighbourNode.SetGCost(neighBourCost);

                //Debug.Log("Movecost: " + moveCost);

                //if(moveCost < neighbourNode.GCost) //NEED TO SET THE GCOST BEFORE THIS?
                //{
                //    neighbourNode.SetGCost(moveCost);
                //    Debug.Log("Set new gcost");
                //    neighbourNode.SetParent(currentNode);

                //    if (!openList.Contains(neighbourNode))
                //    {
                //        openList.Add(neighbourNode);
                //    }
                //}
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
            listrunIndex++;
        }
        Debug.Log("List run: " + listrunIndex + " times");
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
        this.finalPath = path;
        _grid.SetPath(finalPath);
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
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector2(size.x, size.y));

        //if(grid == null)
        //{
        //    return;
        //}
        //if (drawGizmos)
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.fontSize = fontSize;
        //    Gizmos.color = Color.white;
        //    //Node startNode = NodeFromWorldPosition(start.position);
        //    //Node endNode = NodeFromWorldPosition(goal.position);
        //    foreach (Node node in grid)
        //    {
        //        Debug.Log("Draw cubes");

        //        Gizmos.DrawWireCube(node.Position, Vector3.one * nodeRadius);
        //        if (showGCost)
        //        {
        //            style.normal.textColor = Color.red;
        //            Handles.Label(node.Position + (new Vector2(-.5f, .5f) * nodeRadius), "G: " + node.GCost.ToString(), style);
        //        }
        //        if (showHCost)
        //        {
        //            style.normal.textColor = Color.blue;
        //            Handles.Label(node.Position + (new Vector2(.3f, .5f) * nodeRadius), "H: " + node.HCost, style);
        //        }
        //        if (showFCost)
        //        {
        //            style.normal.textColor = Color.green;
        //            Handles.Label(node.Position, "F: " + node.FCost.ToString(), style);
        //        }
        //    }
        //    if (showLine && finalPath != null)
        //    {
        //        for (int i = 0; i < finalPath.Count; i++)
        //        {
        //            Gizmos.color = Color.red;
        //            Gizmos.DrawWireCube(finalPath[i].Position, Vector3.one * nodeRadius);
        //        }
        //    }
        //}
    }
}

