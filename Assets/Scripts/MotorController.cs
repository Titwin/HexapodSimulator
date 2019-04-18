using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorController : MonoBehaviour {

    //  Attributes
    [Range(0.0f, 40.0f)]
    public float P;
    [Range(0.0f, 0.05f)]
    public float I;
    [Range(0.0f, 0.1f)]
    public float D;
    [Range(0.0f, 720.0f)]
    public float motorSpeed;

    [Space(10)]

    public bool enableTorque;
    [Range(0.0f, 40.0f)]
    public float globalTorque;
    public HingeJoint[] motors;

    private Quaternion[] initialRotation;
    private bool[] useMotor;
    private float[] maxTorque;
    private float[] positionTarget;
    private float[] positionError;
    private float[] integralError;

    //  Public functions
    public void SetMaxTorque(float torque)
    {
        for (int i = 0; i < maxTorque.Length; i++)
        {
            useMotor[i] = true;
            maxTorque[i] = torque;
        } 
    }
    public void SetMaxTorque(int motor, float torque)
    {
        if (motor < motors.Length)
            maxTorque[motor] = torque;
        useMotor[motor] = true;
    }
    public void SetPosition(int motor, float position)
    {
        if (motor < motors.Length)
            positionTarget[motor] = position;
        useMotor[motor] = true;
    }
    public void OverridePosition(int motor, float position)
    {
        useMotor[motor] = false;
        //Transform jointParent = motors[motor].gameObject.transform;

        motors[motor].gameObject.transform.localRotation = initialRotation[motor] * Quaternion.Euler(motors[motor].axis * position);

        //if (motor < motors.Length)
        //    motors[i] = position;
    }

    // Use this for initialization
    void Start ()
    {
        //  initialize arrays
        useMotor = new bool[motors.Length];
        for (int i = 0; i < useMotor.Length; i++)
            useMotor[i] = true;

        initialRotation = new Quaternion[motors.Length];
        for (int i = 0; i < useMotor.Length; i++)
            initialRotation[i] = motors[i].gameObject.transform.localRotation;

        maxTorque = new float[motors.Length];
        for (int i = 0; i < maxTorque.Length; i++)
            maxTorque[i] = 100.0f;

        positionTarget = new float[motors.Length];
        for (int i = 0; i < positionTarget.Length; i++)
            positionTarget[i] = 0.0f;

        positionError = new float[motors.Length];
        for (int i = 0; i < positionError.Length; i++)
            positionError[i] = 0.0f;

        integralError = new float[motors.Length];
        for (int i = 0; i < integralError.Length; i++)
            integralError[i] = 0.0f;

        for (int i = 0; i < motors.Length; i++)
        {
            motors[i].useMotor = true;
            var motor = motors[i].motor;
            motor.force = maxTorque[i];
            motor.targetVelocity = 0.0f;
            motor.freeSpin = false;
            motors[i].motor = motor;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        for (int i = 0; i < motors.Length; i++)
        {
            if(enableTorque && useMotor[i])
            {
                //  compute errors
                float error = positionTarget[i] - motors[i].angle;
                float derivativeError = error - positionError[i];

                //  update motor hinge
                motors[i].useMotor = true;
                var motor = motors[i].motor;
                motor.force = globalTorque;
                motor.targetVelocity = Mathf.Clamp(P * error + I * integralError[i] + D * derivativeError, -motorSpeed, motorSpeed);
                motor.freeSpin = false;
                motors[i].motor = motor;

                //  increment PID
                integralError[i] += Time.fixedTime * error;
                Mathf.Clamp(integralError[i], 0.3f, 0.3f);
                positionError[i] = error;
            }
            else
            {
                integralError[i] = 0.0f;
                positionError[i] = 0.0f;
                motors[i].useMotor = false;
            }
        }
    }
}
