using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MinigameController : MonoBehaviour
{
    [SerializeField]
    private bool isKnifeMinigame = true;

    [SerializeField]
    private float defaultKnifeSpeed = 0.2f;
    private float knifeSpeed = 0.2f;

    //VASCO
    [SerializeField]
    private float maxKnifeSpeed = 1.1f;
    [SerializeField]
    private float knifeSpeedIncrement = 1.1f;

    [SerializeField]
    private AudioClip knifeUseClip;
    private AudioSource knifeUseSource;

    [SerializeField]
    private AudioClip knifeThunkClip;
    private AudioSource knifeThunkSource;

    private float knifeThunkDelay = 0.2f;

    [SerializeField]
    private AudioClip panUseClip;
    private AudioSource panUseSource;

    [SerializeField]
    private AudioClip panLoopClip;
    private AudioSource panLoopSource;

    [SerializeField]
    private AudioClip ingredientReadyClip;
    private AudioSource ingredientReadySource;

    //

    [SerializeField]
    private float minXKnifeUISelection = -1.613f;

    [SerializeField]
    private float maxXKnifeUISlection = 1.613f;

    // Updated for every new object.
    public GameObject taskUI;
    public GameObject taskUISelection;
    public List<GameObject> spotsToHit = new List<GameObject>();

    public List<GameObject> currentSpostsToHit = new List<GameObject>();

    private float minigameTimer = 0.02f;
    public bool IsInMinigame { get; set; } = false;

    private KnifeAndPanController knifeController;

    public Ingredient currentIngredient;

    private ArrowController arrowController;

    // Used if is panMinigame.
    private bool isIncrementing = true;


    private void Start()
    {
        knifeController = FindObjectOfType<KnifeAndPanController>();
        arrowController = FindObjectOfType<ArrowController>();

        //
        //
        //VASCO
        knifeSpeed = defaultKnifeSpeed;
        knifeSpeed = defaultKnifeSpeed;
        maxKnifeSpeed *= knifeSpeed;

        knifeUseSource = gameObject.AddComponent<AudioSource>();
        knifeUseSource.clip = knifeUseClip;
        knifeUseSource.playOnAwake = false;

        knifeThunkSource = gameObject.AddComponent<AudioSource>();
        knifeThunkSource.clip = knifeThunkClip;
        knifeThunkSource.playOnAwake = false;

        panUseSource = gameObject.AddComponent<AudioSource>();
        panUseSource.clip = panUseClip;
        panUseSource.playOnAwake = false;

        panLoopSource = gameObject.AddComponent<AudioSource>();
        panLoopSource.clip = panLoopClip;
        panLoopSource.playOnAwake = false;
        panLoopSource.loop = true;

        ingredientReadySource = gameObject.AddComponent<AudioSource>();
        ingredientReadySource.clip = ingredientReadyClip;
        ingredientReadySource.playOnAwake = false;

    }

    private void PopulateInitialSpotsToHit()
    {
        foreach (GameObject spot in spotsToHit)
        {
            currentSpostsToHit.Add(spot);
        }
    }

    // Called when an ingredient reaches the minigame.
    public void UpdateIngredientUI(GameObject taskUI)
    {
        GameObject taskUICreated = Instantiate(taskUI, new Vector3(-4.22f, 2.30f, 0f), Quaternion.identity);
        // Stay hidden until player clicks on spacebar.

        this.taskUI = taskUICreated;
        foreach (Transform child in taskUICreated.transform)
        {
            if (child.tag == "TaskUISelection")
            {
                taskUISelection = child.gameObject;
            }

            if (child.tag == "HitSpot")
            {
                spotsToHit.Add(child.gameObject);
            }
        }
        PopulateInitialSpotsToHit();


        //VASCO PLAY FRYIN PAN OIL SOUND
        panLoopSource.Play();
    }

    // Called when finishes minigame, clear everything.
    public void UpdateIngredientUI(string order)
    {
        if (order == "clear")
        {
            currentIngredient = null;
            taskUI = null;
            taskUISelection = null;
            spotsToHit.Clear();

            //VASCO PLAY SOUND WHEN INGREDIENT IS READY
            ingredientReadySource.Play();
            panLoopSource.Stop();


            //
        }
    }

    private void Update()
    {
        if (IsInMinigame)
        {
            if (isKnifeMinigame)
            {
                if (minigameTimer < 1f)
                {
                    minigameTimer += Time.deltaTime * knifeSpeed;
                }
                else
                {
                    minigameTimer = 0;
                }
            }
            // Is pan minigame.
            else
            {
                if (1 - minigameTimer < 0.05f || minigameTimer - 0 < 0.001f)
                {
                    isIncrementing = !isIncrementing;
                }

                if (isIncrementing)
                {
                    minigameTimer += Time.deltaTime * knifeSpeed;
                }
                else
                {
                    minigameTimer -= Time.deltaTime * knifeSpeed;

                }
            }

            taskUISelection.transform.localPosition = new Vector3(
                MapRangeClamped(minigameTimer, 0f, 1f, minXKnifeUISelection,
                maxXKnifeUISlection), 0, 0);
        }
    }

    public void CheckCorrectKnifeHit()
    {
        //VASCO PLAY USE SOUND EVERYTIME KNIFE IS USED EVEN IF MISSED
        knifeUseSource.Play();
        panUseSource.Play();

        foreach (GameObject spot in currentSpostsToHit)
        {
            IncreaseKnifeSpeed();

            if (spot.GetComponent<BoxCollider2D>().bounds.Contains(taskUISelection.transform.position))
            {
                Instantiate(GetCurrentIngredient().myParticleSystem.gameObject);

                spot.gameObject.SetActive(false);
                currentSpostsToHit.Remove(spot);

                //VASCO PLAY THUNK SOUND WHEN ITS IN THE RIGHT SPOT
                knifeThunkSource.PlayDelayed(knifeThunkDelay);


                // If finished the minigame.
                if (currentSpostsToHit.Count == 0)
                {
                    if (isKnifeMinigame)
                    {
                        GameObject cutObject = Instantiate(currentIngredient.gameObject);
                        cutObject.GetComponent<Ingredient>().CutIngredient();
                        Destroy(currentIngredient.gameObject);
                    }
                    else
                    {
                        GameObject cookedObject = Instantiate(currentIngredient.gameObject);
                        cookedObject.GetComponent<Ingredient>().CookIngredient();
                        Destroy(currentIngredient.gameObject);

                    }
                    // Reset minigame data.
                    knifeController.SetWillActivateMinigame(true);
                    IsInMinigame = false;
                    ActivateTaskUI(false);
                    knifeSpeed = defaultKnifeSpeed;
                    if (isKnifeMinigame)
                    {
                        minigameTimer = 0;
                    }
                    else
                    {
                        minigameTimer = 0.02f;
                    }
                    UpdateIngredientUI("clear");
                    arrowController.CanChangeTask = true;
                    return;
                }
                return;
            }
        }
    }

    //VASCO
    private void IncreaseKnifeSpeed()
    {
        if (knifeSpeed < maxKnifeSpeed)
        {
            knifeSpeed *= knifeSpeedIncrement;
        }
        else
        {
            knifeSpeed = maxKnifeSpeed;
        }
    }
    //

    public void ActivateTaskUI(bool value)
    {
        taskUI.SetActive(value);
        taskUISelection.SetActive(value);

        foreach (var spot in spotsToHit)
        {
            spot.SetActive(true);
        }
    }

    public float MapRangeClamped(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    public void SetCurrentIngredient(Ingredient ingredient)
    {
        currentIngredient = ingredient;
    }

    public Ingredient GetCurrentIngredient()
    {

        return currentIngredient;
    }
}
