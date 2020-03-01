using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TobiiGlasses
{
    public class LunetteReferentiel : MonoBehaviour
    {
        public Transform lunetteRef;
        public Transform lunetteObj;

        StreamReader stream ;

        // Start is called before the first frame update
        void Start()
        {
            if (Utils.mock) {
                stream = new StreamReader("./Assets/mocks/lunette.txt");
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
                lunetteRef.position = new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
                lunetteRef.rotation = new Quaternion(float.Parse(strings[3]), float.Parse(strings[4]), float.Parse(strings[5]), float.Parse(strings[6]));
                lunetteObj.localPosition = Vector3.zero;
            }

        }
    }
}
