using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using TobiiGlasses;
using System.Net;
using System.Net.Sockets;


namespace TobiiGlasses
{
    public class Gaze : MonoBehaviour
    {
        [SerializeField] private LayerMask screenLayer;
        [SerializeField] private LayerMask droneLayer;
        [SerializeField] private Material highlightedMaterial;
        [SerializeField] private Material normalMaterial;
        private Transform _selection;


        StreamReader stream ;
        static int liveCtrlPort = 49152;
        static IPAddress ipGlasses = IPAddress.Parse("192.168.71.50");
        static IPEndPoint ipEndPoint = new IPEndPoint(ipGlasses, liveCtrlPort);
        static Socket socketData = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static Socket socketVideo = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        string dataReceiveString;

        StreamWriter swReality, swExpected, topLeftFile,topRightFile, bottomLeftFile,bottomRightFile;
        Vector3 mTopLeft = new Vector3(1.3156f,1.18569f,1.24492f);
        Vector3 mTopRight = new Vector3(1.30179f, 1.18901f, 0.70849f);
        Vector3 mBottomLeft = new Vector3(1.31939f, 0.8445f, 1.24694f);
        Vector3 mBottomRight = new Vector3(1.30525f, 0.8466f, 0.70845f);
        Vector3 mCenter = new Vector3(1.309f , 1.0155f, 0.97815f);
        

        // Start is called before the first frame update
        void Start()
        {

            socketData.Connect(ipEndPoint);
            socketVideo.Connect(ipEndPoint);
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            if (Utils.mock)
            {
                stream = new StreamReader("./Assets/mocks/gaze.txt");
            }

            swReality = new StreamWriter(@"C:\Users\EyeTracking\Documents\GitHub\TopLeftReality.csv");
            swExpected = new StreamWriter(@"C:\Users\EyeTracking\Documents\GitHub\TopLeftExpected.csv");


            topLeftFile = new StreamWriter(@"C:\Users\EyeTracking\Documents\GitHub\topLeftFile.csv");
            topLeftFile.Write("euler Angles reality " + ";" + "euler Angles expected" + ";" + "Error rate" + "\r\n");
            topLeftFile.Flush();

            //ecran
            topRightFile = new StreamWriter(@"C:\Users\EyeTracking\Documents\GitHub\topRightFile.csv");
            topRightFile.Write("euler Angles reality " + ";" + "euler Angles expected" + ";" + "Error rate" + "\r\n");
            topRightFile.Flush();
            bottomLeftFile = new StreamWriter(@"C:\Users\EyeTracking\Documents\GitHub\bottomLeftFile.csv");
            bottomLeftFile.Write("euler Angles reality " + ";" + "euler Angles expected" + ";" + "Error rate" + "\r\n");
            bottomLeftFile.Flush();
            bottomRightFile = new StreamWriter(@"C:\Users\EyeTracking\Documents\GitHub\bottomRightFile.csv");
            bottomRightFile.Write("euler Angles reality " + ";" + "euler Angles expected" + ";" + "Error rate" + "\r\n");
            bottomRightFile.Flush();

        }

