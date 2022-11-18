using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] Grid _grid = default;

    [SerializeField] Transform _start = default, _goal = default;

    [SerializeField] bool _manhatten;

     List<Node> _finalPath = new List<Node>();

     [Header("Path")] [SerializeField] private GameObject _openPath;

     [SerializeField] private float _minAgentNodeDistance;
     [SerializeField] private float _agentSpeed;
     [SerializeField] private float _lerpSpeed;
     

     private bool _pathFound;

     private int _nodeIndex = 0;

     private Vector3 _targetPos;

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            FindPath(_start.position, _goal.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                Debug.Log("Hit" + hit.collider.name);
                Collider hitCol = hit.collider;
                if (hitCol.TryGetComponent(out SpaceType space))
                {
                    Debug.Log("Found space");
                    _targetPos = hitCol.transform.position;
                    Node targetNode = _grid.NodeFromWorldPosition(_targetPos);
                    FindPath(_start.position, _targetPos);
                }
            }
            // Vector3 mousePos = Input.mousePosition;
            // Vector2 gridSize = _grid.GridSize;
            // int x = (int)(mousePos.x + gridSize.x * 0.5f);
            // int y = (int)(mousePos.y + gridSize.y * 0.5f);
            // Debug.Log((mousePos));
            // Debug.Log((x + " " + y));
            // if (x > 0 && x < gridSize.x && y > 0 && y < gridSize.y)
            // {
            //     _targetPos = new Vector3(x, y, 0);
            //     FindPath(_start.position, (Vector2)_targetPos);
            // }
        }
        if (!_pathFound)
        {
            return;
        }
        MoveAgent(_finalPath);
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
                ChangeSpaceType(endNode.Position, TypeOfSpace.Target);
                _pathFound = true;
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
        //this._finalPath.Clear();
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode && !_pathFound)
        {
            path.Add(currentNode);
            //GameObject closed = Instantiate(_openPath, currentNode.Position, quaternion.identity);
            ChangeSpaceType(currentNode.Position, TypeOfSpace.Path);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        this._finalPath = path;
        _grid.SetPath(_finalPath);
        MoveAgent(this._finalPath);
    }
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

    void MoveAgent(List<Node> path)
    {
        //make the agent move towards the next node in the path
        //when its close enough, move towards the next node
        //repeat until it's reached the end node
        Vector2 pathPos = _finalPath[_nodeIndex].Position;
        Vector2 agentPos = _start.transform.position;
        _start.transform.position = Vector2.MoveTowards(agentPos, pathPos, _agentSpeed * Time.deltaTime);
        float dist = Vector2.SqrMagnitude((Vector3)agentPos - _finalPath[_nodeIndex].Position);
        if (dist <= _minAgentNodeDistance)
        {
            Vector2 lerpPos = Vector2.Lerp(agentPos, _finalPath[_nodeIndex].Position, _lerpSpeed * Time.deltaTime);
            _start.position = lerpPos;
            _nodeIndex++;
            if (_nodeIndex == _finalPath.Count)
            {
                _pathFound = false;
                _nodeIndex = 0;
            }
        }
    }

    void ChangeSpaceType(Vector2 pos, TypeOfSpace newType)
    {
        Collider[] col = Physics.OverlapSphere(pos, _grid.NodeRadius);
        foreach (Collider c in col)
        {
            if (c.TryGetComponent(out SpaceType space))
            {
                space.SetType(newType);
            }
        }
    }
}

