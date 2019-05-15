using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavmeshNode : MonoBehaviour {

    public int id;
    public NavmeshNode[] neighbours;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(NavmeshNode n in neighbours)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (NavmeshNode n in neighbours)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
            Gizmos.DrawWireCube(n.transform.position, new Vector3(0.5f,0.5f,0.5f));
        }
    }
}
