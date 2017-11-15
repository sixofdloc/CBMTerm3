using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CBMTerm3.Forms;
using CBMTerm3.Classes;
using System.Threading;
using System.IO;
using FileTransferProtocols.Interfaces;
using FileTransferProtocols;
using CBMTerm3.Properties;

namespace CBMTerm3
{
    public partial class MainForm : Form, Classes.IDataReceiver, IConnectionMonitor, IDataSender
    {

        private System.Timers.Timer StatusTimer { get; set; }
        private TCPConnection Connection { get; set; }
        public Debug debug { get; set; }
        public DateTime ConnectedTime { get; set; }

        public  List<byte> BaudBuffer = new List<byte>();
        public  List<byte> CaptureBuffer = new List<byte>();

        public System.Windows.Forms.Timer BaudTimer;

        private File_Transfers FileTransfers { get; set; }


        #region Form Stuff
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Settings.Default.Background != null)
            {
                terminalControl1.c64screen.Background_Color = Settings.Default.Background;
            }
            else
            {
                terminalControl1.c64screen.Background_Color = 0x00;
            }

            if (Settings.Default.Border != null)
            {
                terminalControl1.c64screen.Border_Color = Settings.Default.Border;
            }
            else
            {
                terminalControl1.c64screen.Border_Color = 0x00;
            }

            if (Settings.Default.Cursor != null)
            {
                terminalControl1.c64screen.Set_Cursor_Color((byte)Settings.Default.Cursor);
            }
            else
            {
                terminalControl1.c64screen.Set_Cursor_Color(0x01);
            }
            if (Settings.Default.Shifted != null)
            {
                terminalControl1.c64screen.CShift = Settings.Default.Shifted;
                shiftedToolStripMenuItem.Checked = Settings.Default.Shifted;
            }
            else
            {
                terminalControl1.c64screen.CShift = false;
                shiftedToolStripMenuItem.Checked = false ;
            }
            if (Settings.Default.LocalEcho != null)
            {
                localEchoToolStripMenuItem.Checked = Settings.Default.LocalEcho;
            }
            else
            {
                localEchoToolStripMenuItem.Checked = false;
            }
            StatusTimer = new System.Timers.Timer(100);
            StatusTimer.Elapsed += new System.Timers.ElapsedEventHandler(StatusTimer_Elapsed);
            StatusTimer.Enabled = true;
            //NoBaudLimitMenuItem.Ra
            //debug = new Debug();
            //debug.Show();

            //Setup Terminal Control Usercontrol
            terminalControl1.Dock = DockStyle.Fill;
            terminalControl1.KeyDown +=new KeyEventHandler(terminalControl1_KeyDown);
            terminalControl1.KeyUp +=new KeyEventHandler(terminalControl1_KeyUp);
            terminalControl1.KeyPress +=new KeyPressEventHandler(terminalControl1_KeyPress);
            
            //Setup baud emulation
            BaudTimer = new System.Windows.Forms.Timer();
            BaudTimer.Tick += new EventHandler(BaudTimer_Tick);
            BaudTimer.Interval = 1000;
            BaudTimer.Enabled = true;

            //Capture Buffer
            CaptureBuffer = new List<byte>();
            BufferLabel.Text = "Buffer: " + CaptureBuffer.Count.ToString() + " bytes";

