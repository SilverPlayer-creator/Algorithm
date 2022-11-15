using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Grid : MonoBehaviour
{
    public List<Node> Path
    {
        get { return _path; }
    }

    [SerializeField]  Vector2 _gridSize;
    [SerializeField] float _nodeRadius;
    [SerializeField] LayerMask _unwalkeableMask;

    [Header("Debug")]
    [SerializeField] bool _showGCost;
    [SerializeField] bool _showHCost;
    [SerializeField] bool _showFCost;
    [SerializeField] bool _showPath;
    [SerializeField] int _fontSize;

    float _nodeDiameter;
    int _gridSizeX, _gridSizeY;
    Node[,] _grid;
    public Node[,] GetGrid { get { return _grid; } }

    List<Node> _path = new List<Node>();
    [ContextMenu("Create Grid")]
    public void CreateGrid()
    {
        _nodeDiameter = _nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(_gridSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_gridSize.y / _nodeDiameter);
        _grid = new Node[_gridSizeX, _gridSizeY];

        Vector3 bottomLeft = transform.position - Vector3.right * _gridSize.x / 2 - Vector3.up * _gridSize.y / 2;
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPos = bottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) + Vector3.up * (y * _nodeDiameter + _nodeRadius);
                bool walkeable = !(Physics.CheckSphere(worldPos, _nodeRadius, _unwalkeableMask));
                _grid[x, y] = new Node(walkeable, worldPos, x, y);
            }
        }
    }
    public Node NodeFromWorldPosition(Vector3 worldPos)
    {
        //Vector2 bottomLeft = (Vector2)transform.position - Vector2.right * _gridSize.x / 2 - Vector2.up * _gridSize.y / 2;
        //int xPoint = Mathf.FloorToInt(((worldPos.x - bottomLeft.x) / _nodeDiameter));

        //int yPoint = Mathf.FloorToInt(((worldPos.y - bottomLeft.y) / _nodeDiameter));
        //Debug.Log("worldPos: " + worldPos);
        //Debug.Log("Before clamping: " + "\n" + "xPoint: " + xPoint + " ,yPoint: " + yPoint);

        //Debug.Log("After clamping: " + "\n" + "xPoint: " + xPoint + " ,yPoint: " + yPoint);

        float percentX = (worldPos.x + _gridSize.x / 2) / _gridSize.x;
        float percentY = (worldPos.y + _gridSize.y / 2) / _gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return _grid[x, y];
    }
    public List<Node> NeighbourNodes(Node node)
    {
        List<Node> neighbouringNode = new List<Node>();
        int xCheck;
        int yCheck;

        //right side
        xCheck = node.GridX + 1;
        yCheck = node.GridY;
        if (xCheck >= 0 && xCheck < _gridSizeX)
        {
            if (yCheck >= 0 && yCheck < _gridSizeY)
            {
                neighbouringNode.Add(_grid[xCheck, yCheck]);
            }
        }
        //left side
        xCheck = node.GridX - 1;
        yCheck = node.GridY;
        if (xCheck >= 0 && xCheck < _gridSizeX)
        {
            if (yCheck >= 0 && yCheck < _gridSizeY)
            {
                neighbouringNode.Add(_grid[xCheck, yCheck]);
            }
        }
        //top side
        xCheck = node.GridX;
        yCheck = node.GridY + 1;
        if (xCheck >= 0 && xCheck < _gridSizeX)
        {
            if (yCheck >= 0 && yCheck < _gridSizeY)
            {
                neighbouringNode.Add(_grid[xCheck, yCheck]);
            }
        }
        //bottom side
        xCheck = node.GridX;
        yCheck = node.GridY - 1;
        if (xCheck >= 0 && xCheck < _gridSizeX)
        {
            if (yCheck >= 0 && yCheck < _gridSizeY)
            {
                neighbouringNode.Add(_grid[xCheck, yCheck]);
            }
        }
        return neighbouringNode;
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    public void SetPath(List<Node> finalPath)
    {
        _path = finalPath;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridSize.x, _gridSize.y, 1));
        GUIStyle style = new GUIStyle();
        style.fontSize = _fontSize;
        if (_grid != null)
        {
            foreach (Node n in _grid)
            {
                Gizmos.color = Color.white;
                if (_path != null)
                    if (_path.Contains(n) && _showPath)
                        Gizmos.color = Color.yellow;

                Gizmos.DrawWireCube(n.Position, Vector3.one * (_nodeDiameter - .1f));
                if (_showGCost)
                {
                    style.normal.textColor = Color.red;
                    Handles.Label(n.Position + (new Vector3(-.5f, .5f, 0) * _nodeRadius), "G: " + n.GCost.ToString(), style);
                }
                if (_showHCost)
                {
                    style.normal.textColor = Color.blue;
                    Handles.Label(n.Position + (new Vector3(.5f, .5f, 0) * _nodeRadius), "H: " + n.HCost.ToString(), style);
                }
                if (_showFCost)
                {
                    style.normal.textColor = Color.green;
                    Handles.Label(n.Position + (new Vector3(0, -.5f, 0) * _nodeRadius), "F: " + n.FCost.ToString(), style);
                }
            }
        }

    }
}
