using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Referable instance of THIS script
    public static PlayerManager Instance;

    // Scripts to adjust for gamestate
    [HideInInspector]
    public PlayerMovement PlayerMovementScript;
    GrappleHook GrappleHookScript;

    [HideInInspector]
    public PlayerState state;

    [HideInInspector]
    public Rigidbody2D R_Body;

    // Raycast layers to ignore
    [HideInInspector]
    public int ignoreLayers;

    // A stored jump for when player is not grounded
    [HideInInspector]
    public bool jumpException;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        R_Body = GetComponent<Rigidbody2D>();

        PlayerMovementScript = FindObjectOfType<PlayerMovement>();
        GrappleHookScript = FindObjectOfType<GrappleHook>();

        state = PlayerState.BaseState;

        ignoreLayers = (1 << 3) + (1 << 6);

        jumpException = false;
    }

    private void Update()
    {
        switch (state)
        {
            case PlayerState.BaseState:
                PlayerMovementScript.enabled = true;
                GrappleHookScript.enabled = true;
                break;

            case PlayerState.Climbing:
                PlayerMovementScript.enabled = true;
                GrappleHookScript.enabled = false;
                break;

            case PlayerState.Grappling:
                PlayerMovementScript.enabled = false;
                break;

            default:
                break;
        }
    }

    public enum PlayerState
    {
        BaseState,
        Climbing,
        Grappling
    }

    // Gets mouse position on screen    
    public Vector2 MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // WASD Input
    public Vector2 M_Input()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    // Returns a normalized direction vector based on origin and destination parameters
    public Vector3 RelativeDirection(Vector2 origin, Vector2 destination)
    {
        return (destination - origin).normalized;
    }

    // Ground check using raycasts
    public bool Grounded()
    {
        bool isGrounded = false;
        Ray2D leftRay = new Ray2D(new Vector2(transform.position.x - 0.4f, transform.position.y), transform.TransformDirection(0, -1, 0) * 0.6f);
        Ray2D centerRay = new Ray2D(new Vector2(transform.position.x, transform.position.y), transform.TransformDirection(0, -1, 0) * 0.6f);
        Ray2D rightRay = new Ray2D(new Vector2(transform.position.x + 0.4f, transform.position.y), transform.TransformDirection(0, -1, 0) * 0.6f);

        // If any raycasts detect a surface grounded is true
        if (Physics2D.Raycast(leftRay.origin, Vector2.down, 0.6f, ~ignoreLayers) || Physics2D.Raycast(centerRay.origin, Vector2.down, 0.6f, ~ignoreLayers) || Physics2D.Raycast(rightRay.origin, Vector2.down, 0.6f, ~ignoreLayers))
        {
            isGrounded = true;
        }
        else { isGrounded = false; }

        return isGrounded;
    }
}
