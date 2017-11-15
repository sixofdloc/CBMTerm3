using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;

namespace CBMTerm2
{
    class C64Screen
    {
        //C64 color constants
        public const byte BLACK = 0x00;
        public const byte WHITE = 0x01;
        public const byte RED = 0x02;
        public const byte CYAN = 0x03;
        public const byte PURPLE = 0x04;
        public const byte GREEN = 0x05;
        public const byte BLUE = 0x06;
        public const byte YELLOW = 0x07;
        public const byte ORANGE = 0x08;
        public const byte BROWN = 0x09;
        public const byte PINK = 0x0a;
        public const byte GREY1 = 0x0b;
        public const byte GREY2 = 0x0c;
        public const byte LT_GREEN = 0x0d;
        public const byte LT_BLUE = 0x0e;
        public const byte GREY3 = 0x0f;

        //C64 key constants
        public const byte C64_CURSOR_UP = 0x91;
        public const byte C64_CURSOR_DOWN = 0x11;
        public const byte C64_CURSOR_LEFT = 0x9d;
        public const byte C64_CURSOR_RIGHT = 0x1d;
        public const byte C64_KEY_BLACK =144;
        public const byte C64_KEY_WHITE = 5;
        public const byte C64_KEY_RED = 28;
        public const byte C64_KEY_CYAN = 159;
        public const byte C64_KEY_PURPLE = 156;
        public const byte C64_KEY_GREEN = 30;
        public const byte C64_KEY_BLUE = 31;
        public const byte C64_KEY_YELLOW = 158;
        public const byte C64_KEY_ORANGE = 129;
        public const byte C64_KEY_BROWN = 149;
        public const byte C64_KEY_PINK = 150;
        public const byte C64_KEY_GREY1 = 151;
        public const byte C64_KEY_GREY2 = 152;
        public const byte C64_KEY_LT_GREEN = 153;
        public const byte C64_KEY_LT_BLUE = 154;
        public const byte C64_KEY_GREY3 = 155;
        public const byte C64_KEY_REVS_ON = 18;
        public const byte C64_KEY_REVS_OFF = 146;


        public static Color[] Colors = { Color.Black,
                                  Color.White,
                                  new Color(132,52,32),  //Red
                                  new Color(0,255,255), //Cyan
                                  new Color(192,64,192), //Purple
                                  new Color(32,132,52), //Green
                                  new Color(32,52,132), //Blue
                                  new Color(255,255,0), //Yellow
                                  new Color(255,128,0), //Orange
                                  new Color(90,55,16), //Brown
                                  new Color(255,100,100), //Pink
                                  new Color(64,64,64), //Grey 1
                                  new Color(128,128,128), //Grey 2
                                  new Color(128,255,128), //Light Green
                                  new Color(128,128,255), //Light blue
                                  new Color(192,192,192) //Grey 3
                                };

        public int Border_Color { get; set; }
        public int Background_Color { get; set; }
        public byte Cursor_Color { get; set; }
        public List<byte> Screen_RAM { get; set; }
        public List<byte> Color_RAM { get; set; }

        public Coords CursorPos { get; set; }

        public bool CursorOn { get; set; }
        public bool RevsOn { get; set; }
        public bool CShift { get; set; }  // Are we in the alternate cset?

        public Timer CursorTimer { get; set; }

        private byte Cursor_Color_Stash { get; set; }

        public C64Screen()
        {
            Border_Color = GREY1;
            Background_Color = BLACK;
            Cursor_Color = WHITE;
            Cursor_Color_Stash = WHITE;
            Screen_RAM = Enumerable.Repeat((byte)0x00, 1000).ToList();
            Color_RAM = Enumerable.Repeat((byte)0x00, 1000).ToList();
            RevsOn = false;
            CShift = false;
            CursorTimer = new Timer(400);
            CursorTimer.Elapsed +=new ElapsedEventHandler(CursorTimer_Elapsed);
            CursorTimer.Enabled = true;
            CLS();
        }

