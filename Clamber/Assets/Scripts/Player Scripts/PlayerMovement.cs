using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerManager Instance;

    //Player Movement Variables
    public float m_Speed;
    public float jumpForce;
    float timer;

    private void Start()
    {
        Instance = PlayerManager.Instance;
        timer = 0;
    }

    private void Update()
    {
        if (Instance.Grounded() || Instance.jumpException)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    private void FixedUpdate()
    {
        // If movement keys pressed move character
        if (Instance.M_Input().x > 0 || Instance.M_Input().x < 0)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        // If the player is in the air, apply a constant force in the current direction
        if (!Instance.Grounded())
        {
            Instance.R_Body.velocity = new Vector2(Instance.M_Input().x * m_Speed, Instance.R_Body.velocity.y);
        }
        else
        {
            // If the player is on the ground, move based on user input
            Instance.R_Body.position += new Vector2(Instance.M_Input().x * m_Speed * Time.deltaTime, 0f);

            float moveSoundTimer = FindObjectOfType<AudioManager>().SoundLength("Grass Walking");
            if (timer <= 0)
            {
                timer = moveSoundTimer;
                FindObjectOfType<AudioManager>().Play("Grass Walking");
            }
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
        }
    }

    public void Jump()
    {
        Instance.R_Body.velocity = Vector2.up * jumpForce;
    }
}