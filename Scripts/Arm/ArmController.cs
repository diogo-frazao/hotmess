using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    [SerializeField]
    private float mouseSpeed = 5f;

    [SerializeField]
    private Sprite openHandSprite;

    [SerializeField]
    private Sprite closedHandSprite;

    private SpriteRenderer mySpriteRenderer;


    private void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        MoveMouseClamped();

        SetMouseSprites();
    }

    private void MoveMouseClamped()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        transform.position = Vector3.Lerp(transform.position, mousePosition, mouseSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 1.13f, 9f),
                             Mathf.Clamp(transform.position.y, -5.38f, 2.25f),
                             0f);
    }

    private void SetMouseSprites()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mySpriteRenderer.sprite = closedHandSprite;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mySpriteRenderer.sprite = openHandSprite;
        }
    }
}
