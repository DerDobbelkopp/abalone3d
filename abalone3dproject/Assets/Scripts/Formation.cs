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
    bool formationRotates;
    Vector3 oldCenter;
    Quaternion oldRot;
    float remainingVel;

    public Formation(List<Vector3> relativePositions)
    {
        this.relativePositions = relativePositions;
        maxVelocity = float.PositiveInfinity;
        maxAccelleration = float.PositiveInfinity;
        this.marbles = new List<MarbleBehaviour>();
        formationRotates = true;
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

    public void setMarbles(IEnumerable<MarbleBehaviour> newMarbles)
    {
        maxVelocity = float.PositiveInfinity;
        maxAccelleration = float.PositiveInfinity;
        this.marbles = new List<MarbleBehaviour>();
        foreach (MarbleBehaviour marble in newMarbles)
        {
            addMarble(marble);
        }
        {// oldRot, oldcenter
            Vector3 center = Vector3.zero;
            for (int i = 0; i < marbles.Count; i++)
            {
                center += marbles[i].myRigidbody.position;
            }
            center /= marbles.Count;

            Quaternion rot = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.right, goal - center, Vector3.up), 0);

            Vector3[] dependendRelativePositions = new Vector3[marbles.Count];
            for (int i = 0; i < marbles.Count; i++)
            {
                dependendRelativePositions[i] = rot * relativePositions[i];
            }

            oldCenter = Vector3.zero;
            for (int i = 0; i < marbles.Count; i++)
            {
                oldCenter += marbles[i].myRigidbody.position - dependendRelativePositions[i];
            }
            oldCenter /= marbles.Count;


            oldRot = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.right, goal - oldCenter, Vector3.up), 0);
        }
    }

    public void assignGoal(Vector3 goal, bool goalDirection)
    {
        this.goal = goal;
        this.goalDirection = goalDirection;
    }

    public void move()
    {
        Vector3 velocity = Vector3.zero;
        for (int i = 0; i < marbles.Count; i++)
        {
            velocity += marbles[i].myRigidbody.velocity;
        }
        velocity /= marbles.Count;
	
        Vector3 approxDir = oldCenter + velocity * Time.fixedDeltaTime - goal;
        Quaternion rot;
        if (approxDir.magnitude > 3f && formationRotates)
        {
            rot = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.right, approxDir, Vector3.up), 0);
        }
        else
        {
            rot = oldRot;
        }
        oldRot = rot;


        Vector3[] dependendRelativePositions = new Vector3[marbles.Count];
        for (int i = 0; i < marbles.Count; i++)
        {
            dependendRelativePositions[i] = rot * relativePositions[i];
        }


        Vector3 center = Vector3.zero;
        for (int i = 0; i < marbles.Count; i++)
        {
            center += marbles[i].myRigidbody.position - dependendRelativePositions[i];
        }
        center /= marbles.Count;
        oldCenter = center;


        for (int i = 0; i < marbles.Count; i++)
        {
            rot = Quaternion.Euler(0, 0.5f * Vector3.SignedAngle(dependendRelativePositions[i], marbles[i].myRigidbody.position - center, Vector3.up), 0);
            dependendRelativePositions[i] = (rot * dependendRelativePositions[i]);
        }


        Vector3 dir = goal - center;
        dir.y = 0; // ignore height differences

        Vector3 wholeFormationVel = Vector3.ClampMagnitude(dir.normalized * Mathf.Sqrt((dir + velocity * Time.fixedDeltaTime).magnitude * maxAccelleration), remainingVel);
        remainingVel = float.PositiveInfinity;
        // calc a force proportional to the error (clamped to maxForce)
        for (int i = 0; i < marbles.Count; i++)
        {
            Vector3 dirMarble = center + dependendRelativePositions[i] - marbles[i].transform.position;
            dir.y = 0; // ignore height differences

            Vector3 marblePosVel = dirMarble.normalized * Mathf.Sqrt((dirMarble + marbles[i].myRigidbody.velocity * Time.fixedDeltaTime).magnitude * marbles[i].marbleStrength / marbles[i].myRigidbody.mass);

            remainingVel = Mathf.Min(remainingVel, marbles[i].marbleMaxVelocity - (Vector3.Cross(marblePosVel, dir.normalized).magnitude + Mathf.Max(0, Vector3.Dot(marblePosVel, dir.normalized))));

            // calc a force proportional to the error (clamped to maxForce)
            marbles[i].myRigidbody.GetComponent<ConstantForce>().force = Vector3.ClampMagnitude(10 * (marblePosVel + wholeFormationVel - marbles[i].myRigidbody.velocity) * marbles[i].myRigidbody.mass, marbles[i].marbleStrength);
        }
    }
}

