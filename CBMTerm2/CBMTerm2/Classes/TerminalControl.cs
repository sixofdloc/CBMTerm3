using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace CBMTerm2
{
    class TerminalControl: GraphicsDeviceControl
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        Texture2D LCSet;
        Texture2D UCSet;
        public C64Screen c64screen;
        Texture2D DummyTexture;
        Rectangle ScreenRect = new Rectangle(40, 40, 640, 400);
        Rectangle[] CharRects;
        Rectangle[] FontRects;
        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");

            CharRects = new Rectangle[1000];
            FontRects = new Rectangle[256];
            for (int row = 0; row <= 24; row++)
            {
                for (int col = 0; col <= 39; col++)
                {
                    CharRects[(row*40)+col] = new Rectangle(40 + (col * 16), 40 + (row * 16), 16, 16);
                }
            }
            for (int c = 0; c < 256; c++)
            {
                //byte c = c64screen.Screen_RAM[(row * 40) + col];
                int charrow = c / 40;
                int charcol = c % 40;
               FontRects[c] = new Rectangle(charcol * 16, charrow * 16, 16, 16);

            }

            spriteBatch = new SpriteBatch(GraphicsDevice);
            c64screen = new C64Screen();
            LCSet = content.Load<Texture2D>("c64font2");
            UCSet = content.Load<Texture2D>("c64font1");
            DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            DummyTexture.SetData(new Color[] { new Color(255,255,255,255) });
            Application.Idle += delegate { Invalidate(); };

        }


        /// <summary>
        /// Disposes the control, unloading the ContentManager.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Unload();
            }

            base.Dispose(disposing);
        }


        /// <summary>
        /// Draws the control, using SpriteBatch and SpriteFont.
        /// </summary>
        protected override void Draw()
        {
            
            GraphicsDevice.Clear(C64Screen.Colors[c64screen.Border_Color]);
            
            spriteBatch.Begin();
            spriteBatch.Draw(DummyTexture, ScreenRect, C64Screen.Colors[c64screen.Background_Color]);
            int index = 0;
            for (int row = 0; row<=24;row++){
                for (int col = 0; col <=39; col++)
                {
                    byte c = c64screen.Screen_RAM[(row*40)+col];
//                    int charrow = c / 40;
//                    int charcol = c % 40;
//                    Rectangle srcRect = new Rectangle(charcol * 16, charrow * 16, 16, 16);
                    //Rectangle destRect = new Rectangle(40+(col * 16), 40+(row * 16), 16, 16);
                    spriteBatch.Draw(((c64screen.CShift) ? UCSet : LCSet), CharRects[index], FontRects[c64screen.Screen_RAM[index]], C64Screen.Colors[c64screen.Color_RAM[index]]);
                    index++;
                }
            }
            

            spriteBatch.End();
        }
    }
}
