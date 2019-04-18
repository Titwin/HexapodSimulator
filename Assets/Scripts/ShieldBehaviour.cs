using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehaviour : MonoBehaviour
{
    //  Atributes
    public GameObject springObject;
    
    private Rigidbody springRB;

    [Space(10)]

    public float alpha;
    public float teta;
    [Range(0.0f, 50.0f)]
    public float rigidity;
    [Range(0.0f, 100.0f)]
    public float damping;
    public string contact;
    
    private float springAlphaSpeed;
    private float springTetaSpeed;
    private Vector3 anchorPosition;

    // Use this for initialization
    void Start ()
    {
        springRB = springObject.GetComponent<Rigidbody>();

        springAlphaSpeed = 0.0f;
        springTetaSpeed = 0.0f;

        anchorPosition = transform.localPosition;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        //  freeze local position and rotation
        transform.localPosition = anchorPosition;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);

        //  spring recurence variables
        float a = transform.localEulerAngles.x;
        float t = transform.localEulerAngles.y;
        if (a > 180.0f) a -= 360.0f;
        if (t > 180.0f) t -= 360.0f;

        springAlphaSpeed = Mathf.Deg2Rad * a - alpha;
        springTetaSpeed = Mathf.Deg2Rad * t - teta;
        alpha = Mathf.Deg2Rad * a;
        teta = Mathf.Deg2Rad * t;

        //  apply torques
        float magnitudeAlpha = rigidity * alpha + damping * springAlphaSpeed;
        float magnitudeTeta = rigidity * teta + damping * springTetaSpeed;
        springRB.AddRelativeTorque(-springRB.mass * magnitudeAlpha, -springRB.mass * magnitudeTeta, 0.0f, ForceMode.Impulse);
    }

    //  Detect shild collision
    void Update()
    {
        contact = "";
        if (alpha > 0.07f) contact += "A";
        else if (alpha < -0.07f) contact += "B";
        if (teta > 0.07f) contact += "C";
        else if (teta < -0.07f) contact += "D";
    }
}
