using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public PivotPoint PivotPointScript;

    public LineRenderer GrappleLine;

    public GameObject PivotPoint;
    public GameObject Player;

    public Vector3 launchPoint;
    public Vector3 mousePos;
    Vector2 mouseClickedPos;

    bool canUpdateClickPos;

    public float maxGrappleLength;
    public float grappleSpeed;
    float currentGrappleLength;

    private void Start()
    {
        canUpdateClickPos = true;
        currentGrappleLength = 0;
    }

    private void Update()
    {
        launchPoint = transform.position;
        GrappleLine.SetPosition(0, launchPoint);
        mousePos = PivotPointScript.MousePositionOnScreen();
        GrappleObject();
    }

    void GrappleObject()
    {
        if (canUpdateClickPos)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PivotPointScript.ChangeLocked(true);
                mouseClickedPos = mousePos;
                canUpdateClickPos = false;
            }
        }
        else
        {
            if (currentGrappleLength < maxGrappleLength)
            {
                currentGrappleLength += grappleSpeed * Time.deltaTime;
            }
            GrappleLine.SetPosition(1, transform.position + PointDirection(transform.position, mouseClickedPos) * currentGrappleLength);
            PivotPoint.transform.position = Player.transform.position + PointDirection(Player.transform.position, mouseClickedPos);

            if (Input.GetMouseButtonUp(0))
            {
                currentGrappleLength = 0;
                PivotPointScript.ChangeLocked(false);
                canUpdateClickPos = true;
            }
        }
        
        if(canUpdateClickPos)
        {
            GrappleLine.SetPosition(1, transform.position);
        }
    }

    Vector3 PointDirection(Vector2 origin, Vector2 destination)
    {
        return (destination - origin).normalized;
    }
}
