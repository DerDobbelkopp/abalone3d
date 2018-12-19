using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    private List<Vector3> relativePositions;
    public List<MarbleBehaviour> marbles;
    private float maxVelocity;
    private float maxAccelleration;
    float coherence = 0.9f;
    /// <summary>
    /// goal point or goal direction?
    /// </summary>
    public bool goalDirection;
    private Vector3 goal;

    public Formation(List<Vector3> relativePositions)
    {
        this.relativePositions = relativePositions;
        maxVelocity = float.PositiveInfinity;
        maxAccelleration = float.PositiveInfinity;
        this.marbles = new List<MarbleBehaviour>();
    }

    public void addMarble(MarbleBehaviour marble)
    {
        if (marbles.Count < relativePositions.Count)
        {
            marbles.Add(marble);
            if (marble.marbleMaxVelocity < maxVelocity)
            {
                maxVelocity = marble.marbleMaxVelocity;
            }
            if (marble.marbleStrength / marble.myRigidbody.mass < maxAccelleration)
            {
                maxAccelleration = marble.marbleStrength / marble.myRigidbody.mass;
            }
            marble.f = this;
        }
    }

    public void setMarbles(List<MarbleBehaviour> marbles)
    {
        maxVelocity = float.PositiveInfinity;
        maxAccelleration = float.PositiveInfinity;
        this.marbles = new List<MarbleBehaviour>();
        foreach (MarbleBehaviour marble in marbles)
        {
            addMarble(marble);
        }
    }

    public void assignGoal(Vector3 goal, bool goalDirection)
    {
        this.goal = goal;
        this.goalDirection = goalDirection;
    }

    public void move()
    {
        Vector3 center = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        for (int i = 0; i < marbles.Count; i++)
        {
            center += marbles[i].myRigidbody.position - relativePositions[i];
            velocity += marbles[i].myRigidbody.velocity;
        }
        center /= marbles.Count;
        velocity /= marbles.Count;


        Vector3 dir = goal - center;
        dir.y = 0; // ignore height differences

        Vector3 tgtVel = Vector3.ClampMagnitude(dir.normalized * Mathf.Sqrt((dir + velocity * Time.fixedDeltaTime).magnitude * maxAccelleration), maxVelocity);
        // calc a force proportional to the error (clamped to maxForce)
        for (int i = 0; i < marbles.Count; i++)
        {
            Vector3 dirMarble = center + relativePositions[i] - marbles[i].transform.position;
            dir.y = 0; // ignore height differences

            Vector3 tgtVelMarble = tgtVel + dirMarble.normalized * Mathf.Sqrt((dirMarble + marbles[i].myRigidbody.velocity * Time.fixedDeltaTime).magnitude * marbles[i].marbleStrength / marbles[i].myRigidbody.mass);
            // calc a force proportional to the error (clamped to maxForce)
            marbles[i].myRigidbody.GetComponent<ConstantForce>().force = Vector3.ClampMagnitude(20 * (tgtVelMarble - marbles[i].myRigidbody.velocity) * marbles[i].myRigidbody.mass, marbles[i].marbleStrength);
        }
    }
}

