using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace TobiiGlasses
{
    public class Target2D : MonoBehaviour
    {
        public Transform target;
        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        static int liveCtrlPort = 49152;
        static IPAddress ipGlasses = IPAddress.Parse("192.168.71.50");
        static IPEndPoint ipEndPoint = new IPEndPoint(ipGlasses, liveCtrlPort);
        // Création des sockets
        static Socket socketData = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static Socket socketVideo = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Vector3 vect;
        public int i = 0;
        // Use this for initialization
        void Start()
        {
            socketData.Connect(ipEndPoint);
            socketVideo.Connect(ipEndPoint);
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            //SendAndReceive();
        }
        void Update()
        {
            i++;
            if (i == 60)
            {
                SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
                i = 0;
            }
            //Console.WriteLine("Données reçues"+dataReceiveString);
            string dataReceiveString = ReceiveData.RData(socketData);
            if (dataReceiveString.Contains("gp3")) { }
            else if (dataReceiveString.Contains("gp"))
            {
                float[] position = new float[2];
                position = ConvertGP2Data.CData(dataReceiveString);
                if (position[1] + position[0] != 0)
                {
                   // Vector3 targetPosition = target.TransformPoint(new Vector3(position[0] * 5, position[1] * 5, 10));
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(position[0] * 5, position[1] * 5, 10), ref velocity, smoothTime);
                   // transform.position = new Vector3(position[0]*5, position[1]*5,10);
                    Debug.Log("x : " + position[0] + " y : " + position[1]);
                    print(transform.position);
                }

            }
        }
    }
}
