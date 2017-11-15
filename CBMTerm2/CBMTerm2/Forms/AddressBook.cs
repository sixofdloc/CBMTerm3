using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CBMTerm2
{
    public partial class AddressBook : Form
    {
        public List<AddressEntry> Addresses { get; set; }
        public AddressBook()
        {
            InitializeComponent();
        }

        public void LoadAddresses()
        {
            Addresses = new List<AddressEntry>();
            string[] AddressBulk = Settings.Default.AddressBook.Split('~');
            foreach (string s in AddressBulk)
            {
                string[] AddressData = s.Replace("%TILDE%","~").Split('|');
                Addresses.Add(new AddressEntry(s));
            }
            RefreshGrid();
        }

        public void SaveAddresses()
        {
            String t = "";
            foreach (AddressEntry ae in Addresses)
            {
                if (t != "") t = t + "~";
                t = t + ae.ToString().Replace("~", "%TILDE%");
            }
            Settings.Default.AddressBook = t;
            Settings.Default.Save();
        }

        public  void RefreshGrid()
        {
            dataGridView1.Rows.Clear();
            foreach (AddressEntry ae in Addresses)
            {
                dataGridView1.Rows.Add(ae.SystemName, ae.Address, ae.Port, ae.Description);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Add Entry
            AddressEdit ae = new AddressEdit();
            if (ae.ShowDialog() == DialogResult.OK)
            {
                AddressEntry ade = new AddressEntry();
                ade.Address = ae.tbAddress.Text;
                ade.SystemName = ae.tbSystemName.Text;
                ade.Description = ae.tbDescription.Text;
                ade.Port = ae.tbPort.Text;
                Addresses.Add(ade);
                SaveAddresses();
                LoadAddresses();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Delete selected entry
            if (MessageBox.Show("This cannot be undone.  Are you sure you want to delete this entry?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Addresses.RemoveAll(p=>p.SystemName.Equals(dataGridView1.SelectedRows[0].Cells[0].Value.ToString())
                    && p.Address.Equals(dataGridView1.SelectedRows[0].Cells[1].Value.ToString())
                    && p.Port.Equals(dataGridView1.SelectedRows[0].Cells[2].Value.ToString())
                    && p.Description.Equals(dataGridView1.SelectedRows[0].Cells[3].Value.ToString())
                    );
                SaveAddresses();
                LoadAddresses();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Edit Entry
            AddressEdit ae = new AddressEdit();
            ae.tbSystemName.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            ae.tbAddress.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            ae.tbPort.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            ae.tbDescription.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            if (ae.ShowDialog() == DialogResult.OK)
            {
                Addresses.RemoveAll(p => p.SystemName.Equals(dataGridView1.SelectedRows[0].Cells[0].Value.ToString())
                 && p.Address.Equals(dataGridView1.SelectedRows[0].Cells[1].Value.ToString())
                 && p.Port.Equals(dataGridView1.SelectedRows[0].Cells[2].Value.ToString())
                 && p.Description.Equals(dataGridView1.SelectedRows[0].Cells[3].Value.ToString())
                 );
                AddressEntry ade = new AddressEntry();
                ade.Address = ae.tbAddress.Text;
                ade.SystemName = ae.tbSystemName.Text;
                ade.Description = ae.tbDescription.Text;
                ade.Port = ae.tbPort.Text;
                Addresses.Add(ade);
                SaveAddresses();
                LoadAddresses();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Connect to System
            this.DialogResult = DialogResult.OK;
        }
    }
}
