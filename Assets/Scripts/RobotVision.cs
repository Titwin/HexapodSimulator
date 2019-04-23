using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
public class RobotVision : MonoBehaviour
{
    //  UDP message catcher
    [SerializeField]
    UDPReceive udpReceive;
    [SerializeField]
    Transform hexapodTransform;
    [SerializeField]
    Transform totemTransform;


    public bool visionAvailable;
    public Vector3 visionPosition;
    public Vector3 visionAngles;

    //  marker dictionary id & position
    [SerializeField]
    List<int> FixedMarkerIDS;
    [SerializeField]
    List<Transform> FixedMarkerPosition;

    //  debug
    /*[SerializeField]
    List<Transform> seenMarkerProxy;*/

    private Dictionary<int, Transform> FixedMarkers = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> reconstructedCamera = new Dictionary<int, Transform>();

    [SerializeField]
    List<int> frameMarkerIds;
    public float maxError;
    public float variance;
    public float standardDeviation;


    // UNITY callbacks
    void Start()
    {
        for (int idx = 0; idx < FixedMarkerIDS.Count; ++idx)
        {
            FixedMarkers[FixedMarkerIDS[idx]] = FixedMarkerPosition[idx];
            Transform rec = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            rec.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

            rec.parent = FixedMarkerPosition[idx];
            rec.localPosition = Vector3.zero;
            rec.localRotation = Quaternion.identity;
            rec.localScale = Vector3.one * 0.5f;

            reconstructedCamera[FixedMarkerIDS[idx]] = rec;
            rec.gameObject.name = "reconstructed" + FixedMarkerIDS[idx];
        }
    }
    void Update()
    {
        visionAvailable = false;
        frameMarkerIds.Clear();
        foreach (Transform rc in reconstructedCamera.Values)
            rc.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        ParseMarkerPositions();
        if(frameMarkerIds.Count > 0)
        {
            ComputeTransform();
            visionAvailable = true;
        }
    }



    void ParseMarkerPositions()
    {
        string markerString = udpReceive.getLatestUDPPacket(0);
        string[] markers = markerString.Split('\n');

        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i].Length > 0)
            {
                string[] infos = markers[i].Split(':');
                try
                {
                    //  parse message
                    int id = int.Parse(infos[0]);
                    string[] pos = infos[1].Trim().Split(' ');
                    Vector3 position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                    Quaternion quaternion = new Quaternion(float.Parse(pos[3]), float.Parse(pos[4]), float.Parse(pos[5]), float.Parse(pos[6]));
                    
                    reconstructedCamera[id].localRotation = Quaternion.identity;
                    reconstructedCamera[id].localPosition = Vector3.zero;
                    reconstructedCamera[id].Rotate(Quaternion.Inverse(quaternion).eulerAngles);
                    reconstructedCamera[id].Translate(-10 * position, Space.Self);
                    reconstructedCamera[id].gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                    frameMarkerIds.Add(id);
                }
                catch (Exception e)
                {
                    //Debug.LogException(e, this);
                    //Debug.LogWarning("cannot parse: " + markers[i]);
                }
            }
        }
    }
    void ComputeTransform()
    {
        Vector3 position = Vector3.zero;
        float angle = 0.0f;
        float weightSum = 0.0f;
        string s = "";
        
        foreach (int id in frameMarkerIds)
        {
            Vector3 direction = FixedMarkers[id].position - reconstructedCamera[id].position;
            float weight = 1.3f - Math.Abs(Vector3.Dot(FixedMarkers[id].forward, Vector3.Normalize(direction)));
            weight = Mathf.Clamp(weight, 0.3f, 1.0f);
            s += id.ToString() + " : " + (weight * weight).ToString() + " ";

            position += weight * weight * reconstructedCamera[id].position;
            angle += weight * weight * reconstructedCamera[id].eulerAngles.y;
            weightSum += weight * weight;
        }
        //Debug.Log(s);

        position /= weightSum;
        angle /= weightSum;

        visionPosition = new Vector3(position.x, 1, position.z);
        visionAngles = new Vector3(0, angle, 0);
        
        hexapodTransform.localPosition = visionPosition;
        //hexapodTransform.LookAt(totemTransform);
        


        //  statistics
        maxError = 0.0f;
        foreach (int id in frameMarkerIds)
        {
            maxError = Mathf.Max(maxError, (visionPosition - reconstructedCamera[id].position).magnitude);
        }
        variance = 0.0f;
        foreach (int id in frameMarkerIds)
        {
            variance += (visionPosition - reconstructedCamera[id].position).sqrMagnitude;
        }
        variance /= frameMarkerIds.Count;
        standardDeviation = Mathf.Sqrt(variance);
    }
}
