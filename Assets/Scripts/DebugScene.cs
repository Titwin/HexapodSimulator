using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
public class DebugScene : MonoBehaviour {

    [Header("links")]
    [SerializeField]
    Transform robot;
    [SerializeField]
    Transform totem0;
    [SerializeField]
    Transform totem1;
    [SerializeField]
    Transform totem2;

    [Header("status")]
    public float dTotem0;
    public float dTotem1;
    public float dTotem2;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        dTotem0 = Vector3.Distance(robot.position, totem0.position);
        dTotem1 = Vector3.Distance(robot.position, totem1.position);
        dTotem2 = Vector3.Distance(robot.position, totem2.position);
    }

    void OnDrawGizmos()

    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(robot.position, totem0.position);
        Gizmos.DrawLine(robot.position, totem1.position);
        Gizmos.DrawLine(robot.position, totem2.position);

        Handles.color = Color.white;
        Handles.Label((robot.position+ totem0.position)/2,
            ""+ dTotem0*10 +"cm");
        Handles.Label((robot.position + totem1.position) / 2,
           "" + dTotem1 * 10 + "cm");
        Handles.Label((robot.position + totem2.position) / 2,
           "" + dTotem2 * 10 + "cm");
    }
}
