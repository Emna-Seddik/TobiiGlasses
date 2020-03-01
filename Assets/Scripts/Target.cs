using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage ;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Globalization;

namespace TobiiGlasses
{
  
    public class Target : MonoBehaviour
    {

        public Camera CameraLunettes;
        public string dataleft;
        public string dataright;
        public GameObject go;
        public Transform target;
        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        public GameObject objet;
        public Transform markerEcran;
        public Transform markerLunette;
        static int liveCtrlPort = 49152;
        static IPAddress ipGlasses = IPAddress.Parse("192.168.71.50");
        static IPEndPoint ipEndPoint = new IPEndPoint(ipGlasses, liveCtrlPort);
        // Création des sockets
        static Socket socketData = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static Socket socketVideo = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Vector3 vect;
        public int i = 0;
        private List<string[]> rowData = new List<string[]>();
        string[] rowDataTemp = new string[2];
        

        //string path = @"D:\Amal\donnee2.csv";
        StreamWriter sw;

        // Use this for initialization
        void Start()
        {
            int screenHeig = Screen.height;
            int Screanweidth = Screen.width;
            Vector3 positionObject = transform.position;
            
           // Debug.Log("Données reçues");
 
            socketData.Connect(ipEndPoint);
            socketVideo.Connect(ipEndPoint);
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            //SendAndReceive();
            objet.transform.position = new Vector3(2, 3, 10);
            sw = new StreamWriter(@"C:\Users\EyeTracking\Downloads\TobiiGlasses V2\centre.csv");

            //swb = new StreamWriter(@"D:\Amal\test17.csv");
            // sw = File.CreateText(path);
            // sw.Write("boule");
            //sw.Write("Xgp3"+ "; " + "Ygp3 " + "Xgp3"+ "\r");
            //swb.Write("" + "; " + "Yboule"+ ";"+ "Zboule" + ";" + " Xregard" + ";" + "Yregard"+ "; " + "Zregard" + "\r");

            //sw.Write("2");
            //sw.Flush();

            //before your loop
            //var csv = new StringBuilder();

            //in your loop
            //string first = "boule";
            //string second = "regard";
            //Suggestion made by KyleMit
            // var newLine = string.Format("{0},{1}", first, second);
            // csv.AppendLine(newLine);

            //after your loop
            // File.WriteAllText(@"D:\Amal\donnee2.csv", csv.ToString());
            //swb.Flush();

            // FileStream file = File.Create(Application.persistentDataPath + "/donnee1.csv");

        }


        void drawRay(Vector3 position)
        {
            //Debug.Log("position(" + position.x + ";" + position.y + ";" + position.z + ")");
            Vector3 mousePosFar = new Vector3(position.x,
                                              position.y,
                                              Camera.main.farClipPlane);
            Vector3 mousePosNear = new Vector3(position.x,
                                               position.y,
                                               Camera.main.nearClipPlane);
            Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
            Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);
            //Debug.DrawRay(mousePosN, mousePosF - mousePosN, Color.cyan);
        }




        void Update()
        {

            //Debug.Log("Données reçues target" );
            string dataReceiveString = ReceiveData.RData(socketData);
            //Debug.Log("Données reçues target" + dataReceiveString);
            while (!dataReceiveString.Contains("gp3"))
            {
                dataReceiveString = ReceiveData.RData(socketData);
                //Debug.Log("Données reçues target" + dataReceiveString);
                if (dataReceiveString.Contains("gd") && dataReceiveString.Contains("right"))
                {
                   
                    dataright = dataReceiveString;
                    
                }
                if (dataReceiveString.Contains("gd") && dataReceiveString.Contains("left"))
                {
                    dataleft = dataReceiveString;
                }
            }
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);

            
             sw.Write(Time.time + " "  + dataReceiveString + "\r\n");
             sw.Write(Time.time + " rigidBody écran ( " + markerEcran.transform.position.x +" ; "+ markerEcran.transform.position.y + " ; " + markerEcran.transform.position.z + " )" + markerEcran.rotation +  "\r\n");
             sw.Write(Time.time + " rigidBody lunettes ( " + markerLunette.transform.position.x + " ; " + markerLunette.transform.position.y+ " ; " + markerLunette.transform.position.z + ")" + markerLunette.rotation+ "\r\n");
             sw.Flush();
            


