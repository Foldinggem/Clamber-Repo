using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    #region Class Variables
    PivotPoint PivotPointScript;
    PlayerManager Instance;

    // Spring joint attached to the player
    SpringJoint2D Joint;

    // Line renderer to act as grapple cable
    LineRenderer Line;

    // Parent objects
    GameObject Player;
    GameObject PivotPointObj;

    Vector3 jointPosition;

    // Max grapple distance
    public float maxLength;
    #endregion

    private void Start()
    {
        // Set script reference
        PivotPointScript = gameObject.transform.parent.gameObject.transform.parent.GetComponent<PivotPoint>();
        Instance = PlayerManager.Instance;

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
    }

    private void Update()
    {
        Joint.connectedBody.transform.position = jointPosition;

        SetJoint();
        TrackPointWithGun();
        DrawLine();

        if(Joint.enabled)
        {
            Instance.state = PlayerManager.PlayerState.Grappling;
        }
        else
        {
            Instance.PlayerMovementScript.enabled = true;
        }
    }

    // Set position of the joint
    void SetJoint()
    {
        // Use raycast to set position of joint on the object directed to
        if (Input.GetMouseButtonDown(0))
        {
            Ray2D ray = new Ray2D(transform.position, Instance.RelativeDirection(transform.position, Instance.MousePosition()));
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxLength, ~Instance.ignoreLayers);

            if (hit.collider != null)
            {
                FindObjectOfType<AudioManager>().Play("Grapple Throw");
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
            PivotPointObj.transform.position = Player.transform.position + (Instance.RelativeDirection(transform.position, jointPosition).normalized * PivotPointScript.pivotDistanceFromPlayer);
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

    // When disabled by player manager reset gameobjects
    private void OnDisable()
    {
        Joint.enabled = false;
        Line.enabled = false;
    }
}
