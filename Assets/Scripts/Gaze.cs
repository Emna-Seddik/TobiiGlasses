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
            
        }

        // Update is called once per frame
        void Update()
        {
            
            Vector3 gazePosition = Vector3.zero;
            float[] gp3 = new float[3];
            dataReceiveString = ReceiveData.RData(socketData);
            
            SendKeepAliveMessage.SendKAM(ipEndPoint, socketData, socketVideo);
            while (!dataReceiveString.Contains("gp3"))
            {
                dataReceiveString = ReceiveData.RData(socketData);
                
            }

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
                // get gaze position from tobii ^^'
                if (dataReceiveString.Contains("gp3"))
                {
                    gp3 = ConvertGP3Data.CData(dataReceiveString);
                    gazePosition = new Vector3(-gp3[0]/1000, gp3[1]/1000, gp3[2]/1000);
                    if((Mathf.Abs(gp3[0])<15f) && (Mathf.Abs(gp3[1]) < 15f))
                    {
                        Debug.Log("Gaze Position x= " + gazePosition.x);
                        Debug.Log("Gaze Position y= " + gazePosition.y);
                        Debug.Log("Rotation de GazeRef =" + this.transform.parent.rotation.eulerAngles);
                        Application.Quit();
                    }
                    Debug.Log(dataReceiveString);
                }
            }

            this.transform.localPosition = gazePosition;

            // make the gaze bject look forrward
            //TODO : verify ths line
            this.transform.LookAt(2 * this.transform.position - this.transform.parent.position);


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
                    // hit.collider.
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
