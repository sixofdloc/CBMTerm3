using CBMTerm2.Classes;
using FileTransferProtocols.Interfaces;
namespace CBMTerm2
{
    partial class MainForm : IDataReceiver, IConnectionMonitor, IDataSender
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuQuickConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewCG = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveScreen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveBuffer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSendXModem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReceiveXModem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSendPunter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReceivePunter = new System.Windows.Forms.ToolStripMenuItem();
            this.multiReceivePunterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiSendPunterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shiftedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localEchoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBackgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBorderColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.baudLimitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NoBaudLimitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Baud300MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Baud1200MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Baud2400MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bufferOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearBufferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ConnectionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.BufferLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.terminalControl1 = new CBMTerm2.TerminalControl();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.controlToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(725, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuQuickConnect,
            this.mnuViewCG,
            this.mnuSaveScreen,
            this.mnuSaveBuffer,
            this.toolStripMenuItem6,
            this.toolStripMenuItem3,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // mnuQuickConnect
            // 
            this.mnuQuickConnect.Name = "mnuQuickConnect";
            this.mnuQuickConnect.Size = new System.Drawing.Size(178, 22);
            this.mnuQuickConnect.Text = "Quick Connect";
            this.mnuQuickConnect.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // mnuViewCG
            // 
            this.mnuViewCG.Name = "mnuViewCG";
            this.mnuViewCG.Size = new System.Drawing.Size(178, 22);
            this.mnuViewCG.Text = "View CG File";
            this.mnuViewCG.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // mnuSaveScreen
            // 
            this.mnuSaveScreen.Name = "mnuSaveScreen";
            this.mnuSaveScreen.Size = new System.Drawing.Size(178, 22);
            this.mnuSaveScreen.Text = "Save Screen";
            // 
            // mnuSaveBuffer
            // 
            this.mnuSaveBuffer.Name = "mnuSaveBuffer";
            this.mnuSaveBuffer.Size = new System.Drawing.Size(178, 22);
            this.mnuSaveBuffer.Text = "Save Capture Buffer";
            this.mnuSaveBuffer.Click += new System.EventHandler(this.mnuSaveBuffer_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.mnuSendXModem,
            this.mnuReceiveXModem,
            this.mnuSendPunter,
            this.mnuReceivePunter,
            this.multiReceivePunterToolStripMenuItem,
            this.multiSendPunterToolStripMenuItem});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuItem6.Text = "File Transfers";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.toolStripMenuItem1.Text = "SendFile RAW";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click_1);
            // 
            // mnuSendXModem
            // 
            this.mnuSendXModem.Name = "mnuSendXModem";
            this.mnuSendXModem.Size = new System.Drawing.Size(195, 22);
            this.mnuSendXModem.Text = "Send File(XModem)";
            this.mnuSendXModem.Click += new System.EventHandler(this.mnuSendXModem_Click);
            // 
            // mnuReceiveXModem
            // 
            this.mnuReceiveXModem.Name = "mnuReceiveXModem";
            this.mnuReceiveXModem.Size = new System.Drawing.Size(195, 22);
            this.mnuReceiveXModem.Text = "Receive File (XModem)";
            this.mnuReceiveXModem.Click += new System.EventHandler(this.mnuReceiveXModem_Click);
            // 
            // mnuSendPunter
            // 
            this.mnuSendPunter.Name = "mnuSendPunter";
            this.mnuSendPunter.Size = new System.Drawing.Size(195, 22);
            this.mnuSendPunter.Text = "Send File (Punter)";
            this.mnuSendPunter.Click += new System.EventHandler(this.mnuSendPunter_Click);
            // 
            // mnuReceivePunter
            // 
            this.mnuReceivePunter.Name = "mnuReceivePunter";
            this.mnuReceivePunter.Size = new System.Drawing.Size(195, 22);
            this.mnuReceivePunter.Text = "Receive File (Punter)";
            this.mnuReceivePunter.Click += new System.EventHandler(this.mnuReceivePunter_Click);
            // 
            // multiReceivePunterToolStripMenuItem
            // 
            this.multiReceivePunterToolStripMenuItem.Name = "multiReceivePunterToolStripMenuItem";
            this.multiReceivePunterToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.multiReceivePunterToolStripMenuItem.Text = "Multi-Receive (Punter)";
            this.multiReceivePunterToolStripMenuItem.Click += new System.EventHandler(this.multiReceivePunterToolStripMenuItem_Click);
            // 
            // multiSendPunterToolStripMenuItem
            // 
            this.multiSendPunterToolStripMenuItem.Name = "multiSendPunterToolStripMenuItem";
            this.multiSendPunterToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.multiSendPunterToolStripMenuItem.Text = "Multi-Send (Punter)";
            this.multiSendPunterToolStripMenuItem.Click += new System.EventHandler(this.multiSendPunterToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuItem3.Text = "&Address Book";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Enabled = false;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuItem2.Text = "&Disconnect";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shiftedToolStripMenuItem,
            this.localEchoToolStripMenuItem,
            this.setBackgroundColorToolStripMenuItem,
            this.setBorderColorToolStripMenuItem,
            this.toolStripMenuItem4,
            this.baudLimitToolStripMenuItem,
            this.bufferOnToolStripMenuItem,
            this.clearBufferToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.controlToolStripMenuItem.Text = "&Control";
            this.controlToolStripMenuItem.Click += new System.EventHandler(this.controlToolStripMenuItem_Click);
            // 
            // shiftedToolStripMenuItem
            // 
            this.shiftedToolStripMenuItem.Name = "shiftedToolStripMenuItem";
            this.shiftedToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.shiftedToolStripMenuItem.Text = "Shifted";
            this.shiftedToolStripMenuItem.Click += new System.EventHandler(this.shiftedToolStripMenuItem_Click);
            // 
            // localEchoToolStripMenuItem
            // 
            this.localEchoToolStripMenuItem.Name = "localEchoToolStripMenuItem";
            this.localEchoToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.localEchoToolStripMenuItem.Text = "Local Echo";
            this.localEchoToolStripMenuItem.Click += new System.EventHandler(this.localEchoToolStripMenuItem_Click);
            // 
            // setBackgroundColorToolStripMenuItem
            // 
            this.setBackgroundColorToolStripMenuItem.Name = "setBackgroundColorToolStripMenuItem";
            this.setBackgroundColorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setBackgroundColorToolStripMenuItem.Text = "Set Background Color";
            this.setBackgroundColorToolStripMenuItem.Click += new System.EventHandler(this.setBackgroundColorToolStripMenuItem_Click);
            // 
            // setBorderColorToolStripMenuItem
            // 
            this.setBorderColorToolStripMenuItem.Name = "setBorderColorToolStripMenuItem";
            this.setBorderColorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setBorderColorToolStripMenuItem.Text = "Set Border Color";
            this.setBorderColorToolStripMenuItem.Click += new System.EventHandler(this.setBorderColorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(189, 22);
            this.toolStripMenuItem4.Text = "Set Cursor Color";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click_1);
            // 
            // baudLimitToolStripMenuItem
            // 
            this.baudLimitToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NoBaudLimitMenuItem,
            this.Baud300MenuItem,
            this.Baud1200MenuItem,
            this.Baud2400MenuItem});
            this.baudLimitToolStripMenuItem.Name = "baudLimitToolStripMenuItem";
            this.baudLimitToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.baudLimitToolStripMenuItem.Text = "Baud Limit";
            // 
            // NoBaudLimitMenuItem
            // 
            this.NoBaudLimitMenuItem.Checked = true;
            this.NoBaudLimitMenuItem.CheckOnClick = true;
            this.NoBaudLimitMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NoBaudLimitMenuItem.Name = "NoBaudLimitMenuItem";
            this.NoBaudLimitMenuItem.Size = new System.Drawing.Size(103, 22);
            this.NoBaudLimitMenuItem.Text = "None";
            this.NoBaudLimitMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // Baud300MenuItem
            // 
            this.Baud300MenuItem.Name = "Baud300MenuItem";
            this.Baud300MenuItem.Size = new System.Drawing.Size(103, 22);
            this.Baud300MenuItem.Text = "300";
            this.Baud300MenuItem.Click += new System.EventHandler(this.Baud300MenuItem_Click);
            // 
            // Baud1200MenuItem
            // 
            this.Baud1200MenuItem.Name = "Baud1200MenuItem";
            this.Baud1200MenuItem.Size = new System.Drawing.Size(103, 22);
            this.Baud1200MenuItem.Text = "1200";
            this.Baud1200MenuItem.Click += new System.EventHandler(this.Baud1200MenuItem_Click);
            // 
            // Baud2400MenuItem
            // 
            this.Baud2400MenuItem.Name = "Baud2400MenuItem";
            this.Baud2400MenuItem.Size = new System.Drawing.Size(103, 22);
            this.Baud2400MenuItem.Text = "2400";
            this.Baud2400MenuItem.Click += new System.EventHandler(this.Baud2400MenuItem_Click);
            // 
            // bufferOnToolStripMenuItem
            // 
            this.bufferOnToolStripMenuItem.CheckOnClick = true;
            this.bufferOnToolStripMenuItem.Name = "bufferOnToolStripMenuItem";
            this.bufferOnToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.bufferOnToolStripMenuItem.Text = "Buffer On";
            this.bufferOnToolStripMenuItem.Click += new System.EventHandler(this.bufferOnToolStripMenuItem_Click);
            // 
            // clearBufferToolStripMenuItem
            // 
            this.clearBufferToolStripMenuItem.Name = "clearBufferToolStripMenuItem";
            this.clearBufferToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.clearBufferToolStripMenuItem.Text = "Clear Buffer";
            this.clearBufferToolStripMenuItem.Click += new System.EventHandler(this.clearBufferToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectionLabel,
            this.BufferLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 511);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(725, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ConnectionLabel
            // 
            this.ConnectionLabel.Name = "ConnectionLabel";
            this.ConnectionLabel.Size = new System.Drawing.Size(118, 17);
            this.ConnectionLabel.Text = "toolStripStatusLabel1";
            // 
            // BufferLabel
            // 
            this.BufferLabel.Name = "BufferLabel";
            this.BufferLabel.Padding = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.BufferLabel.Size = new System.Drawing.Size(218, 17);
            this.BufferLabel.Text = "toolStripStatusLabel2";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // terminalControl1
            // 
            this.terminalControl1.Location = new System.Drawing.Point(520, 113);
            this.terminalControl1.Name = "terminalControl1";
            this.terminalControl1.Size = new System.Drawing.Size(75, 23);
            this.terminalControl1.TabIndex = 5;
            this.terminalControl1.Text = "terminalControl1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 533);
            this.Controls.Add(this.terminalControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CBMTerm 2.0 - 2014 Six/Style";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shiftedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuQuickConnect;
        private System.Windows.Forms.ToolStripMenuItem localEchoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBackgroundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBorderColorToolStripMenuItem;
               private System.Windows.Forms.ToolStripStatusLabel ConnectionLabel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem baudLimitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NoBaudLimitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Baud300MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Baud1200MenuItem;
        private System.Windows.Forms.ToolStripMenuItem Baud2400MenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuViewCG;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveScreen;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem mnuSendXModem;
        private System.Windows.Forms.ToolStripMenuItem mnuReceiveXModem;
        private System.Windows.Forms.ToolStripMenuItem mnuSendPunter;
        private System.Windows.Forms.ToolStripMenuItem mnuReceivePunter;
        private System.Windows.Forms.ToolStripMenuItem bufferOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearBufferToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel BufferLabel;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveBuffer;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private TerminalControl terminalControl1;
        private System.Windows.Forms.ToolStripMenuItem multiReceivePunterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiSendPunterToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}