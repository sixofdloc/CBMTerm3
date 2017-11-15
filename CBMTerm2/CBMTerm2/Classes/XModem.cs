using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CBMTerm2.Interfaces;
using System.Collections;
using System.Threading;

namespace CBMTerm2.Classes
{
    public class XModem : IFileTransferProtocol
    {

        //Loosely Based on the VB XModem unit by Andreas Wögerbauer // 
        public  IDataSender DataOut;
        public IStatusRecipient StatusRecipeient;

        //
        public const int SEND_SUCCESS = 0;
        public const int SEND_FAILED = -1;
        public const int SEND_FAILED_ON_RETRIES = -2;
        public const int SEND_FAILED_RECEIVER_CANCELLED = -3;



        private const byte SOH = 0x01; //header for 128byte-packets 
        private const byte STX = 0x02;  //header for 1024byte-packets 
        private const byte EOT = 0x04;   //end of transmission 
        private const byte ACK = 0x06;   //acknowledge 
        private const byte NAK = 0x15; //negativ acknowledge 
        private const byte CAN = 0x18; //Cancel transfer 
        private const byte CTRLZ = 0x1a; //padding char to fill data blocks < buffer size 
        private const byte C_pad = 0x43; //start of a CRC request 
        private const byte MAXRETRANS = 25;

        private String lockstring = "Lock";


        //Send File Shite
        private byte[] send_buff = new byte[1029]; //1024 for XModem 1k + 3 head chars + 2 crc + nul 
        private int send_bufsz = 128;
        private byte send_packetno = 0x01;
        private int send_retry;
        private byte send_byteread;

        private bool useCRC = false;
        private int crc = 1;
        private int len = 0;

        //Receive File Shite
        private MemoryStream receive_buffer = new MemoryStream();
        private byte[] receive_readbuffer = new byte[1029]; //1024 for XModem 1k + 3 head chars + 2 crc + nul 
        private double received_kb = 0;
        private byte receive_byteread;
        private byte receive_packetno = 0x01;
        private int receive_retry = 0;
        private int receive_bufsz = 128;


        public List<byte> ReceiveBuffer = new List<byte>(); //Buffer from our socket

        public XModem(){}  //Empty constructor

        public void Initialize(IDataSender dataout, IStatusRecipient statusrecipient){
            DataOut = dataout;
            StatusRecipeient = statusrecipient;
        }

        #region Socket Interface

        public void OnDataReceived(byte[] data, int count)
        {
            lock (ReceiveBuffer)
            {
                for (int i = 0; i < count; i++)
                {
                    ReceiveBuffer.Add(data[i]);
                }
            }
        }

        private void ClearReceiveBuffer()
        {
            lock (ReceiveBuffer)
            {
                ReceiveBuffer.Clear();
            }
        }

