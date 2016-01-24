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
        public int hp;
        public Matrix moonTransformation;
        public Rectangle hitBox;
        public Moon(Texture2D texture, Vector2 pos,int hp) : base(texture,1,1,pos,0f,Color.White)
        {

            this.hp = hp;
            
            hitBox = new Rectangle((int)pos.X - frameWidth / 2, (int)pos.Y - frameHeight / 2, frameWidth, frameHeight);
        }
        public override void Update(GameTime gameTime)
        {
            if(hp<=0)
            {
                active = false;
            }
            if (active)
            {
                angle += 0.01f;
                base.Update(gameTime);

            }

        }
        public override void Draw(SpriteBatch sb)
        {
            
            base.Draw(sb);
        }
    }
}
