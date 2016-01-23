using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    class Moon : Animation
    {
        int hp;
        public Matrix moonTransformation;
        public Rectangle hitBox;
        public Moon(Texture2D texture, Vector2 pos,int hp) : base(texture,1,1,pos,0f,Color.White)
        {
            this.hp = hp;
            
            hitBox = new Rectangle((int)pos.X - frameWidth / 2, (int)pos.Y - frameHeight / 2, frameWidth, frameHeight);
        }
        public void Update(GameTime gameTime)
        {
            if(hp<=0)
            {
                active = false;
            }
            if (active)
            {
                base.Update(gameTime);
                moonTransformation =
            Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
            Matrix.CreateScale(scale) *
            Matrix.CreateTranslation(new Vector3(position, 0.0f));
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
            }

        }
        public void Draw(SpriteBatch sb)
        {
      
            base.Draw(sb);
        }
    }
}
