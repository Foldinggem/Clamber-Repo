using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    float lastInputX;

    // Delay Jump Variables
    private bool delayJump = false;
    private float delayDuration = 0.4f; // Delay duration in seconds
    private float delayEndTime;

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

        // QOL Update: If a player is mounted to a ladder and drops with d or a, they still have 0.3 of a second to hit space to jump
        //             This is to prevent the player from hitting space and (a/d) at the same time and not jumping

        if (LadderTimer() && onLadder)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
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
        // If the player is in the air, apply a constant force in the current direction
        if (!Grounded())
        {
            rb.velocity = new Vector2(inputX * m_Speed, rb.velocity.y);
        }
        else
        {
            // If the player is on the ground, move based on user input
            rb.position += new Vector2(inputX * m_Speed * Time.deltaTime, 0f);
        }
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

    // Delay jump function
    bool LadderTimer()
    {
        if (LadderMounted())
        {
            delayJump = true;

            // Set the end time for the delay
            delayEndTime = Time.time + delayDuration;
        }

        // Check if the delay period has passed
        if (delayJump && Time.time >= delayEndTime)
        {
            delayJump = false;
        }
        return delayJump;
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