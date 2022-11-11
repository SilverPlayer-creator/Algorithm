using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PAthfinding : MonoBehaviour
{
    public Transform goal, start;
    public Vector2 size;
    public float nodeRadius;
    float nodeDiameter;

    public int fontSize;

    Node[,] grid;
    public List<Node> finalPath = new List<Node>();
    int gridSizeX, gridSizeY;

    public bool drawGizmos;
    public bool showGCost, showHCost, showFCost;
    public bool showLine;

    [ContextMenu("Grid")]
    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(size.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(size.y / nodeDiameter);
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 bottomLeft = (Vector2)transform.position - Vector2.right * size.x / 2 - Vector2.up * size.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPos = bottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);

                grid[x, y] = new Node(worldPos, x, y);
            }
        }
        FindPath(start.position, goal.position);
    }
    void FindPath(Vector2 startPos, Vector2 endPos)
    {
        //check each surrounding node from a starting point
        //compare the nodes f cost
        //go with the node that has the smallest f cost
        Node startNode = NodeFromWorldPosition(startPos);
        Node endNode = NodeFromWorldPosition(endPos);

        List<Node> openList = new List<Node>();
        HashSet<Node> closeList = new HashSet<Node>();

        openList.Add(startNode);
        while(openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                int gCost = openList[i].GetGCost(startPos);
                openList[i].SetGCost(gCost);
                int hCost = openList[i].getHCost(endPos);
                openList[i].SetHCost(hCost);
                if(openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closeList.Add(currentNode);
            if(currentNode.Position == endNode.Position)
            {
                GetFinalPath(startNode, endNode);
            }
            foreach (Node neighbourNode in NeighbourNodes(currentNode))
            {
                if (closeList.Contains(neighbourNode))
                {
                    continue;
                }
                int moveCost = currentNode.FCost;

                if(moveCost < neighbourNode.GCost || !openList.Contains(neighbourNode))
                {
                    //neighbourNode.SetGCost(moveCost);
                    //neighbourNode.SetHCost(GetManhattenDistance(neighbourNode, endNode));
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
        List<Node> finalPath = new List<Node>();
        Node currentNode = endNode;
        while(currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        finalPath.Reverse();
        this.finalPath = finalPath;
    }
    List<Node> NeighbourNodes(Node neighbourNode)
    {
        List<Node> neighbouringNode = new List<Node>();
        int xCheck;
        int yCheck;

        //right side
        xCheck = neighbourNode.PosX + 1;
        yCheck = neighbourNode.PosY;
        if(xCheck >= 0 && xCheck < gridSizeX)
        {
            if(yCheck >= 0 && yCheck < gridSizeY)
            {
                neighbouringNode.Add(grid[xCheck, yCheck]);
            }
        }
        //left side
        xCheck = neighbourNode.PosX - 1;
        yCheck = neighbourNode.PosY;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighbouringNode.Add(grid[xCheck, yCheck]);
            }
        }
        //top side
        xCheck = neighbourNode.PosX;
        yCheck = neighbourNode.PosY + 1;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighbouringNode.Add(grid[xCheck, yCheck]);
            }
        }
        //bottom side
        xCheck = neighbourNode.PosX;
        yCheck = neighbourNode.PosY - 1;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighbouringNode.Add(grid[xCheck, yCheck]);
            }
        }
        return neighbouringNode;
    }
        Node NodeFromWorldPosition(Vector2 worldPos)
    {
        float xPoint = ((worldPos.x * size.x / 2) / size.x);
        float yPoint = ((worldPos.y * size.y / 2) / size.y);

        xPoint = Mathf.Clamp01(xPoint);
        yPoint = Mathf.Clamp01(yPoint);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
        int y = Mathf.RoundToInt((gridSizeY-1) * yPoint);

        return grid[x, y];
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(size.x, size.y));

        if(grid == null)
        {
            return;
        }
        if (drawGizmos)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            Gizmos.color = Color.white;
            Node startNode = NodeFromWorldPosition(start.position);
            Node endNode = NodeFromWorldPosition(goal.position);
            foreach (Node node in grid)
            {
                Debug.Log("Draw cubes");

                Gizmos.DrawWireCube(node.Position, Vector3.one * nodeRadius);
                if (showGCost)
                {
                    style.normal.textColor = Color.red;
                    Handles.Label(node.Position + (new Vector2(-.5f, .5f) * nodeRadius), "G: " + node.GetGCost(start.position).ToString(), style);
                }
                if (showHCost)
                {
                    style.normal.textColor = Color.blue;
                    Handles.Label(node.Position + (new Vector2(.3f, .5f) * nodeRadius), "H: " + node.getHCost(goal.position).ToString(), style);
                }
                if (showFCost)
                {
                    style.normal.textColor = Color.green;
                    Handles.Label(node.Position, "F: " + node.FCost.ToString(), style);
                }
            }
            if (showLine && finalPath.Count != 0)
            {
                for (int i = 0; i < finalPath.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(finalPath[i].Position, Vector3.one * nodeRadius);
                }
            }
        }
    }
}

