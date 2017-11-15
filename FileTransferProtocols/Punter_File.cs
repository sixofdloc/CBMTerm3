using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTransferProtocols
{
    public class Punter_File
    {
        public byte[] Filename { get; set; }
        public byte FileType { get; set; }
        public byte[] FileData { get; set; }
    }
}
