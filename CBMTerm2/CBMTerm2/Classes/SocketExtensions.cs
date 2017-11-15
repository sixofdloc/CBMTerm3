using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CBMTerm2.Classes
{
    static class SocketExtensions
    {
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                bool b =  !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
                return b;
            }
            catch (Exception) { return false; }
        }

        public static void ActivateKeepAlives(this Socket socket)
        {
            int size = sizeof(UInt32);
            UInt32 on = 1;
            UInt32 keepAliveInterval = 10000; //Send a packet once every 10 seconds.
            UInt32 retryInterval = 1000; //If no response, resend every second.
            byte[] inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(on), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(retryInterval), 0, inArray, size * 2, size);
            socket.IOControl(IOControlCode.KeepAliveValues, inArray, null);
        }
    }
}
