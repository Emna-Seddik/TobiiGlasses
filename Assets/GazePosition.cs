using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

using TobiiGlasses;

public class GazePosition : MonoBehaviour
{
    //public Transform Gaze;
    public GameObject go;
    static int liveCtrlPort = 49152;
    static IPAddress ipGlasses = IPAddress.Parse("192.168.71.50");
    static IPEndPoint ipEndPoint = new IPEndPoint(ipGlasses, liveCtrlPort);
    static Socket socketData = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    static Socket socketVideo = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    // Start is called before the first frame update
    void Start()
    {
        socketData.Connect(ipEndPoint);
        socketVideo.Connect(ipEndPoint);

        SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);

    }

    // Update is called once per frame
    void Update()
    {
        string dataReceiveString = ReceiveData.RData(socketData);
        while (dataReceiveString.Contains("gp3"))
        {
            float[] gp3 = new float[3];
            gp3 = ConvertGP3Data.CData(dataReceiveString);
            Vector3 gazePosition = new Vector3(gp3[0], gp3[1], gp3[2]);
            go.transform.position = gazePosition;
        }
        SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);

    }
}
