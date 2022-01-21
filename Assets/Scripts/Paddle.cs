using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    // public Rigidbody2D paddleRb;
    public float speed = 1f;
    public float leftBound, rightBound;
    public Rock rock;

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        // moves paddle using right and left arrow keys
        float x = Input.GetAxis("Horizontal");

        // move only when ball is in play
        if (rock.inPlay)
        {
            transform.Translate(Vector2.right * x * Time.deltaTime * speed);
            if (transform.localPosition.x < leftBound)
            {
                transform.localPosition = new Vector2(leftBound, transform.localPosition.y);
            }
            if (transform.localPosition.x > rightBound)
            {
                transform.localPosition = new Vector2(rightBound, transform.localPosition.y);
            }
        }


    }
}