        // Update is called once per frame
        void Update()
        {
            
            Vector3 gazePosition = Vector3.zero;
            

            if (Utils.mock)
            {
                string line = null;
                // Open the text file using a stream reader.

                // Read the stream to a string, and write the string to the con--sole.
                line = stream.ReadLine();
                Debug.Log(line);
                Vector3 gazeV = ConvertGP3Data.getValidGP3(line);
                Debug.Log("gazeV magnitude =" + gazeV.magnitude);
                //left handed sys coor
                gazePosition = new Vector3(-gazeV.x, gazeV.y, gazeV.z);
            } else
            {
                float[] gp3 = new float[3];
                dataReceiveString = ReceiveData.RData(socketData);

                SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
                while (!dataReceiveString.Contains("gp3"))
                {
                    dataReceiveString = ReceiveData.RData(socketData);

                }
                // get gaze position from tobii ^^'
                if (dataReceiveString.Contains("gp3"))
                {
                    Debug.Log(dataReceiveString);
                    Vector3 gazeV = ConvertGP3Data.getValidGP3(dataReceiveString);
                    gazePosition = new Vector3(-gazeV.x, gazeV.y, gazeV.z);

                    //gazePosition = new Vector3(-gp3[0]/1000, gp3[1]/1000, gp3[2]/1000);
                    //this.transform.parent.LookAt(mCenter);
                    if ((Mathf.Abs(gp3[0])<15f) && (Mathf.Abs(gp3[1]) < 15f))
                    {
                        Debug.Log("Gaze Position x= " + gazePosition.x);
                        Debug.Log("Gaze Position y= " + gazePosition.y);
                        Debug.Log("Rotation de GazeRef =" + this.transform.parent.rotation.eulerAngles);
                        Application.Quit();
                    }
                                        
                }
            }

            this.transform.localPosition = gazePosition;

            // make the gaze bject look forrward
            //TODO : verify ths line
            this.transform.LookAt(2 * this.transform.position - this.transform.parent.position);
            //swReality.Write(gazePosition.x + ";" + gazePosition.y + ";" + gazePosition.z + ";" + this.transform.parent.rotation.eulerAngles.x + ";" + this.transform.parent.rotation.eulerAngles.y + ";" + this.transform.parent.rotation.eulerAngles.z + "\r\n");
            //swReality.Flush();
            //this.transform.parent.LookAt(mCenter);
            //swExpected.Write(gazePosition.x + ";" + gazePosition.y + ";" + gazePosition.z + ";" + this.transform.parent.rotation.eulerAngles.x + ";" + this.transform.parent.rotation.eulerAngles.y + ";" + this.transform.parent.rotation.eulerAngles.z + "\r\n");
            //swExpected.Flush();

            if (Input.GetKeyDown(KeyCode.A))
            {
                remplir(topLeftFile,mTopLeft);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                remplir(topRightFile, mTopRight);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                remplir(bottomLeftFile, mBottomLeft);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                remplir(bottomRightFile, mBottomRight);
            }





            if (_selection != null)
            {
                var selectionRenderer = _selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = normalMaterial;
                    _selection = null;
                }
            }
            RaycastHit hitEcran;
            RaycastHit hitDrone;
            // Does the ray intersect any objects 
            if (Physics.Raycast(transform.parent.position, this.transform.forward, out hitEcran, Mathf.Infinity, screenLayer))
            {
                var selection = hitEcran.transform;
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightedMaterial;
                    _selection = selection;
                }

                if (hitEcran.collider != null)
                {
                     
                        Debug.Log("hit point :"+ hitEcran.point);
                }
                Debug.DrawRay(transform.parent.position, this.transform.forward * hitEcran.distance * 10, Color.green);
                //Debug.DrawRay(transform.parent.position, this.transform.forward * , Color.green);
                Debug.Log("Did Hit");
            }
            else
            {
                Debug.DrawRay(transform.parent.position, this.transform.forward * 1000, Color.white);
                Debug.Log("Did not Hit");
            }

            //does the ray intersect drones
            if (Physics.Raycast(transform.parent.position, this.transform.forward, out hitDrone, Mathf.Infinity, droneLayer))
            {
                var selection = hitDrone.transform;
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightedMaterial;
                    _selection = selection;
                }

                if (hitDrone.collider != null)
                {

                    Debug.Log("hit point :" + hitDrone.point);
                }
                Debug.DrawRay(transform.parent.position, this.transform.forward * hitDrone.distance * 10, Color.green);
                //Debug.DrawRay(transform.parent.position, this.transform.forward * , Color.green);
                Debug.Log("Did Hit");
            }
            else
            {
                Debug.DrawRay(transform.parent.position, this.transform.forward * 1000, Color.white);
                Debug.Log("Did not Hit");
            }

        }
        public void remplir(StreamWriter file, Vector3 position )
        {
            float angleRealityX = this.transform.parent.rotation.eulerAngles.x;
            float angleRealityY = this.transform.parent.rotation.eulerAngles.y;
            float angleRealityZ = this.transform.parent.rotation.eulerAngles.z;
            
            this.transform.parent.LookAt(position);
            float angleExpectedX = this.transform.parent.rotation.eulerAngles.x;
            float angleExpectedY = this.transform.parent.rotation.eulerAngles.y;
            float angleExpectedZ = this.transform.parent.rotation.eulerAngles.z;

            float deltaX = angleRealityX - angleExpectedX;
            float deltaY = angleRealityY - angleExpectedY;
            float deltaZ = angleRealityZ - angleExpectedZ;


            file.Write("(" + angleRealityX + "," + angleRealityY + "," + angleRealityZ + ")" + ";" 
                     + "(" + this.transform.parent.rotation.eulerAngles.x + "," + this.transform.parent.rotation.eulerAngles.y + "," + this.transform.parent.rotation.eulerAngles.z + ")" + ";" 
                     + "("+ deltaX +"," + deltaY+","+ deltaZ+")"+"\r\n");


            file.Flush();
        }
    }
}
