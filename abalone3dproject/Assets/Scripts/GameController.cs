using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public bool godModeActivated;
    public string playerMarbleType;
    public List<MarbleBehaviour> selectedMarbles;
    private HashSet<MarbleBehaviour> unassignedMarbles;

    public List<MarbleBehaviour> marbles;
    public List<Formation> formations;

    // Use this for initialization
    void Start()
    {
        formations = new List<Formation>();
        List<Vector3> relativePositions = new List<Vector3>();
        relativePositions.Add(new Vector3(0, 0, 0));
        relativePositions.Add(new Vector3(5, 0, 0));
        relativePositions.Add(new Vector3(-2, 0, 0));
        Formation straightFormation = new Formation(relativePositions);
        formations.Add(straightFormation);
        unassignedMarbles = new HashSet<MarbleBehaviour>(marbles);
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.tag.Contains("Marble"))
                {

                    if (godModeActivated || hit.collider.gameObject.tag == playerMarbleType)
                    {
                        selectedMarbles.Add(hit.collider.gameObject.GetComponent<MarbleBehaviour>());
                    }
                }
                else if (hit.collider.gameObject.tag.Contains("Ground"))
                {
                    if (Input.GetKey("f"))
                    {
                        //TODO work
                        Formation f = formations[0];
                        unassignedMarbles.UnionWith(f.marbles);
                        unassignedMarbles.ExceptWith(selectedMarbles);
                        f.setMarbles(selectedMarbles);
                        f.assignGoal(hit.point, false);
                    }
                    else
                    {
                        foreach (MarbleBehaviour selected in selectedMarbles)
                        {
                            selected.assignGoal(hit.point);
                        }
                    }
                    selectedMarbles.Clear();
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Formation formation in formations)
        {
            formation.move();
        }
        foreach (MarbleBehaviour marble in unassignedMarbles)
        {
            marble.move();
        }

    }

}
//Testbehaviour (Push in static direciton)
/*if (hit.collider.gameObject.tag.Contains("Marble"))
{
    if (hit.collider.gameObject.tag.Contains("White"))
    {
        Rigidbody selected = hit.collider.gameObject.GetComponent<Rigidbody>();
        selected.AddForce(new Vector3(0, 0, Random.Range(30,80)));
    }
    else if (hit.collider.gameObject.tag.Contains("Black"))
    {
        Rigidbody selected = hit.collider.gameObject.GetComponent<Rigidbody>();
        selected.AddForce(new Vector3(0, 0, -60));
    }
}*/
