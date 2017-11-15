using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBMTerm2.Interfaces
{
    interface IFileTransferProtocol
    {
        void Initialize(IDataSender dataout, IStatusRecipient statusrecipient);
        int SendFile(byte[] data); //Send File
        byte[] ReceiveFile(); //Receive File
        void Cancel();
        void OnDataReceived(byte[] data, int count);
        void OnDisconnect();
    }
}