        private void CursorTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            CursorTimer.Enabled = false;
            int cursoridx = CursorPos.X + (CursorPos.Y * 40);
                if (CursorOn)
                {
                    Color_RAM[cursoridx] = Cursor_Color_Stash;
                    Screen_RAM[cursoridx] -= 0x80;
                    CursorOn = false;
                }
                else
                {
                    Cursor_Color_Stash = Color_RAM[cursoridx];
                    Color_RAM[cursoridx] = Cursor_Color;
                    Screen_RAM[cursoridx] += 0x80;
                    CursorOn = true;
                }
                CursorTimer.Enabled = true;
        }

        public void Set_Cursor_Color(byte color)
        {
            switch (color)
            {
                case BLACK:
                    Chrout(C64_KEY_BLACK);
                    break;
                case WHITE:
                    Chrout(C64_KEY_WHITE);
                    break;
                case RED:
                    Chrout(C64_KEY_RED);
                    break;
                case CYAN:
                    Chrout(C64_KEY_CYAN);
                    break;
                case PURPLE:
                    Chrout(C64_KEY_PURPLE);
                    break;
                case GREEN:
                    Chrout(C64_KEY_GREEN);
                    break;
                case BLUE:
                    Chrout(C64_KEY_BLUE);
                    break;
                case YELLOW:
                    Chrout(C64_KEY_YELLOW);
                    break;
                case ORANGE:
                    Chrout(C64_KEY_ORANGE);
                    break;
                case BROWN:
                    Chrout(C64_KEY_BROWN);
                    break;
                case PINK:
                    Chrout(C64_KEY_PINK);
                    break;
                case GREY1:
                    Chrout(C64_KEY_GREY1);
                    break;
                case GREY2:
                    Chrout(C64_KEY_GREY2);
                    break;
                case LT_GREEN:
                    Chrout(C64_KEY_LT_GREEN);
                    break;
                case LT_BLUE:
                    Chrout(C64_KEY_LT_BLUE);
                    break;
                case GREY3:
                    Chrout(C64_KEY_GREY3);
                    break;
            }

        }

        public void Chrout(byte c)
        {
            //while (CursorOn) { };
            CursorTimer.Enabled = false;
            if (CursorOn)
            {
                Color_RAM[CursorPos.X + (CursorPos.Y * 40)] = Cursor_Color_Stash;
                Screen_RAM[CursorPos.X + (CursorPos.Y * 40)] -= 0x80;
                CursorOn = false;
            }
            switch (c)
            {   
                case 0x05: 
                    Cursor_Color = WHITE;
                    Cursor_Color_Stash = WHITE;
                    break;
                case 0x07:
                    //Play bell?
                    break;
                case 0x0d:
                    Carriage_Return();
                    break;
                case 0x8d:
                    Carriage_Return();
                    break;
                case 0x0e:
                    Set_Lowercase();
                    break;
                case 0x11:
                    Cursor_Down();
                    break;
                case 0x12:
                    RevsOn = true;
                    break;
                case 0x13:
                    Home();
                    break;
                case 0x14:
                    DEL();
                    break;
                case 0x1c:
                    Cursor_Color = RED;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x1d:
                    Cursor_Right();
                    break;
                case 0x1e:
                    Cursor_Color = GREEN;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x1f:
                    Cursor_Color = BLUE;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x81: 
                    Cursor_Color = ORANGE ;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x8e:
                    Set_Uppercase();
                    break;
                case 0x90:
                    Cursor_Color = BLACK;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x91:
                    Cursor_Up();
                    break;
                case 0x92:
                    RevsOn = false;
                    break;
                case 0x93:
                    CLS();
                    break;
                case 0x95: 
                    Cursor_Color = BROWN;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x96: 
                    Cursor_Color = PINK;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x97: 
                    Cursor_Color = GREY1;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x98: 
                    Cursor_Color = GREY2;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x99: 
                    Cursor_Color = LT_GREEN;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x9a: 
                    Cursor_Color = LT_BLUE;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x9b: 
                    Cursor_Color = GREY3;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x9c: 
                    Cursor_Color = PURPLE;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x9d: 
                    Cursor_Left();
                    break;
                case 0x9e: 
                    Cursor_Color = YELLOW;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0x9f: 
                    Cursor_Color = CYAN;
                    Cursor_Color_Stash = Cursor_Color;
                    break;
                case 0xff: 
                    ScrnOut(94);
                    break;
                default:
                    if ((c >= 0x20) && (c < 0x40)) ScrnOut(c);
                    if ((c >= 0x40) && (c <= 0x5f)) ScrnOut((byte)(c-0x40));
                    if ((c >= 0x60) && (c <= 0x7f)) ScrnOut((byte)(c - 0x20));
                    if ((c >= 0xa0) && (c <= 0xbf)) ScrnOut((byte)(c - 0x40));
                    if ((c >= 0xc0) && (c <= 0xfe)) ScrnOut((byte)(c - 0x80));
                    break;
            }
            CursorTimer.Enabled = true;
        }

