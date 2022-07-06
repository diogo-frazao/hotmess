using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHotDog : MonoBehaviour
{
    private Vector3[] deliveredIngredientsPos = new Vector3[3];

    private BoxCollider2D myBoxCollider;

    private int deliveredIngredients = 0;
    private int sortingIngredientLayerIndex = 1;

    private Transform plateTransform;

    //VASCO

    [SerializeField]
    private AudioClip ingredientDropClip;
    private AudioSource ingredientDropSource;

    //

    private void Awake()
    {
        myBoxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        deliveredIngredientsPos[0] = transform.Find("DeliveryPos1").transform.position;
        deliveredIngredientsPos[1] = transform.Find("DeliveryPos2").transform.position;
        deliveredIngredientsPos[2] = transform.Find("DeliveryPos3").transform.position;

        ingredientDropSource = gameObject.AddComponent<AudioSource>();
        ingredientDropSource.clip = ingredientDropClip;
        ingredientDropSource.playOnAwake = false;
        ingredientDropSource.volume = 2f;

        plateTransform = GameObject.FindGameObjectWithTag("Plate").transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Ingredient>() != null &&
            other.GetComponent<Ingredient>().isReadyForDelivery &&
            other.GetComponent<Ingredient>().ingredientState != Ingredient.IngredientState.Delivered)
        {
            GameObject createdIngredient = Instantiate(other.gameObject, 
                deliveredIngredientsPos[deliveredIngredients], Quaternion.identity);
            createdIngredient.GetComponent<Ingredient>().ingredientState = Ingredient.IngredientState.Delivered;

            createdIngredient.GetComponent<SpriteRenderer>().sortingOrder = sortingIngredientLayerIndex;

            createdIngredient.transform.SetParent(plateTransform);

            GameManager.Instance.CheckIngredientCorrect(other.GetComponent<Ingredient>(), deliveredIngredients);

            Destroy(other.gameObject);

            deliveredIngredients++;
            sortingIngredientLayerIndex++;

            ingredientDropSource.Play();
        }
    }
}
