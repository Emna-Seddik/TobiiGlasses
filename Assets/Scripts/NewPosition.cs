using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace TobiiGlasses
{
    public class NewPosition : MonoBehaviour
    {
        static int liveCtrlPort = 49152;
        static IPAddress ipGlasses = IPAddress.Parse("192.168.71.50");
        static IPEndPoint ipEndPoint = new IPEndPoint(ipGlasses, liveCtrlPort);
        // Création des sockets
        static Socket socketData = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static Socket socketVideo = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Vector3 vect;

        public static NewPosition Instance { get; private set; }
        void Awake()
        {
            Instance = this;
        }


        void SendAndReceive()
        {
            
            Target targetObject = new Target();
            socketData.Connect(ipEndPoint);
            socketVideo.Connect(ipEndPoint);
            Console.WriteLine("Sockets connectées");
            int i = 0;

            while (i < 3)
            {
                SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
                i++;
                int j = 0;
                while (j < 10)
                {
                    string dataReceiveString = ReceiveData.RData(socketData);
                    //Console.WriteLine("Données reçues"+dataReceiveString);
                    if (dataReceiveString.Contains("gp3"))
                    {
                        float[] position = new float[3];
                        position = ConvertGP3Data.CData(dataReceiveString);
                        Debug.Log("x : " + position[0] + " y : " + position[1] + " z : " + position[2]);
                        targetObject.SendMessage("PositionUpdate", new Vector3(position[0], position[1], position[2]));
                        j++;
                    }
                }
            }
        }
        // Use this for initialization
        void Start()
        {
            SendAndReceive();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}