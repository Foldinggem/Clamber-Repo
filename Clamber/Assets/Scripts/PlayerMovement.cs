using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    GameObject ladderObject;

    //Player Movement Variables
    public float m_Speed;
    public float climb_Speed;
    public float jumpForce;
    float inputX;
    float inputY;

    int playerLayer;
    int ladderLayer;
    int raycastIgnoreLayers;

    bool mounted;
    bool onLadder;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mounted = false;
    }

    private void Update()
    {
        // Getting the layers in binary format for layermask
        playerLayer = 1 << 3;
        ladderLayer = 1 << 6;
        raycastIgnoreLayers = playerLayer + ladderLayer;
        // Get key inputs from the unity input system
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        // Call jump function if player is grounded and key is pressed
        if (Grounded() || LadderMounted())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (onLadder)
                {
                    // Jump off the ladder
                    mounted = false;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rb.velocity = Vector2.up * jumpForce;
                }
                else
                {
                    Jump();
                    mounted = false;
                }
            }
        }

        // If mounted on a ladder climb or descend ladder
        if (LadderMounted())
        {
            transform.position = new Vector2(ladderObject.transform.position.x, transform.position.y);
            if (inputY > 0 || inputY < 0)
            {
                transform.position += new Vector3(0, inputY * climb_Speed * Time.deltaTime, 0);
            }
            else
            {
                transform.position = transform.position;
            }
        }
    }

    private void FixedUpdate()
    {
        // If movement keys pressed move character
        if (inputX > 0 || inputX < 0)
        {
            MovePlayer();
        }


    }

    void MovePlayer()
    {
        rb.position += new Vector2(inputX * m_Speed * Time.deltaTime, 0f);
    }

    void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
    }

    // Ground check using raycasts
    bool Grounded()
    {
        bool isGrounded = false;
        Ray2D leftRay = new Ray2D(new Vector2(transform.position.x - 0.4f, transform.position.y), transform.TransformDirection(0, -1, 0) * 0.6f);
        Ray2D centerRay = new Ray2D(new Vector2(transform.position.x, transform.position.y), transform.TransformDirection(0, -1, 0) * 0.6f);
        Ray2D rightRay = new Ray2D(new Vector2(transform.position.x + 0.4f, transform.position.y), transform.TransformDirection(0, -1, 0) * 0.6f);

        // If any raycasts detect a surface grounded is true
        if (Physics2D.Raycast(leftRay.origin, Vector2.down, 0.6f, ~raycastIgnoreLayers) || Physics2D.Raycast(centerRay.origin, Vector2.down, 0.6f, ~raycastIgnoreLayers) || Physics2D.Raycast(rightRay.origin, Vector2.down, 0.6f, ~raycastIgnoreLayers) || LadderMounted())
        {
            isGrounded = true;
        }
        else { isGrounded = false; }

        return isGrounded;
    }

    // Mount / Dismount the player on the ladder
    bool LadderMounted()
    {
        if (onLadder)
        {
            if (inputY < 0 || inputY > 0)
            {
                mounted = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            if (inputX < 0 && inputY == 0 || inputX > 0 && inputY == 0)
            {
                mounted = false;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else
        {
            mounted = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        return mounted;
    }

    // Trigger detects when player is touching a climbable surface
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            onLadder = true;
            ladderObject = collision.gameObject;
        }
    }

    // Trigger exit detects when player is no longer touching a climbable surface
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            onLadder = false;
            ladderObject = null;
        }
    }
}