using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Profiler : MonoBehaviour
{
    public int iterations;
    private List<Vector3> _positionsA = new List<Vector3>();
    private List<Vector3> _positionsB = new List<Vector3>();

    private void Start()
    {
        AddPositions();
    }

    private void Update()
    {
        CompareDistanceManual();
        CompareVectorsDistance();
        StringCompareTest();
    }

    void AddPositions()
    {
        for (int i = 0; i < iterations; i++)
        {
            Vector3 newPos = Random.insideUnitCircle;
            _positionsA.Add(newPos);
        }

        for (int i = 0; i < iterations; i++)
        {
            Vector3 newPos = Random.insideUnitCircle;
            _positionsB.Add(newPos);
        }
    }

    void CompareVectorsDistance()
    {
        for (int i = 0; i < iterations; i++)
        {
            float distance = Vector3.Distance(_positionsA[i], _positionsB[i]);
        }
    }

    void CompareDistanceManual()
    {
        for (int i = 0; i < iterations; i++)
        {
            float distance = (_positionsA[i] - _positionsB[i]).magnitude;
        }
    }

    void StringCompareTest()
    {
        string a = "Hello";
        string b = "Hiya";
        for (int i = 0; i < iterations; i++)
        {
            StringCompareA(a, b);
            StringCompareB(a, b);
            StringCompareC(a, b);
        }
    }

    bool StringCompareA(string a, string b)
    {
        return a == b;
    }

    bool StringCompareB(string a, string b)
    {
        for (int i = 0; i < Mathf.Min(a.Length, b.Length); i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }

    bool StringCompareC(string a, string b)
    {
        if (string.Compare(a, b) == 0)
        {
            return true;
        }

        return false;
    }
}
