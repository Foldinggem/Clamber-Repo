using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [HideInInspector]
    public PlayerState state;

    [HideInInspector]
    public static Action<PlayerState> OnStateChange;

    [HideInInspector]
    public Rigidbody2D R_Body;

    [HideInInspector]
    public int ignoreLayers;

    [HideInInspector]
    public bool jumpException;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        R_Body = GetComponent<Rigidbody2D>();

        UpdatePlayerState(PlayerState.BaseState);

        ignoreLayers = (1 << 3) + (1 << 6);

        jumpException = false;
    }

    public void UpdatePlayerState(PlayerState newState)
    {
        state = newState;

        switch (newState)
        {
            case PlayerState.BaseState:
                break;

            case PlayerState.Climbing:
                break;

            case PlayerState.Grappling:
                break;

            default:
                break;
        }

        OnStateChange?.Invoke(newState);
    }

    public enum PlayerState
    {
        BaseState,
        Climbing,
        Grappling
    }

    public Vector2 MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public Vector2 M_Input()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

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