            if (dataReceiveString.Contains("gp3"))
            {
                float[] gp3 = new float[3];
                gp3 = ConvertGP3Data.CData(dataReceiveString);
                Vector3 vposition = new Vector3(gp3[0], gp3[1],2);
                vposition.x = -vposition.x;
                go.transform.position = vposition / 200;
                Debug.Log("Gaze position: " + vposition / 200);
                float ratio = Screen.dpi / 25.4f;
                
                //Debug.Log("hhhhhhhhhhhhhhhhhhhhhhhhhh " + markerLunette.position);
                //drawRay(markerLunette.position);
                //CameraLunettes.transform.position = markerLunette.position;
                //go.transform.parent = markerLunette.transform;
                //go.transform.position = vposition;
                

                if (gp3[1] + gp3[2] + gp3[0] != 0)
                {
                    Vector3 camPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
                    
                    
                    float positiony = gp3[1] + camPosition.y;
                    float positionx = -gp3[0] + camPosition.x;
                    float positionz = 15;
                    
                    Double gazeanglex = Math.Atan(gp3[2] / gp3[0]);
                    //Debug.Log("marker" + Math.Abs(markerEcran.position.z - markerLunette.position.z) * 100);

                    //calculer x real en double
                    //distance entre l'utilisateur et l'écran donnée opar optitrack
                    Double disUE = Math.Abs(markerEcran.position.x - markerLunette.position.x) * 100;
                   
                    //deplacement (deltaZ dans le système optitrack) par rapport à la position de la reference(x constante) à la meme distance de l'ecran et à la meme hauteur (y constante)      
                    Double deltaz = Math.Abs(markerEcran.position.z - markerLunette.position.z) * 100;
                    //difference d'hauteur entre la lunette et l'ecran
                    Double deltax = Math.Abs(markerEcran.position.y - markerLunette.position.y) * 100;
                    Double xreald = (disUE * (1 / Math.Tan(gazeanglex)));
                    //convertir x real en string
                    string xreals = xreald.ToString("0.00");
                    //Debug.Log("x unity calculé " + xreals);
                    //convertir x real en float
                    float xreal = float.Parse(xreals, CultureInfo.InvariantCulture);
                    //calculer l'angle de gaze direction par rapport le repere y
                    Double gazeangley = Math.Atan(gp3[1] / gp3[0]);
                    //calculer le y real en double
                    Double yreald = xreald * (Math.Tan(gazeangley));
                    //Debug.Log("y reçu Lunette " + gp3[1]);
                    // convertir y real en string
                    string yreals = yreald.ToString("0.00");
                  //Debug.Log("y unity calculé " + yreals);
                    // cinvertir y real en float
                    float yreal = float.Parse(yreals, CultureInfo.InvariantCulture);
                    
                    //deltaY : déplacement par rapport à la position idéale. 50cm est le déplacement deltaY à une distance de 140cm de l'écran qui sera pris comme valeur de référence

                    Double deltaY =(disUE * 50) / 140;
                    //Debug.Log("deltaz" + deltaz + "position y"+ (float)((yreald - deltaY) / 2) + "position x" + (-(float)xreald));
                   //
                  //Vector3 newPosition = new Vector3((float)(-xreald), (float)((yreald - deltaY)/2)  , positionz);
                  Vector3 newPosition = new Vector3((float)(-xreald + deltaz)/2, (float)((yreald - deltaY) / 2), positionz);

                    objet.transform.position = newPosition;
                    //target.Translate(newPosition);






                    int screenHeig = Screen.height;
                    int Screanweidth = Screen.width;
                    //code pour ne pas faire kiter le sphére

                    //Vector3 newPosition1;
                    /*
                    if (Math.Abs((float)xreald) > Screanweidth / 2)
                    {
                        if (-(float)xreald < 0)
                        {
                            newPosition = new Vector3((float)xreald / 2, (float)((yreald - deltaY) / 2), positionz);

                        }
                        else
                        {
                            newPosition = new Vector3(-(float)xreald / 2, (float)((yreald - deltaY) / 2), positionz);

                        }

                    }
                    if (Math.Abs((float)((yreald - deltaY) / 2)) > screenHeig / 2)
                    {

                        if ((float)((yreald - deltaY) / 2) < 0)
                        {
                            newPosition = new Vector3(-(float)xreald, -(float)((yreald - deltaY) / 2) / 2, positionz);

                        }
                        else
                        {
                            newPosition = new Vector3(-(float)xreald, (float)((yreald - deltaY) / 2) / 2, positionz);

                        }

                    }
                    */
                    

                    //transform.position = Vector3.SmoothDamp(transform.position, Camera.main.ScreenToWorldPoint(newPosition), ref velocity, smoothTime);
                   
                    //////////
                    //Vector2 roundedSampleInput = new Vector2(Mathf.RoundToInt(position[0]), Mathf.RoundToInt(position[1]));
                    //Vector3 newPos = new Vector3(-position[0], position[1], 700) - new Vector3(Camera.main.pixelWidth / 4, Camera.main.pixelHeight / 4, 0);
                    //transform.position = newPos;
                    //transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
                    /////


                    //donnees.Add(new Donnees("donnee", position[0], transform.position.x));
                    //Save();
                    //transform.position = new Vector3(position[0], position[1], position[2]);
                    //sw.WriteLine("positionBoule");
                    //sw.NewLine();
                    //string xx= "("+ transform.position.x.ToString() + position[0].ToString()
                    //left

                    /*string[] wordsleft = dataleft.Split('[',']');
                    string aaaleft = wordsleft[1].ToString();
                    string aaaleftX = aaaleft.Split(',')[0];
                    string aaaleftY = aaaleft.Split(',')[1];
                    string aaaleftZ = aaaleft.Split(',')[2];
                    //right
                    string[] wordsright = dataright.Split('[', ']');
                    string aaaright = wordsright[1].ToString();
                    string aaarightX = aaaright.Split(',')[0];
                    string aaarightY = aaaright.Split(',')[1];
                    string aaarightZ = aaaright.Split(',')[2];

                    float x = (float)xreald;*/
                    //swb.Write(position[0].ToString() + ";" + position[1].ToString() + ";" + position[2].ToString() + "\r\n");


                    //swb.Write( position[0].ToString() + ";" + position[1].ToString()+";"+ position[2].ToString() +";" + yreald.ToString() + ";" + xreald.ToString() + "\r\n");
                    //sw.Write( vposition + "\r\n") ;
                    //sw.WriteLine("positionRegrad");
                    //sw.WriteLine("," + position[0]);
                    //sw.Flush();
                    
                    //Console.WriteLine("Detecte Gaze Position 3D " + "x1" + position[0]+ "\ny" + position[1] + "\nz" + position[2]);

                    //print(transform.position);

                }
                
            }
        }
        
    }
}
