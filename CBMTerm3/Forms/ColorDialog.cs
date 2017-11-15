using System;
using System.Windows.Forms;

namespace CBMTerm3.Forms
{
    public partial class ColorDialog : Form
    {

        public int SelectedColor = 0;

        public ColorDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedColor = 0;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectedColor = 1;
            this.DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectedColor = 2;
            this.DialogResult = DialogResult.OK;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            SelectedColor = 3;
            this.DialogResult = DialogResult.OK;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SelectedColor = 4;
            this.DialogResult = DialogResult.OK;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SelectedColor = 5;
            this.DialogResult = DialogResult.OK;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SelectedColor = 6;
            this.DialogResult = DialogResult.OK;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SelectedColor = 7;
            this.DialogResult = DialogResult.OK;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            SelectedColor = 8;
            this.DialogResult = DialogResult.OK;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            SelectedColor = 9;
            this.DialogResult = DialogResult.OK;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            SelectedColor = 10;
            this.DialogResult = DialogResult.OK;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            SelectedColor = 11;
            this.DialogResult = DialogResult.OK;

        }

        private void button12_Click(object sender, EventArgs e)
        {
            SelectedColor = 12;
            this.DialogResult = DialogResult.OK;

        }

        private void button11_Click(object sender, EventArgs e)
        {
            SelectedColor = 13;
            this.DialogResult = DialogResult.OK;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            SelectedColor = 14;
            this.DialogResult = DialogResult.OK;

        }

        private void button9_Click(object sender, EventArgs e)
        {
            SelectedColor = 15;
            this.DialogResult = DialogResult.OK;

        }

        private void button17_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
