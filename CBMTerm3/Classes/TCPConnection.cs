using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;

namespace CBMTerm3.Classes
{
    public class TCPConnection 
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static System.Threading.Timer lostTimer;

        // State object for receiving data from remote device.
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        public static List<IDataReceiver> Receivers;
        public static List<IConnectionMonitor> ConnectionMonitors;
        public static StateObject Stateobject;

        public static void Connect(string address, int port, IDataReceiver receiver, IConnectionMonitor monitor)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(address);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Receivers = new List<IDataReceiver>();
                Receivers.Add(receiver);
                ConnectionMonitors = new List<IConnectionMonitor>();
                ConnectionMonitors.Add(monitor);
                // Create a TCP/IP socket.
                Stateobject = new StateObject();
                Stateobject.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Connect to the remote endpoint.
                Stateobject.workSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), Stateobject.workSocket);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to connect");
            }
        }

        public static void Send(byte[] data)
        {
            try
            {
                Stateobject.workSocket.BeginSend(data, 0, data.Length, 0,
                    new AsyncCallback(SendCallback), Stateobject.workSocket);
            }
            catch (Exception e)
            {
                //fuck it.
            }
        }

        public static void Send(string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            Send(byteData);
        }

        public static void Send(byte[] data, int count)
        {
            byte[] byteData = new byte[count];
            for (int i = 0; i < count; i++)
                byteData[i] = data[i];
            Send(byteData);
        }
        
        public static void Send(byte[] data, int count,int delay)
        {
            for (int i = 0; i < count; i++)
            {
                Send(data[i]);
                Thread.Sleep(delay);
            }
        }


        public static void Send(byte data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = new byte[1];
            byteData[0] = data;
            Send(byteData);
        }


        #region callbacks

        public static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);
                client.ActivateKeepAlives();
                client.ReceiveTimeout = 1000;
                client.SendTimeout = 1000;

                // Signal that the connection has been made.
                connectDone.Set();
                client.BeginReceive( Stateobject.buffer, 0, StateObject.BufferSize, 0,new AsyncCallback(ReceiveCallback), Stateobject);
                foreach (IConnectionMonitor icm in ConnectionMonitors)
                {
                    icm.OnConnect();
                }
                TimerCallback timerDelegate = new TimerCallback(CheckConnect);
                lostTimer = new System.Threading.Timer(timerDelegate, null, 3000, 1000);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private static void CheckConnect(object eventState)
        {
            bool b = false;
            lostTimer.Change(System.Threading.Timeout.Infinite,System.Threading.Timeout.Infinite);
            try
            {
                if (!Stateobject.workSocket.IsConnected())
                {
                    Stateobject.workSocket = null;
                    DisconnectNotify();
                    lostTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    b = true;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
               if (!b) lostTimer.Change(1000, 1000);
            }

        }

        public static void DisconnectNotify()
        {
            foreach (IConnectionMonitor icm in ConnectionMonitors)
            {
                icm.OnDisconnect();
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int bytesRead = 0;
                try
                {
                    // Read data from the remote device.
                    bytesRead = client.EndReceive(ar);
                }
                catch (ObjectDisposedException ode)
                {
                    bytesRead = 0;
                    DisconnectNotify();
                }
                catch (SocketException se)
                {
                    bytesRead = 0;
                }
                catch (Exception e)
                {
                    bytesRead = 0;
                }

                if (bytesRead == 0)
                {
                    client.Disconnect(false);
                }
                else
                {
                    if (Receivers.Count > 0)
                    {
                        foreach (IDataReceiver ir in Receivers)
                        {
                            ir.OnDataReceived(state.buffer, bytesRead);
                        }
                    }
                    if (client.Connected)
                    {
                        try
                        {
                            client.BeginReceive(Stateobject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), Stateobject);
                        }
                        catch (Exception e)
                        {
                            DisconnectNotify();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                if (Stateobject.workSocket != null)  Stateobject.workSocket.Close();
                DisconnectNotify();
            }
        }


        public static void Disconnect()
        {
            if (Stateobject.workSocket != null)
            {
                Stateobject.workSocket.Close();
                DisconnectNotify();
            }
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
               MessageBox.Show(e.ToString());
            }
        }
        #endregion
    }
}