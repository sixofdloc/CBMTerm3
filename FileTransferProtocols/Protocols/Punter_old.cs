//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.IO;
//using FileTransferProtocols.Interfaces;

//namespace FileTransferProtocols.Protocols
//{
//    public class Punter : IFileTransferProtocol
//    {
//        //Influenced heavily by MagerValp's punter transfer for CGTerm
//        //Especially the bit about abstracting punter_handshaking to a single bool-returning proc.
//        //And moving block_receive out and having it return the nextblocknum.  Briliant, that. MV Rules.

//        private class PunterBlockInfo
//        {
//            public int NextBlockNum { get; set; }
//            public byte NextBlockSize { get; set; }
//            public bool Continue_Download { get; set; } //To indicate there was an error
//            public int BlockError { get; set; } //If Not Continue_Download
//        }

//        private class PunterTimedByte
//        {
//            public bool Timeout { get; set; }
//            public byte Byte_Received { get; set; }
//        }

//        public IDataSender DataOut;
//        public IStatusRecipient StatusRecipeient;

//        public const int FIRST_PACKET_SIZE = 8;
//        public const int HEADER_SIZE = 7;

//        public const int BLOCK_FAILED_SENDER_CANCELLED = -1;
//        public const int BLOCK_FAILED_BUT_RETRY = -2;
//        public const int BLOCK_FAILED_NO_RETRY = -3;

//        //Byte positions in header
//        public const int HEADER_CSUM_LO = 0; //Low-byte of additive checksum
//        public const int HEADER_CSUM_HI = 1; //High-byte of additive checksum
//        public const int HEADER_CRC_LO = 2; //Lo-byte of CRC
//        public const int HEADER_CRC_HI = 3; //High-byte of CRC
//        public const int NEXT_BLOCK_SIZE = 4; //Size of next block
//        public const int BLOCKNUM_LO = 5; //Lo-byte of block number
//        public const int BLOCKNUM_HI = 6; //High-byte of block number

//        public const int HS_ACK = 0;
//        public const int HS_GOO = 1;
//        public const int HS_BAD = 2;
//        public const int HS_SYN = 3;
//        public const int HS_S_B = 4;

//        private static byte[] ACK = { 0x41, 0x43, 0x4b };
//        private static byte[] GOO = { 0x47, 0x4f, 0x4f };
//        private static byte[] BAD = { 0x42, 0x41, 0x44 };
//        private static byte[] SYN = { 0x53, 0x59, 0x4e };
//        private static byte[] S_B = { 0x53, 0x2f, 0x42 };

//        //Possible ACK Rotations
//        private static byte[] CKA = { 0x43, 0x4b, 0x41 };
//        private static byte[] KAC = { 0x4b, 0x41, 0x43 };

//        //possible GOO Rotations
//        private static byte[] OGO = { 0x4f, 0x47, 0x4f };
//        private static byte[] OOG = { 0x4f, 0x4f, 0x47 };

//        //possible BAD Rotations
//        private static byte[] DBA = { 0x44, 0x42, 0x41 };
//        private static byte[] ADB = { 0x41, 0x44, 0x42 };

//        //Possible SYN Rotations
//        private static byte[] NSY = { 0x43, 0x53, 0x59 };
//        private static byte[] YNS = { 0x59, 0x43, 0x53 };

//        //Possible S_B Rotations
//        private static byte[] BS_ = { 0x42, 0x53, 0x2f };
//        private static byte[] _BS = { 0x2f, 0x42, 0x53 };


//        //Bullshit for sending files
//        private List<byte> Send_Buffer = new List<byte>();


//        public bool triggerflag = false;
//        //Socket's receive buffer
//        public List<byte> SocketBuffer = new List<byte>(); //Buffer from our socket

//        public void Initialize(IDataSender dataout, IStatusRecipient statusrecipient)
//        {
//            DataOut = dataout;
//            StatusRecipeient = statusrecipient;
//        }

//        #region Socket Interface

//        public void OnDataReceived(byte[] data, int count)
//        {
//            lock (SocketBuffer)
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    if (triggerflag)
//                    {
//                        Console.WriteLine("Ack");
//                    }
//                    SocketBuffer.Add(data[i]);
//                }
//            }
//        }

//        private void ClearReceiveBuffer()
//        {
//            lock (SocketBuffer)
//            {
//                SocketBuffer.Clear();
//            }
//        }

