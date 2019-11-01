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
        //StreamReader sr = new StreamReader("C:/tmp/data.txt");
        StreamReader sr = new StreamReader("C:/tmp/fakeData3.txt");

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
            Debug.Log("Screan dpi=" + Screen.dpi);
            Debug.Log("ration=" + Screen.dpi / 25.4f);

        }

        void drawRay(Vector3 position) {
            Debug.Log("position(" + position.x + ";" + position.y + ";" + position.z + ")");
            Vector3 mousePosFar = new Vector3(position.x,
        position.y,
        Camera.main.farClipPlane);
            Vector3 mousePosNear = new Vector3(position.x,
                position.y,
                Camera.main.nearClipPlane);
            Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
            Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);
            Debug.DrawRay(mousePosN, mousePosF - mousePosN, Color.cyan);
        }
        void Update()
        {
            if (Input.GetMouseButton(0)) {
                drawRay(Input.mousePosition);
                Debug.Log("Screan dpi=" + Screen.dpi);
                Debug.Log("ration=" + Screen.dpi / 25.4f);
            }

            String line = null;
            try
            {   // Open the text file using a stream reader.
                //using (sr)
                //{
                    // Read the stream to a string, and write the string to the console.
                    line = sr.ReadLine();
                    Debug.Log(line);
                    Console.WriteLine(line);
                //}
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            int screenHeight = Screen.height;
            int screenWidth = Screen.width;
           /* Debug.Log("Screen height : " + screenHeight);
            Debug.Log("Screen Width : " + screenWidth);

            Debug.Log("far clip plane  : " + Camera.main.farClipPlane);
            Debug.Log("near clip plane  : " + Camera.main.nearClipPlane);*/


            //string dataReceiveString = ReceiveData.RData(socketData);
            string dataReceiveString = line;
            //Console.WriteLine("Données reçues"+dataReceiveString);
            while (dataReceiveString==null || !dataReceiveString.Contains("gp3"))
            {
                dataReceiveString = ReceiveData.RData(socketData);
            }
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            
               
                float[] position = new float[3];
                position = ConvertGP3Data.CData(dataReceiveString);
            float ratio = Screen.dpi / 25.4f;  //*scale
                  Vector3 vposition = new Vector3(position[0], position[1], position[2]);
                drawRay(vposition*ratio);






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
