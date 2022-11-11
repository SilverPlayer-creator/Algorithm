using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSort : MonoBehaviour
{
    public int[] integers;

    [ContextMenu("Sort")]
    public void MenuSort()
    {
        Sort(integers);
    }
    

    void Sort(int[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            int min = i; //take the smaller int
            for (int j = i+1; j < inputs.Length; j++) //loop through everything ahead of current position
            {
                if(inputs[j] < inputs[min]) //if the current smallest number is bigger than the next number
                {
                    min = j; //the next number is the smallest one
                }
            }
            int temp = inputs[i];

            inputs[i] = inputs[min];
            inputs[min] = temp;
        }
    }
}
