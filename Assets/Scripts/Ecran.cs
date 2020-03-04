using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace TobiiGlasses
{
    public class Ecran : MonoBehaviour
    {
        public Camera cameraFixe;
        public Camera cameraMobile;
        public Transform lunette;
        private float ecranHeight;
        private float ecranWidth;

        StreamReader stream ;

        // Start is called before the first frame update
        void Start()
        {
            float aspect = Screen.width / Screen.height;
            Debug.Log("Screen.width  :" + Screen.width);
            Debug.Log("Screen.height :" + Screen.height);
            Debug.Log("aspect screen :"+aspect);
            Debug.Log("dpi screen :" + Screen.dpi);
            Debug.Log("aspect camera :"+ Camera.main.aspect);
            ecranHeight = (Screen.height / Screen.dpi) * 0.0254f;
            Debug.Log("Screen.height en metre:" + ecranHeight);
            ecranWidth = (Screen.width / Screen.dpi) * 0.0254f;
            Debug.Log("Screen.width en metre:" + ecranWidth);
            this.transform.localScale = new Vector3(0.01f, ecranHeight, ecranWidth);

            Debug.Log("fieldOfView camera Fixe:" + cameraFixe.fieldOfView);
            Debug.Log("aspect camera Fixe:" + cameraFixe.aspect);
            Debug.Log("fieldOfView camera Mobile :" + cameraMobile.fieldOfView);
            Debug.Log("aspect camera Mobile :" + cameraMobile.aspect);
            float tanVFOV = Mathf.Tan(Mathf.Deg2Rad * (cameraFixe.fieldOfView/2));
            float deltaX = ecranHeight / (2 * tanVFOV);
            Debug.Log(" camera x :" + deltaX);
            Vector3 positionCamFixe = Vector3.zero;
            positionCamFixe.x= -deltaX;
            cameraFixe.transform.localPosition = positionCamFixe;



            if (Utils.mock)
            {
                stream = new StreamReader("./Assets/mocks/ecran.txt");
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Utils.mock)
            {
                string line = null;
                // Open the text file using a stream reader.

                // Read the stream to a string, and write the string to the con--sole.
                line = stream.ReadLine();
                Debug.Log(line);
                string[] strings = line.Split(';');
                this.transform.parent.position = new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
                this.transform.parent.rotation = new Quaternion(float.Parse(strings[3]), float.Parse(strings[4]), float.Parse(strings[5]), float.Parse(strings[6]));
                
            }
            


        }
        private void LateUpdate()
        {
            Vector3 ecranPosition = this.transform.position;
            cameraMobile.transform.position = lunette.position;
            cameraMobile.transform.LookAt(ecranPosition);
            Vector3 ecranTop = new Vector3(ecranPosition.x, ecranPosition.y + (ecranHeight / 2), ecranPosition.z);
            float verticalFovAngle = Vector3.Angle(ecranPosition - cameraMobile.transform.position, ecranTop - cameraMobile.transform.position);
            Debug.Log("Angle : " + verticalFovAngle);
            cameraMobile.fieldOfView = verticalFovAngle*2;
        }
    }
}
