using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    private List<Vector3> relativePositions;
    private List<MarbleBehaviour> marbles;
    private float maxVelocity;
    /// <summary>
    /// goal point or goal direction?
    /// </summary>
    public bool goalDirection;
    private Vector3 goal;

    public Formation(List<Vector3> relativePositions)
    {
        this.relativePositions = relativePositions;
        maxVelocity = float.PositiveInfinity;
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
        }
    }

    public void setMarbles(List<MarbleBehaviour> marbles)
    {
        maxVelocity = float.PositiveInfinity;
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
        Vector3 formationMemberCenterPoint = Vector3.zero;
        foreach (MarbleBehaviour marble in marbles)
        {
            formationMemberCenterPoint += marble.myRigidbody.position;
        }
        formationMemberCenterPoint /= marbles.Count;

        Vector3 intermediateGoal;
        if (goalDirection)
        {
            intermediateGoal = formationMemberCenterPoint + goal * maxVelocity * Time.fixedDeltaTime;
        }
        else
        {
            if ((goal - formationMemberCenterPoint).magnitude < maxVelocity * Time.fixedDeltaTime)
            {
                intermediateGoal = goal;
            }
            else
            {
                intermediateGoal = formationMemberCenterPoint + (goal - formationMemberCenterPoint).normalized * maxVelocity * Time.fixedDeltaTime;
            }
            
        }

        for (int i = 0; i < marbles.Count; i++)
        {
            //TODO rotate from x to target direction
            marbles[i].assignGoal(intermediateGoal + relativePositions[i]);
        }
    }
}