//        //private byte[] GetBlock(int count)
//        //{
//        //    byte[] b = new byte[count];
//        //    for (int i = 0; i < count; i++) b[i] = GetByte(500);
//        //    return b;
//        //}

//        private PunterTimedByte GetByte(int timeout_in_ms)
//        {
//            PunterTimedByte pb = new PunterTimedByte();
//            pb.Timeout = true;
//            pb.Byte_Received = 0x00;
//            try
//            {
//                DateTime Timeout = DateTime.Now;
//                bool bytereceived = false;
//                while ((Timeout.AddMilliseconds(timeout_in_ms) > DateTime.Now) && !bytereceived)
//                {
//                    if (SocketBuffer.Count > 0)
//                    {
//                        lock (SocketBuffer)
//                        {
//                            pb.Byte_Received = SocketBuffer[0];
//                            pb.Timeout = false;
//                            SocketBuffer.RemoveAt(0);
//                            bytereceived = true;
//                        }
//                    }
//                    if (!bytereceived)Thread.Sleep(5);
//                }
//            }
//            catch (Exception e)
//            {
//                pb.Timeout = true;
//            }
//            return pb;

//        }

//        //private byte GetByte()
//        //{
//        //    byte b = 0x00;
//        //    try
//        //    {
//        //        DateTime Timeout = DateTime.Now;
//        //        bool bytereceived = false;
//        //        while ((Timeout.AddMilliseconds(500) > DateTime.Now) && !bytereceived)
//        //        {
//        //            if (SocketBuffer.Count > 0)
//        //            {
//        //                lock (SocketBuffer)
//        //                {
//        //                    b = SocketBuffer[0];
//        //                    SocketBuffer.RemoveAt(0);
//        //                    bytereceived = true;
//        //                }
//        //            }
//        //            Thread.Sleep(10);
//        //        }
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        b = 0x00;
//        //    }
//        //    return b;
//        //}

//        private void writeBuffer(byte[] xbuff, int size)
//        {
//            DataOut.Send(xbuff, size);
//        }

//        private void writeByte(byte b)
//        {
//            DataOut.Send(b);
//        }

//        #endregion

//        #region Handshaking Routines



//        private byte[] GetResponse()
//        {
//            byte[] b = new byte[3];
//            b[0] = GetByte(500).Byte_Received;
//            b[1] = GetByte(500).Byte_Received;
//            b[2] = GetByte(500).Byte_Received;
//            return b;
//        }

//        private bool CompareBytes(byte[] a, byte[] b, int start, int len)
//        {
//            bool x = true;
//            for (int i = start; i < (start + len); i++)
//            {
//                if (a[i] != b[i]) x = false;
//            }
//            return x;
//        }

//        private bool Punter_Handshake(int call, int response)
//        {
//            bool b = false;
//            int retry = 0;
//            while (retry < 10)
//            {
//                switch (call)
//                {
//                    case HS_ACK:
//                        writeBuffer(ACK, 3);
//                        break;
//                    case HS_GOO:
//                        writeBuffer(GOO, 3);
//                        break;
//                    case HS_BAD:
//                        writeBuffer(BAD, 3);
//                        break;
//                    case HS_SYN:
//                        writeBuffer(SYN, 3);
//                        break;
//                    case HS_S_B:
//                        writeBuffer(S_B, 3);
//                        break;
//                }
//                byte[] resp = GetResponse();
//                switch (response)
//                {
//                    case HS_ACK:
//                        if (CompareBytes(ACK, resp, 0, 3))
//                        {
//                            b = true;
//                        }
//                        else
//                        {
//                            if (CompareBytes(KAC, resp, 0, 3))
//                            {
//                                GetByte(500);
//                            }
//                            else
//                            {
//                                if (CompareBytes(CKA, resp, 0, 3))
//                                {
//                                    GetByte(500);
//                                    GetByte(500);
//                                }
//                            }
//                        }
//                        break;
//                    case HS_GOO:
//                        if (CompareBytes(GOO, resp, 0, 3))
//                        {
//                            b = true;
//                        }
//                        else
//                        {
//                            if (CompareBytes(OGO, resp, 0, 3))
//                            {
//                                GetByte(500);
//                            }
//                            else
//                            {
//                                if (CompareBytes(OOG, resp, 0, 3))
//                                {
//                                    GetByte(500);
//                                    GetByte(500);
//                                }
//                            }
//                        }
//                        break;
//                    case HS_BAD:
//                        if (CompareBytes(BAD, resp, 0, 3))
//                        {
//                            b = true;
//                        }
//                        else
//                        {
//                            if (CompareBytes(DBA, resp, 0, 3))
//                            {
//                                GetByte(500);
//                            }
//                            else
//                            {
//                                if (CompareBytes(ADB, resp, 0, 3))
//                                {
//                                    GetByte(500);
//                                    GetByte(500);
//                                }
//                            }
//                        }

