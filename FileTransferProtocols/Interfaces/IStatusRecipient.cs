using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTransferProtocols.Interfaces
{
    public interface IStatusRecipient
    {
        void Initialize(int totalbytesinfile, string filename, bool sending, string caption, Action xferAbort);
        void Update(int bytestransferred);
        void MicroUpdate(string text);
        void Done(bool good);
        void Abort();
    }
}
