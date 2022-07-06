using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAndPanController : MonoBehaviour
{
    private Animator myAnimator;

    private bool willActivateMinigame = true;

    private MinigameController knifeMinigameController;
    private MinigameController panMinigameController;
    private ArrowController arrowController;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        arrowController = FindObjectOfType<ArrowController>();
        CreateReferenceToMinigameControllers();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Is first hit.
            if (willActivateMinigame)
            {
                if (arrowController.IsInKnifeMinigame)
                {
                    if (knifeMinigameController.GetCurrentIngredient() != null)
                    {
                        knifeMinigameController.IsInMinigame = true;
                        //VASCO PLAY THE PICKUP SOUND (IS PLAYING EVERYTIME SPACE IS PRESSED????)
                        //knifeMinigameController.PlayPickupSound();
                        //
                        knifeMinigameController.ActivateTaskUI(true);
                        willActivateMinigame = false;
                    }
                }
                else
                {
                    if (panMinigameController.GetCurrentIngredient() != null)
                    {
                        panMinigameController.IsInMinigame = true;
                        panMinigameController.ActivateTaskUI(true);
                        willActivateMinigame = false;
                    }
                }
            }
            else
            {
                if (arrowController.IsInKnifeMinigame)
                {
                    if (knifeMinigameController.GetCurrentIngredient() != null)
                    {
                        myAnimator.SetTrigger("cut");
                        knifeMinigameController.CheckCorrectKnifeHit();
                    }
                }
                else
                {
                    if (panMinigameController.GetCurrentIngredient() != null)
                    {
                        myAnimator.SetTrigger("shake");
                        panMinigameController.CheckCorrectKnifeHit();
                    }
                }
            }
        }
    }

    public void SetWillActivateMinigame(bool value)
    {
        willActivateMinigame = value;
    }
}
