using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    // Parent to the grapple gun and child to the player, marks where the gun should be
    public GameObject PivotPointObject;

    public float pivotDistanceFromPlayer;

    bool locked;

    private void Start()
    {
        locked = false;
    }

    private void Update()
    {
        // Updates the position of the pivot point relative to player and mouse position
        if (!locked)
        {
            PivotPointHandeler(MousePositionOnScreen());
        }
    }

    // Pivot point location function
    private void PivotPointHandeler(Vector3 mousePos)
    {
        Vector3 direction = (mousePos - transform.position).normalized;
        PivotPointObject.transform.position = transform.position + direction * pivotDistanceFromPlayer;
    }

    // Get mouse position relative to the screen
    public Vector2 MousePositionOnScreen()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void PivotLocked(bool newLocked)
    {
        locked = newLocked;
    }
}