            FileTransfers = new File_Transfers(this);

        }

        void BaudTimer_Tick(object sender, EventArgs e)
        {
            BaudTimer.Enabled = false;
            try
            {
                if (BaudBuffer.Count > 0)
                {
                    if (NoBaudLimitMenuItem.Checked)
                    {
                        DumpBuffer();
                    }
                    else
                    {
                        BufferChrout();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            BaudTimer.Enabled = true;
        }
        #endregion

        #region Connection Monitoring Timer

        public delegate void SetConnectionHandler(bool connected);

        public void SetConnectionStatus(bool connected)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    object[] parametersArray = new object[] { connected };
                        this.Invoke(new SetConnectionHandler(SetConnectionStatus), parametersArray);
                }
                else
                {
                    if (connected)
                    {
                        ConnectionLabel.Text = "Connected " + (DateTime.Now - ConnectedTime).ToString(@"hh\:mm\:ss");
                        toolStripMenuItem2.Enabled = true;
                    }
                    else
                    {
                        ConnectionLabel.Text = "Disconnected.";
                        toolStripMenuItem2.Enabled = false;
                    }
                    Application.DoEvents();
                }
            }
            catch (ObjectDisposedException e)
            {

            }
            catch (Exception ex)
            {

            }
        }


        void StatusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
                StatusTimer.Enabled = false;
                SetConnectionStatus(IsConnected());
               if (!FormDead) StatusTimer.Enabled = true;
        }
        #endregion

        #region Top Menu
        #region Baud Menu
        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!NoBaudLimitMenuItem.Checked)
            {
                NoBaudLimitMenuItem.Checked = true;
                Baud300MenuItem.Checked = false;
                Baud1200MenuItem.Checked = false;
                Baud2400MenuItem.Checked = false;
                SetBaudTimerInterval();
            }
        }


        private void Baud300MenuItem_Click(object sender, EventArgs e)
        {
            if (!Baud300MenuItem.Checked)
            {
                NoBaudLimitMenuItem.Checked = false;
                Baud300MenuItem.Checked = true;
                Baud1200MenuItem.Checked = false;
                Baud2400MenuItem.Checked = false;
                SetBaudTimerInterval();
            }

        }

        private void Baud1200MenuItem_Click(object sender, EventArgs e)
        {
            if (!Baud1200MenuItem.Checked)
            {
                NoBaudLimitMenuItem.Checked = false;
                Baud300MenuItem.Checked = false;
                Baud1200MenuItem.Checked = true;
                Baud2400MenuItem.Checked = false;
                SetBaudTimerInterval();
            }
        }

        private void Baud2400MenuItem_Click(object sender, EventArgs e)
        {
            if (!Baud2400MenuItem.Checked)
            {
                NoBaudLimitMenuItem.Checked = false;
                Baud300MenuItem.Checked = false;
                Baud1200MenuItem.Checked = false;
                Baud2400MenuItem.Checked = true;
                SetBaudTimerInterval();
            }

        }

        private void SetBaudTimerInterval()
        {
            if (Baud300MenuItem.Checked) BaudTimer.Interval = 1000 / 33;
            if (Baud1200MenuItem.Checked) BaudTimer.Interval = 1000 / 132;
            if (Baud2400MenuItem.Checked) BaudTimer.Interval = 1000 / 264;
            if (NoBaudLimitMenuItem.Checked) BaudTimer.Interval = 100;
        }

        #endregion

        # region Capture Buffer Menu
        private void bufferOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void clearBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaptureBuffer.Clear();
        }
        #endregion

        private void setBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.ColorDialog cd = new Forms.ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                terminalControl1.c64screen.Background_Color = cd.SelectedColor;
                Settings.Default.Background = cd.SelectedColor;
                Settings.Default.Save();
                terminalControl1.Invalidate();
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (IsConnected())
            {
                TCPConnection.Disconnect();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void shiftedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            terminalControl1.c64screen.CShift = !terminalControl1.c64screen.CShift;
            shiftedToolStripMenuItem.Checked = terminalControl1.c64screen.CShift;
            Settings.Default.Shifted = shiftedToolStripMenuItem.Checked;
            Settings.Default.Save();

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            AddressBook ab = new AddressBook();
            ab.LoadAddresses();
            if (ab.ShowDialog() == DialogResult.OK)
            {
                //Connect to the selected system
            }
            ab.Dispose();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //byte[] b = File.ReadAllBytes("jess.seq");
            //for (int i = 0; i < b.Length;i++ )
            //{
            //    Chrout(b[i]);
            //    terminalControl1.Invalidate();
            //    Application.DoEvents();
            //   // Thread.Sleep(250);
            //}

            QuickConnect qc = new QuickConnect();
            if (qc.ShowDialog() == DialogResult.OK)
            {
                TryConnect(qc.textBox1.Text, int.Parse(qc.textBox2.Text));

            }
        }

        private void toolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            Forms.ColorDialog cd = new Forms.ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                terminalControl1.c64screen.Set_Cursor_Color((byte)(cd.SelectedColor));
                Settings.Default.Cursor = cd.SelectedColor;
                Settings.Default.Save();
                terminalControl1.Invalidate();
            }

        }

        private void setBorderColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.ColorDialog cd = new Forms.ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                terminalControl1.c64screen.Border_Color = cd.SelectedColor;
                Settings.Default.Border = cd.SelectedColor;
                Settings.Default.Save();
                terminalControl1.Invalidate();
            }
        }

        private void localEchoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            localEchoToolStripMenuItem.Checked = !localEchoToolStripMenuItem.Checked;
            Settings.Default.LocalEcho = localEchoToolStripMenuItem.Checked;
            terminalControl1.Invalidate();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] b = File.ReadAllBytes(openFileDialog1.FileName);
                BaudBuffer.AddRange(b);
            }
        }

        private void mnuSaveBuffer_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, CaptureBuffer.ToArray<byte>());
            }
        }

        private void mnuSendXModem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] fb = File.ReadAllBytes(openFileDialog1.FileName);
                FileTransfers.XModem_Send(fb);
            }
        }
        private void mnuReceiveXModem_Click(object sender, EventArgs e)
        {
            FileTransfers.XModem_Receive();
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] f = File.ReadAllBytes(openFileDialog1.FileName);
                TCPConnection.Send(f);
            }
        }

        private void mnuReceivePunter_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save downloaded file as";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileTransfers.Punter_Receive(saveFileDialog1.FileName);
            }
        }

        #endregion

        #region IDataReceiver Interface
        public void OnDataReceived(byte[] data, int count)
        {
            if (FileTransfers.FileTransferActive)
            {
                //xmodem.OnDataReceived(data, count);
                FileTransfers.OnDataReceived(data, count);
            }
            else
            {
                if (NoBaudLimitMenuItem.Checked)
                {
                    for (int i = 0; i < count; i++)
                    {
                        terminalControl1.c64screen.Chrout(data[i]);
                        if (bufferOnToolStripMenuItem.Checked)
                        {
                            CaptureBufferAdd(data[i]);
                        }
                    }
                }
                else
                {
                    lock (BaudBuffer)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            BaudBuffer.Add(data[i]);
                        }
                    }
                }
            }
