using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Rock : MonoBehaviour
{
    public Rigidbody2D rb;

    public float speed;
    public Vector2 startPos, startPaddle, velocity;
    public bool inPlay;
    public AudioSource hit;    
    public GameObject paddle;
    public GameManager gameManager;
    public GameObject instructionsPanel;

    // Start is called before the first frame update
        
    void Start()
    {
        // Set tag 
        gameObject.tag = "Player";

        rb = GetComponent<Rigidbody2D>();

        // get start position of ball
        startPos = rb.transform.position;
        startPaddle = paddle.transform.localPosition;
        instructionsPanel.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        // press space bar to put ball in play
        // if (Input.GetKeyDown("space") && !currentPlay && !gameManager._isLoading)
        if (Input.GetKeyDown("space") && !inPlay)
        {
            // unfreeze ball, set ball to in play
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
            inPlay = true;
            // directionsText.SetActive(false);

            // launch rock in random direction
            Launch();
        }


    }
    // Launch rock either left or right
    void Launch()
    {
        instructionsPanel.SetActive(false);
        if (Random.Range(0, 2) == 0)
        {
            rb.AddForce(new Vector2(4, 4));
        }
        else
        {
            rb.AddForce(new Vector2(-4, 4));
        }
    }

    private void FixedUpdate()
    {
        // if ball currently not in play, return paddle to start position
        if(!inPlay) {
            paddle.transform.localPosition = startPaddle;
        }

        // keep rock at constant speed
        rb.velocity = rb.velocity.normalized * speed;
        velocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // bounce ball off paddle and walls
        rb.velocity = Vector2.Reflect(velocity, other.contacts[0].normal);

        // change the rock's face on collision
        // this.gameObject.GetComponent<SpriteRenderer>().sprite = hitRock;

        // switch rock back

        // this.gameObject.GetComponent<SpriteRenderer>().sprite = rock;

        if (other.gameObject.CompareTag ("Brick") )
        {
            Brick brick = other.gameObject.GetComponent<Brick>();

            if (brick.brickHits > 1)
            {
                brick.MultipleHits();
            } else
            {
                Destroy(other.gameObject);
                GameManager.Instance.UpdateBricks(this.tag);
            }

            // play sound, update points for brick hit
            hit.Play();
            GameManager.Instance.UpdateScore(brick.points, this.tag); 

            // update speed based on brick hit
            if (gameManager.isMultiplayer) speed = brick.speed;
            else speed = 0.8f * brick.speed;
        }


    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bottom"))
        {
            // update # of lives
            gameManager.UpdateLives(this.tag);
            rb.velocity = Vector2.zero;

            inPlay = false;
            transform.position = startPos;

            // return paddle to start position and freeze
            paddle.transform.localPosition = startPaddle;
            
        }
    }
}
