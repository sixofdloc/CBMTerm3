using System;
using System.Drawing;
using System.Windows.Forms;

namespace CBMTerm3.Forms
{
    public partial class About : Form
    {

        private byte[] sintab0 = {128,131,134,137,140,143,146,149,152,155,158,162,165,167,170,173,
               176,179,182,185,188,190,193,196,198,201,203,206,208,211,213,215,
               218,220,222,224,226,228,230,232,234,235,237,238,240,241,243,244,
               245,246,248,249,250,250,251,252,253,253,254,254,254,255,255,255,
               255,255,255,255,254,254,254,253,253,252,251,250,250,249,248,246,
               245,244,243,241,240,238,237,235,234,232,230,228,226,224,222,220,
               218,215,213,211,208,206,203,201,198,196,193,190,188,185,182,179,
               176,173,170,167,165,162,158,155,152,149,146,143,140,137,134,131,
               127,124,121,118,115,112,109,106,103,100,97,93,90,88,85,82,
               79,76,73,70,67,65,62,59,57,54,52,49,47,44,42,40,
               37,35,33,31,29,27,25,23,21,20,18,17,15,14,12,11,
               10,9,7,6,5,5,4,3,2,2,1,1,1,0,0,0,
               0,0,0,0,1,1,1,2,2,3,4,5,5,6,7,9,
               10,11,12,14,15,17,18,20,21,23,25,27,29,31,33,35,
               37,40,42,44,47,49,52,54,57,59,62,65,67,70,73,76,
               79,82,85,88,90,93,97,100,103,106,109,112,115,118,121,124};



        private byte[] sintab1 = {68,68,68,70,71,74,76,80,84,88,92,96,101,106,110,114,
               118,122,125,128,130,131,133,134,134,134,134,134,133,132,132,132,
               131,130,130,130,130,132,133,134,136,138,140,142,146,148,152,154,
               158,161,164,166,168,170,172,174,175,176,177,178,178,178,178,177,
               177,177,176,176,176,177,178,178,179,180,181,182,184,184,186,187,
               188,189,190,189,188,188,186,184,180,177,172,168,162,156,150,143,
               136,128,120,112,105,98,90,84,78,73,68,64,62,60,58,58,
               58,60,62,65,68,72,76,81,86,90,95,100,104,108,112,115,
               118,120,122,122,122,123,122,122,120,119,118,117,115,114,113,112,
               112,112,112,114,116,117,120,122,126,129,133,137,142,146,150,154,
               158,162,164,168,171,173,174,176,176,177,177,176,176,174,174,172,
               170,168,166,164,162,161,160,158,156,155,154,152,152,150,150,148,
               147,146,144,142,140,137,134,130,127,124,119,115,110,106,102,96,
               92,88,84,80,76,74,72,70,70,70,70,72,73,76,78,82,
               86,90,94,99,104,108,112,116,119,122,124,126,127,128,127,126,
               124,122,118,114,111,106,102,97,92,88,84,80,76,74,72,70};


        private byte[] sintab2 = {128,140,152,165,176,188,198,208,218,226,234,240,245,250,253,254,
               255,254,253,250,245,240,234,226,218,208,198,188,176,165,152,140,
               127,115,103,90,79,67,57,47,37,29,21,15,10,5,2,1,
               0,1,2,5,10,15,21,29,37,47,57,67,79,90,103,115,
               128,140,152,165,176,188,198,208,218,226,234,240,245,250,253,254,
               255,254,253,250,245,240,234,226,218,208,198,188,176,165,152,140,
               127,115,103,90,79,67,57,47,37,29,21,15,10,5,2,1,
               0,1,2,5,10,15,21,29,37,47,57,67,79,90,103,115,
               128,140,152,165,176,188,198,208,218,226,234,240,245,250,253,254,
               255,254,253,250,245,240,234,226,218,208,198,188,176,165,152,140,
               127,115,103,90,79,67,57,47,37,29,21,15,10,5,2,1,
               0,1,2,5,10,15,21,29,37,47,57,67,79,90,103,115,
               128,140,152,165,176,188,198,208,218,226,234,240,245,250,253,254,
               255,254,253,250,245,240,234,226,218,208,198,188,176,165,152,140,
               127,115,103,90,79,67,57,47,37,29,21,15,10,5,2,1,
               0,1,2,5,10,15,21,29,37,47,57,67,79,90,103,115};



        private SolidBrush[] brushes;

        private byte[] sine0 = { 
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
                               };

        private byte[] sine1 = { 
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
                                   0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
                               };

        private int x1 = 0;
        private int y1 = 0;
        private int z1 = 0;
        private int sc1 = 0;
        private int sc2 = 0;
        private int sc3 = 0;
        private int sc4 = 0;
        private int sc5 = 0;
        private int rb = 0;
        private int bb = 0;
        private int gb = 0;



        public About()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.Width = 349;

        }

