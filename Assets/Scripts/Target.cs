using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;



namespace TobiiGlasses
{
    //[XmlRoot("AbscissesCollection")]
    public class Target : MonoBehaviour
    {

        /*[XmlArray("Abscisses")]
        [XmlArrayItem("Abscisse")]
        public List<ToXmlfile> abscisse = new List<ToXmlfile>();*/


        public Transform target;
        public Camera cam;
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

        string path = @"D:/Projet S5/Test.txt";
        StreamWriter sw;

        string path1 = @"D:/Projet S5/Test1.csv";
        //StreamWriter sw1 = new StreamWrither(path1,true);

        // Use this for initialization
        void Start()
        {
            
            socketData.Connect(ipEndPoint);
            socketVideo.Connect(ipEndPoint);
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            //SendAndReceive();

            sw = File.CreateText(path);
            //sw1 = File.CreateText(path1);
            // pour csv 

        }
        void Update()
        {

            int screenHeight = Screen.height;
            int screenWidth = Screen.width;
            Debug.Log("Screen height : " + screenHeight);
            Debug.Log("Screen Width : " + screenWidth);

            
            string dataReceiveString = ReceiveData.RData(socketData);
            //Console.WriteLine("Données reçues"+dataReceiveString);
            while (!dataReceiveString.Contains("gp3"))
            {
                dataReceiveString = ReceiveData.RData(socketData);
            }
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            if (dataReceiveString.Contains("gp3"))
            {
               
                float[] position = new float[3];
                position = ConvertGP3Data.CData(dataReceiveString);
                Vector3 vposition = new Vector3(position[0], position[1], position[2]);



               



                if (position[1] + position[2] + position[0] != 0)
                {


                    //here what I added
                    /*Vector3 scale = Vector3.one * scale;
                    Matrix4x4 mat = Matrix4x4.TRS(position, Quaternion.identity, scale);

                    Vector3 Map(Vector3 aPos)
                    {
                        aPos *= scale;
                        aPos += position;
                        return aPos;   
                    }
                    Vector3 newPosition = new Vector3(Map(position));*/


                    Vector3 positionBoule = transform.position;
                    if (Math.Abs(positionBoule.x) > screenWidth/2) {
                        if (positionBoule.x > 0)
                        {
                            positionBoule.x = screenWidth/2;
                        }
                        else positionBoule.x = -screenWidth/2;
                    }

                    if (Math.Abs(positionBoule.y) > screenHeight/2)
                    {
                        if (positionBoule.y > 0)
                        {
                            positionBoule.y = screenHeight/2;
                        }
                        else positionBoule.y = -screenHeight/2;
                    }
                    Debug.Log("heeeeeeeeeeeeeeeeeeeeeeeeeeeeey  postion de la boule  " + positionBoule.ToString());


                    //
                    //Write some text to the test.txt file


                    /* if (!File.Exists(path))
                     {
                         // Create a file to write to.
                         using (StreamWriter sw = File.CreateText(path))
                         {
                             sw.WriteLine(positionBoule);

                         }
                     }*/

                    //text file
                    sw.WriteLine("positionBoule");
                    sw.WriteLine(positionBoule);
                    sw.WriteLine("positionRegrad");
                    sw.WriteLine(vposition);

                    //csv file
                    //sw1.WriteLine(positionBoule);
                    //sw1.WriteLine(vposition);


                    Vector3 camPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
                    //Vector3 camDirection = cam.FindGameObjectWithTag("MainCamera").transform.forward;

                    //Vector3 newPosition = new Vector3(-position[0] + camPosition.x, position[1] + camPosition.y, position[2] + camPosition.z);
                    Vector3 newPosition = new Vector3(-position[0] + camPosition.x, position[1] + camPosition.y, 700);

                    /*Vector3 product = Vector3.Dot(camDirection, vposition);
                    Vector3 gazeDirection2D = cam.WorldToScreenPoint(vposition);*/
                   


                   // Vector3 newPosition = new Vector3(-position[0] , position[1] , 700);
                    transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

                    /*Vector3 screenPos = cam.WorldToScreenPoint(newPosition);
                    Debug.Log("target is " + screenPos.x + " pixels from the left");*/

                    //transform.position = new Vector3(position[0], position[1], position[2]);

                    Debug.Log("x : " + position[0] + " y : " + position[1] + " z : " + position[2]);
                    print(transform.position);



                   
                }
                
            }
        }
    }
}
