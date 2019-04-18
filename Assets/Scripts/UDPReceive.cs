using UnityEngine;
using System.Collections;
using System.Linq;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
 

public class UDPReceive : MonoBehaviour
{
    //  Attributes
    Thread receiveThread;
    private UdpClient client;
    private IPEndPoint receiveIPE;

    public int port;
    public enum Mode
    {
        sensors=4, vision=5
    }
    public Mode mode;

    public enum MessageType
    {
        SYSTEM = 0,
        MOTORS,
        LEGBOARDS
    }

    [System.Serializable]
    public class Message
    {
        public ulong timestamp;
        public string message;
    }

    //public int messageTypeCount;
    public Message[] lastMessages;

    //  process
    private static void Main()
    {
        UDPReceive receiveObj = new UDPReceive();
        receiveObj.init();
    }
    public void Start()
    {
        init();
        //lastMessages = new Message[];
        foreach (Message msg in lastMessages)
        {
            msg.timestamp = 0;
            msg.message = "";
        }
    }
    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        client.Close();
    }
    private void init()
    {
        //port = 5014;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                //if (mode == Mode.sensors)
                {

                    if (data.Length < 15 || data[0] != 0xFF || data[1] != 0xFF)
                    {
                        Debug.LogWarning("Wrong size or header");
                    }
                    else
                    {
                        int offset = 0;
                        if (mode == Mode.sensors) offset = 4;
                        else if (mode == Mode.vision) offset = 5;

                        ulong timestamp = toLong(SubArray(data, 2, 8));
                        ulong messageSize = toLong(SubArray(data, 10, offset));
                        byte messageType = data[10 + offset];
                        byte expectedCRC = data[data.Length - 1];
                        byte computedCRC = getCRC(SubArray(data, 0, data.Length - 1));
                        string message = Encoding.UTF8.GetString(SubArray(data, 11 + offset, data.Length - 12 - offset));
                        int strLength = SubArray(data, 11 + offset, data.Length - 12 - offset).Length;

                        if (strLength != (int)messageSize)
                            Debug.LogWarning("Wrong message length received. expected : " + (int)messageSize + " received : " + strLength);
                        else if (expectedCRC != computedCRC)
                            Debug.LogWarning("Wrong crc received");
                        else if (messageType >= lastMessages.Length)
                            Debug.LogWarning("Invalid message type received (" + (int)messageType + ")");
                        else if (lastMessages[messageType].timestamp < timestamp)
                        {
                            lastMessages[messageType].timestamp = timestamp;
                            lastMessages[messageType].message = message;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.LogError(err.ToString());
            }
        }
    }
    

    //  functions helper
    private byte getCRC(byte[] msg)
    {
        byte crc = 0;
        for (int i = 0; i < msg.Length; i++)
            crc += msg[i];
        return crc;
    }
    private ulong toLong(byte[] data)
    {
        string str = Encoding.UTF8.GetString(data);
        return Convert.ToUInt64(str, 16);
    }
    public static byte[] SubArray(byte[] data, int bgn, int len)
    {
        byte[] result = new byte[len];
        Array.Copy(data, bgn, result, 0, len);
        return result;
    }

    //  get
    public string getLatestUDPPacket(int messageType)
    {
        return lastMessages[messageType].message;
    }
}

