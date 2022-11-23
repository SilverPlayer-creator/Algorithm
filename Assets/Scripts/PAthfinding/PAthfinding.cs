using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public class Pathfinding : MonoBehaviour
{
    public Action<List<Node>> OnPathChosen;

    [FormerlySerializedAs("_grid")] [SerializeField] Grid grid = default;

    [SerializeField] private Transform _start = default;

    [SerializeField] bool _manhatten;

     private List<Node> _finalPath = new List<Node>();

     private bool _pathFound;

     private Vector3 _targetPos;
     private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                //Debug.Log("Hit" + hit.collider.name);
                Collider hitCol = hit.collider;
                if (hitCol.TryGetComponent(out SpaceType space))
                {
                    //Debug.Log("Found space");
                    _targetPos = hitCol.transform.position;
                    FindPath(_start.position, _targetPos);
                }
            }
        }
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        _pathFound = false;
            //check each surrounding node from a starting point
            //compare the nodes f cost
            //go with the node that has the smallest f cost
            Node startNode = grid.NodeFromWorldPosition(startPos);
            Node endNode = grid.NodeFromWorldPosition(endPos);

            List<Node> openList = new List<Node>();
            HashSet<Node> closeList = new HashSet<Node>();
            foreach (Node n in grid.GetGrid)
            {
                n.ResetCosts();
            }

            openList.Add(startNode); //add the starting node to openList
            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                for (var i = 1; i < openList.Count; i++)
                {
                    if (openList[i].FCost < currentNode.FCost ||
                        openList[i].FCost == currentNode.FCost) //compare the costs
                    {
                        if (openList[i].HCost < currentNode.HCost)
                        {
                            currentNode = openList[i];
                        }
                    }
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);
                if (currentNode == endNode) //alternatively check if positions are the same?
                {
                    GetFinalPath(startNode, endNode);
                    ChangeSpaceType(endNode.Position, TypeOfSpace.Target);
                    return;
                }

                List<Node> neighbourNodes =
                    _manhatten ? grid.ManhattenNeighbours(currentNode) : grid.GetNeighbours(currentNode);

                foreach (var neighbourNode in neighbourNodes)
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
            _pathFound = false;
            if (_finalPath.Count != 0)
            {
                //reset the previous path to empty
                ResetPath();
            }
            List<Node> path = new List<Node>();
            Debug.Log(path.Count);
            var currentNode = endNode;

            while (currentNode != startNode && !_pathFound)
            {
                path.Add(currentNode);
                //GameObject closed = Instantiate(_openPath, currentNode.Position, quaternion.identity);
                ChangeSpaceType(currentNode.Position, TypeOfSpace.Path);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            this._finalPath = path;
            //grid.SetPath(_finalPath);
            OnPathChosen(_finalPath);
            _pathFound = true;
        }

        static int GetDistance(Node nodeA, Node nodeB)
        {
            var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }

            return 14 * dstX + 10 * (dstY - dstX);
        }

        void ResetPath()
        {
            foreach (Node n in _finalPath)
            {
                ChangeSpaceType(n.Position, TypeOfSpace.Empty);
            }
            _finalPath.Clear();
        }

        private void ChangeSpaceType(Vector2 pos, TypeOfSpace newType)
        {
            Collider[] col = Physics.OverlapSphere(pos, grid.NodeRadius);
            foreach (Collider c in col)
            {
                if (c.TryGetComponent(out SpaceType space))
                {
                    space.SetType(newType);
                }
            }
        }
    }

