using System;
using System.Collections.Generic;
using FileTransferProtocols.Interfaces;
using System.Threading;

namespace FileTransferProtocols.Protocols
{
    public class Punter : IFileTransferProtocol
    {
        private class PunterTimedByte
        {
            public bool Timeout { get; set; }
            public byte Byte_Received { get; set; }
        }


        public IDataSender DataOut;
        public IStatusRecipient StatusRecipeient;
        public List<byte> SocketBuffer = new List<byte>(); //Buffer from our socket
        private List<byte> Receive_Buffer = new List<byte>();

        #region Handshaking Constatnts

        public static byte[] MULTIPUNTER_FILE_PREAMBLE = { 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09 };
        public static byte[] MULTIPUNTER_FINALIZE      = { 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04 };

        public const int HS_ACK = 0;
        public const int HS_GOO = 1;
        public const int HS_BAD = 2;
        public const int HS_SYN = 3;
        public const int HS_S_B = 4;

        private static byte[] ACK = { 0x41, 0x43, 0x4b };
        private static byte[] GOO = { 0x47, 0x4f, 0x4f };
        private static byte[] BAD = { 0x42, 0x41, 0x44 };
        private static byte[] SYN = { 0x53, 0x59, 0x4e };
        private static byte[] S_B = { 0x53, 0x2f, 0x42 };

        //Possible ACK Rotations
        private static byte[] CKA = { 0x43, 0x4b, 0x41 };
        private static byte[] KAC = { 0x4b, 0x41, 0x43 };

        //possible GOO Rotations
        private static byte[] OGO = { 0x4f, 0x47, 0x4f };
        private static byte[] OOG = { 0x4f, 0x4f, 0x47 };

        //possible BAD Rotations
        private static byte[] DBA = { 0x44, 0x42, 0x41 };
        private static byte[] ADB = { 0x41, 0x44, 0x42 };

        //Possible SYN Rotations
        private static byte[] NSY = { 0x43, 0x53, 0x59 };
        private static byte[] YNS = { 0x59, 0x43, 0x53 };

        //Possible S_B Rotations
        private static byte[] BS_ = { 0x42, 0x53, 0x2f };
        private static byte[] _BS = { 0x2f, 0x42, 0x53 };
        #endregion


        public void Initialize(IDataSender dataout, IStatusRecipient statusrecipient)
        {
            DataOut = dataout;
            StatusRecipeient = statusrecipient;
        }

        #region Socket Interface

        public void OnDataReceived(byte[] data, int count)
        {
            lock (SocketBuffer)
            {
                for (int i = 0; i < count; i++)
                {
                    SocketBuffer.Add(data[i]);
                }
            }
        }

        private void ClearReceiveBuffer()
        {
            lock (SocketBuffer)
            {
                SocketBuffer.Clear();
            }
        }

        //private byte[] GetBlock(int count)
        //{
        //    byte[] b = new byte[count];
        //    for (int i = 0; i < count; i++) b[i] = GetByte(500);
        //    return b;
        //}
        private ReceivedBytes GetLine(int timeout)
        {
            ReceivedBytes result = new ReceivedBytes();
            PunterTimedByte b = new PunterTimedByte() { Byte_Received = 0x00, Timeout = true };
            bool done = false;
            DateTime Timeout = DateTime.Now;
            while (!done)
            {
                b = GetByte(500);
                if (!b.Timeout)
                {
                    if (b.Byte_Received == 0x0d)
                    {
                        result.TimedOut = false;
                        done = true;
                    }
                    else
                    {
                        if (result.bytes == null) result.bytes = new List<byte>();
                        result.bytes.Add(b.Byte_Received);
                    }
                }
                if (!done)
                {
                    if ((Timeout.AddMilliseconds(timeout) < DateTime.Now))
                    {
                        result.TimedOut = true;
                        done = true;
                    }
                }
            }
            return result;
        }
        private PunterTimedByte GetByte(int timeout_in_ms)
        {
            PunterTimedByte pb = new PunterTimedByte();
            pb.Timeout = true;
            pb.Byte_Received = 0x00;
            try
            {
                DateTime Timeout = DateTime.Now;
                bool bytereceived = false;
                while ((Timeout.AddMilliseconds(timeout_in_ms) > DateTime.Now) && !bytereceived)
                {
                    if (SocketBuffer.Count > 0)
                    {
                        lock (SocketBuffer)
                        {
                            pb.Byte_Received = SocketBuffer[0];
                            pb.Timeout = false;
                            SocketBuffer.RemoveAt(0);
                            bytereceived = true;
                        }
                    }
                    if (!bytereceived) Thread.Sleep(1);
                }
            }
            catch (Exception e)
            {
                pb.Timeout = true;
            }
            return pb;

        }

