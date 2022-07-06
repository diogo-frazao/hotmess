using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private GameObject selectionArrow = null;
    private bool isArrowInKnifeMinigame = true;
    private Vector3[] arrowPositions = new[] { new Vector3(-6.2f, -2.4f, 0), new Vector3(-2.62f, -2.4f, 0) };

    public bool IsInKnifeMinigame { get; private set; } = true;

    public bool CanChangeTask { get; set; } = true;

    [SerializeField]
    private GameObject knifeMinigame;

    [SerializeField]
    private GameObject panMinigame;

    [SerializeField]
    private AudioClip panPickupClip;
    private AudioSource panPickupSource;

    [SerializeField]
    private AudioClip knifePickupClip;
    private AudioSource knifePickupSource;


    private void Start()
    {
        selectionArrow = GameObject.FindGameObjectWithTag("SelectionArrow");

        knifePickupSource = gameObject.AddComponent<AudioSource>();
        knifePickupSource.clip = knifePickupClip;
        knifePickupSource.playOnAwake = false;

        panPickupSource = gameObject.AddComponent<AudioSource>();
        panPickupSource.clip = panPickupClip;
        panPickupSource.playOnAwake = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && CanChangeTask)
        {
            selectionArrow.transform.position = arrowPositions[0];
            isArrowInKnifeMinigame = true;
        }

        if (Input.GetKeyDown(KeyCode.D) && CanChangeTask)
        {
            isArrowInKnifeMinigame = false;
            selectionArrow.transform.position = arrowPositions[1];
        }

        if (Input.GetKeyDown(KeyCode.Space) && CanChangeTask)
        {
            if (isArrowInKnifeMinigame)
            {
                IsInKnifeMinigame = true;
                knifeMinigame.SetActive(true);
                panMinigame.SetActive(false);
                knifePickupSource.Play();
            }
            // Arrow is in pan minigame.
            else
            {
                IsInKnifeMinigame = false;
                panMinigame.SetActive(true);
                knifeMinigame.SetActive(false);
                panPickupSource.Play();
            }
        }
    }
}
