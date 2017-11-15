using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTransferProtocols
{
    public class ReceivedBytes
    {
        public List<byte> bytes { get; set; }
        public bool TimedOut { get; set; }
    }
}
