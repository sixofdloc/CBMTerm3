using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBMTerm2.Classes
{
    public interface IDataReceiver
    {
        void OnDataReceived(byte[] data, int count);
        void OnDisconnect();
    }
}
