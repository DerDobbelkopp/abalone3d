using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleBehaviour : MonoBehaviour
{

    public float marbleStrength;

    public Rigidbody myRigidbody;

    public bool keepRolling = false;
    private bool reached = false;

    public Vector3 targetPosition;

    public float marbleMaxVelocity;


    // Use this for initialization
    void Start()
    {
        myRigidbody = this.gameObject.GetComponent<Rigidbody>();
        marbleMaxVelocity = Mathf.Sqrt(marbleStrength / (myRigidbody.drag + myRigidbody.angularDrag));
    }
	
    // Update is called once per frame
    public void move()
    {
        Vector3 dist = targetPosition - transform.position;
        dist.y = 0; // ignore height differences
        if (!keepRolling)
        {
            // calc a target vel proportional to distance (clamped to maxVel)
            Vector3 tgtVel = 1.5f * dist;// Vector3.ClampMagnitude(toVel * dist, maxVel);
            // calculate the velocity error
            Vector3 error = tgtVel - myRigidbody.velocity;
            // calc a force proportional to the error (clamped to maxForce)
            myRigidbody.GetComponent<ConstantForce>().force = Vector3.ClampMagnitude(5 * error, marbleStrength);
        }
        else if (!reached)
        {
            myRigidbody.GetComponent<ConstantForce>().force = dist.normalized * marbleStrength;
            if (dist.magnitude < 0.1f)
            {
                reached = true;
            }
        }
        else
        {
            myRigidbody.GetComponent<ConstantForce>().force = -dist.normalized * marbleStrength;
        }
    }

    public void assignGoal(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        this.reached = false;
    }

    public void setSelected(bool selected)
    {
        // Create/Remove outline here
    }
}
