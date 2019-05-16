using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


public class RobotSyncronizer : MonoBehaviour
{
    // Attributes
    public MotorController motorController;
    public DistanceScanner scanner;
    public UDPReceive udpreceiver;
    public Transform robotTransform;

    [Space(10)]
    [Range(0.0f, 1.0f)]
    public float speedSynchro;
    [Range(0.0f, 4.0f)]
    public float angularSynchro;

    [Space(10)]
    public float[] sens;
    public float[] offset;


    [Serializable]
    private class SerializedSCS15
    {
        public ushort i = 0;
        public ushort p = 0;
        public ushort P = 0;
        public ushort s = 0;
        public ushort t = 0;
        public ushort tM = 0;
        public ushort T = 0;
    }
    [Serializable]
    private class SerializedLegBoard
    {
        public ushort i = 0;
        public ushort s = 0;
        public ushort S = 0;
        public ushort F = 0;
        public ushort d = 0;
        public ushort r = 0;
        public ushort g = 0;
        public ushort b = 0;
    }
    [Serializable]
    private class Vec3 { public float x = 0.0f; public float y = 0.0f; public float z = 0.0f; }
    [Serializable]
    private class Vec4 { public float x = 0.0f; public float y = 0.0f; public float z = 0.0f; public float w = 0.0f; }
    // Update is called once per frame
    void Update ()
    {
        updateMotors();
        updateLegBoards();
        updateRobotPosition();
    }

    private void updateMotors()
    {
        string json = udpreceiver.getLatestUDPPacket((int)UDPReceive.MessageType.MOTORS);
        if (json.Length < 10) return;
        int startIndex = json.IndexOf("{")+1;
        json = json.Substring(startIndex + 1, json.Length - 2 - startIndex);
        string[] motorArray = json.Split('m');

        for(int i = 0; i < motorArray.Length; i++)
        {
            int index = motorArray[i].IndexOf("{");
            motorArray[i] = motorArray[i].Substring(index, motorArray[i].Length - index);
            
            SerializedSCS15 scs15 = JsonUtility.FromJson<SerializedSCS15>(motorArray[i]);
            int id = scs15.i - 2;
            float pos = sens[id] * 100.0f * ((float)(scs15.p) - offset[id] - 512.0f) /512.0f;
            motorController.SetPosition(id, pos);
        }
    }
    private void updateLegBoards()
    {
        string json = udpreceiver.getLatestUDPPacket((int)UDPReceive.MessageType.LEGBOARDS);
        if (json.Length < 10) return;
        int startIndex = json.IndexOf("{") + 1;
        json = json.Substring(startIndex + 1, json.Length - 2 - startIndex);
        string[] legBoardsArray = json.Split(new string[] { "lb" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < legBoardsArray.Length; i++)
        {
            int index = legBoardsArray[i].IndexOf("{");
            legBoardsArray[i] = legBoardsArray[i].Substring(index, legBoardsArray[i].Length - index);

            SerializedLegBoard lb = JsonUtility.FromJson<SerializedLegBoard>(legBoardsArray[i]);
            int id = lb.i - 2;
            scanner.SetDistance(id, lb.d * 0.01f);
        }
    }
    private void updateRobotPosition()
    {
        string json = udpreceiver.getLatestUDPPacket((int)UDPReceive.MessageType.SYSTEM);
        string[] jsonArray = json.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < jsonArray.Length; i++)
        {
            if (jsonArray[i].IndexOf("position") == 0)
            {
                int index = jsonArray[i].IndexOf("position") + 8;
                Vec3 v = JsonUtility.FromJson<Vec3>(jsonArray[i].Substring(index, jsonArray[i].Length - index));
                robotTransform.localPosition = 0.1f * new Vector3(v.x, v.y, v.z);
            }
            else if (jsonArray[i].IndexOf("orientation") == 0)
            {
                int index = jsonArray[i].IndexOf("orientation") + 11;
                Vec3 v = JsonUtility.FromJson<Vec3>(jsonArray[i].Substring(index, jsonArray[i].Length - index));
                //robotTransform.localRotation = new Quaternion(v.x, v.y, v.z, v.w);
                robotTransform.localEulerAngles = new Vector3(v.x, v.y, v.z);
                //robotTransform.localEulerAngles = robotTransform.localEulerAngles;
            }
        }
    }
}
