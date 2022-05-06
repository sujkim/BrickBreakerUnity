using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.Barracuda;


public class MLagent : Agent
{
    public Rigidbody2D paddlerb;
    public MLrock rock;
    public float speed;
    Vector3 ballLocation;
    public int collisions, numberBricks, bricksCurrent;
    public Vector3 startPaddle;
    public float left_bound, right_bound;
    public GameObject[] bricks;
    public GameObject bricksPrefab, agentBricks;
    public GameManager gameManager;

    public NNModel Normal;
    public NNModel Hard;


    public override void Initialize()
    {
        paddlerb = GetComponent<Rigidbody2D>();
        speed = 7.0f;
        left_bound = -6.6f;
        right_bound = 6.6f;
        ballLocation = rock.transform.localPosition;
        startPaddle = transform.localPosition;
    }


    public override void OnEpisodeBegin()
    {

        if (this.transform.localPosition.x != 0)
        {
            this.transform.localPosition = startPaddle;
        }
        bricks = GameObject.FindGameObjectsWithTag("Brick");
        collisions = 0;

        /* Use for training */
        // reset the bricks for training
        // foreach(GameObject brick in bricks) {
        //     GameObject.Destroy(brick);
        // }

        // Instantiate(bricksPrefab, agentBricks.transform);

        numberBricks = GameObject.FindGameObjectsWithTag("Brick").Length;
        bricks = GameObject.FindGameObjectsWithTag("Brick");

        bricksCurrent = 0;
    }


    public override void CollectObservations(VectorSensor sensor)
    {

        // get position of the paddle 
        sensor.AddObservation(this.transform.position.x);

        //velocity of paddle
        sensor.AddObservation(paddlerb.velocity);

        //position of Ball
        sensor.AddObservation(rock.transform.localPosition);

        // distance from paddle to rock
        float distancePaddleBall = Vector2.Distance(this.transform.localPosition, rock.transform.localPosition);
        sensor.AddObservation(distancePaddleBall);

        // direction of rock relative to center of paddle
        var heading = rock.transform.localPosition - this.transform.localPosition;
        var distance = heading.magnitude;
        var direction = heading / distance;
        sensor.AddObservation(direction);

        // velocity of rock
        sensor.AddObservation(rock.velocity);

        // bricks
        bricks = GameObject.FindGameObjectsWithTag("Brick");

    }


    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

        //move paddle 
        Vector2 controlSignal = Vector2.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];

        if (!gameManager._isLoading && rock.MLPlay)
        {
            this.transform.localPosition += new Vector3(controlSignal.x, 0, 0) * Time.deltaTime * speed;

            if (this.transform.localPosition.x < left_bound)
            {
                this.transform.localPosition = new Vector2(left_bound, this.transform.localPosition.y);
            }
            if (this.transform.localPosition.x > right_bound)
            {
                this.transform.localPosition = new Vector2(right_bound, this.transform.localPosition.y);
            }
        }

        // Set Rewards - if rock falls, large penalty
        if (rock.transform.localPosition.y < transform.localPosition.y - 0.2f)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        else
        {
            // small reward for rock movement
            AddReward(0.01f);
        }

        // if all bricks are cleared, large reward
        if (GameObject.FindGameObjectsWithTag("Brick").Length == 0)
        {
            AddReward(1.0f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        AddReward(0.3f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
    }



}
