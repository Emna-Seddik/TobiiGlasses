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

        StreamWriter swReality, swExpected;
        Vector3 mTopLeft = new Vector3(1.31474f,1.1847f,1.24651f);
        Vector3 mTopRight = new Vector3(1.29703f, 1.18625f, 0.70979f);
        Vector3 mBottomLeft = new Vector3(1.31929f, 0.84442f, 1.24656f);
        Vector3 mBottomRight = new Vector3(1.3026f, 0.84673f, 0.97792f);
        Vector3 mCenter = new Vector3(1.301899f , 1.017719f , 0.9737017f);
        

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
                    
                    Vector3 gazeV = ConvertGP3Data.getValidGP3(dataReceiveString);
                    gazePosition = new Vector3(-gazeV.x, gazeV.y, gazeV.z);
                    
                    //gazePosition = new Vector3(-gp3[0]/1000, gp3[1]/1000, gp3[2]/1000);
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
            this.transform.parent.LookAt(mCenter);
            //swExpected.Write(gazePosition.x + ";" + gazePosition.y + ";" + gazePosition.z + ";" + this.transform.parent.rotation.eulerAngles.x + ";" + this.transform.parent.rotation.eulerAngles.y + ";" + this.transform.parent.rotation.eulerAngles.z + "\r\n");
            //swExpected.Flush();


            if (_selection != null)
            {
                var selectionRenderer = _selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = normalMaterial;
                    _selection = null;
                }
            }
            RaycastHit hit;
            // Does the ray intersect any objects 
            if (Physics.Raycast(transform.parent.position, this.transform.forward, out hit, Mathf.Infinity, screenLayer))
            {
                var selection = hit.transform;
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightedMaterial;
                    _selection = selection;
                }

                if (hit.collider != null)
                {
                     
                        Debug.Log("hit point :"+ hit.point);
                }
                Debug.DrawRay(transform.parent.position, this.transform.forward * hit.distance * 10, Color.green);
                //Debug.DrawRay(transform.parent.position, this.transform.forward * , Color.green);
                Debug.Log("Did Hit");
            }
            else
            {
                Debug.DrawRay(transform.parent.position, this.transform.forward * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
            

        }
    }
}
