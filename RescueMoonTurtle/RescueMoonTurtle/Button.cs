using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    //Inherits from animation class (EXAMPLE OF INHERITANCE)
   public class Button:Animation
   {
        MouseState mouseState;
        public bool selected = false;
        public string buttonName;
        public bool pressed = false;

        //Constructor creates new button and button animation via the base class constructor
        public Button(Vector2 position, Texture2D buttonTexture, string buttonname)
            : base(buttonTexture, position)
        {
            //Button label
            this.buttonName = buttonname;
            //Set position
            this.Position = position;
        }
       //Checks if button can be clicked
        public bool CheckForClick()
        {
            //If button is selected, take user input
            if (selected)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || mouseState.LeftButton == ButtonState.Pressed)
                {
                    //Returns that the button has been pressed
                    pressed = true;
                    return true;

                }
            }
            pressed = false;
            return false;

        }

        //Button game update logic
        public override void Update(GameTime gameTime)
        {
            //Gets mouse valuses
            mouseState = Mouse.GetState();
            //Changes button size if selected
            if (selected == true)
            {
                scale = 1.2f;
            }
            else
            {
                scale = 1f;
            }

            base.Update(gameTime);
        }

        //Button draw logic
        public override void Draw(SpriteBatch sb)
        {
            //Animation base class draw
            base.Draw(sb);
        }

    }
}