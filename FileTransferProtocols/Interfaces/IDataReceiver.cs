using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTransferProtocols.Interfaces
{
    public interface IDataReceiver
    {
        void OnDataReceived(byte[] data, int count);
        void OnDisconnect();
    }
}
