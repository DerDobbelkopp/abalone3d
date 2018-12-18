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
        Vector3 dir = targetPosition - transform.position;
        dir.y = 0; // ignore height differences
        if (!keepRolling)
        {
            Vector3 tgtVel = dir.normalized * Mathf.Sqrt((dir + myRigidbody.velocity * Time.fixedDeltaTime).magnitude * marbleStrength / myRigidbody.mass);
            // calc a force proportional to the error (clamped to maxForce)
            myRigidbody.GetComponent<ConstantForce>().force = Vector3.ClampMagnitude(20 * (tgtVel - myRigidbody.velocity) * myRigidbody.mass, marbleStrength);
        }
        else if (!reached)
        {
            myRigidbody.GetComponent<ConstantForce>().force = dir.normalized * marbleStrength;
            if (dir.magnitude < 0.1f)
            {
                reached = true;
            }
        }
        else
        {
            myRigidbody.GetComponent<ConstantForce>().force = -dir.normalized * marbleStrength;
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
