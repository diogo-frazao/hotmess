using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyIngredientsBox : MonoBehaviour
{
    private Vector3[] readyIngredientsPosition = new Vector3[4]; 

    private int readyIngredientIndex = 0;

    private int numberOfIngredientsInBox = 0;

    private void Start()
    {
        readyIngredientsPosition[0] = new Vector3(0.842f, 1.309f, 0);
        readyIngredientsPosition[1] = new Vector3(1.788f, 1.309f, 0);
        readyIngredientsPosition[2] = new Vector3(2.761f, 1.309f, 0);
        readyIngredientsPosition[3] = new Vector3(3.725f, 1.309f, 0);
    }

    public void AddToIngredientBox(Ingredient ingredient)
    {
        // DOn't place if there's no more space.
        numberOfIngredientsInBox++;
        if (numberOfIngredientsInBox > 4)
        {
            return;
        }

        if (readyIngredientIndex < readyIngredientsPosition.Length)
        {
            ingredient.transform.position = readyIngredientsPosition[readyIngredientIndex];
            ingredient.transform.localEulerAngles = new Vector3(0, 0, 60f);
            readyIngredientIndex++;
        }
        else
        {
            ingredient.transform.position = readyIngredientsPosition[readyIngredientIndex];
            ingredient.transform.localEulerAngles = new Vector3(0, 0, 60f);
            readyIngredientIndex = 0;
        }
    }
}
