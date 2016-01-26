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
    class Moon : Animation
    {
        public int hp;

        //Constructor creates new moon and moon animation via the base class constructor
        public Moon(Texture2D texture, Vector2 position, int hp)
            : base(texture, position)
        {
            //Sets hp
            this.hp = hp;
        }

        //Moon game update logic
        public override void Update(GameTime gameTime)
        {
            //Disables moon on 0 or lower hp
            if (hp <= 0)
            {
                active = false;
            }
            //Rotates moon per millisecond
            angle += 0.001f * gameTime.ElapsedGameTime.Milliseconds;

            //Animation base class update
            base.Update(gameTime);



        }
        //Moon draw logic
        public override void Draw(SpriteBatch sb)
        {
            //Animation base class draw
            base.Draw(sb);
        }
    }
}