//                        break;
//                    case HS_SYN:
//                        if (CompareBytes(SYN, resp, 0, 3))
//                        {
//                            b = true;
//                        }
//                        else
//                        {
//                            if (CompareBytes(NSY, resp, 0, 3))
//                            {
//                                GetByte(500);
//                            }
//                            else
//                            {
//                                if (CompareBytes(YNS, resp, 0, 3))
//                                {
//                                    GetByte(500);
//                                    GetByte(500);
//                                }
//                            }
//                        }

//                        break;
//                    case HS_S_B:
//                        if (CompareBytes(S_B, resp, 0, 3))
//                        {
//                            b = true;
//                        }
//                        else
//                        {
//                            if (CompareBytes(BS_, resp, 0, 3))
//                            {
//                                GetByte(500);
//                            }
//                            else
//                            {
//                                if (CompareBytes(_BS, resp, 0, 3))
//                                {
//                                    GetByte(500);
//                                    GetByte(500);
//                                }
//                            }
//                        }

//                        break;
//                }
//                if (b)
//                {
//                   break;
//                }
//                else
//                {
//                    Thread.Sleep(500);
//                    retry++;
//                }
//            }
//            return b;
//        }


//        #endregion

//        private void Build_Out_Buffer(List<byte> blockdata)
//        {

//        }


//        //private bool Simple_Receive_Block(byte blocksize)
//        //{
//        //    Receive_Buffer.Clear();
//        //    for (byte i = 0; i < (blocksize + HEADER_SIZE); i++)
//        //    {
//        //        Receive_Buffer.Add(GetByte());
//        //    }

//        //    //Validate Checksum

//        //    return true;
//        //}



//        public int SendFile(byte[] data)
//        {
//            throw new NotImplementedException();
//        }

//        private PunterBlockInfo Start_Receive()
//        {
//            PunterBlockInfo pbi = null;
//            if (Punter_Handshake(HS_GOO, HS_ACK))
//            {
//                //Get File Type Block
//                pbi = Receive_Block(FIRST_PACKET_SIZE);
//                if (pbi.Continue_Download)
//                {
//                    Receive_Buffer.Clear();
//                    //Goo the first block.  

//                    //i = pbi.NextBlockSize;
//                    if (Punter_Handshake(HS_S_B, HS_SYN))
//                    {
//                        if (Punter_Handshake(HS_SYN, HS_S_B))
//                        {
//                            if (Punter_Handshake(HS_GOO, HS_S_B))
//                            {
//                                    if (Punter_Handshake(HS_GOO, HS_ACK))
//                                    {

//                                        pbi = Receive_Block(HEADER_SIZE);
//                                    }
//                                //                                    pbi = Receive_Block(pbi.NextBlockSize);

//                                //Get start block
//                                //if (Simple_Receive_Block(HEADER_SIZE))
//                                //{
//                                //    i = Receive_Buffer[NEXT_BLOCK_SIZE];
//                                //    Receive_Buffer.Clear();
//                                //}
//                            }
//                        }

//                    }
//                }

//            }
 
//            return pbi;
//        }

//        public byte[] ReceiveFile()
//        {
//            MemoryStream InFile = new MemoryStream();
//            byte[] ReturnValue = null;
//            PunterBlockInfo pbi = Start_Receive();
//            if (pbi.NextBlockSize > 0)
//            {
//                bool done = false;
//                byte NextBlockSize = pbi.NextBlockSize;
//                int NextBlockNum = pbi.NextBlockNum;

