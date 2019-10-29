using System;

namespace TobiiGlasses
{
    public class ConvertGP2Data
    {

        // Class GazePosition3D
        public class Data { }
        public class GazePosition2D : Data
        {
            public string ts { get; set; }
            public string gp { get; set; }
            public string s { get; set; }
            public string l { get; set; }

            public GazePosition2D(JSONObject jo)
            {
                this.gp = jo.GetField("gp").ToString();
                this.ts = jo.GetField("ts").ToString();
                this.s = jo.GetField("s").ToString();
                this.l = jo.GetField("l").ToString();
            }
        }
        public static float[] CData(string dataReceivedString)
        {
            JSONObject jobject = new JSONObject(dataReceivedString);
            GazePosition2D gazePosition2D = new GazePosition2D(jobject);
            String coordonnee = gazePosition2D.gp.Replace("[", "").Replace("]", "").Replace(" ", "");
            string[] stringSeparators = new string[] { "," };
            string[] result = coordonnee.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            string xString = (string)result.GetValue(0);
            string yString = (string)result.GetValue(1);
            float x = Convert.ToSingle(xString);
            float y = Convert.ToSingle(yString);
            float[] output = new float[2];
            output[0] = x;
            output[1] = y;
            return output;
        }
    }
}
