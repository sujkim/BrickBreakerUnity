using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int points, speed;
    public int brickHits;


    public void MultipleHits()
    {
        brickHits--;
    }
}
