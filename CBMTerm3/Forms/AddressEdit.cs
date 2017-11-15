using System;
using System.Windows.Forms;

namespace CBMTerm3
{
    public partial class AddressEdit : Form
    {
        public AddressEdit()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
