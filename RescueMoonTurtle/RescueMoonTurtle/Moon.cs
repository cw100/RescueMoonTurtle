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
    
        public Moon(Texture2D texture, Vector2 position, int hp)
            : base(texture, 1, 1, position, 0f, Color.White)
        {

            this.hp = hp;

        }
        public override void Update(GameTime gameTime)
        {
            if(hp<=0)
            {
                active = false;
            }
            if (active)
            {
                angle += 0.001f*gameTime.ElapsedGameTime.Milliseconds;
                base.Update(gameTime);

            }

        }
        public override void Draw(SpriteBatch sb)
        {
            
            base.Draw(sb);
        }
    }
}
