using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Globalization;
using System.Collections.Generic;




namespace TobiiGlasses
{

    public class ConvertGP3Data
    {

        // Class GazePosition3D
        public class Data { }
        public class GazePosition3D : Data
        {
            public string ts { get; set; }
            public string gp3 { get; set; }
            public string s { get; set; }

            public GazePosition3D(JSONObject jo)
            {
                this.gp3 = jo.GetField("gp3").ToString();
                this.ts = jo.GetField("ts").ToString();
                this.s = jo.GetField("s").ToString();
            }
        }
        public static float[] CData(string dataReceivedString)
        {
           

            JSONObject jobject = new JSONObject(dataReceivedString);
            //JSONNode data = JSON.Parse(dataReceivedString);
            //string json = JsonUtility.ToJson(dataReceivedString);
            
            
            //string x = JsonConvert.serialiseObject(dataReceivedString);
            
            GazePosition3D gazePosition3D = new GazePosition3D(jobject);
            String coordonnee = gazePosition3D.gp3.Replace("[", "").Replace("]", "").Replace(" ", "");
            string[] stringSeparators = new string[] { "," };
            string[] result = coordonnee.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            string[] words = dataReceivedString.Split('[');
            string aaa = words[1].ToString();
            string[] xyz = aaa.Split(']');
            



            string xString = xyz[0].Split(',')[0];
            string yString = xyz[0].Split(',')[1];
            string zString = xyz[0].Split(',')[2];

            //Debug.Log("contenu de string 1" + xyz[0].Split(',')[0]);
            //Debug.Log("contenu de string 2" + xyz[0].Split(',')[1]);
            //Debug.Log("contenu de string 3" + xyz[0].Split(',')[2]);



            float x = float.Parse(xString.ToString(), CultureInfo.InvariantCulture);
            float y = float.Parse(yString.ToString(), CultureInfo.InvariantCulture);
            float z = float.Parse(zString.ToString(), CultureInfo.InvariantCulture);
            //float x = float.Parse("0,0");
            //Console.WriteLine(x);
            
            float[] output = new float[3];
            output[0] = x;
            output[1] = y;
            output[2] = z;
            return output;
        }

        public static Vector3 getValidGP3(string dataReceivedString)
        {


            JSONObject jobject = new JSONObject(dataReceivedString);
            //Vector3 gaze = null;

            GazePosition3D gazePosition3D = new GazePosition3D(jobject);
            if (gazePosition3D.s.Equals("0"))
            {

                String coordonnee = gazePosition3D.gp3.Replace("[", "").Replace("]", "").Replace(" ", "");
                string[] stringSeparators = new string[] { "," };
                string[] result = coordonnee.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                string[] words = dataReceivedString.Split('[');
                string aaa = words[1].ToString();
                string[] xyz = aaa.Split(']');
                
                string xString = xyz[0].Split(',')[0];
                string yString = xyz[0].Split(',')[1];
                string zString = xyz[0].Split(',')[2];
                                 
                float x = float.Parse(xString.ToString(), CultureInfo.InvariantCulture)/1000;
                float y = float.Parse(yString.ToString(), CultureInfo.InvariantCulture)/1000;
                float z = float.Parse(zString.ToString(), CultureInfo.InvariantCulture)/1000;

                return new Vector3(x,y,z);
            }
            return Vector3.positiveInfinity;
        }
    }
}