        //private byte GetByte()
        //{
        //    byte b = 0x00;
        //    try
        //    {
        //        DateTime Timeout = DateTime.Now;
        //        bool bytereceived = false;
        //        while ((Timeout.AddMilliseconds(500) > DateTime.Now) && !bytereceived)
        //        {
        //            if (SocketBuffer.Count > 0)
        //            {
        //                lock (SocketBuffer)
        //                {
        //                    b = SocketBuffer[0];
        //                    SocketBuffer.RemoveAt(0);
        //                    bytereceived = true;
        //                }
        //            }
        //            Thread.Sleep(10);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        b = 0x00;
        //    }
        //    return b;
        //}

        private void writeBuffer(byte[] xbuff, int size)
        {
            DataOut.Send(xbuff, size);
        }

        private void writeByte(byte b)
        {
            DataOut.Send(b);
        }

        #endregion

        #region Handshaking Routines


        private void SendHandshake(int handshake)
        {
            switch (handshake)
            {
                case HS_ACK:
                    writeBuffer(ACK, 3);
                    break;
                case HS_GOO:
                    writeBuffer(GOO, 3);
                    break;
                case HS_BAD:
                    writeBuffer(BAD, 3);
                    break;
                case HS_SYN:
                    writeBuffer(SYN, 3);
                    break;
                case HS_S_B:
                    writeBuffer(S_B, 3);
                    break;
            }

        }


        private byte[] ReadBytes(int numbytes, int timeout)
        {
            byte[] b = new byte[numbytes];
            for (int i =0;i<numbytes;i++){
               b[i] = GetByte(timeout).Byte_Received;
            }
            return b;
        }

        private int WaitHandshake()
        {
            ClearReceiveBuffer();
            int retries = 0;
            int response = -1;
            while (response == -1)
            {
                response = GetHandshake();
                if (response == -1)Thread.Sleep(100);
                retries++;
                if (retries > 12) return -1;
            }
            return response;
        }

        private int GetHandshake()
        {
 
            byte[] resp = ReadBytes(3,1000);
            //Console.WriteLine(resp[0].ToString("X2") + "," + resp[1].ToString("X2") + "," + resp[2].ToString("X2"));
            if (CompareBytes(ACK, resp, 0, 3))
            {
                return HS_ACK;
            }
            if (CompareBytes(GOO, resp, 0, 3))
            {
                return HS_GOO;
            }
            if (CompareBytes(BAD, resp, 0, 3))
            {
                return HS_BAD;
            }
            if (CompareBytes(SYN, resp, 0, 3))
            {
                return HS_SYN;
            }
            if (CompareBytes(S_B, resp, 0, 3))
            {
                return HS_S_B;
            }

            if (CompareBytes(KAC, resp, 0, 3))
            {
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(OGO, resp, 0, 3))
            {
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(DBA, resp, 0, 3))
            {
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(NSY, resp, 0, 3))
            {
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(BS_, resp, 0, 3))
            {
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(CKA, resp, 0, 3))
            {
                GetByte(500);
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(OOG, resp, 0, 3))
            {
                GetByte(500);
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(ADB, resp, 0, 3))
            {
                GetByte(500);
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(YNS, resp, 0, 3))
            {
                GetByte(500);
                GetByte(500);
                return GetHandshake();
            }
            if (CompareBytes(_BS, resp, 0, 3))
            {
                GetByte(500);
                GetByte(500);
                return GetHandshake();
            }
            return -1;

        }

        private byte[] GetResponse()
        {
            byte[] b = new byte[3];
            b[0] = GetByte(500).Byte_Received;
            b[1] = GetByte(500).Byte_Received;
            b[2] = GetByte(500).Byte_Received;
            return b;
        }

        private byte[] ArraySegment(byte[] array, int start, int len)
        {
            byte[] response = new byte[len];
            int j = 0;
            for (int i = start; i < start + len; i++)
            {
                response[j] = array[i];
                j++;
            }
            return response;

        }

        private bool CompareBytes(byte[] a, byte[] b, int start, int len)
        {
            bool x = true;
            for (int i = start; i < (start + len); i++)
            {
                if (a[i] != b[i]) x = false;
            }
            return x;
        }

