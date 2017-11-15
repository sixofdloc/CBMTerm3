using CBMTerm3.Properties;
using System;
using System.Windows.Forms;

namespace CBMTerm3
{
    public partial class QuickConnect : Form
    {
        public QuickConnect()
        {
            InitializeComponent();
            textBox1.Text = Settings.Default.QuickConnect_Address;
            textBox2.Text = Settings.Default.QuickConnect_Port;
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Connect
            Settings.Default.QuickConnect_Address = textBox1.Text;
            Settings.Default.QuickConnect_Port = textBox2.Text;
            Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
        }
    }
}
