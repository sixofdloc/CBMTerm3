using System.Collections.Generic;
using System.Linq;

namespace FileTransferProtocols
{
    class PunterBlock
    {
        public ushort AdditiveChecksum { get; set; }
        public ushort CLCChecksum { get; set; }
        public byte NextBlockSize { get; set; }
        public ushort BlockNumber { get; set; }
        public List<byte> PacketBody { get; set; }
        
        //For receiving only
        public bool ReceiveError { get; set; }

        public PunterBlock()
        {
            PacketBody = new List<byte>();
        }

        public PunterBlock(byte[] FileSegment, byte nextblocksize, ushort blocknumber)
        {
            NextBlockSize = nextblocksize;
            BlockNumber = blocknumber;
            PacketBody = FileSegment.ToList();
            Generate_Checksum();
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[PacketBody.Count + 7];
            bytes[0] = (byte)(AdditiveChecksum & 0x00ff);
            bytes[1] = (byte)((AdditiveChecksum >> 8) & 0x00ff);
            bytes[2] = (byte)(CLCChecksum & 0x00ff);
            bytes[3] = (byte)((CLCChecksum >> 8) & 0x00ff);
            bytes[4] = NextBlockSize;
            bytes[5] = (byte)(BlockNumber & 0x00ff);
            bytes[6] = (byte)((BlockNumber >> 8) & 0x00ff);
            for (int i = 0; i < PacketBody.Count; i++)
            {
                bytes[7 + i] = PacketBody[i];
            }
            return bytes;
        }

        public void ReceiveBytes(byte[] bytes)
        {
            PacketBody.Clear();
            AdditiveChecksum = (ushort)(((bytes[1] << 8) & 0xff00) + bytes[0]);
            CLCChecksum = (ushort)(((bytes[3] << 8) & 0xff00) + bytes[2]);
            BlockNumber = (ushort)(((bytes[6] << 8) & 0xff00) + bytes[5]);
            NextBlockSize = bytes[4];
            for (int i = 7; i < bytes.Length; i++)
            {
                PacketBody.Add(bytes[i]);
            }
        }

        public void Generate_Checksum()
        {
            AdditiveChecksum = 0;
            CLCChecksum = 0;
            byte[] TempPacket = ToBytes();
            bool b = false;
            for (int i = 4; i < TempPacket.Length; i++)
            {
                AdditiveChecksum += TempPacket[i];
                CLCChecksum ^= TempPacket[i];
                //i++;
                CLCChecksum = (ushort)((CLCChecksum << 1) | (CLCChecksum >> 15));
            }
        }

        public bool Verify_Checksum()
        {
            bool b = false;
            ushort TempAdd = AdditiveChecksum;
            ushort TempCLC = CLCChecksum;

            Generate_Checksum();
            b = ((TempAdd == AdditiveChecksum) && (TempCLC == CLCChecksum));
            AdditiveChecksum = TempAdd;
            CLCChecksum = TempCLC;
            return b;
        }

    }
}
