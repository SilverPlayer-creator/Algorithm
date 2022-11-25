using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    bool _walkeable;
    public bool Walkeable
    {
        get { return _walkeable; }
    }
    int _gridX, _gridY;
    public int GridX
    {
        get { return _gridX; }
    }
    public int GridY
    {
        get{ return _gridY; }
    }

    int _gCost, _hCost; //g cost = distance from start, h cost = distance from goal

    Node parent;

    public Node Parent
    {
        get { return parent; }
    }
    public int GCost
    {
        get { return _gCost; }
    }
    public int HCost
    {
        get { return _hCost; }
    }
    Vector3 _position;
    public Vector3 Position
    {
        get { return _position; }
    }
    public int FCost { get { return _gCost + _hCost; } }
    public Node(bool walkeable, Vector3 pos, int posX, int posY)
    {
        this._walkeable = walkeable;
        this._position = pos;
        this._gridX = posX;
        this._gridY = posY;
    }
    public void SetGCost(int newCost)
    {
        _gCost = newCost;
    }
    public void SetHCost(int newCost)
    {
        _hCost = newCost;
    }
    public void SetParent(Node parentNode)
    {
        this.parent = parentNode;
    }
    public void SetAsObstacle()
    {
        _walkeable = false;
    }
    public void ResetCosts()
    {
        _gCost = 0;
        _hCost = 0;
    }
}
