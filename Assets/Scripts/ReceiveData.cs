using System.Net.Sockets;
using System.Text;

namespace TobiiGlasses
{
    public class ReceiveData
    {

        public static string RData(Socket socketData)
        {
            socketData.ReceiveTimeout = 1000;
            byte[] dataReceivedBytes = new byte[100];
            socketData.Receive(dataReceivedBytes);
            string dataReceivedString = Encoding.ASCII.GetString(dataReceivedBytes);
            //string dataReceivedString = Encoding.UTF8.GetString(dataReceivedBytes);
            return dataReceivedString;
        }
    }
}