//                while (!done)
//                {
//                    if ((NextBlockNum < 0xff00) && (NextBlockSize >= 7))
//                    {
//                        PunterBlockInfo pi = Receive_Block(NextBlockSize);
//                        if (pi.Continue_Download)
//                        {
//                            InFile.Write(Receive_Buffer.ToArray(), 0, Receive_Buffer.Count);
//                            NextBlockSize = pi.NextBlockSize;
//                            NextBlockNum = pi.NextBlockNum;
//                        }
//                        else
//                        {
//                            ReturnValue = null;
//                            done = true;
//                        }
//                    }
//                    else
//                    {
//                        //Finalize file
//                        if (Punter_Handshake(HS_S_B, HS_SYN))
//                        {
//                            Punter_Handshake(HS_SYN, HS_S_B);
//                        }
//                        ReturnValue =  InFile.ToArray();
//                        done = true;
//                    }
//                }
//            }
//            return ReturnValue;
//        }

//        private bool Verify_Checksum(int packetlen)
//        {
//            ushort cksum = 0;
//            ushort clc = 0;
//            int i = 4;
//            packetlen -= 4;
//            bool b = false;
//            while (packetlen > 0)
//            {
//                cksum += Receive_Buffer[i];
//                clc ^= Receive_Buffer[i];
//                i++;
//                clc = (ushort)((clc << 1) | (clc >> 15));
//                packetlen--;
//            }
//            ushort packetcsum = (ushort)(Receive_Buffer[0] | (Receive_Buffer[1] << 8));
//            ushort packetclc = (ushort)(Receive_Buffer[2] | (Receive_Buffer[3] << 8));
//            if (cksum == packetcsum)
//            {
//                if (clc == packetclc)
//                {
//                    b = true;
//                }
//            }
//            return b;
//        }


//        private PunterBlockInfo Receive_Block(int blocksize) //Returns nextblocknum/nextblocksize
//        {
//            Receive_Buffer.Clear();
//            int bytesreceived = 0;
//            PunterBlockInfo pbi = new PunterBlockInfo();
//            pbi.NextBlockNum = 0;
//            pbi.NextBlockSize = 0;

//            writeBuffer(S_B, 3);
//            triggerflag = true;

//            int nextpacketlen = ReceiveBytes(blocksize);
//            if (nextpacketlen > 0)
//            {
//                if (Verify_Checksum(blocksize))
//                {
//                    if (Punter_Handshake(HS_GOO, HS_ACK))
//                    {
//                        //All was well.
//                        pbi.NextBlockSize = Receive_Buffer[NEXT_BLOCK_SIZE];
//                        pbi.NextBlockNum = (Receive_Buffer[BLOCKNUM_HI] * 256) + Receive_Buffer[BLOCKNUM_LO];
//                        pbi.Continue_Download = true ;

//                    }
//                    else
//                    {
//                        //Handshake Timeout
//                        pbi.Continue_Download = false;
//                    }
//                }
//                else
//                {
//                    //Bad Checksum
//                    if (Punter_Handshake(HS_BAD, HS_ACK))
//                    {
//                        //Retry this block
//                    }
//                    else
//                    {
//                        //Handshake Timeout
//                    }
//                }
//            }
//            //if (punter_checksum(bytecnt))
//            //{
//            //    if (punter_handshake("GOO", "ACK") == 0)
//            //    {
//            //        menu_update_xfer_progress("Handshake timed out", xfer_saved_bytes, 0);
//            //        gfx_vbl();
//            //        //printf("punter_recv_block: goo handshake timeout\n");
//            //        return (-1);
//            //    }
//            //    menu_update_xfer_progress("Downloading...", xfer_saved_bytes, 0);
//            //    gfx_vbl();
//            //    if (len <= 8)
//            //    {
//            //        //printf("punter_recv_block: short block, returning %d\n", xfer_buffer[4]);
//            //        return (xfer_buffer[4]);
//            //    }
//            //    if (xfer_save_data(xfer_buffer + 7, len - 7))
//            //    {
//            //        //printf("punter_recv_block: returning %d\n", xfer_buffer[4]);
//            //        return (xfer_buffer[4]);
//            //    }
//            //    else
//            //    {
//            //        punter_fail("Write error!");
//            //        gfx_vbl();
//            //        return (-1);
//            //    }
//            //}
//            //else
//            //{
//            //    menu_update_xfer_progress("Checksum failed, retrying", xfer_saved_bytes, 0);
//            //    gfx_vbl();
//            //    //printf("punter_recv_block: checksum failed\n");
//            //    if (punter_handshake("BAD", "ACK"))
//            //    {
//            //        if (errorcnt--)
//            //        {
//            //            goto restart;
//            //        }
//            //        else
//            //        {
//            //            menu_update_xfer_progress("Checksum failed", xfer_saved_bytes, 0);
//            //            gfx_vbl();
//            //            return (-1);
//            //        }
//            //    }
//            //    else
//            //    {
//            //        menu_update_xfer_progress("Handshake timed out", xfer_saved_bytes, 0);
//            //        gfx_vbl();
//            //        //printf("punter_recv_block: bad handshake timeout\n");
//            //        return (-1);
//            //    }
//            //}
//            return pbi;

