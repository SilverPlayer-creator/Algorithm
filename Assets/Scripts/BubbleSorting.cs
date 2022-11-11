using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class BubbleSorting : MonoBehaviour
{
    public int[] integers;
    Stopwatch stopwatch;
    [ContextMenu("Sort")]
    public void Sort()
    {
        BubbleSort(integers);
    }

    void BubbleSort(int[] input)
    {
        stopwatch = new Stopwatch();
        Randomize(input);
        stopwatch.Start();
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input.Length-i-1; j++)
            {
                if(input[j] > input[j + 1])
                {
                    int temp = input[j];
                    input[j] = input[j + 1];
                    input[j + 1] = temp;
                }
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log("Time for algorithm: " + stopwatch.ElapsedMilliseconds);
    }
    void Randomize(int[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = UnityEngine.Random.Range(0, 1000);
        }
    }
}