        private bool Punter_Handshake(int call, int response)
        {
            bool b = false;
            int retry = 0;
            while (retry < 10)
            {
                SendHandshake(call);
                int resp = GetHandshake();
                b = (resp == response);
                if (b)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(500);
                    retry++;
                }
            }
            return b;
        }


        #endregion

        private bool ReallyWaitHandshake(int handshake, int retries)
        {
            int waittries = 0;
            while (WaitHandshake() != handshake)
            {
                waittries++;
                if (waittries == retries) return false;
            }
            return true;
        }

        public bool SendMulti(List<Punter_File> files)
        {
            bool b = false;
            foreach (Punter_File pf in files)
            {
                DataOut.Send(MULTIPUNTER_FILE_PREAMBLE);
                DataOut.Send(pf.Filename);
                DataOut.Send(0x2c);
                DataOut.Send(pf.FileType);
                DataOut.Send(0x0d);
                SendHandshake(HS_GOO);
                Send(pf.FileData, pf.FileType.Equals(0x01));
            }
            //Send finalization
            DataOut.Send(MULTIPUNTER_FILE_PREAMBLE);
            DataOut.Send(MULTIPUNTER_FINALIZE);
            DataOut.Send(0x0d);
            return b;
        }

        public List<Punter_File> RecvMulti()
        {
            List<Punter_File> ReceivedFiles = new List<Punter_File>();
            bool FinalReceived = false;
            while (!FinalReceived)
            {
                //Listen for pre-amble of 0x09
                ReceivedBytes preamble = GetLine(30000);
                if (!preamble.TimedOut)
                {
                    if (preamble.bytes[preamble.bytes.Count - 1] != 0x04)
                    {
                        Punter_File pf = new Punter_File();
                        //Receive File
                        pf.FileData = Receive();
                        if (pf.FileData != null)
                        {
                            //Fish our filename and type out of the preamble
                            int FilenameStart = preamble.bytes.LastIndexOf(0x09)+1;
                            int FilenameLen = preamble.bytes.Count - (FilenameStart+2);
                            pf.Filename = ArraySegment(preamble.bytes.ToArray(), FilenameStart, FilenameLen);
                            pf.FileType = preamble.bytes[preamble.bytes.Count - 1];
                            
                            ReceivedFiles.Add(pf);
                        }
                        else
                        {
                            FinalReceived = true;
                        }
                    }
                    else
                    {
                        FinalReceived = true;
                    }

                }
                else
                {
                    FinalReceived = true; //Timeout on pre-amble, we;'re done.
                }
            }
            return ReceivedFiles;
        }


