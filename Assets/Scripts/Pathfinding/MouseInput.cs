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

    public void OnSetPath(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            OnPathSet?.Invoke(mousePos);
        }
    }

    public void OnMakeObstacle(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            OnObstacleSet?.Invoke(mousePos);
        }
    }
    public UnityAction<Vector2> OnPathSet;
    public UnityAction<Vector2> OnObstacleSet;
}
