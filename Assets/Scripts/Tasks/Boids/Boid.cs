using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Radius")] [SerializeField] private float _detectionRadius;

    [Header("Speed")] 
    [SerializeField] private float _rotateSpeed;

    [SerializeField] private float _moveSpeed;

    private BoidManager _manager;
    private List<Boid> _flockmates = new List<Boid>();
    public  List<Boid> FlockMates
    {
        get { return _flockmates; }
    }
    
    private  Quaternion _rotation = quaternion.identity;
    private Vector2 _forward = Vector2.zero;
    private Vector2 _position = Vector2.zero;

    private SpriteRenderer _sprite = default;
    public Vector2 Forward => _rotation * Vector2.up;

    private void Awake()
    {
        _sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateLocalFlockmates();
        //Rotate();
    }

    public void Initialize(BoidManager manager)
    {
        this._manager = manager;
        _position = transform.position;
    }

    void UpdateLocalFlockmates()
    {
        //regularly clear list of flockmates
        _flockmates.Clear();
        Boid[] boids = _manager.GetBoids();
        for (int i = 0; i < boids.Length; i++)
        {
            if (boids[i] == this)
            {
                continue; //boid should ignore itself
            }
            //get the distance from the other nodes
            Vector3 distance = boids[i].transform.position - transform.position;
            float distanceSqr = distance.magnitude;
            if (distanceSqr <= _detectionRadius)
            {
                _flockmates.Add(boids[i]);
            }
        }
    }

    public void Move(Vector2 vel)
    {
        transform.up = vel;
        transform.position += (Vector3)vel * Time.deltaTime;
    }
    void Rotate()
    {
        quaternion targetRot = Quaternion.LookRotation(_forward);
        _rotation = Quaternion.RotateTowards(_rotation, targetRot, _forward.magnitude * _rotateSpeed * Time.deltaTime);
        transform.rotation = _rotation;
    }
    public float DetectionRadius => _detectionRadius;
    public Vector3 Position => transform.position;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