        private byte GetByte()
        {
            byte b = 0x00;
            try
            {
                DateTime Timeout = DateTime.Now;
                bool bytereceived = false;
                while ((Timeout.AddSeconds(10) > DateTime.Now) && !bytereceived)
                {
                    if (ReceiveBuffer.Count > 0)
                    {
                        lock (ReceiveBuffer)
                        {
                            b = ReceiveBuffer[0];
                            ReceiveBuffer.RemoveAt(0);
                            bytereceived = true;
                        }
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                b = 0x00;
            }
            return b;
        }

        private void writeBuffer(byte[] xbuff, int size)
        {
            DataOut.Send(xbuff, size);
        }

        private void writeByte(byte b)
        {
            DataOut.Send(b);
        }

        #endregion

        #region Receive_File

        public byte[] ReceiveFile()
        {
            receive_retry = 0;
            //Tell the sender what error checking to use
            StatusRecipeient.SetStatus("Receiving File", 0, 0);
            if (useCRC)
            {
                writeByte(C_pad); 
            }
            else
            {
                writeByte(NAK);
            }
            receive_packetno = 1;
            while (receive_retry < 16)
            {
                receive_byteread = GetByte();
                switch (receive_byteread)
                {
                    case SOH:
                        //Normal Packet, 128 bytes
                        receive_bufsz = 128;
                        ReceivePacket();
                        break;
                    case STX:
                        //Normal Packet, 1024 bytes
                        receive_bufsz = 1024;
                        ReceivePacket();
                        break;
                    case EOT:
                        //End of Download
                        writeByte(ACK);
                        return receive_buffer.ToArray();
                        break;
                    case CAN:
                        //Cancel Download
                        byte by = GetByte();
                        if (by == CAN)
                        {
                            writeByte(ACK);
                            return null;
                        }
                        break;
                    default:
                        //Bad Packet
                        receive_retry ++;
                        ClearReceiveBuffer();
                        writeByte(NAK);
                        break;
                }
            }
            //Download Timed Out
            writeByte(CAN);
            writeByte(CAN);
            writeByte(CAN);
            ClearReceiveBuffer();
            return null;
        }

        private void ReceivePacket()
        {
            receive_readbuffer[0] = receive_byteread;
            for (int i = 0; i < (receive_bufsz + (useCRC ? 1 : 0) + 3); i++)
            {
                receive_readbuffer[i + 1] = GetByte();
            }
            bool packetno_inverted = ((receive_readbuffer[1] == (byte)(~receive_readbuffer[2])));
            bool packetno_correct = (receive_readbuffer[1] == receive_packetno);
            bool checksum_or_crc_correct = (CheckBytes(useCRC, receive_readbuffer, 3, receive_bufsz));
            if (packetno_correct && packetno_inverted && checksum_or_crc_correct)
            {
                receive_buffer.Write(receive_readbuffer, 3, receive_bufsz);
                writeByte(ACK);
                receive_retry = 0;

                received_kb += (receive_bufsz / 1024);
                //TODO: Update download display
                receive_packetno++;
            }
            else
            {
                ClearReceiveBuffer();
                writeByte(NAK);
               
                receive_retry++;

            }


        }

        #endregion

        #region Send_File
        public int SendFile(byte[] src)
        {
            ClearReceiveBuffer();
            int srcsz = src.Length;
            byte c;
            for (send_retry = 0; send_retry < 15; send_retry++)
            {
                //Try to determine receiver's error checking method
                c = GetByte();
                if (c >= 0)
                {
                    switch (c)
                    {
                        case C_pad:
                            crc = 1;
                            return startTrans(src, srcsz);
                            break;
                        case NAK:
                            crc = 0;
                            return startTrans(src, srcsz);
                            break;
                        case CAN:
                            c = GetByte();
                            if (c == CAN)
                            {
                                writeByte(ACK);
                                ClearReceiveBuffer();
                                return SEND_FAILED_RECEIVER_CANCELLED;
                            }
                            break;
                    }
                }
            }
            //Cancel Transmission
            writeByte(CAN);
            writeByte(CAN);
            writeByte(CAN);
            ClearReceiveBuffer();
            return SEND_FAILED_ON_RETRIES;
        }


        private void Generate_Send_Packet(byte[] src,int this_packet_size)
        {
            memset(ref send_buff, 3, 0, send_bufsz);
            if (this_packet_size == 0)
            {
                send_buff[3] = CTRLZ;
            }
            else
            {
                memcpy(ref send_buff, 3, src, len, this_packet_size);
                if (this_packet_size < send_bufsz)
                {
                    send_buff[3 + this_packet_size] = CTRLZ;
                }
            }
            if (crc == 1)
            {
                //Generate CRC
                ushort ccrc = CRC16.SimpleCRC(send_buff, 3, send_bufsz);//CRC16.CRC16_ccitt(xbuff, 3, bufsz);
                send_buff[send_bufsz + 3] = (byte)((ccrc >> 8) & 0xff);
                send_buff[send_bufsz + 4] = (byte)(ccrc & 0xff);
            }
            else
            {
                //Generate Checksum
                byte ccks = 0;
                for (int i = 3; i < 131; i++)
                {
                    ccks += send_buff[i];
                }
                send_buff[send_bufsz + 3] = ccks;
            }

        }


        private int startTrans(byte[] src, int srcsz)
        {
            while (true)
            {
                send_buff[0] = SOH;
                send_buff[1] = send_packetno;
                send_buff[2] = (byte)(~send_packetno);
                int this_packet_size = srcsz - len;
                if (this_packet_size > send_bufsz)
                {
                    this_packet_size = send_bufsz;
                }
                if (this_packet_size >= 0)
                {
                    Generate_Send_Packet(src,this_packet_size);
                    send_retry = 0;
                    bool success = false;
                    while ((send_retry < MAXRETRANS) && (!success))
                    {
                        send_retry += 1;
                        writeBuffer(send_buff, send_bufsz + 4 + crc);
                        ClearReceiveBuffer();
                        send_byteread = GetByte();
                        if (send_byteread >= 0)
                        {
                            switch (send_byteread)
                            {
                                case ACK:
                                    //Receiver says all good
                                    StatusRecipeient.SetStatus("Uploading", send_packetno, send_bufsz);
                                    send_packetno++;
                                    len += send_bufsz;
                                    success = true;
                                    break;
                                case CAN:
                                    //Receiver cancelled?
                                    send_byteread = GetByte();
                                    if (send_byteread == CAN)
                                    {
                                        writeByte(ACK);
                                        ClearReceiveBuffer();
                                        return SEND_FAILED_RECEIVER_CANCELLED;
                                    }
                                    break;
                                case NAK:
                                    //We're going to repeat this packet.
                                    break;
                                default:  
                                    //Same as NAK
                                    break;
                            }
                        }
                    }

                    if (!success)
                    {
                        //Send failed
                        writeByte(CAN);
                        writeByte(CAN);
                        writeByte(CAN);
                        ClearReceiveBuffer();
                        return SEND_FAILED_ON_RETRIES;
                    }

                }
                else
                {
                    return FinishSend();
                }
            }
        }

        private int FinishSend()
        {
            for (send_retry = 0; send_retry < 9; send_retry++)
            {
                writeByte(EOT);
                Thread.Sleep(500);
                send_byteread = GetByte();
                if (send_byteread == ACK)
                {
                    break;
                }
            }
            ClearReceiveBuffer();
            if (send_byteread == ACK)
            {
                return len;
            }
            else
            {
                return SEND_FAILED_ON_RETRIES;
            }
        }

        #endregion


        private bool CheckBytes(bool isCRC, byte[] buf, int index, int sz)
        {
            if (isCRC)
            {
                ushort crc = CRC16.CRC16_ccitt(buf, index, sz);
                ushort tcrc = (ushort)(((buf[sz + index] << 8) + buf[sz + index + 1]));
                if (crc == tcrc)
                {
                    return true;
                }
            }
            else
            {
                byte cks = 0;
                for (int i = 3; i < 131; i++)
                {
                    cks += buf[i];
                }
                if (cks == buf[sz + index])
                {
                    return true;
                }
            }
            return false;
        }


        //'sets the first num bytes pointed by buffer to the value specified by c parameter. 
        private void memset(ref byte[] xbuff, int index, byte c, int num)
        {
            for (int i = 0; i < num; i++)
            {
                xbuff[i + index] = c;
            }
        }

        //'copies num bytes from src buffer to memory location pointed by dest. 
        private void memcpy(ref byte[] dest, int d_i, byte[] src, int d_s, int num)
        {
            for (int i = 0; i < num; i++)
            {
                dest[i + d_i] = src[i + d_s];
            }
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
