using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class MouseInput : MonoBehaviour
{
    Vector2 _mousePos;
    private PlayerInputAction _inputAction;

    private void Awake()
    {
        _inputAction = new PlayerInputAction();
    }

    public void OnSetPath(InputValue value)
    {
        //Debug.Log("Mouse one clicked");
        Vector2 mousePos = Mouse.current.position.ReadValue();
        //Debug.Log("Mouse hit at vector2: " + mousePos);
        OnPathSet?.Invoke(mousePos);
    }

    void OnMakeObstacle(InputValue value)
    {
//        Debug.Log("Mouse two");
        Vector2 mousePos = Mouse.current.position.ReadValue();
        OnObstacleSet?.Invoke(mousePos);
    }
    public UnityAction<Vector2> OnPathSet;
    public UnityAction<Vector2> OnObstacleSet;
}
