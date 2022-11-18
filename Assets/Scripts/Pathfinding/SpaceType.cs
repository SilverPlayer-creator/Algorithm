using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public  enum TypeOfSpace{Empty, Obstacle, Path, Target}
public class SpaceType : MonoBehaviour
{

    private TypeOfSpace _type;
    [SerializeField] private Color _red, _blue, _yellow;
    [SerializeField] private Material _clear;
    public TypeOfSpace Type => _type;
    private MeshRenderer _meshRenderer;
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _type = TypeOfSpace.Empty;
    }

    public void SetType(TypeOfSpace newType)
    {
        _type = newType;
        switch (newType)
        {
            case TypeOfSpace.Empty:
                _meshRenderer.material = _clear;
                break;
            case  TypeOfSpace.Obstacle:
                    _meshRenderer.material.color = _red;
                    break;
            case  TypeOfSpace.Path:
                _meshRenderer.material.color = _blue;
                break;
            case  TypeOfSpace.Target:
                _meshRenderer.material.color = _yellow;
                break;
        }
    }
}
