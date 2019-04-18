using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegBehaviour : MonoBehaviour
{
    //  Public attributes
    public GameObject originObject;
    public GameObject springObject;
    public Transform springMesh;
    
    private Transform springT;
    private Rigidbody originRB;
    private Rigidbody springRB;

    [Space(10)]

    public float springLength;
    [Range(0.0f, 100.0f)]
    public float rigidity;
    [Range(0.0f, 100.0f)]
    public float damping;

    private float springLengthSpeed;
    //



    // Use this for initialization
    void Start ()
    {
        originRB = originObject.GetComponent<Rigidbody>();
        springT = springObject.transform;
        springRB = springObject.GetComponent<Rigidbody>();

        springLengthSpeed = 0.0f;
        springLength = 0.0f;
        
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //  freeze local position and rotation
        Vector3 localVelocity = transform.InverseTransformDirection(springRB.velocity);
        springRB.velocity = transform.TransformDirection(new Vector3(0.0f, localVelocity.y, 0.0f));
        //springT.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        springT.localPosition = new Vector3(0.0f, springT.localPosition.y, 0.0f);

        springT.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        //  spring recurence variables
        springLengthSpeed = springT.localPosition.y - springLength;
        springLength = springT.localPosition.y;

        //  relocalize spring mesh
        springMesh.localScale = new Vector3(1.0f, 1.0f - Mathf.Abs(springLength)/0.2f, 1.0f);
        springMesh.localPosition = new Vector3(0.0f, -0.8f + 0.5f * Mathf.Abs(springLength), 0.0f);

        //  apply forces
        float magnitude = rigidity * springLength + damping * springLengthSpeed;

        /*originRB.AddRelativeForce(0.0f, magnitude * originRB.mass, 0.0f, ForceMode.Impulse);
        springRB.AddRelativeForce(0.0f,-magnitude * springRB.mass, 0.0f, ForceMode.Impulse);*/
    }
}