//        DebugMessage(count.ToString() + " bytes received\r\n");

        }
        #endregion

        #region MainForm Dummy Keyroutines
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region Keyboard Emulation
        private void terminalControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.Control)
            {
                terminalControl1.c64screen.CShift = !terminalControl1.c64screen.CShift;
                shiftedToolStripMenuItem.Checked = terminalControl1.c64screen.CShift;
                e.SuppressKeyPress = true;
            }
            if (e.Modifiers.Equals(Keys.None))
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        Chrout(0x0d);
                        e.SuppressKeyPress = true;
                        break;
                }
            }
            if (e.Modifiers.Equals(Keys.Control))
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        Chrout(144);
                        break;
                    case Keys.D2:
                        Chrout(5);
                        break;
                    case Keys.D3:
                        Chrout(28);
                        break;
                    case Keys.D4:
                        Chrout(159);
                        break;
                    case Keys.D5:
                        Chrout(156);
                        break;
                    case Keys.D6:
                        Chrout(30);
                        break;
                    case Keys.D7:
                        Chrout(31);
                        break;
                    case Keys.D8:
                        Chrout(158);
                        break;
                    case Keys.D9:
                        Chrout(18);
                        break;
                    case Keys.D0:
                        Chrout(146);
                        break;
                }
                e.SuppressKeyPress = true;
            }
            if (e.Modifiers.Equals(Keys.Alt))
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        Chrout(129);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D2:
                        Chrout(149);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D3:
                        Chrout(150);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D4:
                        Chrout(151);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D5:
                        Chrout(152);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D6:
                        Chrout(153);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D7:
                        Chrout(154);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D8:
                        Chrout(155);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D9:
                        Chrout(18);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D0:
                        Chrout(146);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.A:
                        Chrout(176);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.B:
                        Chrout(191);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.C:
                        Chrout(188);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D:
                        Chrout(172);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.E:
                        Chrout(177);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.F:
                        Chrout(187);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.G:
                        Chrout(165);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.H:
                        Chrout(180);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.I:
                        Chrout(162);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.J:
                        Chrout(181);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.K:
                        Chrout(161);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.L:
                        Chrout(182);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.M:
                        Chrout(167);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.N:
                        Chrout(170);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.O:
                        Chrout(185);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.P:
                        Chrout(175);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Q:
                        Chrout(171);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.R:
                        Chrout(178);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.S:
                        Chrout(174);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.T:
                        Chrout(163);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.U:
                        Chrout(184);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.V:
                        Chrout(190);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.W:
                        Chrout(179);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.X:
                        Chrout(189);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Y:
                        Chrout(183);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Z:
                        Chrout(173);
                        e.SuppressKeyPress = true;
                        break;    
                }
            }
        }

        private void terminalControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!e.Handled)
            {
                Chrout((byte)(c64Utils.ASC2PET(e.KeyChar)));
            }

        }

        private void terminalControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers.Equals(Keys.Alt)) e.Handled = true;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    Chrout(C64Screen.C64_CURSOR_UP);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    Chrout(C64Screen.C64_CURSOR_DOWN);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Left:
                    Chrout(C64Screen.C64_CURSOR_LEFT);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Right:
                    Chrout(C64Screen.C64_CURSOR_RIGHT);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Home:
                    if (e.Modifiers.Equals(Keys.Shift))
                    {
                        Chrout(0x93);
                    }
                    else
                    {
                        Chrout(0x13);
                    }
                    e.SuppressKeyPress = true;
                    break;
            }

        }
        #endregion

        #region IConnectionMonitor Interface
        public void OnConnect()
        {
            ConnectedTime = DateTime.Now;
            SetConnectionStatus(true);
           // toolStripMenuItem2.Enabled = true;
        }

        delegate void OnDisconnectDelegate();
        public void OnDisconnect()
        {

            SetConnectionStatus(false);
        }


        #endregion


        #region IDataSender Passthrough to TCPConnection
        public void Send(byte[] data)
        {
            TCPConnection.Send(data);
        }

        public void Send(string data)
        {
            TCPConnection.Send(data);
        }

        public void Send(byte data)
        {
            TCPConnection.Send(data);
        }
        public void Send(byte[] data, int count)
        {
            TCPConnection.Send(data, count);
        }

        public void Send(byte[] data, int count,int delay)
        {
            TCPConnection.Send(data, count,delay);
        }
        #endregion



        
        private void CaptureBufferAdd(byte b)
        {
            CaptureBuffer.Add(b);
            BufferLabel.Text = "Buffer: " + CaptureBuffer.Count.ToString() + " bytes";
        }

        private void TerminalOut(byte b)
        {
            terminalControl1.c64screen.Chrout(b);
            if (bufferOnToolStripMenuItem.Checked)
            {
                CaptureBufferAdd(b);// CaptureBuffer.Add(b);
            }
        }

        private void BufferChrout()
        {
            lock (BaudBuffer)
            {
                byte b = BaudBuffer[0];
                TerminalOut(b);
                BaudBuffer.RemoveAt(0);
            }
        }

        private void DumpBuffer()
        {
            lock (BaudBuffer)
            {
                foreach (byte b in BaudBuffer)
                {
                    TerminalOut(b);
                }
            }
        }

        private void TryConnect(string address, int port)
        {
            TCPConnection.Connect(address, port, this, this);
            DateTime StartTrying = DateTime.Now;
            while (!IsConnected())
            {
                if (DateTime.Now.AddMinutes(1) > StartTrying) break;
                Thread.Sleep(100);
            }
        }

        //private void DebugMessage(string message)
        //{
        //    if (Settings.Default.Debug)
        //    {
        //        if (debug.textBox1.InvokeRequired)
        //        {
        //            debug.textBox1.Invoke(new MethodInvoker(
        //                delegate
        //                {
        //                    this.DebugMessage(message);
        //                }
        //                ));
        //            return;
        //        }
        //        debug.textBox1.Text += message;
        //    }
        //}

        public bool IsConnected()
        {
            return ((TCPConnection.Stateobject != null) && (TCPConnection.Stateobject.workSocket != null) && (TCPConnection.Stateobject.workSocket.IsConnected()));
        }

        public void Chrout(byte c)
        {
            if (localEchoToolStripMenuItem.Checked) terminalControl1.c64screen.Chrout(c);
            if (IsConnected()) TCPConnection.Send(c);
            //DebugMessage(c.ToString() + " sent\r\n");
        }

        private void controlToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public static bool FormDead = false;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormDead = true;
            StatusTimer.Enabled = false;
            Thread.Sleep(1000);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormDead = true;
            StatusTimer.Enabled = false;
            Thread.Sleep(1000);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mnuSendPunter_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Select file to upload";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                byte[] filebytes = File.ReadAllBytes(openFileDialog1.FileName);

                FileTransfers.Punter_Send(Path.GetFileName(openFileDialog1.FileName), true, filebytes);
            }
            
        }

        private void multiReceivePunterToolStripMenuItem_Click(object sender, EventArgs e)
        {
           folderBrowserDialog1.Description = "Save downloaded files to:";
            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                FileTransfers.Punter_ReceiveMulti(folderBrowserDialog1.SelectedPath);
            }

        }

        private void multiSendPunterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Select files to upload";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<Punter_File> Files = new List<Punter_File>();
                foreach (string f in openFileDialog1.FileNames)
                {
                    Punter_File pf = new Punter_File();
                    pf.Filename = Encoding.ASCII.GetBytes(Path.GetFileName(f));
                    pf.FileData = File.ReadAllBytes(f);
                    pf.FileType = 0x50;
                    Files.Add(pf);
                }
                FileTransfers.Punter_SendMulti(Files);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }




    }
}
