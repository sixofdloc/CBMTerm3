using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using SharpDX.Windows;


namespace CBMTerm2
{
    class TerminalPanel : RenderControl
    {
        Bitmap LCSet;
        Bitmap UCSet;
        public C64Screen c64screen;
        Rectangle ScreenRect = new Rectangle(40, 40, 640, 400);
        Rectangle[] CharRects;
        Rectangle[] FontRects;
        //ColorMatrix[] ColorMatrices;
        ImageAttributes[] attributes;

        public TerminalPanel()
        {
            try
            {
                LCSet = new Bitmap("Images\\c64font1.png");
                UCSet = new Bitmap("Images\\c64font2.png");
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
                CharRects = new Rectangle[1000];
                FontRects = new Rectangle[256];
                attributes = new ImageAttributes[16];
                for (int c = 0; c < 16; c++)
                {
                    attributes[c] = new ImageAttributes();
                    attributes[c].SetColorMatrix(
                    new ColorMatrix(
                        new float[][]{
                        new float[] {0,0,0,0,0},
                        new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {C64Screen.Colors[c].R / 255.0f,
                                 C64Screen.Colors[c].G / 255.0f,
                                 C64Screen.Colors[c].B / 255.0f,
                                 0, 1}
                    }
                        )
                        );
                }
                for (int row = 0; row <= 24; row++)
                {
                    for (int col = 0; col <= 39; col++)
                    {
                        CharRects[(row * 40) + col] = new Rectangle(40 + (col * 16), 40 + (row * 16), 16, 16);
                    }
                }
                for (int c = 0; c < 256; c++)
                {
                    //byte c = c64screen.Screen_RAM[(row * 40) + col];
                    int charrow = c / 40;
                    int charcol = c % 40;
                    FontRects[c] = new Rectangle(charcol * 16, charrow * 16, 16, 16);

                }

                c64screen = new C64Screen();
                //LCSet = content.Load<Texture2D>("c64font2");
                //UCSet = content.Load<Texture2D>("c64font1");
                //DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
                //DummyTexture.SetData(new Color[] { new Color(255, 255, 255, 255) });
                //            Application.Idle += delegate { Invalidate(); };
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
               // base.OnPaint(e);
                Graphics g = e.Graphics;
                g.Clear(C64Screen.Colors[c64screen.Border_Color]);
                Brush back = new SolidBrush(C64Screen.Colors[c64screen.Background_Color]);


                g.FillRectangle(back, ScreenRect);
                int index = 0;
                for (int row = 0; row <= 24; row++)
                {
                    for (int col = 0; col <= 39; col++)
                    {
                        byte c = c64screen.Screen_RAM[(row * 40) + col];
                        //                    int charrow = c / 40;
                        //                    int charcol = c % 40;
                        //                    Rectangle srcRect = new Rectangle(charcol * 16, charrow * 16, 16, 16);
                        //Rectangle destRect = new Rectangle(40+(col * 16), 40+(row * 16), 16, 16);
                        //attributes.SetColorMatrix(ColorMatrices[c64screen.Color_RAM[index]]);
                        g.DrawImage(((c64screen.CShift) ? UCSet : LCSet), CharRects[index], FontRects[c64screen.Screen_RAM[index]].Left, FontRects[c64screen.Screen_RAM[index]].Top, FontRects[c64screen.Screen_RAM[index]].Width, FontRects[c64screen.Screen_RAM[index]].Height, GraphicsUnit.Pixel, attributes[c64screen.Color_RAM[index]]);
                        //, CharRects[index], FontRects[c64screen.Screen_RAM[index]], C64Screen.Colors[c64screen.Color_RAM[index]]);
                        index++;
                    }
                }
                c64screen.NeedsDraw = false;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