        public bool Send(byte[] FileBytes, bool isPRG)
        {
            StatusRecipeient.MicroUpdate("Beginning upload...");
            int CurrentSendPointer = 0;
            ClearReceiveBuffer();
            if (!ReallyWaitHandshake(HS_GOO,10)) return false;
            SendHandshake(HS_ACK);
            if (!ReallyWaitHandshake(HS_S_B, 10)) return false;

            PunterBlock InitialPacket = new PunterBlock();
            InitialPacket.NextBlockSize = 0x04;
            InitialPacket.BlockNumber = 0xffff;
            InitialPacket.PacketBody.Add((byte)(isPRG ? 1 : 2));
            InitialPacket.Generate_Checksum();
            //Send Packet
            StatusRecipeient.MicroUpdate("Sending filetype packet");
            if (SendPacket(InitialPacket))
            {
                StatusRecipeient.MicroUpdate("Doing punter dance");
                SendHandshake(HS_SYN);
                if (!ReallyWaitHandshake(HS_SYN, 10)) return false;
                SendHandshake(HS_S_B);
                //SendHandshake(HS_S_B);
                if (!ReallyWaitHandshake(HS_GOO, 10)) return false;
                SendHandshake(HS_ACK);
                if (!ReallyWaitHandshake(HS_S_B, 10)) return false;
                StatusRecipeient.MicroUpdate("Sending dummy packet...");
                PunterBlock DummyPacket = new PunterBlock();
                DummyPacket.NextBlockSize = (byte)((FileBytes.Length > 248) ? 255 : FileBytes.Length + 7);
                DummyPacket.BlockNumber = 0x0000;
                DummyPacket.Generate_Checksum();
                if (SendPacket(DummyPacket))
                {
                    
                    //Send File Packets
                    int PacketsInFile = (FileBytes.Length / 248) + (((FileBytes.Length % 248) > 0) ? 1 : 0);
                    byte LastPacketLen = (byte)(FileBytes.Length % 248);
                    int PayloadLen = 248;
                    ushort PacketNumber  = 0x0001;
                    int PayloadSrcStart = 0x0000;
                    byte NextBlockSize = 255;
                    byte[] sendbytes;
                    for (int i = 0; i < PacketsInFile; i++) //All but the final packet
                    {
                        if (i < PacketsInFile - 2)
                        {
                            StatusRecipeient.MicroUpdate("Sending block " + i.ToString() + " of " + PacketsInFile.ToString());
                            NextBlockSize = 255;
                            PayloadLen = 248;
                        }
                        else
                        {
                            if (i == PacketsInFile - 2)
                            {
                                StatusRecipeient.MicroUpdate("Sending next to last block");
                                NextBlockSize = (byte)(LastPacketLen + 7);
                            }
                            else
                            {
                                StatusRecipeient.MicroUpdate("Sending last block");
                                //Last packet
                                NextBlockSize = 0;
                                PayloadLen = LastPacketLen;
                                PacketNumber = (ushort)(0xff00 | PacketNumber);
                            }

                        }
                        sendbytes = ArraySegment(FileBytes, PayloadSrcStart, PayloadLen);
                        PunterBlock FilePacket = new PunterBlock(sendbytes, NextBlockSize, PacketNumber);
                        int SendTries = 0;
                        bool SendSuccess = false;
                        while (!SendSuccess)
                        {
                            if (SendPacket(FilePacket))
                            {
                                StatusRecipeient.Update(PayloadSrcStart);
                                PayloadSrcStart += 248;
                                PacketNumber++;
                                SendSuccess = true;
                            }
                            else
                            {
                                SendTries++;
                                if (SendTries > 12) return false;
                            }
                        }
                    }
                    StatusRecipeient.MicroUpdate("Finalizing...");

                    //Final handshaking
                    SendHandshake(HS_SYN);
                    if (!ReallyWaitHandshake(HS_SYN, 10)) return false;
                    SendHandshake(HS_S_B);
                    return true;
                }
            }

            return false;
        }

        private int ReceiveBytes(int bytes_to_receive)
        {
            int result = 0; ;
            int bytesreceived = 0;
            bool keepgoing = true;
            int badblocks = 0;
            while ((bytesreceived < bytes_to_receive) && keepgoing)
            {
                PunterTimedByte ptb = GetByte(2000);
                if (!ptb.Timeout)
                {
                    Receive_Buffer.Add(ptb.Byte_Received);
                    bytesreceived++;
                }
                else
                {
                    return -1;
                }

            }
            return result;
        }

        private PunterBlock Receive_Block_With_Retry(int blocksize, int retries)
        {
            int retry = 0;
            bool received = false;
            PunterBlock pb = new PunterBlock() { ReceiveError = true };
            while (retry < retries && !received)
            {
                pb = Receive_Block(blocksize);
                if (pb.ReceiveError)
                {
                    SendHandshake(HS_BAD);
                    retry++;
                }
                else
                {
                    received = true;
                }
            }
            return pb;
        }

        private PunterBlock Receive_Block(int blocksize) //Returns nextblocknum/nextblocksize
        {
            Receive_Buffer.Clear();
            int bytesreceived = 0;
            //Request blocksend
            SendHandshake(HS_S_B);
            PunterBlock pbi = new PunterBlock();
            pbi.ReceiveBytes(ReadBytes(blocksize,500));
            if (pbi.Verify_Checksum()){
                    pbi.ReceiveError = !Punter_Handshake(HS_GOO,HS_ACK);
            }
                else
            {
                pbi.ReceiveError = true;
            }
            return pbi;

        }


        //private PunterPacket Start_Receive()
        //{
        //    PunterPacket pbi = null;
        //    if (Punter_Handshake(HS_GOO, HS_ACK))
        //    {
        //        //Get File Type Block
        //        pbi = Receive_Block(FIRST_PACKET_SIZE);
        //        if (pbi.Continue_Download)
        //        {
        //            Receive_Buffer.Clear();
        //            //Goo the first block.  

        //            //i = pbi.NextBlockSize;
        //            if (Punter_Handshake(HS_S_B, HS_SYN))
        //            {
        //                if (Punter_Handshake(HS_SYN, HS_S_B))
        //                {
        //                    if (Punter_Handshake(HS_GOO, HS_S_B))
        //                    {
        //                        if (Punter_Handshake(HS_GOO, HS_ACK))
        //                        {

