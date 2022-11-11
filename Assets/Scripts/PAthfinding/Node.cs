using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    int _posX, _posY;
    public int PosX
    {
        get { return _posX; }
    }
    public int PosY
    {
        get{ return _posY; }
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
    Vector2 _position;
    public Vector2 Position
    {
        get { return _position; }
    }
    public int FCost { get { return _gCost + _hCost; } }
    public Node(Vector2 pos, int posX, int posY)
    {
        this._position = pos;
        this._posX = posX;
        this._posY = posY;
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
}
