using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MLrock : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public int points = 10;
    public Vector2 startPos, startPaddle, velocity;
    public GameManager gameManager;
    public bool MLPlay;
    public MLagent Paddle;
    public int hitBrick;


    void Start()
    {
        // set tag
        gameObject.tag = "Agent";

        rb = GetComponent<Rigidbody2D>();
        startPos = rb.transform.position;
        startPaddle = Paddle.transform.localPosition;
        MLPlay = false;
    }


    void FixedUpdate()
    {
        // if game over or game won, no lives, ball and paddle disappears, no longer in play
        if (gameManager.agentGameOver || gameManager.agentGameWon || gameManager.agentNoLives)
        {
            gameObject.SetActive(false);
            Paddle.gameObject.SetActive(false);
            return;
        }

        // if ball currently not in play, return ball and paddle to start position
        if (!MLPlay & !gameManager._isLoading)
        {
            transform.position = startPos;
            
            // Invoke("Launch", 1.0f);
            Launch();
            MLPlay = true;
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }

        // keep ball at constant speed
        rb.velocity = rb.velocity.normalized * speed;
        velocity = rb.velocity;
    }

    void Launch()
    {
        if (Random.Range(0, 2) == 0)
        {
            rb.AddForce(new Vector2(4, 4));
        }
        else
        {
            rb.AddForce(new Vector2(-4, 4));
        }
    }



    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bottom"))
        {
            Debug.Log("We hit the bottom!");
            // update Ball count
            gameManager.UpdateLives(this.tag);

            // zero out ball's velocity
            rb.velocity = Vector2.zero;

            // update ball to not in play
            MLPlay = false;

            // return ball and paddle to start positions
            transform.position = startPos;
            Paddle.transform.localPosition = startPaddle;

        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        // bounce ball off paddle and walls
        rb.velocity = Vector2.Reflect(velocity, other.contacts[0].normal);

        // if ball collides with brick
        if (other.gameObject.CompareTag("Brick"))
        {
            Brick brick = other.gameObject.GetComponent<Brick>();

            if (brick.brickHits > 1)
            {
                brick.MultipleHits();
            } 
            else
            {
                hitBrick++;
                Destroy(other.gameObject);
                GameManager.Instance.UpdateBricks(this.tag);
            }
            GameManager.Instance.UpdateScore(brick.points, this.tag);

            // update speed
            speed = brick.speed;
        }
    }
}

