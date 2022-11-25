using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private float agentSpeed;
    
    //the min distance the agent has to be to a node to consider it "traveled" to
    [SerializeField] private float minAgentNodeDistance;
    
    //the speed at which the agent lerps to the node it's arrived to
    [SerializeField] private float lerpSpeed;


    private List<Node> _finalPath = new List<Node>();
    private bool _pathFound;
    private int _nodeIndex = 0;

    private void Awake()
    {
        pathfinding.OnPathChosen += GetPath;
    }

    private void Update()
    {
        if (!_pathFound)
        {
            return;
        }
        MoveAgent(_finalPath);
    }

    private void MoveAgent(List<Node> path)
    {
        //make the agent move towards the next node in the path
        //when its close enough, move towards the next node
        //repeat until it's reached the end node
        Vector2 pathPos = path[_nodeIndex].Position;
        Vector2 agentPos = transform.position;
        transform.position = Vector2.MoveTowards(agentPos, pathPos, agentSpeed * Time.deltaTime);
        float dist = Vector2.SqrMagnitude(agentPos - pathPos);
        if (dist <= minAgentNodeDistance)
        {
            //lerping uneccesary, makes work for artists and programmers harder
            //Vector2 lerpPos = Vector2.Lerp(agentPos, path[_nodeIndex].Position, lerpSpeed * Time.deltaTime);
            transform.position = pathPos;
            _nodeIndex++;
            if (_nodeIndex == path.Count)
            {
                _pathFound = false;
                _nodeIndex = 0;
            }
        }
    }

    void GetPath(List<Node> path)
    {
        //reset the node index when a new path is created
        _nodeIndex = 0;
        _finalPath.Clear();
        _finalPath = path;
        _pathFound = true;
    }

    private void OnDisable()
    {
        pathfinding.OnPathChosen -= GetPath;
    }
}
