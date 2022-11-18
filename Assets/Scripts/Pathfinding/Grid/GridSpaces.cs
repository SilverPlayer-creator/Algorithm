using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridSpaces : MonoBehaviour
{
    [SerializeField] private SpaceType _space;
    private Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    public void Initialize(Node[,] grid)
    {
        foreach (Node n in grid)
        {
            SpaceType newSpace = Instantiate(_space, n.Position, quaternion.identity);
            if (!n.Walkeable)
            {
                newSpace.SetType(TypeOfSpace.Obstacle);
            }
            else
            {
                newSpace.SetType((TypeOfSpace.Empty));
            }
        }
    }
}
