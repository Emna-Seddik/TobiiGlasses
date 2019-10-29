using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace TobiiGlasses
{

    public class SendKeepAliveMessage
    {
        // Class KeepAliveMessage
        public class KeepAliveMessage
        {
            public string op;
            public string type;
            public string key;
        }

        public static void SendKAM(IPEndPoint ipEndPoint, Socket socketData, Socket socketVideo)
        {
            // Création des messages KeepAlive en JSON
            KeepAliveMessage kamDataJson = new KeepAliveMessage() { op = "start", type = "live.data.unicast", key = "some_GUID" };
            KeepAliveMessage kamVideoJson = new KeepAliveMessage() { op = "start", type = "live.video.unicast", key = "some_Other_GUID" };
            // Conversion des messages KeepAlive du JSON au string n
            string kamDataString = JsonUtility.ToJson(kamDataJson);
            string kamVideoString = JsonUtility.ToJson(kamVideoJson);
            Console.WriteLine(kamDataString);
            // Conversion des messages KeepAlive du string au bytes
            byte[] kaDataBytes = Encoding.ASCII.GetBytes(kamDataString);
            byte[] kaDataVideo = Encoding.ASCII.GetBytes(kamVideoString);
            // Envoi des KeepAlive message
            socketData.SendTo(kaDataBytes, ipEndPoint);
            socketVideo.SendTo(kaDataVideo, ipEndPoint);
        }
    }
}