        private void Randomize()
        {
            Random x = new Random(DateTime.Now.Millisecond);
            x1 = x.Next(255);
            y1 = x.Next(255);
            z1 = x.Next(255);
            sc1 = x.Next(3) + 1;
            sc2 = x.Next(3) + 1;
            sc3 = x.Next(3) + 1;
            sc4 = x.Next(3) + 1;
            sc5 = x.Next(3) + 1;
            rb = x.Next(255);
            bb = x.Next(255);
            gb = x.Next(255);
            InitPlasma();
        }
        private void SetControls()
        {
            hScrollBar1.Value = sc1;
            label3.Text = sc1.ToString();
            hScrollBar2.Value = sc2;
            label6.Text = sc2.ToString();
            hScrollBar3.Value = sc3;
            label8.Text = sc3.ToString();
            hScrollBar4.Value = sc4;
            label10.Text = sc4.ToString();
            hScrollBar5.Value = sc5;
            label12.Text = sc5.ToString();
            hScrollBar6.Value = rb;
            label14.Text = rb.ToString();
            hScrollBar7.Value = gb;
            label16.Text = gb.ToString();
            hScrollBar8.Value = bb;
            label18.Text = bb.ToString();
        }

        private void InitPlasma()
        {
            brushes = new SolidBrush[256];
            for (int i = 0; i < 256; i++)
            {
                SolidBrush s = new SolidBrush(Color.FromArgb(255, (i + rb) % 255, Math.Abs((bb - i) % 255), (i / ((gb + 1) * 2)) % 255));
                brushes[i] = s;
            }
            CalcPlasma();
        }

        private void CalcPlasma()
        {
            int x0;
            int y0;
            x0 = x1;
            y0 = y1;

            for (int i = 0; i < 160; i++)
            {
                sine0[i] = sintab0[x0];
                x0 += sc1;
                if (x0 > 255) x0 = 0;
            }
            for (int j = 0; j < 100; j++)
            {
                sine1[j] = sintab0[y0];
                y0 += sc2;
                if (y0 > 255) y0 = 0;
            }
            x1 += sc5;
            if (x1 > 255) x1 = 0;
            y1 += sc4;
            if (y1 > 255) y1 = 0;
            z1 += sc3;
            if (z1 > 255) z1 = 0;
        }

        private void DrawPlasma(Graphics g)
        {
            int q;
            for (int x = 0; x < 320; x += 2)
            {
                for (int y = 0; y < 200; y += 2)
                {
                    q = (sine0[x / 2] + sine1[y / 2] + sintab2[z1]) % 255;
                    g.FillRectangle(brushes[q], x, y, 2, 2);
                }
            }
        }

        Bitmap bufl;

        private void Draw()
        {
            using (Graphics g = Graphics.FromImage(bufl))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, panel1.Width, panel1.Height));
                CalcPlasma();
                DrawPlasma(g);
                panel1.CreateGraphics().DrawImageUnscaled(bufl, 0, 0);
            }
            //            bufl.Dispose();
        }
        private void Init()
        {
            bufl = new Bitmap(panel1.Width, panel1.Height);
            Randomize();
            SetControls();
            timer1.Interval = 1000 / 40;
            timer1.Enabled = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Draw();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Hide();
        }

        private void About_Shown(object sender, EventArgs e)
        {
            Init();
        }

        private void About_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.Width = (this.Width == 579) ? 349 : 579;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Randomize();
            SetControls();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            sc1 = e.NewValue;
            label3.Text = e.NewValue.ToString();
            InitPlasma();
        }

        private void hScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            sc2 = e.NewValue;
            label6.Text = e.NewValue.ToString();
            InitPlasma();

        }

        private void hScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {
            sc3 = e.NewValue;
            label8.Text = e.NewValue.ToString();
            InitPlasma();

        }

        private void hScrollBar4_Scroll(object sender, ScrollEventArgs e)
        {
            sc4 = e.NewValue;
            label10.Text = e.NewValue.ToString();
            InitPlasma();

        }

        private void hScrollBar5_Scroll(object sender, ScrollEventArgs e)
        {
            sc5 = e.NewValue;
            label12.Text = e.NewValue.ToString();
            InitPlasma();

        }

        private void hScrollBar6_Scroll(object sender, ScrollEventArgs e)
        {
            rb = e.NewValue;
            label14.Text = e.NewValue.ToString();
            InitPlasma();
        }

        private void hScrollBar7_Scroll(object sender, ScrollEventArgs e)
        {
            gb = e.NewValue;
            label16.Text = e.NewValue.ToString();
            InitPlasma();

        }

        private void hScrollBar8_Scroll(object sender, ScrollEventArgs e)
        {
            bb = e.NewValue;
            label18.Text = e.NewValue.ToString();
            InitPlasma();

        }

        private void About_Load(object sender, EventArgs e)
        {

        }



    }
}