        //                            pbi = Receive_Block(HEADER_SIZE);
        //                        }
        //                        //                                    pbi = Receive_Block(pbi.NextBlockSize);

        //                        //Get start block
        //                        //if (Simple_Receive_Block(HEADER_SIZE))
        //                        //{
        //                        //    i = Receive_Buffer[NEXT_BLOCK_SIZE];
        //                        //    Receive_Buffer.Clear();
        //                        //}
        //                    }
        //                }

        //            }
        //        }

        //    }

        //    return pbi;
        //}

        public bool PunterDance(bool Receiver, bool skipfinal)
        {
            bool result = false;
            if (Receiver)
            {
                if (Punter_Handshake(HS_S_B, HS_SYN))
                {
                    if (Punter_Handshake(HS_SYN, HS_S_B))
                    {
                        if (skipfinal) return true;
                        if (Punter_Handshake(HS_GOO, HS_ACK))
                        {
                            
                            result = true;
                        }
                    }
                }
            }
            else
            {
                if (Punter_Handshake(HS_SYN, HS_SYN))
                {
                    if (Punter_Handshake(HS_S_B, HS_GOO))
                    {
                        if (Punter_Handshake(HS_ACK, HS_S_B))
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public byte[] Receive()
        {
            StatusRecipeient.MicroUpdate("Beginning download...");

            List<byte> recvbytes = new List<byte>();

            //Pre-receive.  Send goo until it's acked.
            bool started = false;
            while (!started)
            {
                StatusRecipeient.MicroUpdate("Awaiting intial ACK...");
               //if (Punter_Handshae
                SendHandshake(HS_GOO);
                if (GetHandshake() == HS_ACK) started = true;
               // if (GetHandshake() == HS_ACK) started = true;
                Thread.Sleep(500);
            }
            
            PunterBlock filetypepacket = Receive_Block_With_Retry(8,20);

            if (!filetypepacket.ReceiveError)
            {
                StatusRecipeient.MicroUpdate("Received filetype...");
                if (PunterDance(true,false))
                {
                    //Get that dummy packet
                    PunterBlock dummypacket = Receive_Block_With_Retry(7,20);
                    if (!dummypacket.ReceiveError)
                    {
                        StatusRecipeient.MicroUpdate("Received initial block");

                        int nextblocksize = dummypacket.NextBlockSize;
                        bool filedone = false;
                        while (!filedone)
                        {
                            PunterBlock packet = Receive_Block_With_Retry(nextblocksize,20);
                            if (packet.ReceiveError)
                            {
                                    StatusRecipeient.Done(false);
                                    return null;
                            }
                            else
                            {
                                recvbytes.AddRange(packet.PacketBody);
                                StatusRecipeient.MicroUpdate("Received "+recvbytes.Count.ToString()+" bytes");
                                if ((packet.BlockNumber & 0xff00) == 0xff00)
                                {
                                    filedone = true;
                                }
                                else
                                {
                                    nextblocksize = packet.NextBlockSize;
                                }
                            }
                        }
                        //Do End-off
                        StatusRecipeient.MicroUpdate("Finalizing download...");
                        PunterDance(true,true);
                        StatusRecipeient.Done(true);
                            return recvbytes.ToArray();
                        
                    }
                    else
                    {
                        StatusRecipeient.Done(false);
                        return null;
                    }
                }
                else
                {
                    //Didn't do punter dance.
                    StatusRecipeient.Done(false);
                    return null;

                }
            }
            else
            {
                //Failed to negotiate initial packet
                StatusRecipeient.Done(false);
                return null;

            }
            
            //Should never get here.
            StatusRecipeient.Done(false);
            return null;

        }

        private bool SendPacket(PunterBlock packet)
        {
            byte[] sendbytes = packet.ToBytes();
            
            DataOut.Send(sendbytes, sendbytes.Length,1);

            int recvshake = WaitHandshake();
            if (WaitHandshake() == HS_GOO)
            {
                SendHandshake(HS_ACK);
                return ReallyWaitHandshake(HS_S_B, 12);
            }
            else
            {
                if (recvshake == HS_BAD)
                {
                    SendHandshake(HS_ACK);
                    ReallyWaitHandshake(HS_S_B, 12);
                }
            }
            return false;
        }





        public int SendFile(byte[] data)
        {
            return (Send(data, true) ? 1 : 0);
        }

        public byte[] ReceiveFile()
        {
            return Receive();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void OnDisconnect()
        {
            throw new NotImplementedException();
        }
    }
}
