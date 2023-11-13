using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    #region Class Variables
    PivotPoint PivotPointScript;

    // Spring joint attached to the player
    SpringJoint2D Joint;

    // Line renderer to act as grapple cable
    LineRenderer Line;

    // Parent objects
    GameObject Player;
    GameObject PivotPointObj;

    Vector3 jointPosition;

    // Layermask values for the grapple raycast to ignore
    int playerLayer = 1 << 3;
    int ladderLayer = 1 << 6;
    int ignoreLayers;

    // Max grapple distance
    public float maxLength;
    #endregion

    private void Start()
    {
        // Set script reference
        PivotPointScript = gameObject.transform.parent.gameObject.transform.parent.GetComponent<PivotPoint>();

        // Set parent references
        Player = gameObject.transform.parent.gameObject.transform.parent.gameObject;
        PivotPointObj = gameObject.transform.parent.gameObject;

        // Get joint from player parent and assign it to Joint,
        Joint = Player.GetComponent<SpringJoint2D>();
        jointPosition = Joint.connectedBody.transform.position;
        Joint.enabled = false;

        // Get line renderer
        Line = gameObject.GetComponent<LineRenderer>();
        Line.enabled = false;

        // Combine layermask values
        ignoreLayers = playerLayer + ladderLayer;
    }

    private void Update()
    {
        Joint.connectedBody.transform.position = jointPosition;

        SetJoint();
        TrackPointWithGun();
        DrawLine();
    }

    // Set position of the joint
    void SetJoint()
    {
        // Use raycast to set position of joint on the object directed to
        if (Input.GetMouseButtonDown(0))
        {
            Ray2D ray = new Ray2D(transform.position, PointDirection(transform.position, MousePosition()));
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength, ~ignoreLayers);

            if (hit.collider != null)
            {
                FindObjectOfType<AudioManager>().Play("Grapple Throw");
                //FindObjectOfType<AudioManager>().Play("Grapple Hit");
                Joint.enabled = true;
                jointPosition = hit.point;
            }
        }
        // Disable joint
        if (Input.GetMouseButtonUp(0))
        {
            Joint.enabled = false;
        }
    }

    // Set the gun to follow the joint position relative to the player
    void TrackPointWithGun()
    {
        // If grapple is deployed
        if (Joint.enabled)
        {
            PivotPointScript.PivotLocked(true);
            Vector3 direction = PointDirection(transform.position, jointPosition).normalized;
            PivotPointObj.transform.position = Player.transform.position + (direction * PivotPointScript.pivotDistanceFromPlayer);
        }
        else
        {
            PivotPointScript.PivotLocked(false);
        }
    }

    // Draw the grapple line from gun to the joint
    void DrawLine()
    {
        // Set both ends of the line to their supposed positions
        Line.SetPosition(0, transform.position);
        if (Joint.enabled)
        {
            Line.enabled = true;
            Line.SetPosition(1, jointPosition);
        }
        else
        {
            Line.enabled = false;
        }
    }

    // Get mouse position
    Vector2 MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // Get relative direction formula
    Vector3 PointDirection(Vector2 origin, Vector2 destination)
    {
        return (destination - origin).normalized;
    }
}
