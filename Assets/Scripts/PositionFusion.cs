using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class PositionFusion : MonoBehaviour
{
    public Transform robotTransform;
    public UDPReceive udpWalkReceiver;
    public RobotVision visionSystem;

    public GameObject errorDebug;

    [Serializable]
    private class Vec3 { public float x = 0.0f; public float y = 0.0f; public float z = 0.0f; }

    // Use this for initialization
    void Start ()
    {
        errorDebug.transform.localScale = new Vector3(0, errorDebug.transform.localScale.y, 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        updateOdometry();
    }

    private void updateOdometry()
    {
        Vector3 dv = Vector3.zero;
        Vector3 da = Vector3.zero;

        string json = udpWalkReceiver.getLatestUDPPacket((int)UDPReceive.MessageType.SYSTEM);
        string[] jsonArray = json.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < jsonArray.Length; i++)
        {
            if (jsonArray[i].IndexOf("velocity") == 0)
            {
                int index = jsonArray[i].IndexOf("velocity") + 8;
                Vec3 v = JsonUtility.FromJson<Vec3>(jsonArray[i].Substring(index, jsonArray[i].Length - index));
                dv = Time.fixedTime * 0.013f * (v.y * errorDebug.transform.forward + v.x * errorDebug.transform.right + v.z * errorDebug.transform.up);
                errorDebug.transform.position += dv;
            }
            else if (jsonArray[i].IndexOf("angular") == 0)
            {
                int index = jsonArray[i].IndexOf("angular") + 7;
                Vec3 v = JsonUtility.FromJson<Vec3>(jsonArray[i].Substring(index, jsonArray[i].Length - index));
                da = new Vector3(0, Mathf.Rad2Deg * v.x, 0);
                errorDebug.transform.eulerAngles += da;
            }
        }

        if (dv != Vector3.zero)
        {
            Vector3 de = dv.magnitude * new Vector3(0.1f, 0.0f, 0.1f);
            errorDebug.transform.localScale += de;
            Debug.Log(dv.magnitude + " " + de);
        }
    }
}
