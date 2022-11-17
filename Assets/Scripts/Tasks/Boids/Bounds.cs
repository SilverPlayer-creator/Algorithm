using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{
    [SerializeField] private Vector2 _bounds;
    public  Vector2 BoidBounds
    {
        get { return _bounds; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_bounds.x, _bounds.y, 1));
    }
}
