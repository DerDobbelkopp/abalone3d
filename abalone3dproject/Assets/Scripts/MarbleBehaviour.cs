using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleBehaviour : MonoBehaviour {

    public float marbleMaxVelocity; //Nutzer wird marbleMaxSpeed sehen
    public float marbleStrength;

    public float factorRelCount; //Gewichtungsfaktor für die relative Position der Kugel in einer Formation

    private Rigidbody myRigidbody;

    private bool movingToLocationActive = false;
    private Vector3 targetPosition;
    private Vector3 relativePosition;
    private List<Rigidbody> formationMembers;

    //Parameters for marble-moving behaviour
    public float stopDistance;

    // Use this for initialization
    void Start () {
        myRigidbody = this.gameObject.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (movingToLocationActive)
        {
            Vector3 targetDirection = targetPosition - myRigidbody.position;
            Vector3 relativeDirection = Vector3.zero;

            if (!relativePosition.Equals(Vector3.zero)) {

                Vector3 formationMemberCenterPoint = Vector3.zero;
                foreach (Rigidbody member in formationMembers)
                {
                    formationMemberCenterPoint += member.position;
                }
                formationMemberCenterPoint /= formationMembers.Count;
                relativeDirection = formationMemberCenterPoint - myRigidbody.position + relativePosition;
            }

            if (targetDirection.magnitude < stopDistance)
            {
                myRigidbody.GetComponent<ConstantForce>().force = Vector3.zero;
                movingToLocationActive = false;
            }
            else
            {
                Vector3 targetDirectionNormalized = targetDirection.normalized;
                Vector3 relativeDirectionNormalized = relativeDirection.normalized;

                Vector3 appliedForce = marbleStrength * ((1-factorRelCount) * targetDirectionNormalized + factorRelCount * relativeDirectionNormalized);

                if (appliedForce.magnitude > marbleMaxVelocity) appliedForce = appliedForce.normalized*marbleMaxVelocity;

                myRigidbody.GetComponent<ConstantForce>().force = appliedForce;
            }
        }
	}

    public List<Rigidbody> getFormationMembers()
    {
        return formationMembers;
    }

    public void moveTo(Vector3 targetPosition)
    {
        movingToLocationActive = true;
        this.targetPosition = targetPosition;
        this.relativePosition = Vector3.zero;
    }

    public void moveToWithFormation(Vector3 targetPosition, Vector3 relativePosition, List<Rigidbody> formationMembers)
    {
        movingToLocationActive = true;
        this.targetPosition = targetPosition;
        this.relativePosition = relativePosition;
    }

    public void setSelected(bool selected)
    {
        // Create/Remove outline here
    }
}
