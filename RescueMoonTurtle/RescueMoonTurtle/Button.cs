using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
   public class Button:Animation
    {
        public bool selected = false;
        public string buttonName;
        public bool pressed = false;
        public Button(Vector2 position, Texture2D buttonTexture, string buttonname)
            : base(buttonTexture, 1, 1, position, 0f, Color.White)
        {
          
            this.buttonName = buttonname;
            this.Position = position;
        }
        public bool CheckForClick()
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
            {
                if (selected)
                {
                    pressed = true;
                    return true;
                }
            }
            pressed = false;
            return false;

        }

        public override void Update(GameTime gameTime)
        {
        
            if (selected == true)
            {
                scale = 1.2f;
                color = Color.White;
            }
            else
            {
                scale = 1f;
                flip = SpriteEffects.None;
                color = Color.White;

            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
        }



    }
}