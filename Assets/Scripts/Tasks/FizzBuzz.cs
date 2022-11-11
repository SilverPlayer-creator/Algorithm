using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizzBuzz : MonoBehaviour
{
    public int maxNumber;
    public string[] stringArray;

    [ContextMenu("FizzBuzz")]
    void FizzBuzzAlgo()
    {
        stringArray = new string[maxNumber];
        for (int i = 0; i < maxNumber; i++)
        {
            stringArray[i] = FizzOrBuzz(i);
        }
    }
    string FizzOrBuzz(int numberToCheck)
    {
        if(numberToCheck % 3 == 0 && numberToCheck % 5 != 0)
        {
            return "Fizz";
        }
        if(numberToCheck % 5 == 0 && numberToCheck % 3 != 0)
        {
            return "Buzz";
        }
        if(numberToCheck % 3 == 0 && numberToCheck % 5 == 0)
        {
            return "FizzBuzz";
        }
        return numberToCheck.ToString();
    }
}
