using System;
using System.Globalization;
//using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Globalization;



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
            GazePosition3D gazePosition3D = new GazePosition3D(jobject);
            String coordonnee = gazePosition3D.gp3.Replace("[", "").Replace("]", "").Replace(" ", "");
            string[] stringSeparators = new string[] { "," };
            string[] result = coordonnee.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            //float.Parse((string)result.GetValue(0), CultureInfo.InvariantCulture);
            //*((float*)parameter.value) = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);

            //JObject o = JObject.Parse(dataReceivedString);

            //Console.WriteLine(o.ToString());

            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            //Debug.Log(dataReceivedString);
            //Debug.Log(dataReceivedString.GetType()); // string

            /*int index = dataReceivedString.IndexOf("[");
            string xyz = dataReceivedString.Substring(index, dataReceivedString.Length-1);
            Debug.Log(xyz);*/

            string[] words = dataReceivedString.Split('[');
            string aaa = words[1].ToString();
            string[] xyz = aaa.Split(']');
            // xyz[0] contient la chaine suivante "x,y,z"


            string xString = xyz[0].Split(',')[0];
            string yString = xyz[0].Split(',')[1];
            string zString = xyz[0].Split(',')[2];

            //Debug.Log("emnaaaaaaaaaaaa" + zString);

            float x = float.Parse(xString.ToString(), CultureInfo.InvariantCulture);
            float y = float.Parse(yString.ToString(), CultureInfo.InvariantCulture);
            float z = float.Parse(zString.ToString(), CultureInfo.InvariantCulture);

            

            //float x = float.Parse("0,0");
            //Console.WriteLine(x);
            //Console.WriteLine("Detecte Gaze Position 3D " + gp3D + "x"+ xString + "\ny"+ yString + "\nz"+ zString);
            float[] output = new float[3];
            output[0] = x;
            output[1] = y;
            output[2] = z;
            return output;
        }
    }
}
