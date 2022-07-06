using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private string[] ingredientsNamesNeededThisLevel;

    [SerializeField]
    private Canvas gameWinCanvasThisLevel;

    [SerializeField]
    private Canvas gameOverCanvas;

    [SerializeField]
    private Transform hotDogUITransform;

    [SerializeField]
    private Image[] winStars = new Image[3];

    [SerializeField]
    private Image clockImage;

    private List<Ingredient> ingredientsNeededThisLevel = new List<Ingredient>();

    private int deliveredIngredients = 0;
    private int correctIngredients = 0;

    private int numberOfStars = 0;

    [SerializeField]
    private float clockTimeThisLevel = 30f;
    private float clockTime = 0f;

    private bool canStopWatchSound = true;

    private void Awake()
    {
        Ingredient.CanDragIndregients = true;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        clockTime = clockTimeThisLevel;
    }

    private void Update()
    {
        if (clockTime > 0f)
        {
            clockTime -= Time.deltaTime;
        }
        else
        {
            if (canStopWatchSound) 
            { 
                clockImage.GetComponent<AudioSource>().Stop();
                canStopWatchSound = false;
                ShowUI(false);
            }
        }

        clockImage.fillAmount = MapRangeClamped(clockTime, 0f, clockTimeThisLevel, 0f, 1f);
        
    }

    public float MapRangeClamped(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    public void CheckIngredientCorrect(Ingredient ingredient, int index)
    {
        switch (ingredientsNamesNeededThisLevel[index])
        {
            case "Cebola Cortada":

                if (ingredient.CompareTag("Cebola") && ingredient.ingredientState == Ingredient.IngredientState.Cut)
                {
                    AddDeliveredIngredient(true);
                    break;
                }
                AddDeliveredIngredient(false);
                break;

            case "Tomate Cortado":

                if (ingredient.CompareTag("Tomate") && ingredient.ingredientState == Ingredient.IngredientState.Cut)
                {
                    AddDeliveredIngredient(true);
                    break;
                }
                AddDeliveredIngredient(false);
                break;

            case "Tomate Cozido":

                if (ingredient.CompareTag("Tomate") && ingredient.ingredientState == Ingredient.IngredientState.Cooked)
                {
                    AddDeliveredIngredient(true);
                    break;
                }
                AddDeliveredIngredient(false);
                break;

            case "Malagueta Cortada":

                if (ingredient.CompareTag("Malagueta") && ingredient.ingredientState == Ingredient.IngredientState.Cut)
                {
                    AddDeliveredIngredient(true);
                    break;
                }
                AddDeliveredIngredient(false);
                break;

            case "Malagueta Cozida":

                if (ingredient.CompareTag("Malagueta") && ingredient.ingredientState == Ingredient.IngredientState.Cooked)
                {
                    AddDeliveredIngredient(true);
                    break;
                }
                AddDeliveredIngredient(false);
                break;

            case "Batata":

                if (ingredient.CompareTag("Batata") && ingredient.ingredientState == Ingredient.IngredientState.Normal)
                {
                    AddDeliveredIngredient(true);
                    break;
                }
                AddDeliveredIngredient(false);
                break;
        }
    }

    private void AddDeliveredIngredient(bool isCorrect)
    {
        deliveredIngredients++;

        if (isCorrect) { correctIngredients++; }

        if (deliveredIngredients == ingredientsNamesNeededThisLevel.Length)
        {
            if (correctIngredients == ingredientsNamesNeededThisLevel.Length)
            {
                // Set number of stars depending on reamining time.
                numberOfStars = 1;
                if (clockTime > clockTimeThisLevel / 2)
                {
                    numberOfStars = 3;
                }
                else if (clockTime > 5f)
                {
                    numberOfStars = 2;
                }
                ShowUI(true);
                ShowWinStarts(numberOfStars);
            }
            else
            {
                ShowUI(false);
            }
        }
    }

    public void ShowUI(bool isWin)
    {
        Time.timeScale = 0f;

        if (isWin)
        {
            gameWinCanvasThisLevel.gameObject.SetActive(true);
        }
        else
        {
            gameOverCanvas.gameObject.SetActive(true);
        }

        GameObject finalHotDog = Instantiate(GameObject.FindGameObjectWithTag("Plate"), 
            hotDogUITransform.position, Quaternion.identity);

        finalHotDog.GetComponent<SpriteRenderer>().sortingLayerName = "FinalHotDog";

        foreach (Transform child in finalHotDog.transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                child.GetComponent<SpriteRenderer>().sortingLayerName = "FinalHotDog";
            }
        }
    }

    private void ShowWinStarts(int numberStars)
    {
        switch (numberOfStars)
        {
            case 1:
                winStars[0].gameObject.SetActive(true);
                winStars[1].gameObject.SetActive(false);
                winStars[2].gameObject.SetActive(false);
                break;
            case 2:
                winStars[0].gameObject.SetActive(true);
                winStars[1].gameObject.SetActive(true);
                winStars[2].gameObject.SetActive(false);
                break;
            case 3:
                winStars[0].gameObject.SetActive(true);
                winStars[1].gameObject.SetActive(true);
                winStars[2].gameObject.SetActive(true);
                break;
        }
    }
}
