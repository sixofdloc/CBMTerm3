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
    class EasterEgg: GraphicsDeviceControl
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        Texture2D DummyTexture;

        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            DummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            DummyTexture.SetData(new Color[] { Color.White });
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

            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin();
            

            spriteBatch.End();
        }
    }
}
