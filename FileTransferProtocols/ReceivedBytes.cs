using System.Collections.Generic;

namespace FileTransferProtocols
{
    public class ReceivedBytes
    {
        public List<byte> bytes { get; set; }
        public bool TimedOut { get; set; }
    }
}
