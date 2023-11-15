using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    PlayerManager Instance;
    GameObject ladderObject;

    //Player Movement Variables
    public float climb_Speed;

    // Delay Jump Variables
    private bool delayJump = false;
    private float delayDuration = 0.4f; // Delay duration in seconds
    private float delayEndTime;

    bool mounted;
    bool overLadder;

    private void Start()
    {
        Instance = PlayerManager.Instance;
        mounted = false;
    }

    private void Update()
    {

        // If mounted on a ladder climb or descend ladder
        if (LadderMounted())
        {
            transform.position = new Vector2(ladderObject.transform.position.x, transform.position.y);
            if (Instance.M_Input().y > 0 || Instance.M_Input().y < 0)
            {
                transform.position += new Vector3(0, Instance.M_Input().y * climb_Speed * Time.deltaTime, 0);
            }
        }

        Instance.jumpException = JumpExceptionTimer();
    }

    // Mount / Dismount the player on the ladder
    bool LadderMounted()
    {
        if (overLadder)
        {
            if (Instance.M_Input().y < 0 || Instance.M_Input().y > 0)
            {
                mounted = true;
                Instance.R_Body.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            if (Instance.M_Input().x < 0 && Instance.M_Input().y == 0 || Instance.M_Input().x > 0 && Instance.M_Input().y == 0)
            {
                mounted = false;
                Instance.R_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else
        {
            mounted = false;
            Instance.R_Body.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        return mounted;
    }

    // Delay jump function
    bool JumpExceptionTimer()
    {
        if (LadderMounted())
        {
            delayJump = true;

            // Set the end time for the delay
            delayEndTime = Time.time + delayDuration;
        }

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
            overLadder = true;
            ladderObject = collision.gameObject;
        }
    }

    // Trigger exit detects when player is no longer touching a climbable surface
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            overLadder = false;
            ladderObject = null;
        }
    }
}
