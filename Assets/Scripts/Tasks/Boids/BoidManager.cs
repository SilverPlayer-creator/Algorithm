using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class BoidManager : MonoBehaviour
{
    [Header("Boids")]
    [SerializeField] private int _boidsAmount;
    [SerializeField] private Boid _boidPrefab;
    [SerializeField] private float _detectionRadius;
    private Boid[] _boids;
    
    [Header(("Weights"))] 
    [SerializeField] private float _separation;
    [SerializeField] private float _cohesion;
    [SerializeField] private float _alignment;
    [SerializeField] private float _boundsWeight;

    private Bounds _bounds = default;

    private void Awake()
    {
        _bounds = GetComponent<Bounds>();
        _boids = new Boid[_boidsAmount];
        for (int i = 0; i < _boidsAmount; i++)
        {
            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * _boidsAmount * 0.08f;
            Quaternion randomRot = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
            GameObject newBoid = Instantiate(_boidPrefab.gameObject, randomPos, randomRot );
            if (newBoid.TryGetComponent(out Boid boid))
            {
                _boids[i] = boid;
                _boids[i].Initialize(this);
            }
        }
    }

    private void Update()
    {
        MoveBoids();
    }

    public Boid[] GetBoids()
    {
        return _boids;
    }
    Vector2 Separation(Boid mainBoid, List<Boid> flockmates)
    {
        if (flockmates.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 separation = Vector2.zero;
        int avoid = 0;
        foreach (Boid flockmate in flockmates)
        {
            if (Vector2.SqrMagnitude(mainBoid.transform.position - flockmate.transform.position) <
                mainBoid.DetectionRadius)
            {
                avoid++;
                separation += (Vector2)(mainBoid.transform.position - flockmate.transform.position);
            }
        }

        if (avoid > 0)
        {
            separation /= avoid;
        }
        return separation;
    }

    Vector2 Cohesion(Boid mainBoid, List<Boid> flockmates)
    {
        if (flockmates.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 cohesion = Vector2.zero;
        foreach (Boid flockmate in flockmates)
        {
            cohesion += (Vector2)flockmate.transform.position;
        }

        cohesion /= flockmates.Count;
        cohesion -= (Vector2)mainBoid.transform.position;
        return cohesion;
    }

    Vector2 Alignment(Boid mainBoid, List<Boid> flockmates)
    {
        if (flockmates.Count == 0)
        {
            return Vector2.zero;
        }

        Vector2 alignmentMove = Vector2.zero;
        foreach (Boid flockmate in flockmates)
        {
            alignmentMove += (Vector2)flockmate.transform.up;
        }
        alignmentMove /= flockmates.Count;
        return alignmentMove;
    }

    void MoveBoids()
    {
        foreach (Boid boid in _boids)
        {
            Vector2 move = Vector2.zero;
            move += Separation(boid, boid.FlockMates);
            move += Cohesion(boid, boid.FlockMates);
            move += Alignment(boid, boid.FlockMates);
            
            boid.Move(move);
        }
    }
    void Bounds()
    {
        //if the boid is outside the bounds, make them go the opposite way
        
    }
}
