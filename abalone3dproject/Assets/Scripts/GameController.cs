using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public bool godModeActivated;
    public string playerMarbleType;
    public List<Rigidbody> selectedMarbles;

    public List<Formation> formations = new List<Formation>();

	// Use this for initialization
	void Start () {
        List<Vector3> relativePositions = new List<Vector3>();
        relativePositions.Add(new Vector3(0, 0, 0));
        relativePositions.Add(new Vector3(2, 0, 0));
        relativePositions.Add(new Vector3(-2, 0, 0));
        Formation straightFormation = new Formation(relativePositions);
        formations.Add(straightFormation);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (godModeActivated)
                {
                    if (hit.collider.gameObject.tag.Contains("Marble"))
                    {
                        selectedMarbles.Add(hit.collider.gameObject.GetComponent<Rigidbody>());
                    }
                    else if (hit.collider.gameObject.tag.Contains("Ground"))
                    {
                        foreach (Rigidbody selected in selectedMarbles)
                        {
                            selected.GetComponent<MarbleBehaviour>().moveTo(hit.point);
                        }
                        selectedMarbles.Clear();
                    }
                }
                //Replace this with whatever logic you want to use to validate the objects you want to click on
                else if (hit.collider.gameObject.tag == playerMarbleType)
                {
                    selectedMarbles.Add(hit.collider.gameObject.GetComponent<Rigidbody>());
                }

                if (hit.collider.gameObject.tag.Contains("Ground"))
                {
                    if (Input.GetKeyDown("F"))
                    {
                        Formation f = formations[0];
                        if (selectedMarbles.Count < f.relativePositions.Count)
                        {
                            for (int index = 0; index < selectedMarbles.Count; index++)
                            {
                                selectedMarbles[index].GetComponent<MarbleBehaviour>().moveToWithFormation(hit.point, f.relativePositions[index], selectedMarbles);
                            }
                        }
                    }
                    else
                    {
                        foreach (Rigidbody selected in selectedMarbles)
                        {
                            selected.GetComponent<MarbleBehaviour>().moveTo(hit.point);
                        }
                    }
                }
            }
        }
    }

}

public class Formation
{
    public List<Vector3> relativePositions;
    public Formation(List<Vector3> relativePositions)
    {
        this.relativePositions = relativePositions;
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