        public void ScrnOut(byte b)
        {
            byte c;
            c = b;
            if (RevsOn) c += 0x80;
            Screen_RAM[(int)((CursorPos.Y * 40) + CursorPos.X)] = c;
            Color_RAM[(int)((CursorPos.Y * 40) + CursorPos.X)] = Cursor_Color;
            Cursor_Right();
        }
        #region Cursor Keys
        private void Cursor_Right()
        {
            if (CursorPos.X < 39)
            {
                CursorPos.X++;
            }
            else
            {
                if (CursorPos.Y < 24)
                {
                    CursorPos.Y++;
                    CursorPos.X = 0;
                }
                else
                {
                    ScrollAllUp();
                    CursorPos.X = 0;
                    CursorPos.Y = 24;
                }
            }
        }

        private void Cursor_Up()
        {
            if (CursorPos.Y > 0) CursorPos.Y--;
        }

        private void Cursor_Down()
        {
            if (CursorPos.Y < 24)
            {
                CursorPos.Y++;
            }
            else
            {
                ScrollAllUp();
            }
        }

        private void Cursor_Left()
        {
            if (CursorPos.X > 0)
            {
                CursorPos.X--;
            }
            else
            {
                if (CursorPos.Y > 0)
                {
                    CursorPos.X = 39;
                    CursorPos.Y--;
                }
            }
        }
        #endregion

        #region Case_Control
        public void Set_Lowercase()
        {
            CShift = false;
        }
        public void Set_Uppercase()
        {
            CShift = true;
        }
        #endregion


        private void Carriage_Return()
        {
            Cursor_Down();
            RevsOn = false;
            CursorPos.X = 0;
        }

        private void Home()
        {
            CursorPos.X = 0;
            CursorPos.Y = 0;
        }

        private void CLS()
        {
            CursorPos = new Coords(0, 0);
            for (int i = 0; i < 1000; i++)
            {
                Screen_RAM[i] = 0x20;// (byte)(i % 40);
                Color_RAM[i] = Cursor_Color; // (byte)(i % 16); // Cursor_Color;
            }

        }

        private void DEL()
        {
            Cursor_Left();
            ScrnOut(0x20);
            Cursor_Left();
        }

        private void ScrollAllUp()
        {
            for (int i = 0; i <= 959; i++)
            {
                Screen_RAM[i] = Screen_RAM[i + 40];
                Color_RAM[i] = Color_RAM[i + 40];
            }
            for (int i = 960; i <= 999; i++)
            {
                Screen_RAM[i] = 0x20;
                Color_RAM[i] = Cursor_Color;
            }
        }


    }
}
