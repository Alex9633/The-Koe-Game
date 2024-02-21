using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    Rigidbody2D rb;

    private bool isGrounded = false;
    private Vector3 m_Velocity = Vector3.zero;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;

    private bool onLadder = false;
    [SerializeField] private BoxCollider2D ladderPlatform;

    //private int OrbLevel = 0;
    private bool boostMode = false;

    [SerializeField] private GameObject player, player2, player3;

    private bool inWater = false;

    private new Animation animation;

    private bool pressedLeftAnim = false, waitForPressAnim = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animation = gameObject.GetComponent<Animation>();
    }

    private void Update()
    {
            if (!boostMode)
            {       
                if (!onLadder)
                {
                    CheckIfGrounded();
                    if (isGrounded) Jump();
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
            else
            {
                rb.velocity = new Vector2(35, 30);
                boostMode = false;
            }

            if(waitForPressAnim && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
            {
                pressedLeftAnim = true;
            }

        //transform.rotation = Quaternion.identity;
    }

    void FixedUpdate()
    {
        if (!boostMode)
        {
            Move();
            if (onLadder) Climb();
        }
    }

    void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if (inputX != 0 || rb.velocity.y >= 0)
        {
            Vector3 targetVelocity = new Vector2(inputX * speed, rb.velocity.y);
            if (onLadder) targetVelocity = new Vector2(inputX * (speed + 3), rb.velocity.y);
            if (inWater) targetVelocity = new Vector2(inputX * speed * .5f, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(!inWater) rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            else if (inWater && !player3.activeSelf) rb.velocity = new Vector2(rb.velocity.x, jumpForce / 3);
            else rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.75f);
        }
    }

    void Climb()
    {
        float inputY = Input.GetAxisRaw("Vertical");

        if (inputY != 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, inputY * 7);
        }
    }

    void CheckIfGrounded()
    {
        if (!inWater)
        {
            //Collider2D collider = Physics2D.OverlapBox(isGroundedChecker.position, new Vector2(1, 0), 0f, groundLayer);
            //Collider2D collider2 = Physics2D.OverlapBox(isGroundedChecker.position, new Vector2(1, 0), 0f, ground2Layer);
            if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -transform.up, GetComponent<CircleCollider2D>().radius + .2f, 1 << LayerMask.NameToLayer("Ground")) ||
                Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -transform.up, GetComponent<CircleCollider2D>().radius + .2f, 1 << LayerMask.NameToLayer("Ground2")) ||
                Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0.75f, -1), GetComponent<CircleCollider2D>().radius + .3f, 1 << LayerMask.NameToLayer("Ground")) ||
                Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0.75f, -1), GetComponent<CircleCollider2D>().radius + .3f, 1 << LayerMask.NameToLayer("Ground2")) ||
                Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(-0.75f, -1), GetComponent<CircleCollider2D>().radius + .3f, 1 << LayerMask.NameToLayer("Ground")) ||
                Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(-0.75f, -1), GetComponent<CircleCollider2D>().radius + .3f, 1 << LayerMask.NameToLayer("Ground2")))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = true;
        }
    }
    

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Ladder")
        {
            onLadder = true;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            ladderPlatform.enabled = false;
        }

        if(col.gameObject.layer == 4)
        {
            inWater = true;
            if (player.activeSelf) gameObject.GetComponent<Rigidbody2D>().gravityScale = .3f;
            else if (player2.activeSelf) gameObject.GetComponent<Rigidbody2D>().gravityScale = .15f;
            else gameObject.GetComponent<Rigidbody2D>().gravityScale = -1f;
        }

        if(col.gameObject.tag == "Orb")
        {
            if(player.activeSelf)
            {
                player2.SetActive(true);
                player2.transform.position = player.transform.position;
                player2.GetComponent<Rigidbody2D>().velocity = rb.velocity;
                player.SetActive(false);
            }
            else if(player2.activeSelf)
            {
                player3.SetActive(true);
                player3.transform.position = player2.transform.position;
                player3.GetComponent<Rigidbody2D>().velocity = rb.velocity;
                player2.SetActive(false);
            }
        }

        if (col.gameObject.name == "boost1")
        {
            boostMode = true;
        }

        if (col.gameObject.name == "boost2")
        {
            boostMode = true;
        }

        if (col.gameObject.tag == "Void")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(col.gameObject.name == "anim1")
        {
            animation.Play("ap1");
        }

        if (col.gameObject.name == "anim2")
        {
            animation.Play("ap2");
        }

        if (col.gameObject.name == "anim3")
        {
            animation.Play("ap3_1");
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "Ladder")
        {
            onLadder = false;
            //Debug.Log("No ladder");
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 2.5f;
            ladderPlatform.enabled = true;
        }

        if ((col.gameObject.layer == 4))
        {
            inWater = false;
            rb.gravityScale = 2.5f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spikes")
        {
            if(player.activeSelf)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if(player2.activeSelf)
            {
                player.SetActive(true);
                player.transform.position = player2.transform.position;
                player2.SetActive(false);
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(-rb.velocity.x, 20);
            }
            else if (player3.activeSelf)
            {
                player2.SetActive(true);
                player2.transform.position = player3.transform.position;
                player3.SetActive(false);
                player2.GetComponent<Rigidbody2D>().velocity = new Vector2(-rb.velocity.x, 20);
            }
        }

        if (collision.gameObject.tag == "DmgPlatform")
        {
            if (player3.activeSelf)
            {
                player2.SetActive(true);
                player2.transform.position = player3.transform.position;
                player3.SetActive(false);
                player2.GetComponent <Rigidbody2D>().velocity = new Vector2(rb.velocity.x, 10);
                player2.GetComponent<Animator>().SetBool("Taken damage", true);
            }
        }
    }

    void WaitFotPressAnim()
    {
        waitForPressAnim = true;
    }

    void exitAutopilot2()
    {
        if(pressedLeftAnim)
        {
            animation.Stop("ap3_1");
            animation.Play("ap3_1correct");
        }
        pressedLeftAnim = false;
        waitForPressAnim = false;
    }
}
