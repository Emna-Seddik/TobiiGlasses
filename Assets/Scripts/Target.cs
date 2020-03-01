using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Globalization;

namespace TobiiGlasses
{

    public class Target : MonoBehaviour
    {

        public string dataleft;
        public string dataright;
        public Transform target;
        public float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;

        public GameObject objet;
        public Transform markerEcran;
        public Transform markerLunette;
        public Transform gaze;
        public Transform line;
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

        StreamReader sre;
        StreamReader srl;
        StreamReader srg;


        StreamWriter swb;

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
 
            if(Utils.mock) {
                sre = new StreamReader("E:\\emna\\datas\\24-02-2020\\ecran.txt");
                srl = new StreamReader("E:\\emna\\datas\\24-02-2020\\lunette.txt");
                srg = new StreamReader("E:\\emna\\datas\\24-02-2020\\gaze1.txt");
            }


        }


        void drawRay(Vector3 position)
        {
            Vector3 mousePosFar = new Vector3(position.x,
                                              position.y,
                                              Camera.main.farClipPlane);
            Vector3 mousePosNear = new Vector3(position.x,
                                               position.y,
                                               Camera.main.nearClipPlane);
            Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
            Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);
        }




        void Update()
        {
            Debug.Log(Time.time + ":OHOOOOHHH");
            Console.WriteLine(Time.time + ":OHOOOOHHH BARRAH");
            if (Utils.mock)
            {

                String lineE = null;
                String lineL = null;
                String lineG = null;
                try
                {   // Open the text file using a stream reader.
                    //using (sre)
                    {
                        // Read the stream to a string, and write the string to the con--sole.
                        lineE = sre.ReadLine();
                        Debug.Log(lineE);
                        Console.WriteLine(lineE);
                        string[] strings = lineE.Split(';');
                        markerEcran.position = new Vector3(float.Parse(strings[2]), float.Parse(strings[3]), float.Parse(strings[4]));
                        markerEcran.rotation = new Quaternion(float.Parse(strings[5]), float.Parse(strings[6]), float.Parse(strings[7]), float.Parse(strings[8]));
                    }
                    //using (srg)
                    {
                        // Read the stream to a string, and write the string to the console.
                        lineG = srg.ReadLine();
                        Debug.Log(lineG);
                        Console.WriteLine(lineG);
                        Vector3 gazeV = ConvertGP3Data.getValidGP3(lineG);
                        Debug.Log("gazeV magnitude =" + gazeV.magnitude);
                        //left handed sys coor
                        Vector3 gazeVlh = new Vector3(-gazeV.x, gazeV.y, gazeV.z);
                        gaze.localPosition = gazeVlh;// * 10;
                        //gaze.position = gazeV;


                    }
                    // using (srl)
                    {
                        // Read the stream to a string, and write the string to the console.
                        lineL = srl.ReadLine();
                        Debug.Log(lineL);
                        Console.WriteLine(lineL);
                        string[] strings = lineL.Split(';');
                        Quaternion rotation = Quaternion.Euler(0, 90, 0);
                        //markerLunette.localRotation = rotation;
                        markerLunette.position = new Vector3(float.Parse(strings[2]), float.Parse(strings[3]), float.Parse(strings[4]));
                        //markerLunette.position = Vector3.zero;

                        Quaternion quaternion = new Quaternion(float.Parse(strings[5]), float.Parse(strings[6]), float.Parse(strings[7]), float.Parse(strings[8]));
                        Vector3 euler = quaternion.eulerAngles;

                        
                        //markerLunette.localRotation = quaternion;
                        //markerLunette.Rotate(euler,Space.Self);

                    }

                    Quaternion lunetteGazeRot = Quaternion.FromToRotation(gaze.localPosition, markerEcran.position - markerLunette.position);
                    Debug.Log("lunetteGazeRot=" + lunetteGazeRot);
                    //markerLunette.rotation *= lunetteGazeRot;

                    RaycastHit raycastHit;
                    Physics.Raycast(markerLunette.position, gaze.position, out raycastHit);
                    Debug.DrawRay(markerLunette.position, gaze.position, Color.cyan);
                    Debug.DrawRay(markerLunette.position, gaze.localPosition, Color.red);
                    Debug.Log("gaze position : " + gaze.position + " , local : " + gaze.localPosition + ", rotation:" + gaze.localRotation);
                    Debug.Log("gaze magnitude =" + gaze.localPosition.magnitude);
                    Debug.Log("distance lunette <-> ecran =" + Vector3.Distance(markerLunette.transform.position, markerEcran.transform.position));
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            else
            {

                //Debug.Log("Données reçues target" );
                string dataReceiveString = ReceiveData.RData(socketData);
                //string dataReceiveString = swg;
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
                if (dataReceiveString.Contains("gp3"))
                {
                    float[] gp3 = new float[3];
                    gp3 = ConvertGP3Data.CData(dataReceiveString);
                    Vector3 vposition = new Vector3(gp3[0], gp3[1], gp3[2]);
                    //Debug.Log("vposition " + dataReceiveString);
                    //drawRay(vposition);
                    float ratio = Screen.dpi / 25.4f;


                    //swb.Write(position[0].ToString() + ";" + position[1].ToString() + ";" + position[2].ToString()+ "\r\n");
                    // Debug.Log("x : " + position[0].ToString() + " y : " + position[1].ToString()  + " z : " + position[2].ToString());


                    if (gp3[1] + gp3[2] + gp3[0] != 0)
                    {
                        Vector3 camPosition = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
                        //Vector3 newPosition = new Vector3(-position[0] + camPosition.x, position[1] + camPosition.y, position[2] + camPosition.z);
                        //Vector3 newPosition = new Vector3(-position[0] , position[1] , 700);
                        //transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);










                        float positiony = gp3[1] + camPosition.y;
                        float positionx = -gp3[0] + camPosition.x;
                        float positionz = 15;









                        //float positionx = (float)(Math.Sin(position[1])*Math.Cos(-position[0])+Math.Cos(position[1])*Math.Sin(-position[0])) ;
                        //float positiony = (float)(Math.Sin(-position[0])*Math.Cos(position[1])+Math.Cos(-position[0])*Math.Sin(position[1])) ;
                        // float positionz = (float)Math.Cos(position[0])  * (float)Math.Cos(position[1]) - (float)Math.Sin(position[0]) * (float)Math.Sin(position[1]) ;
                        //positionx = positionx * (UnityEngine.Screen.width/2);
                        //positiony = positiony * (UnityEngine.Screen.height/2);
                        //positiony= UnityEngine.Screen.height/2 - positiony;
                        //positionx = UnityEngine.Screen.width/2 - positionx;
                        Double gazeanglex = Math.Atan(gp3[2] / gp3[0]);
                        //Debug.Log("marker" + Math.Abs(markerEcran.position.z - markerLunette.position.z) * 100);
                        //calculer x real en double
                        //distance entre l'utilisateur et l'écran donnée opar optitrack
                        Double disUE = Math.Abs(markerEcran.position.x - markerLunette.position.x) * 100;
                        //deplacement (deltaZ dans le système optitrack) par rapport à la position de la reference(x constante) à la meme distance de l'ecran et à la meme hauteur (y constante)
                        //marker left of the glasses
                        Double deltaz = Math.Abs(markerEcran.position.z - markerLunette.position.z - 0.09) * 100;
                        //Debug.Log("Distance utilusateur ecran horizontal" + ((markerEcran.position.z - markerLunette.position.z - 0.09) * 100));

                        Debug.Log("marker 6 de l'écran " + markerEcran.position.x);
                        Debug.Log("marker 7 des lunettes " + markerLunette.position.x);

                        Double xreald = (disUE * (1 / Math.Tan(gazeanglex)));

                        //Double xreald = (100) * (1 / Math.Tan(gazeanglex));

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
                        // Debug.Log("hhhhhhhhhhhhhhhhhhhhhhhhhhhh" + yreal);
                        //deltaY : déplacement par rapport à la position idéale. 50cm est le déplacement deltaY à une distance de 140cm de l'écran qui sera pris comme valeur de référence

                        Double deltaY = (disUE * 50) / 140;
                        Vector3 newPosition = new Vector3(-(float)xreald / 2, ((float)(yreald - deltaY)) / 2, positionz);
                        //target.Translate(newPosition);
                        objet.transform.position = newPosition;




                        int screenHeig = Screen.height;
                        int Screanweidth = Screen.width;

                        //Vector3 newPosition1;
                        if (Math.Abs(transform.position.x) > Screanweidth / 2)
                        {
                            if (transform.position.x < 0)
                            {
                                newPosition = new Vector3(-Screanweidth / 2, positiony, positionz);

                            }
                            else
                            {
                                newPosition = new Vector3(Screanweidth / 2, positiony, positionz);

                            }

                        }
                        if (Math.Abs(transform.position.y) > screenHeig / 2)
                        {

                            if (transform.position.y < 0)
                            {
                                newPosition = new Vector3(positionx, -screenHeig / 2, positionz);

                            }
                            else
                            {
                                newPosition = new Vector3(positionx, -screenHeig / 2, positionz);

                            }

                        }
                        //transform.position = Vector3.SmoothDamp(transform.position, Camera.main.ScreenToWorldPoint(newPosition), ref velocity, smoothTime);
                        //transform.position = newPosition;

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

                        string[] wordsleft = dataleft.Split('[', ']');
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

                        float x = (float)xreald;


                        //swb.Write( position[0].ToString() + ";" + position[1].ToString()+";"+ position[2].ToString() +";" + yreald.ToString() + ";" + xreald.ToString() + "\r\n");
                        //sw.Write( position[0] + "\r\n") ;
                        //sw.WriteLine("positionRegrad");
                        //sw.WriteLine("," + position[0]);
                        // swb.Flush();

                        //Console.WriteLine("Detecte Gaze Position 3D " + "x1" + position[0]+ "\ny" + position[1] + "\nz" + position[2]);

                        //print(transform.position);
                    }

                }

            }
        }

    }
}
