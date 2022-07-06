using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToRespawn;

    [SerializeField]
    private GameObject knifeUI;

    [SerializeField]
    private GameObject panUI;

    [SerializeField]
    private Sprite cutSprite;

    [SerializeField]
    private Sprite cookedSprite;

    [SerializeField]
    private bool canBeCut = true;

    [SerializeField]
    private bool canBeCooked = true;

    [SerializeField]
    private AudioClip ingredientPickupClip;
    private AudioSource ingredientPickupSource;

    [SerializeField]
    private AudioClip ingredientThrowClip;
    private AudioSource ingredientThrowSource;

    [SerializeField]
    private AudioClip ingredientLandClip;
    private AudioSource ingredientLandSource;

    //VASCO
    [SerializeField]
    public GameObject myParticleSystem;

    public bool isReadyForDelivery = false;

    public enum IngredientState { Normal, Cut, Cooked, Delivered}

    public IngredientState ingredientState;

    public static bool CanDragIndregients = true;

    private bool isBeingDragged = false;
    private bool hasTouchedIngredient = false;
    private bool isMovingToTask = false;
    private bool canRotateIngredient = true;

    private bool canUpdateMinigameData = true;

    private Rigidbody2D myRigidbody;
    private ArrowController arrowController;
    private BuildHotDog buildHotDog;

    private Vector3 initialPosition;
    private Transform parent;

    private MinigameController knifeMinigameController;
    private MinigameController panMinigameController;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        parent = GameObject.FindGameObjectWithTag("IngredientsParent").transform;
        myRigidbody.simulated = true;
        myRigidbody.gravityScale = 0f;
        arrowController = FindObjectOfType<ArrowController>();
        buildHotDog = FindObjectOfType<BuildHotDog>();

        CreateReferenceToMinigameControllers();

        ingredientPickupSource = gameObject.AddComponent<AudioSource>();
        ingredientPickupSource.clip = ingredientPickupClip;
        ingredientPickupSource.playOnAwake = false;

        ingredientThrowSource = gameObject.AddComponent<AudioSource>();
        ingredientThrowSource.clip = ingredientThrowClip;
        ingredientThrowSource.playOnAwake = false;

        ingredientLandSource = gameObject.AddComponent<AudioSource>();
        ingredientLandSource.clip = ingredientLandClip;
        ingredientLandSource.playOnAwake = false;
        
    }



    private void CreateReferenceToMinigameControllers()
    {
        GameObject leftSideParent = GameObject.FindGameObjectWithTag("LeftSideParent");
        GameObject panParent = null;
        GameObject knifeParent = null;

        foreach (Transform child in leftSideParent.transform)
        {
            if (child.tag == "KnifeMinigame")
            {
                knifeParent = child.gameObject;

            }

            if (child.tag == "PanMinigame")
            {
                panParent = child.gameObject;
            }
        }

        foreach (Transform child in knifeParent.transform)
        {
            if (child.tag == "KnifeController")
            {
                knifeMinigameController = child.GetComponent<MinigameController>();
                break;
            }
        }

        foreach (Transform child in panParent.transform)
        {
            if (child.tag == "PanController")
            {
                panMinigameController = child.GetComponent<MinigameController>();
                break;
            }
        }
    }

    private void Update()
    {
        if (ingredientState == IngredientState.Delivered) { return; }

        if (Input.GetMouseButtonUp(0))
        {
            if (isBeingDragged)
            {
                ingredientThrowSource.Play();
                Vector2 mouseVelocity = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                isBeingDragged = false;
                myRigidbody.gravityScale = 1f;
                myRigidbody.simulated = true;
                myRigidbody.velocity = mouseVelocity * 20f;
            }
        }

        if (isBeingDragged)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
            transform.position = Vector3.Lerp(transform.position, mousePosition, 5f * Time.deltaTime);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0f, 9f),
                     Mathf.Clamp(transform.position.y, -5.38f, 2f),
                     0f);
        }
        else
        {
            // If ingredient is falling.
            RotateIngredientOnFall();
        }

        if (ingredientState == IngredientState.Normal)
        {
            if (isMovingToTask)
            {
                Vector2 desiredPosition = new Vector2(-4.34f, -0.08f);

                if (Vector2.Distance(transform.position, desiredPosition) > 0.1f)
                {
                    myRigidbody.simulated = false;
                    myRigidbody.gravityScale = 0f;
                    transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 5f * Time.deltaTime);
                }
                // If reached task zone.
                else
                {
                    canRotateIngredient = false;
                    transform.localEulerAngles = Vector3.zero;

                    if (arrowController.IsInKnifeMinigame)
                    {
                        if (canUpdateMinigameData)
                        {
                            ingredientLandSource.Play();
                            knifeMinigameController.UpdateIngredientUI(knifeUI);
                            knifeMinigameController.SetCurrentIngredient(this);
                            canUpdateMinigameData = false;
                            arrowController.CanChangeTask = false;
                        }
                    }
                    else
                    {
                        if (canUpdateMinigameData)
                        {
                            ingredientLandSource.Play();
                            panMinigameController.UpdateIngredientUI(panUI);
                            panMinigameController.SetCurrentIngredient(this);
                            canUpdateMinigameData = false;
                            arrowController.CanChangeTask = false;
                        }
                    }
                }
            }
        }
    }

    private void RotateIngredientOnFall()
    {
        if (hasTouchedIngredient && canRotateIngredient)
        {
            if (transform.position.x > 6f)
            {
                transform.localEulerAngles += new Vector3(0f, 0f, -200f * Time.deltaTime);
            }
            else
            {
                transform.localEulerAngles += new Vector3(0f, 0f, 200f * Time.deltaTime);
            }
        }
    }

    private void OnMouseDown()
    {
        // If mouse is inside right side.
        if (GetIsMouseClamped())
        {

            if (!hasTouchedIngredient) { hasTouchedIngredient = true; }

            if (!isBeingDragged)
            {
                ingredientPickupSource.Play();
                isBeingDragged = true;
                myRigidbody.gravityScale = 0f;
                myRigidbody.simulated = false;
            }
        }

        if (ingredientState == IngredientState.Normal)
        {
            Invoke(nameof(SpawnNextIngredient), 2f);
        }
        else if (ingredientState == IngredientState.Cut ||
                 ingredientState == IngredientState.Cooked)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void SpawnNextIngredient()
    {
        var objectCreated = Instantiate(prefabToRespawn, initialPosition, Quaternion.identity);
        objectCreated.transform.SetParent(parent);
    }

    private bool GetIsMouseClamped()
    {
        if (transform.position.x > 0f && transform.position.x < 9f &&
            transform.position.y > -5.38f && transform.position.y < 5.3f)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Divider"))
        {
            if (arrowController.IsInKnifeMinigame && canBeCut ||
                !arrowController.IsInKnifeMinigame && canBeCooked)
            {
                if (!isMovingToTask && CanDragIndregients)
                {
                    isMovingToTask = true;
                    CanDragIndregients = false;
                }
            }
        }
    }

    // Called when the ingredient has been cut.
    public void CutIngredient()
    {
        isReadyForDelivery = true;
        ingredientState = IngredientState.Cut;
        GetComponent<SpriteRenderer>().sprite = cutSprite;
        CanDragIndregients = true;
        FindObjectOfType<ReadyIngredientsBox>().AddToIngredientBox(this);
    }

    // Called when the ingredient has been cooked.
    public void CookIngredient()
    {
        isReadyForDelivery = true;
        ingredientState = IngredientState.Cooked;
        GetComponent<SpriteRenderer>().sprite = cookedSprite;
        CanDragIndregients = true;
        FindObjectOfType<ReadyIngredientsBox>().AddToIngredientBox(this);
    }
}
