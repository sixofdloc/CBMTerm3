using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBMTerm2.Forms;
using System.IO;
using System.Threading;
using FileTransferProtocols.Interfaces;
using FileTransferProtocols.Protocols;
using FileTransferProtocols;

namespace CBMTerm2.Classes
{
    class File_Transfers //: IStatusRecipient
    {
        public XModem xmodem {get; set;}
        //public Punter punter { get; set; }
        public Punter punter3 { get; set; }
        public FileTransferDialog filetransferdialog { get; set; }

        private IDataSender DataOut { get; set; }
        private IStatusRecipient StatusRecipient { get; set; }
        private IFileTransferProtocol CurrentProtocol { get; set; }

        public bool FileTransferActive = false;

        Thread CurrentTransferThread;

        public File_Transfers(IDataSender dataout)
        {
            DataOut = dataout;
            //StatusRecipient = this;
            xmodem = new XModem();
            //punter = new Punter();
            punter3 = new Punter();
        }

        public void OnDataReceived(byte[] data, int count)
        {
            if (CurrentProtocol != null)
            {
                CurrentProtocol.OnDataReceived(data, count);
            }
        }

        public void XModem_Send(byte[] data)
        {
            CurrentTransferThread = new Thread(() => Start_XModem_Send(data));
            CurrentTransferThread.Start();
        }

        public void XModem_Receive()
        {
            CurrentTransferThread = new Thread(() => Start_XModem_Receive());
            CurrentTransferThread.Start();
        }

        public void Punter_Receive(string filename)
        {
            CurrentTransferThread = new Thread(() => Start_Punter_Receive(filename));
            CurrentTransferThread.Start();
        }

        public void Punter_ReceiveMulti(string filepath)
        {
            CurrentTransferThread = new Thread(() => Start_Punter_MultiReceive(filepath));
            CurrentTransferThread.Start();
        }

        public void Punter_Send(string filename, bool isprg, byte[] data)
        {
            CurrentTransferThread = new Thread(() => Start_Punter_Send(filename, isprg, data));
            CurrentTransferThread.Start();
        }

        public void Punter_SendMulti(List<Punter_File> Files)
        {
            CurrentTransferThread = new Thread(() => Start_Punter_MultiSend(Files));
            CurrentTransferThread.Start();
        }



        private void Start_XModem_Send(byte[] data)
        {
            //CurrentProtocol = xmodem;
            //filetransferdialog.Show();
            //xmodem.Initialize(DataOut, this);

            //FileTransferActive = true;
            //xmodem.SendFile(data);
        }

        private void Start_XModem_Receive()
        {
            //CurrentProtocol = xmodem;
            //filetransferdialog.Show();
            //xmodem.Initialize(DataOut, this);

            //FileTransferActive = true;
            //byte[] rec = xmodem.ReceiveFile();
            //if (rec != null)
            //{
            //    File.WriteAllBytes("xmtest.bin", rec);
            //}

        }

        private void Start_Punter_MultiReceive(string filepath)
        {
            CurrentProtocol = punter3;
            filetransferdialog = new FileTransferDialog();
            filetransferdialog.Initialize(0,"Multiple Files", false, "Punter Receive", this.Abort);
            punter3.Initialize(DataOut, filetransferdialog);
            FileTransferActive = true;
            punter3.Initialize(DataOut, filetransferdialog);
            FileTransferActive = true;
            List<Punter_File> pflist = punter3.RecvMulti();
            //byte[] rec = punter3.ReceiveFile();
            //if (rec != null)
            //{
            //    File.WriteAllBytes(filename, rec);
            //}
            //bool b = (rec != null);
            filetransferdialog.Done(true);
            CurrentProtocol = null;
        }


        private void Start_Punter_Receive(string filename)
        {
            CurrentProtocol = punter3;
            filetransferdialog = new FileTransferDialog();
            filetransferdialog.Initialize(0, Path.GetFileName(filename), false, "Punter Receive", this.Abort);
            punter3.Initialize(DataOut, filetransferdialog);
            FileTransferActive = true;
            punter3.Initialize(DataOut, filetransferdialog);
            FileTransferActive = true;
            byte[] rec = punter3.ReceiveFile();
            if (rec != null)
            {
                File.WriteAllBytes(filename, rec);
            }
            bool b = (rec != null);
            filetransferdialog.Done(b);
            CurrentProtocol = null;
        }

        private void Start_Punter_Send(string filename, bool isPrg, byte[] data)
        {
            CurrentProtocol = punter3;
            filetransferdialog = new FileTransferDialog();
            filetransferdialog.Initialize(data.Length, filename, true, "Punter Send", this.Abort);
            punter3.Initialize(DataOut, filetransferdialog);
            FileTransferActive = true;
            bool b = punter3.Send(data, isPrg);
            filetransferdialog.Done(b);
            CurrentProtocol = null;
        }
 
        private void Start_Punter_MultiSend(List<Punter_File> Files)
        {
            CurrentProtocol = punter3;
            filetransferdialog = new FileTransferDialog();
            filetransferdialog.Initialize(Files[0].FileData.Length,Encoding.ASCII.GetString(Files[0].Filename) , true, "Punter Multi-Send", this.Abort);
            punter3.Initialize(DataOut, filetransferdialog);
            FileTransferActive = true;
            bool b = punter3.SendMulti(Files);
            filetransferdialog.Done(b);
            CurrentProtocol = null;
        }

        public void Abort()
        {
            CurrentTransferThread.Abort();
            filetransferdialog.Abort();
        }


    }
}
