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
        startNode.SetGCost(0); 
        openList.Add(startNode);//add the starting node to openList
        int listrunIndex = 0;
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                int gCost = openList[i].GCost;
                Debug.Log("Current node's gCost=" + openList[i].GCost);
                int hCost = openList[i].HCost;
                if(openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost) //compare the costs
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
                Vector2 difference = currentNode.Position - neighbourNode.Position;
                float xF = difference.x;
                float yF = difference.y;
                xF = Mathf.Abs(xF);
                int xPoint = Mathf.FloorToInt(xF);
                yF = Mathf.Abs(yF);
                int yPoint = Mathf.FloorToInt(yF);
                int neighBourCost = xPoint + yPoint;
                int moveCost = currentNode.GCost + neighBourCost;
                neighbourNode.SetGCost(neighBourCost);
                Debug.Log("Movecost: " + moveCost);

                if(moveCost < neighbourNode.GCost) //NEED TO SET THE GCOST BEFORE THIS?
                {
                    neighbourNode.SetGCost(moveCost);
                    Debug.Log("Set new gcost");
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
        Vector2 bottomLeft = (Vector2)transform.position - Vector2.right * size.x / 2 - Vector2.up * size.y / 2;
        int xPoint = Mathf.FloorToInt(((worldPos.x - bottomLeft.x) / nodeDiameter));

        int yPoint = Mathf.FloorToInt(((worldPos.y -bottomLeft.y) / nodeDiameter));
        Debug.Log("worldPos: " + worldPos);
        Debug.Log("Before clamping: " + "\n" + "xPoint: " + xPoint + " ,yPoint: " + yPoint);

        Debug.Log("After clamping: " + "\n" + "xPoint: " + xPoint + " ,yPoint: " + yPoint);

        return grid[xPoint, yPoint];
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
            //Node startNode = NodeFromWorldPosition(start.position);
            //Node endNode = NodeFromWorldPosition(goal.position);
            foreach (Node node in grid)
            {
                Debug.Log("Draw cubes");

                Gizmos.DrawWireCube(node.Position, Vector3.one * nodeRadius);
                if (showGCost)
                {
                    style.normal.textColor = Color.red;
                    Handles.Label(node.Position + (new Vector2(-.5f, .5f) * nodeRadius), "G: " + node.GCost.ToString(), style);
                }
                if (showHCost)
                {
                    style.normal.textColor = Color.blue;
                    Handles.Label(node.Position + (new Vector2(.3f, .5f) * nodeRadius), "H: " + node.HCost, style);
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