//        }

//        private int ReceiveBytes(int bytes_to_receive)
//        {
//            int result = 0; ;
//            int bytesreceived = 0;
//            bool keepgoing = true;
//            int badblocks = 0;
//            while ((bytesreceived < bytes_to_receive) && keepgoing)
//            {
//                PunterTimedByte ptb = GetByte(500);
//                if (!ptb.Timeout)
//                {
//                    Receive_Buffer.Add(ptb.Byte_Received);
//                    bytesreceived++;
//                    //xfer_buffer[bytecnt++] = c;
//                    //if (bytecnt == 4)
//                    //{
//                    //    if (strncmp("ACK", xfer_buffer, 3) == 0)
//                    //    {
//                    //        menu_update_xfer_progress("Lost sync, retrying...", xfer_saved_bytes, 0);
//                    //        gfx_vbl();
//                    //        if (xfer_buffer[3] == 'A')
//                    //        {
//                    //            goto restart;
//                    //        }
//                    //        else
//                    //        {
//                    //            //printf("punter_recv_block: skipping late ack\n");
//                    //            xfer_buffer[0] = xfer_buffer[3];
//                    //            bytecnt = 1;
//                    //        }
//                    //    }
//                    //}
//                    //if (bytecnt == 8)
//                    //{
//                    //    if (strncmp("ACKACK", xfer_buffer + 2, 6) == 0)
//                    //    {
//                    //        menu_update_xfer_progress("Lost sync, retrying...", xfer_saved_bytes, 0);
//                    //        gfx_vbl();
//                    //        //printf("punter_recv_block: lost sync, restarting block\n");
//                    //        goto restart;
//                    //    }
//                    //    if (strncmp("CKACKA", xfer_buffer + 2, 6) == 0)
//                    //    {
//                    //        menu_update_xfer_progress("Lost sync, retrying...", xfer_saved_bytes, 0);
//                    //        gfx_vbl();
//                    //        //printf("punter_recv_block: lost sync, restarting block\n");
//                    //        goto restart;
//                    //    }
//                    //    if (strncmp("KACKAC", xfer_buffer + 2, 6) == 0)
//                    //    {
//                    //        menu_update_xfer_progress("Lost sync, retrying...", xfer_saved_bytes, 0);
//                    //        gfx_vbl();
//                    //        //printf("punter_recv_block: lost sync, restarting block\n");
//                    //        goto restart;
//                    //    }
//                    //}

//                }
//                else
//                {
//                    //Timed out
//                    if (Receive_Buffer.Count == 3)
//                    {
//                        if (Receive_Buffer.ToArray().MatchesAt(S_B, 0, 3))
//                        {
//                            //Sender cancelled.
//                            result = BLOCK_FAILED_SENDER_CANCELLED;
//                            keepgoing = false;
//                            break;
//                        }
//                        else
//                        {
//                            //Simple Timeout
//                            if (Punter_Handshake(HS_BAD, HS_ACK))
//                            {
//                                badblocks++;
//                                if (badblocks == 10)
//                                {
//                                    result = BLOCK_FAILED_BUT_RETRY;
//                                    keepgoing = false;
//                                    break;
//                                }
//                                else
//                                {
//                                    result = BLOCK_FAILED_NO_RETRY;
//                                    keepgoing = false;
//                                    break;
//                                }
//                            }
//                            else
//                            {
//                                //Total timeout
//                                result = BLOCK_FAILED_NO_RETRY;
//                                keepgoing = false;
//                                break;
                               
//                            }

//                        }
//                    }
//                }
//            }
//            if (result == 0) result = Receive_Buffer[NEXT_BLOCK_SIZE];
//            return result;
//        }

//        public void Cancel()
//        {
//            throw new NotImplementedException();
//        }


//        public void OnDisconnect()
//        {
//            throw new NotImplementedException();
//        }




 
//    }
//}
